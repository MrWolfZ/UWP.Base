using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.Globalization;
using Windows.System.UserProfile;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Autofac;
using Autofac.Core;
using Newtonsoft.Json;
using Prism.Autofac.Windows;
using Prism.Logging;
using Prism.Mvvm;
using Prism.Windows.Navigation;

namespace UWP.Base
{
  public abstract class App : PrismAutofacApplication
  {
    private IDictionary<string, Type> ViewRegistrations { get; } = new Dictionary<string, Type>();

    protected override async Task OnLaunchApplicationAsync(LaunchActivatedEventArgs args)
    {
      await Task.Yield();

      ApplicationLanguages.PrimaryLanguageOverride = GlobalizationPreferences.Languages[0];

      if (!string.IsNullOrEmpty(args.Arguments))
      {
        // The app was launched from a Secondary Tile
        // Navigate to the item's page
        var navArgs = JsonConvert.DeserializeObject<NavigationArgs>(args.Arguments);
        this.NavigationService.Navigate(navArgs.TargetViewToken, args.Arguments);
      }

      // Navigate to the initial page
      var initialPage = this.Container.Resolve<IApplicationConfiguration>().InitialPageToken;
      this.NavigationService.Navigate(initialPage, JsonConvert.SerializeObject(new NavigationArgs { TargetViewToken = initialPage }));

      Window.Current.Activate();
    }

    protected override void OnRegisterKnownTypesForSerialization()
    {
      // Set up the list of known types for the SuspensionManager
      this.SessionStateService.RegisterKnownType(typeof(Dictionary<string, Collection<string>>));
    }

    protected override void ConfigureContainer(ContainerBuilder builder)
    {
      builder.RegisterModule<BaseModule>();
      builder.RegisterModule<LoggingModule>();
      builder.RegisterAssemblyModules(this.GetType().GetTypeInfo().Assembly);

      base.ConfigureContainer(builder);
    }

    protected override async Task<Frame> InitializeFrameAsync(IActivatedEventArgs args)
    {
      var frame = await base.InitializeFrameAsync(args);

      this.RegisterInstance<IFrameFacade>(new FrameFacadeAdapter(frame), typeof(IFrameFacade), "", true);

      return frame;
    }

    protected override Type GetPageType(string pageToken)
    {
      Type t;
      return this.ViewRegistrations.TryGetValue(pageToken, out t) ? t : base.GetPageType(pageToken);
    }

    protected override Task OnInitializeAsync(IActivatedEventArgs args)
    {
      var viewRegistrations = this.Container.Resolve<IEnumerable<ViewRegistration>>().ToList();
      foreach (var r in viewRegistrations)
      {
        this.ViewRegistrations.Add(r.Token, r.ViewType);
      }

      var controlRegistrations = this.ResolveControlRegistrations();

      var viewModelRegistry = viewRegistrations.Select(r => new { Type = r.ViewType, r.ViewModelType })
                                               .Concat(controlRegistrations.Select(r => new { Type = r.ControlType, r.ViewModelType }))
                                               .ToDictionary(a => a.Type, a => a.ViewModelType);

      ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver(viewType => viewModelRegistry[viewType]);

      ViewModelLocationProvider.SetDefaultViewModelFactory(this.CreateViewModel);

      // Documentation on working with tiles can be found at http://go.microsoft.com/fwlink/?LinkID=288821&clcid=0x409
      //var _tileUpdater = TileUpdateManager.CreateTileUpdaterForApplication();
      //_tileUpdater.StartPeriodicUpdate(new Uri(Constants.ServerAddress + "/api/TileNotification"), PeriodicUpdateRecurrence.HalfHour);

      return base.OnInitializeAsync(args);
    }

    protected override ILoggerFacade CreateLogger() => new DebugLogger();

    private object CreateViewModel(object view, Type viewModelType)
    {
      var viewModel = this.Container.Resolve(viewModelType);

      if (viewModel is IDisposable d && view is FrameworkElement el)
      {
        el.Unloaded += (sender, args) => d.Dispose();
      }

      return viewModel;
    }

    private IEnumerable<ControlRegistration> ResolveControlRegistrations()
    {
      try
      {
        return this.Container.Resolve<IEnumerable<ControlRegistration>>().ToList();
      }
      catch (DependencyResolutionException ex)
      {
        this.Logger.Log($"Exception while trying to resolve control registrations. Expected if no controls are registered.\n{ex}", Category.Warn, Priority.Low);
        return Enumerable.Empty<ControlRegistration>();
      }
    }

    private sealed class DebugLogger : ILoggerFacade
    {
      public void Log(string message, Category category, Priority priority)
      {
        var log = LoggingModule.LogManager.GetLogger<App>();
        switch (category)
        {
          case Category.Debug:
            log.Trace(message);
            break;
          case Category.Exception:
            log.Error(message);
            break;
          case Category.Info:
            log.Info(message);
            break;
          case Category.Warn:
            log.Warn(message);
            break;
          default:
            throw new ArgumentOutOfRangeException(nameof(category), category, null);
        }
      }
    }
  }
}
