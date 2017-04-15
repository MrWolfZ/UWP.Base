using System;
using Windows.UI.Xaml.Data;

namespace UWP.Base.Converters
{
  public sealed class NullToBooleanConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, string language) => value != null;

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
      throw new NotSupportedException();
    }
  }
}