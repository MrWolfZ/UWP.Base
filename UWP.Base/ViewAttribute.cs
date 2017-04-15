using System;

namespace UWP.Base
{
  [AttributeUsage(AttributeTargets.Class)]
  public sealed class ViewAttribute : Attribute
  {
    public ViewAttribute(string token, Type viewModelType)
    {
      this.Token = token;
      this.ViewModelType = viewModelType;
    }

    public string Token { get; }
    public Type ViewModelType { get; }
  }
}
