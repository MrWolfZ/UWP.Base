﻿using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace UWP.Base.Converters
{
  public sealed class InverseIntegerToVisibilityConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, string language) => GetVisibility(value);

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
      throw new NotSupportedException();
    }

    private static object GetVisibility(object value)
    {
      if (!(value is int))
      {
        return Visibility.Collapsed;
      }

      var objValue = (int)value;
      return objValue == 0 ? Visibility.Visible : Visibility.Collapsed;
    }
  }
}