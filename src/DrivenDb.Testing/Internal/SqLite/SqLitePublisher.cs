using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DrivenDb.Core;
using DrivenDb.Data;

namespace DrivenDb.Testing.Internal.SqLite
{
   internal class SqLitePublisher
      : IEntityPublisher
   {
      private readonly IDbWriter _writer;

      public SqLitePublisher(IDbWriter writer)
      {
         _writer = writer;
      }

      public void Publish<T>(T item)         
      {
         Publish(new T[] {item});
      }

      public void Publish<T>(params T[] items)         
      {
         Publish(items.AsEnumerable());
      }

      public void Publish<T>(IEnumerable<T> items)         
      {         
         var tables = items.Select(e => e.GetType())
            .Select(t => new TableDef(t))
            .Distinct()
            .ToArray();

         CreateTables(tables);

         // TODO:
         //var entities = items
         //   .Select(p => p.Entity)
         //   .ToArray();

         //Writer.Write(entities);
      }

      private void CreateTables(IEnumerable<TableDef> tables)
      {
         foreach (var table in tables)
         {
            CreateTable(table);
         }
      }

      private void CreateTable(TableDef definition)
      {
         var script = String.Format("CREATE TABLE {0} (", definition.Table.Name);

         foreach (var column in definition.Columns)
         {            
            script += String.Format("{0} {1},", column.Attribute.Name, GetSqlType(column.Field));
         }

         script += ")";

         _writer.Write(script);
      }

      private static string GetSqlType(FieldInfo field)
      {
         switch (field.FieldType.Name.ToLower())
         {
            case "string" :
               return "TEXT";
            case "datetime":
            case "decimal": 
               return "NUMERIC";
            case "single":
            case "float":
            case "double":
               return "REAL";               
            default:
               return "INTEGER";
         }
      }

      private class TableDef
      {         
         public TableDef(Type type)
         {
            Table = type.GetCustomAttribute<DbTableAttribute>();
            Columns = type.GetFields()
               .Select(f => new ColumnDef(f, f.GetCustomAttribute<DbColumnAttribute>(true)))
               .ToArray();
         }

         public readonly DbTableAttribute Table;
         public readonly IEnumerable<ColumnDef> Columns;

         public override bool Equals(object obj)
         {
            var other = obj as TableDef;

            if (other == null)
            {
               return false;
            }

            return Equals(Table.Name, other.Table.Name);
         }

         public override int GetHashCode()
         {
            return Table.Name.GetHashCode();
         }
      }

      private class ColumnDef
      {
         public ColumnDef(FieldInfo field, DbColumnAttribute attribute)
         {
            Attribute = attribute;
            Field = field;
         }

         public readonly DbColumnAttribute Attribute;
         public readonly FieldInfo Field;
      }
   }
}
