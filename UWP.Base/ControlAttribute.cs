using System;

namespace UWP.Base
{
  [AttributeUsage(AttributeTargets.Class)]
  public sealed class ControlAttribute : Attribute
  {
    public ControlAttribute(Type viewModelType)
    {
      this.ViewModelType = viewModelType;
    }

    public Type ViewModelType { get; }
  }
}