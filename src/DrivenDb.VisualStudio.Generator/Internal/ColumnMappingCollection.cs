//using System.Collections.Generic;
//using System.Linq;

//namespace DrivenDb.VisualStudio.Generator.Internal
//{
//   internal class ColumnMappingCollection
//   {
//      private readonly Dictionary<string, string> _mappings;

//      public ColumnMappingCollection(IEnumerable<ColumnMapping> mappings)
//      {
//         _mappings = mappings.ToDictionary(m => m.ColumnName, m => m.MappedType);
//      }

//      public string GetMappingOrEmpty(string columnName, string @default)
//      {
//         var result = "";

//         _mappings.TryGetValue(columnName, out result);

//         return result ?? @default;
//      }
//   }
//}