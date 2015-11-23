//using System.Collections.Generic;
//using System.Linq;

//namespace DrivenDb.VisualStudio.Generator.Internal
//{   
//   internal class TableMappingCollection
//   {
//      private readonly ILookup<string, ColumnMapping> _mappings;

//      public TableMappingCollection()
//         : this(new ColumnMapping[0])
//      {
         
//      }

//      public TableMappingCollection(IEnumerable<ColumnMapping> mappings)
//      {
//         _mappings = mappings.ToLookup(m => m.TableName);
//      }

//      public ColumnMappingCollection GetMappingOrEmpty(string tableName)
//      {
//         return new ColumnMappingCollection(_mappings[tableName]);
//      }
//   }
//}