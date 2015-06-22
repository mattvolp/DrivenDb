using System;
using System.Collections.Generic;
using System.Linq;
using DrivenDb.Data;
using DrivenDb.Data.Internal;
using DrivenDb.VisualStudio.Generator.Internal;

namespace DrivenDb.Testing
{
   internal class EntityScripter      
   {      
      private readonly List<TableMap> _tables = new List<TableMap>();
      private ScriptingOptions _options;

      public EntityScripter CreateTable(string tableName)
      {
         CreateTable("dbo", tableName);

         return this;
      }

      public EntityScripter CreateTable(string schema, string tableName)
      {
         var table = new TableDetail(schema, tableName, new ColumnDetail[0]);
         var columnMaps = table.Columns.Select(c => new ColumnMap(c, null));
         var tableMap = new TableMap(table, columnMaps);

         _tables.Add(tableMap);

         return this;
      }

      public EntityScripter AllScriptingOptions()
      {
         SetOptions(ScriptingOptions.All);

         return this;
      }

      public EntityScripter SetOptions(ScriptingOptions options)
      {
         _options = options;

         return this;
      }

      public EntityScripter AffixKeyColumn(string columnName, DbType sqlType, Type mapping = null)
      {
         AffixColumn(columnName, sqlType, true, null, mapping);

         return this;
      }

      public EntityScripter AffixKeyColumn(string columnName, DbType sqlType, string @default)
      {
         AffixColumn(columnName, sqlType, true, @default, null);

         return this;
      }

      public EntityScripter AffixColumn(string columnName, DbType sqlType, Type mapping = null)
      {
         AffixColumn(columnName, sqlType, false, null, mapping);

         return this;
      }

      public EntityScripter AffixColumn(string columnName, DbType sqlType, string @default, Type mapping = null)
      {
         AffixColumn(columnName, sqlType, false, @default, mapping);
         
         return this;
      }

      public EntityFixture Build()
      {
         var compiler = new EntityCompiler();
         var factory = compiler.CompileFactory(_options, _tables.ToArray());

         return new EntityFixture(factory);
      }

      private void AffixColumn(string columnName, DbType sqlType, bool isPrimary, string @default, Type mapping)
      {
         // TODO: should be a parameter given from a new method AffixIdentityColumn or something
         var isGenerated = columnName.ToLower() == "id";
         var existing = _tables.Last();
         var isNullable = sqlType.ToString().Contains("?");
         var column = new ColumnDetail(sqlType, columnName, isNullable, isPrimary, isGenerated, false, @default != null, @default, existing.Columns.Count());
         var columnType = mapping != null
            ? mapping.Namespace + "." + mapping.Name
            : null;

         var columnMap = new ColumnMap(column, columnType);
         var table = new TableDetail(
              existing.Detail.Schema
            , existing.Detail.Name
            , existing.Columns.Select(c => c.Detail).ToArray()
            );
         var columnMaps = existing.Columns.Concat(new[] {columnMap});
         var tableMap = new TableMap(table, columnMaps);
         
         _tables[_tables.Count - 1] = tableMap;
      }
   }
}
