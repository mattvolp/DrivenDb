using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DrivenDb.Data;
using DrivenDb.Data.Internal;

namespace DrivenDb.Scripting.Internal
{
   internal class CsGenerator : IEntityGenerator
   {
      private readonly ScriptingOptions _options;
      private readonly OptionWriter _writer;

      public CsGenerator(ScriptingOptions options, TextWriter writer)
      {
         _writer = new OptionWriter(options, writer);
         _options = options;
      }

      public void Write(string @namespace, string contextName, IEnumerable<TableMap> tables)
      {
         WriteNamespaceOpen(@namespace);
         WriteContext(contextName, tables);

         foreach (var table in tables)
         {
            ScriptEntity(table);
         }

         WriteNamespaceClose();
      }

      protected void WriteNamespaceOpen(string @namespace)
      {
         _writer.WriteLines(new OptionLines()
            {
               {"                                           "},
               {"namespace $0                               "},
               {"{                                          "},
               {"   using DrivenDb.Core;                    "},
               {"   using System;                           "},
               {"   using System.Linq;                      "},
               {"   using System.Collections.Generic;       "},
               {"   using System.Runtime.CompilerServices;  "},
               {"   using System.Data.Linq;                 ", ScriptingOptions.ImplementLinqContext},
               {"   using System.Data.Linq.Mapping;         ", ScriptingOptions.ImplementLinqContext},
               {"   using System.Runtime.Serialization;     ", ScriptingOptions.Serializable},
               {"   using System.ComponentModel;            ", ScriptingOptions.ImplementNotifyPropertyChanged, ScriptingOptions.ImplementNotifyPropertyChanging},
            }, @namespace);
      }

      protected void WriteContext(string contextName, IEnumerable<TableMap> tables)
      {
         if (_options.HasFlag(ScriptingOptions.ImplementLinqContext))
            _writer
               .WriteLines(new OptionLines()
                  {
                     {"                                                                                              "},
                     {"    public class $0 : DataContext                                                             "},
                     {"    {                                                                                         "},
                     {"        private static readonly MappingSource _mappingSource = new AttributeMappingSource();  "},
                     {"                                                                                              "},
                     {"        public $0() : base(\"_\", _mappingSource)                                             "},
                     {"        {                                                                                     "},
                     {"        }                                                                                     "},
                  }, contextName)
               .WriteTemplate(tables, new OptionLines()
                  {
                     {"                                                                                              "},
                     {"        public Table<$0> $0                                                                   "},
                     {"        {                                                                                     "},
                     {"            get { return this.GetTable<$0>(); }                                               "},
                     {"        }                                                                                     "},
                  }, t => new object[] {t.Detail.Name})

               .WriteLines(new OptionLines()
                  {
                     {"    }                                                                                         "},
                  });
      }

      protected void ScriptEntity(TableMap table)
      {
         if (_options.HasFlag(ScriptingOptions.ImplementPrimaryKey))
         {
            WriteKeyClass(table);
         }

         WriteClassOpen(table);
         WriteConstructor(table);

         foreach (var column in table.Columns)
         {
            WriteField(column);
         }

         foreach (var column in table.Columns)
         {
            WritePartial(column);
         }

         if (_options.HasFlag(ScriptingOptions.ImplementPrimaryKey))
         {
            WriteKeyProperty(table);
         }

         foreach (var column in table.Columns)
         {
            WriteProperty(column);
         }

         if (_options.HasFlag(ScriptingOptions.ImplementNotifyPropertyChanging))
         {
            WritePropertyChanging();
         }

         if (_options.HasFlag(ScriptingOptions.ImplementNotifyPropertyChanged))
         {
            WritePropertyChanged();
         }

         if (_options.HasFlag(ScriptingOptions.ImplementValidationCheck))
         {
            WriteValidationCheck(table);
         }

         WriteClassClose();
      }

      // TODO: consider testing mappings to custom types for key classes
      private void WriteKeyClass(TableMap table)
      {
         var primaries = table.Columns.Where(c => c.Detail.IsPrimary)
            .OrderBy(c => c.Detail.ColumnPosition)
            .ToList();

         if (primaries.Count > 8)
         {
            throw new Exception("Unable to script key class for tables with a primary key of more than 8 columns");
         }

         if (primaries.Count < 1)
         {
            return;
         }

         var types = String.Join(", ", primaries.Select(p => p.Detail.SqlType.ToCsString()));
         var typedParameters = String.Join(", ", primaries.Select(p => p.Detail.SqlType.ToCsString() + " @" + p.Detail.Name.ToLower()));
         var baseParameters = String.Join(", ", primaries.Select(p => "@" + p.Detail.Name.ToLower()));
         
         _writer
            .WriteLines(new OptionLines()
               {
                  {"                                        "},
                  {"    public class $0Key                  "},
                  {"        : Tuple<$1>                     "},
                  {"    {                                   "},
                  {"        public $0Key($2)                "},
                  {"            : base($3)                  "},
                  {"        {                               "},
               }, table.Detail.Name, types, typedParameters, baseParameters)

            .WriteTemplate(primaries, new OptionLines()
               {
                  {"            $0 = @$1;                   "},
               }, p => new object[] {p.Detail.Name, p.Detail.Name.ToLower()})

            .WriteLines(new OptionLines()
               {
                  {"        }                               "},
                  {"                                        "},
               })

            .WriteTemplate(primaries, new OptionLines()
               {
                  {"        public readonly $0 $1;          "},
               }, p => new object[] {p.Detail.SqlType.ToCsString(), p.Detail.Name})

            .WriteLines(new OptionLines()
               {
                  {"   }                                    "},
               });
      }

      protected void WriteClassOpen(TableMap table)
      {
         var interfaces = new List<string>();

         if (_options.HasFlag(ScriptingOptions.ImplementNotifyPropertyChanging))
         {
            interfaces.Add("INotifyPropertyChanging");
         }

         if (_options.HasFlag(ScriptingOptions.ImplementNotifyPropertyChanged))
         {
            interfaces.Add("INotifyPropertyChanged");
         }

         if (_options.HasFlag(ScriptingOptions.ImplementStateTracking))
         {
            interfaces.Add("IDbEntityProvider");
         }

         var inheritance = interfaces.Count > 0
            ? $" : {string.Join(", ", interfaces)}"
            : "";

         _writer.WriteLines(new OptionLines()
            {
               {"                                                             "},
               {"    [DataContract]                                           ", ScriptingOptions.Serializable},
               {"    [Table(Name = \"$0.$1\")]                                ", ScriptingOptions.ImplementLinqContext},
               {"    [DbTable(Schema=\"$0\", Name=\"$1\")]                    "},
               {"    public partial class $1 $2                               "},
               {"    {                                                        "},
               {"        [DataMember]                                         ", ScriptingOptions.Serializable | ScriptingOptions.ImplementStateTracking},
               {"        private readonly DbEntity _entity = new DbEntity();  ", ScriptingOptions.ImplementStateTracking},
            }, table.Detail.Schema, table.Detail.Name, inheritance);
      }

      protected void WriteConstructor(TableMap table)
      {
         if (_options.HasFlag(ScriptingOptions.ImplementColumnDefaults) && table.Columns.Any(c => c.Detail.HasDefault))
         {
            _writer.WriteLines(new OptionLines()
               {
                  {"                                    "},
                  {"        public $0()                 "},
                  {"        {                           "},
               }, table.Detail.Name);

            foreach (var column in table.Columns.Where(c => c.Detail.HasDefault))
            {
               var defaultValue = column.Detail.SqlType.ToCsDefault(_options, column.Detail);
               var csType = column.HasCustomType
                  ? column.CustomType
                  : column.Detail.SqlType.ToCsString();

               _writer.WriteLines(new OptionLines()
                  {
                     {"            _$0 = ($1) $2;              "},
                     {"            _entity.Change($0, _$0);    ", ScriptingOptions.ImplementStateTracking},
                  }, column.Detail.Name, csType, defaultValue);
            }

            _writer.WriteLine("        }");
         }

         _writer.WriteLines(new OptionLines()
            {
               {"                                     ", ScriptingOptions.ImplementStateTracking},
               {"        public IDbEntity Entity      ", ScriptingOptions.ImplementStateTracking},
               {"        {                            ", ScriptingOptions.ImplementStateTracking},
               {"            get { return _entity; }  ", ScriptingOptions.ImplementStateTracking},
               {"        }                            ", ScriptingOptions.ImplementStateTracking},
            });
      }

      protected void WriteField(ColumnMap column)
      {
         var csType = column.HasCustomType
            ? column.CustomType
            : column.Detail.SqlType.ToCsString();

         var isPrimary = column.Detail.IsPrimary ? "true" : "false";
         var isGenerated = column.Detail.IsGenerated ? "true" : "false";

         _writer.WriteLines(new OptionLines()
            {
               {"                                                                "},
               {"        [DataMember]                                            ", ScriptingOptions.Serializable},
               {"        [DbColumn(Name=\"$0\", IsPrimary=$1, IsGenerated=$2)]   "},
               {"        private $3 _$0;                                         "},
            }, column.Detail.Name, isPrimary, isGenerated, csType);
      }

      protected void WritePartial(ColumnMap column)
      {
         var csType = column.Detail.SqlType.ToCsString();

         _writer.WriteLines(new OptionLines()
            {
               {"                                                    ", ScriptingOptions.ImplementPartialPropertyChanges},
               {"        partial void On$0Changing(ref $1 value);    ", ScriptingOptions.ImplementPartialPropertyChanges},
               {"        partial void On$0Changed();                 ", ScriptingOptions.ImplementPartialPropertyChanges},
            }, column.Detail.Name, csType);
      }

      // TODO: review for custom mappings consideration
      private void WriteKeyProperty(TableMap table)
      {
         var primaries = table.Columns.Where(c => c.Detail.IsPrimary)
            .OrderBy(c => c.Detail.ColumnPosition)
            .ToList();

         if (primaries.Count < 1)
         {
            return;
         }

         var fields = primaries
            .Select(p => "_" + p.Detail.Name)
            .Join(", ");

         _writer.WriteLines(new OptionLines()
            {
               {"                                              "},
               {"        public $0Key PrimaryKey               "},
               {"        {                                     "},
               {"            get { return new $0Key($1); }     "},
               {"        }                                     "},
            }, table.Detail.Name, fields);
      }

      protected void WriteProperty(ColumnMap column)
      {
         var csType = column.HasCustomType
            ? column.CustomType
            : column.Detail.SqlType.ToCsString();

         _writer
            .WriteLines(new OptionLines()
               {
                  {""},
                  {"        [Column]                   ", ScriptingOptions.ImplementLinqContext},
                  {"        public $0 $1               "},
                  {"        {                          "},
                  {"            get { return _$1; }    "},
                  {"            set                    "},
                  {"            {                      "},
                  {"                if (_$1 != value)  ", ScriptingOptions.MinimizePropertyChanges},
                  {"                {                  ", ScriptingOptions.MinimizePropertyChanges},
               }, csType, column.Detail.Name)

            .WriteIf(csType == "DateTime" || csType == "DateTime?")
            .WriteLines(new OptionLines()
               {
                  {"                    if (!Equals(value, null))    ", ScriptingOptions.UnspecifiedDateTimes},
                  {"                    {                            ", ScriptingOptions.UnspecifiedDateTimes},
               })
            .WriteLine(
               csType == "DateTime?"
                  ? "                        value = new DateTime(value.Value.Ticks, DateTimeKind.Unspecified);"
                  : "                        value = new DateTime(value.Ticks, DateTimeKind.Unspecified);", ScriptingOptions.UnspecifiedDateTimes)

            .WriteIf(column.Detail.SqlType.IsDateOnly() && _options.HasFlag(ScriptingOptions.TruncateTimeForDateColumns))
            .WriteLine(
               _options.HasFlag(ScriptingOptions.UnspecifiedDateTimes)
                  ? "                        value = new DateTime(value.Date.Ticks, DateTimeKind.Unspecified);"
                  : "                        value = value.Date;")

            .WriteIf(csType == "DateTime" || csType == "DateTime?")
            .WriteLines(new OptionLines()
               {
                  {"                    }                ", ScriptingOptions.UnspecifiedDateTimes},
                  {"                                     ", ScriptingOptions.UnspecifiedDateTimes},
               })

            .WriteIf(true)
            .WriteLines(new OptionLines()
               {
                  {"                    On$0Changing(ref value);        ", ScriptingOptions.ImplementPartialPropertyChanges},
                  {"                    OnPropertyChanging();           ", ScriptingOptions.ImplementNotifyPropertyChanging},
                  {"                    _$0 = value;                    "},
                  {"                    _entity.Change(\"$0\", value);  ", ScriptingOptions.ImplementStateTracking},
                  {"                    OnPropertyChanged();            ", ScriptingOptions.ImplementNotifyPropertyChanged},
                  {"                    On$0Changed();                  ", ScriptingOptions.ImplementPartialPropertyChanges},
                  {"                }                                   ", ScriptingOptions.MinimizePropertyChanges},
                  {"            }                                       "},
                  {"        }                                           "},
               }, column.Detail.Name);
      }

      protected void WritePropertyChanging()
      {
         _writer.WriteLines(new OptionLines()
            {
               {"                                                                                              "},
               {"        public event PropertyChangingEventHandler PropertyChanging = delegate {};             "},
               {"                                                                                              "},
               {"        protected virtual void OnPropertyChanging([CallerMemberName] string property = null)  "},
               {"        {                                                                                     "},
               {"            PropertyChanging(this, new PropertyChangingEventArgs(property));                  "},
               {"        }                                                                                     "},
            });
      }

      protected void WritePropertyChanged()
      {
         _writer.WriteLines(new OptionLines()
            {
               {"                                                                                              "},
               {"        public event PropertyChangedEventHandler PropertyChanged = delegate {};               "},
               {"                                                                                              "},
               {"        protected virtual void OnPropertyChanged([CallerMemberName] string property = null)   "},
               {"        {                                                                                     "},
               {"            PropertyChanged(this, new PropertyChangedEventArgs(property));                    "},
               {"        }                                                                                     "},
            });
      }

      // TODO: consider testing custom mapped types in validation check
      private void WriteValidationCheck(TableMap table)
      {
         if (!_options.HasFlag(ScriptingOptions.ImplementStateTracking))
         {
            throw new Exception("Cannot implement validation check without state tracking enabled");
         }

         /*
          * state == current -> true
          * state == deleted -> true
          * state == updated -> if all non-nullable/non-generated string columns have a value then true
          * state == new -> if all non-nullable/non-generated columns have a change recorded
          */
         _writer
            .WriteLines(new OptionLines()
               {
                  {"        partial void HasExtendedRequirementsMet(IList<RequirementFailure> failures);                            "},
                  {"                                                                                                                "},
                  {"        public IEnumerable<RequirementFailure> GetRequirementsFailures()                                        "},
                  {"        {                                                                                                       "},
                  {"            var failures = new List<RequirementFailure>();                                                      "},
                  {"                                                                                                                "},
                  {"            if (_entity.State == EntityState.Current || _entity.State == EntityState.Deleted)                   "},
                  {"            {                                                                                                   "},
                  {"                return failures;                                                                                "},
                  {"            }                                                                                                   "},
                  {"                                                                                                                "},
                  {"            if (_entity.State == EntityState.Updated || _entity.State == EntityState.New)                       "},
                  {"            {                                                                                                   "},
               })

            .WriteTemplate(table.Columns.Where(c => !c.Detail.IsGenerated && !c.Detail.IsNullable && c.Detail.SqlType.ToCsString() == "string"), new OptionLines()
               {
                  {"             if (_$0 == default(string))                                                                        "},
                  {"             {                                                                                                  "},
                  {"                failures.Add(new RequirementFailure(\"$0\", \"Null value not allowed\", default(string)));      "},
                  {"             }                                                                                                  "},
               }, c => new object[] {c.Detail.Name})

            .WriteLines(new OptionLines()
               {
                  {"           }                                                                                                    "},
                  {"                                                                                                                "},
                  {"           if (_entity.State == EntityState.New)                                                                "},
                  {"           {                                                                                                    "},
               })

            .WriteTemplate(table.Columns.Where(c => !c.Detail.IsGenerated && !c.Detail.IsNullable), new OptionLines()
               {
                  {"                if (!_entity.Changes.Any(c => c.ColumnName == \"$0\"))                                          "},
                  {"                {                                                                                               "},
                  {"                    failures.Add(new RequirementFailure(\"$0\", \"Required value not set\", default(string)));  "},
                  {"                }                                                                                               "},
               }, c => new object[] {c.Detail.Name})

            .WriteLines(new OptionLines()
               {
                  {"            }                                                                                                   "},
                  {"                                                                                                                "},
                  {"            HasExtendedRequirementsMet(failures);                                                               "},
                  {"                                                                                                                "},
                  {"            return failures;                                                                                    "},
                  {"        }                                                                                                       "},
               });
      }

      protected void WriteClassClose()
      {
         _writer.WriteLine("    }");
      }

      protected void WriteNamespaceClose()
      {
         _writer.WriteLine("}");
      }
   }
}
