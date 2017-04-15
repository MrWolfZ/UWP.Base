using System.Runtime.Serialization;

namespace UWP.Base
{
  [DataContract]
  public class NavigationArgs
  {
    [DataMember]
    public string TargetViewToken { get; set; }

    [DataMember]
    public string SourceViewToken { get; set; }
  }
}
