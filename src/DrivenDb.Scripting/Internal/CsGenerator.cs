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

      public void Write(string @namespace, string contextName, TableMap[] tables)
      {
         WriteNamespaceOpen(@namespace);

         if (_options.HasFlag(ScriptingOptions.ImplementLinqContext))
            WriteContext(contextName, tables);

         foreach (var table in tables)
         {
            ScriptEntity(table);
         }

         WriteNamespaceClose();
      }

      private void WriteNamespaceOpen(string @namespace)
      {
         _writer.WriteLines(new OptionLines()
            {
               {"                                            "},
               {"namespace $0                                "},
               {"{                                           "},
               {"    using DrivenDb.Core;                    "},
               {"    using System;                           "},
               {"    using System.Linq;                      "},
               {"    using System.Collections.Generic;       "},
               {"    using System.Runtime.CompilerServices;  "},
               {"    using System.Data.Linq;                 ", ScriptingOptions.ImplementLinqContext},
               {"    using System.Data.Linq.Mapping;         ", ScriptingOptions.ImplementLinqContext},
               {"    using System.Runtime.Serialization;     ", ScriptingOptions.Serializable},
               {"    using System.ComponentModel;            ", ScriptingOptions.ImplementNotifyPropertyChanged, ScriptingOptions.ImplementNotifyPropertyChanging},
            }, @namespace);
      }

      private void WriteContext(string contextName, IEnumerable<TableMap> tables)
      {
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
               }, t => new [] {t.Detail.Name})

            .WriteLines(new OptionLines()
               {
                  {"    }                                                                                         "},
               });
      }

      private void ScriptEntity(TableMap table)
      {
         //_writer.WriteSection(table, WriteKeyClass, ScriptingOptions.ImplementPrimaryKey);

         if (_options.HasFlag(ScriptingOptions.ImplementPrimaryKey))
            WriteKeyClass(table);

         WriteClassOpen(table);
         WriteConstructor(table);
         WriteFields(table.Columns);

         //_writer.WriteSection(table, WriteKeyProperty, ScriptingOptions.ImplementPrimaryKey);
         if (_options.HasFlag(ScriptingOptions.ImplementPrimaryKey))
            WriteKeyProperty(table);

         WriteProperties(table.Columns);
         WritePartials(table.Columns);
         
         if (_options.HasFlag(ScriptingOptions.ImplementNotifyPropertyChanging))
            WritePropertyChanging();
         
         if (_options.HasFlag(ScriptingOptions.ImplementNotifyPropertyChanged))
            WritePropertyChanged();
         
         if (_options.HasFlag(ScriptingOptions.ImplementValidationCheck))
            WriteValidationCheck(table);

         WriteClassClose();
      }
      
      protected void WriteKeyClass(TableMap table)
      {
         var primaries = table.GetPrimaryKeyColumns()
            .ToArray();
            
         primaries.GuardAgainstKeyClassOverflow();
         
         if (primaries.Any())
         {
            _writer
               .WriteLines(new OptionLines()
                  {
                     {"                                                          "},
                     {"    public class $0Key                                    "},
                     {"        : Tuple<$1>                                       "},
                     {"    {                                                     "},
                     {"        public $0Key($2)                                  "},
                     {"            : base($3)                                    "},
                     {"        {                                                 "},
                  }
                  , table.Detail.Name
                  , primaries.ScriptAsDelimitedCsTypes()
                  , primaries.ScriptAsDelimitedCsTypedParameterNames()
                  , primaries.ScriptAsDelimitedParameterNames())

               .WriteTemplate(primaries, new OptionLines()
                  {
                     {"            $0 = $1;                                     "},
                  }, p => new[]
                     {
                        p.Detail.Name, p.ScriptAsParameterName()
                     })

               .WriteLines(new OptionLines()
                  {
                     {"        }                                                 "},
                     {"                                                          "},
                  })

               .WriteTemplate(primaries, new OptionLines()
                  {
                     {"        public readonly $0 $1;                            "},
                  }, p => new[]
                     {
                        p.ScriptAsCsType(), p.Detail.Name
                     })

               .WriteLines(new OptionLines()
                  {
                     {"   }                                                      "},
                  });
         }
      }
      
      protected void WriteClassOpen(TableMap table)
      {
         _writer.WriteLines(new OptionLines()
            {
               {"                                                                   "},
               {"    [DataContract]                                                 ", ScriptingOptions.Serializable},
               {"    [Table(Name = \"$0.$1\")]                                      ", ScriptingOptions.ImplementLinqContext},
               {"    [DbTable(Schema=\"$0\", Name=\"$1\")]                          "},
               {"    public partial class $1 $2                                     "},
               {"    {                                                              "},
               {"        [DataMember]                                               ", ScriptingOptions.Serializable | ScriptingOptions.ImplementStateTracking},
               {"        private readonly DbEntity _entity = new DbEntity();        ", ScriptingOptions.ImplementStateTracking},
            }
            , table.Detail.Schema
            , table.Detail.Name
            , _options.ScriptAsDelimitedImpliedInterfaces()
            );
      }

      protected void WriteConstructor(TableMap table)
      {
         var defaults = table.GetColumnsWithDefaults()
            .ToArray();

         _writer
            .WriteLines(new OptionLines()
               {
                  {"                                                                "},
                  {"        public $0()                                             "},
                  {"        {                                                       "},
               }, table.Detail.Name)

            .WriteTemplate(defaults, new OptionLines()
               {
                  {"            _$0 = ($1) $2;                                      ", ScriptingOptions.ImplementColumnDefaults},
                  {"            _entity.Change($0, _$0);                            ", ScriptingOptions.ImplementColumnDefaults | ScriptingOptions.ImplementStateTracking},
               }, d => new []
                  {
                     d.Detail.Name, d.ScriptAsCsType(), d.ScriptAsDefaultValue(_options)
                  })

            .WriteLines(new OptionLines()
               {
                  {"        }                                                       "},
                  {"                                                                ", ScriptingOptions.ImplementStateTracking},
                  {"        public IDbEntity Entity                                 ", ScriptingOptions.ImplementStateTracking},
                  {"        {                                                       ", ScriptingOptions.ImplementStateTracking},
                  {"            get { return _entity; }                             ", ScriptingOptions.ImplementStateTracking},
                  {"        }                                                       ", ScriptingOptions.ImplementStateTracking},
               });
      }

      protected void WriteFields(IEnumerable<ColumnMap> columns)
      {
         foreach (var column in columns)
         {
            WriteField(column);
         }
      }

      protected void WriteField(ColumnMap column)
      {                  
         _writer.WriteLines(new OptionLines()
            {
               {"                                                                "},
               {"        [DataMember]                                            ", ScriptingOptions.Serializable},
               {"        [DbColumn(Name=\"$0\", IsPrimary=$1, IsGenerated=$2)]   "},
               {"        private $3 _$0;                                         "},
            }
         , column.Detail.Name
         , column.Detail.IsPrimary.ScriptAsCsBoolean()
         , column.Detail.IsGenerated.ScriptAsCsBoolean()
         , column.ScriptAsCsType());
      }

      private void WritePartials(IEnumerable<ColumnMap> columns)
      {
         foreach (var column in columns)
         {
            WritePartial(column);
         }
      }

      protected void WritePartial(ColumnMap column)
      {
         _writer.WriteLines(new OptionLines()
            {
               {"                                                                ", ScriptingOptions.ImplementPartialPropertyChanges},
               {"        partial void On$0Changing(ref $1 value);                ", ScriptingOptions.ImplementPartialPropertyChanges},
               {"        partial void On$0Changed();                             ", ScriptingOptions.ImplementPartialPropertyChanges},
            }
            , column.Detail.Name
            , column.ScriptAsCsType());
      }

      protected void WriteKeyProperty(TableMap table)
      {         
         var primaries = table.GetPrimaryKeyColumns()
            .ToArray();

         if (primaries.Length > 0)
         {
            _writer.WriteLines(new OptionLines()
               {
                  {"                                                             "},
                  {"        public $0Key PrimaryKey                              "},
                  {"        {                                                    "},
                  {"            get { return new $0Key($1); }                    "},
                  {"        }                                                    "},
               }
               , table.Detail.Name
               , primaries.ScriptAsDelimitedPrivateMemberNames()
               );
         }
      }

      private void WriteProperties(IEnumerable<ColumnMap> columns)
      {
         foreach (var column in columns)
         {
            WriteProperty(column);
         }
      }

      protected void WriteProperty(ColumnMap column)
      {
         var csType = column.ScriptAsCsType();

         _writer.WriteLines(new OptionLines()
            {
               {""},
               {"        [Column]                                                ", ScriptingOptions.ImplementLinqContext},
               {"        public $0 $1                                            "},
               {"        {                                                       "},
               {"            get { return _$1; }                                 "},
               {"            set                                                 "},
               {"            {                                                   "},
               {"                if (_$1 != value)                               ", ScriptingOptions.MinimizePropertyChanges},
               {"                {                                               ", ScriptingOptions.MinimizePropertyChanges},
            }, csType, column.Detail.Name);

         if (csType == "DateTime" || csType == "DateTime?")
         {
            _writer
               .WriteLines(new OptionLines()
                  {
                     {"                    if (!Equals(value, null))             ", ScriptingOptions.UnspecifiedDateTimes},
                     {"                    {                                     ", ScriptingOptions.UnspecifiedDateTimes},
                  })

               .WriteLine(
                  csType == "DateTime?"
                     ? "                        value = new DateTime(value.Value.Ticks, DateTimeKind.Unspecified);"
                     : "                        value = new DateTime(value.Ticks, DateTimeKind.Unspecified);      ", ScriptingOptions.UnspecifiedDateTimes);

            if (column.Detail.SqlType.IsDateOnly() && _options.HasFlag(ScriptingOptions.TruncateTimeForDateColumns))
            {
               _writer.WriteLine(
                  _options.HasFlag(ScriptingOptions.UnspecifiedDateTimes)
                     ? "                        value = new DateTime(value.Date.Ticks, DateTimeKind.Unspecified);"
                     : "                        value = value.Date;");
            }

            _writer.WriteLines(new OptionLines()
               {
                  {"                    }                                        ", ScriptingOptions.UnspecifiedDateTimes},
                  {"                                                             ", ScriptingOptions.UnspecifiedDateTimes},
               });
         }

         _writer.WriteLines(new OptionLines()
            {
               {"                    On$0Changing(ref value);                    ", ScriptingOptions.ImplementPartialPropertyChanges},
               {"                    OnPropertyChanging();                       ", ScriptingOptions.ImplementNotifyPropertyChanging},
               {"                    _$0 = value;                                "},
               {"                    _entity.Change(\"$0\", value);              ", ScriptingOptions.ImplementStateTracking},
               {"                    OnPropertyChanged();                        ", ScriptingOptions.ImplementNotifyPropertyChanged},
               {"                    On$0Changed();                              ", ScriptingOptions.ImplementPartialPropertyChanges},
               {"                }                                               ", ScriptingOptions.MinimizePropertyChanges},
               {"            }                                                   "},
               {"        }                                                       "},
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
      
      protected void WriteValidationCheck(TableMap table)
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

            .WriteTemplate(table.GetRequiredStringColumns(), new OptionLines()
               {
                  {"                if (_$0 == default(string))                                                                     "},
                  {"                {                                                                                               "},
                  {"                    failures.Add(new RequirementFailure(\"$0\", \"Null value not allowed\", default(string)));  "},
                  {"                }                                                                                               "},
               }, c => new [] {c.Detail.Name})

            .WriteLines(new OptionLines()
               {
                  {"            }                                                                                                   "},
                  {"                                                                                                                "},
                  {"            if (_entity.State == EntityState.New)                                                               "},
                  {"            {                                                                                                   "},
               })

            .WriteTemplate(table.GetRequiredColumns(), new OptionLines()
               {
                  {"                if (!_entity.Changes.Any(c => c.ColumnName == \"$0\"))                                          "},
                  {"                {                                                                                               "},
                  {"                    failures.Add(new RequirementFailure(\"$0\", \"Required value not set\", default(string)));  "},
                  {"                }                                                                                               "},
               }, c => new [] {c.Detail.Name})

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
