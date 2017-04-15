using System;

namespace UWP.Base
{
  public sealed class ControlRegistration
  {
    public ControlRegistration(Type controlType, Type viewModelType)
    {
      this.ControlType = controlType;
      this.ViewModelType = viewModelType;
    }

    public Type ControlType { get; }
    public Type ViewModelType { get; }
  }
}