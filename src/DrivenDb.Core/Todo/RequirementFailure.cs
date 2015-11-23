using System.Runtime.Serialization;

namespace DrivenDb.Core
{
   [DataContract]
   public class RequirementFailure
   {
      public RequirementFailure(string name, string reason, object value)
      {
         Name = name;
         Reason = reason;
         Value = value;
      }

      [DataMember]
      public readonly string Name;

      [DataMember]
      public readonly string Reason;

      [DataMember]
      public readonly object Value;
   }
}
