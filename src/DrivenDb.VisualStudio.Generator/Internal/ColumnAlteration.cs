using System.Runtime.Serialization;

namespace DrivenDb.VisualStudio.Generator.Internal
{
   [DataContract]
   internal class ColumnAlteration
   {
      public ColumnAlteration(
           string columnName
         , string mappedType
         )
      {      
         ColumnName = columnName;
         MappedType = mappedType;
      }

      [DataMember]
      public readonly string ColumnName;

      [DataMember]
      public readonly string MappedType;
   }
}
