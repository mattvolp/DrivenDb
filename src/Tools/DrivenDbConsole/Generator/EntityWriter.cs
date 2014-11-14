﻿/**************************************************************************************
 * Original Author : Anthony Leatherwood (adleatherwood@gmail.com)
 * Source Location : http://drivendb.codeplex.com
 *
 * This source is subject to the Microsoft Public License.
 * Link: http://www.microsoft.com/en-us/openness/licenses.aspx
 *
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
 * EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
 **************************************************************************************/

using DrivenDbConsole.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DrivenDbConsole.Generator
{
   internal class EntityWriter
   {
      private readonly StringBuilder m_Output = new StringBuilder();

      #region PUBLIC METHODS ----------------------------------------------------------------------

      public string Execute(string contextName, string spaceName, bool useLinq, bool lessChanges, bool writeSchema, bool scriptDefaults, bool useUnspecified, IDatabaseInfo database)
      {
         WriteHeader(spaceName);

         if (writeSchema)
         {
            WriteSchema(contextName, database.Tables);
         }

         WriteTables(database.Tables, useLinq, lessChanges, scriptDefaults, useUnspecified);
         WriteFooter();

         return m_Output.ToString();
      }

      #endregion PUBLIC METHODS ----------------------------------------------------------------------

      #region PRIVATE METHODS ---------------------------------------------------------------------

      private void WriteHeader(string nspace)
      {
         WriteLine("/* Generated by the DrivenDb Entity Generator. Do not modify. */");
         WriteLine("");
         WriteLine("using System;");
         WriteLine("using System.ComponentModel;");
         //WriteLine("using System.Data.Linq.Mapping;");
         WriteLine("using System.Runtime.Serialization;");
         WriteLine("using DrivenDb;");
         WriteLine("");
         WriteLine("namespace " + nspace);
         WriteLine("{");
      }

      private void WriteSchema(string contextName, IEnumerable<ITableInfo> tableDefs)
      {
         WriteLine("\tpublic static class " + contextName + "Schema");
         WriteLine("\t{");
         WriteLine("\t\tstatic " + contextName + "Schema()");
         WriteLine("\t\t{");

         foreach (var table in tableDefs)
         {
            WriteLine("\t\t\t" + table.Name + " = new " + table.Name + "();");
         }

         WriteLine("\t\t}");
         WriteLine("");

         foreach (var table in tableDefs)
         {
            WriteLine("\t\tpublic static " + table.Name + " " + table.Name + " { get; private set; }");
         }

         WriteLine("\t}");
         WriteLine("");
      }

      private void WriteTables(IEnumerable<ITableInfo> tableDefs, bool useLinq, bool lessChanges, bool scriptDefaults, bool useUnspecified)
      {
         foreach (var table in tableDefs)
         {
            //
            // DECLARATION
            //
            WriteLine("\t[DataContract]");

            if (useLinq)
            {
               if (!String.IsNullOrWhiteSpace(table.Schema))
               {
                  WriteLine("\t[Table(Name=\"" + table.Schema + "." + table.Name + "\")]");
               }
               else
               {
                  WriteLine("\t[Table(Name=\"" + table.Name + "\")]");
               }
            }
            else
            {
               if (!String.IsNullOrWhiteSpace(table.Schema))
               {
                  WriteLine("\t[DbTable(Schema=\"" + table.Schema + "\", Name=\"" + table.Name + "\")]");
               }
               else
               {
                  WriteLine("\t[DbTable(Name=\"" + table.Name + "\")]");
               }
            }

            WriteLine("\tpublic partial class " + table.Name + " : DbEntity<" + table.Name + ">, INotifyPropertyChanged");
            WriteLine("\t{");

            //
            // MEMBERS
            //
            WriteLine("");

            foreach (var field in table.Fields)
            {
               WriteLine("\t\t[DataMember]");
               WriteLine("\t\tprivate " + field.ClrType + " m_" + field.Name + ";");
            }

            WriteLine("");

            //
            // CONSTRUCTOR
            //
            WriteLine("\tpublic " + table.Name + "()");
            WriteLine("\t{");

            if (scriptDefaults)
            {
               foreach (var field in table.Fields)
               {
                  if (field.HasDefault)
                  {
                     WriteLine("\t\t" + field.Name + (field.HasDefault ? (" = " + field.DefaultValue + ";") : ";"));
                  }
               }
            }

            WriteLine("\t}");

            WriteLine("");

            //
            // PROPERTIES
            //
            foreach (var field in table.Fields)
            {
               if (useLinq)
               {
                  WriteLine("\t\t[Column(Name=\"" + field.Name + "\", DbType=\"" + field.SqlType + "\", IsPrimaryKey=" + (field.IsPrimaryKey ? "true" : "false") + ", IsDbGenerated=" + (field.IsIdentity ? "true" : "false") + ", CanBeNull=" + (field.IsNullable ? "true" : "false") + ")]");
               }
               else
               {
                  WriteLine("\t\t[DbColumn(Name=\"" + field.Name + "\", IsPrimaryKey=" + (field.IsPrimaryKey ? "true" : "false") + ", IsDbGenerated=" + (field.IsIdentity ? "true" : "false") + ")]");
               }

               WriteLine("\t\tpublic " + field.ClrType + " " + field.Name);
               WriteLine("\t\t{");
               WriteLine("\t\t\tget { return m_" + field.Name + "; }");

               if (!field.IsIdentity && !field.IsReadonly)
               {
                  WriteLine("\t\t\tset {");

                  if (lessChanges)
                  {
                     WriteLine("\t\t\t\tif (value == m_" + field.Name + ") return;");
                  }

                  if (useUnspecified && field.ClrType == "DateTime")
                  {
                     WriteLine("\t\t\t\tvalue = new DateTime(value.Ticks, DateTimeKind.Unspecified);");
                  }
                  else if (useUnspecified && field.ClrType == "DateTime?")
                  {
                     WriteLine("\t\t\t\tif (value.HasValue)");
                     WriteLine("\t\t\t\t{");
                     WriteLine("\t\t\t\t\tvalue = new DateTime(value.Value.Ticks, DateTimeKind.Unspecified);");
                     WriteLine("\t\t\t\t}");
                     WriteLine("");
                  }

                  WriteLine("\t\t\t\tBefore" + field.Name + "Changed(ref value);");
                  WriteLine("\t\t\t\tm_" + field.Name + " = value;");
                  WriteLine("\t\t\t\tAfter" + field.Name + "Changed(value);");
                  WriteLine("\t\t\t\tPropertyChanged(this, new PropertyChangedEventArgs(\"" + field.Name + "\"));");
                  WriteLine("\t\t\t}");
               }
               else
               {
                  WriteLine("\t\t\tset { m_" + field.Name + " = value; }");
               }

               WriteLine("\t\t}");
               WriteLine("");
            }

            WriteLine("\t\tpublic event PropertyChangedEventHandler PropertyChanged;");
            WriteLine("");

            //
            // PARTIALS
            //

            foreach (var field in table.Fields.Where(f => !f.IsIdentity && !f.IsReadonly))
            {
               WriteLine("\t\tpartial void Before" + field.Name + "Changed(ref " + field.ClrType + " value);");
            }

            WriteLine("");

            foreach (var field in table.Fields.Where(f => !f.IsIdentity && !f.IsReadonly))
            {
               WriteLine("\t\tpartial void After" + field.Name + "Changed(" + field.ClrType + " value);");
            }

            WriteLine("");
            WriteLine("\t\tpartial void OnSerialization();");
            WriteLine("\t\tpartial void OnDeserialization();");
            WriteLine("");

            WriteLine("\t\tprotected override void BeforeSerialization()");
            WriteLine("\t\t{");
            WriteLine("\t\t\tOnSerialization();");
            WriteLine("\t\t}");
            WriteLine("");

            WriteLine("\t\tprotected override void AfterDeserialization()");
            WriteLine("\t\t{");
            WriteLine("\t\t\tOnDeserialization();");
            WriteLine("\t\t}");

            //
            // CLOSE
            //
            WriteLine("\t}");
            WriteLine("");
         }
      }

      private void WriteFooter()
      {
         WriteLine("}");
      }

      private void WriteLine(string text)
      {
         m_Output.AppendLine(text);
      }

      #endregion PRIVATE METHODS ---------------------------------------------------------------------
   }
}