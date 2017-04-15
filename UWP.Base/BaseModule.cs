using Autofac;

namespace UWP.Base
{
  public sealed class BaseModule : Module
  {
    protected override void Load(ContainerBuilder builder)
    {
      builder.RegisterType<ApplicationLifecycle>()
             .AsImplementedInterfaces();

      base.Load(builder);
    }
  }
}
