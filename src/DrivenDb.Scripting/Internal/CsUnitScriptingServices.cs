using System;
using System.Collections.Generic;
using System.Linq;
using DrivenDb.Data;
using DrivenDb.Data.Internal;

namespace DrivenDb.Scripting.Internal
{
   internal static class CsUnitScriptingServices
   {
      public static void WriteUnit(ScriptTarget target, string @namespace, string contextName, TableMap[] tables)
      {
         target.WriteNamespaceOpenAndContinue(@namespace)
            .WriteContextAndContinue(contextName, tables.GetDetails())
            .WriteEntitiesAndContinue(tables)
            .WriteNamespaceClose();     
      }

      public static void WriteNamespaceOpen(ScriptTarget target, string @namespace)
      {
         target.WriteLines(new ScriptLines()
            {
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

      public static void WriteContext(ScriptTarget target, string contextName, IEnumerable<TableDetail> tables)
      {
         target
            .WriteLinesAndContinue(new ScriptLines()
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

            .WriteTemplateAndContinue(tables, new ScriptLines()
               {
                  {"                                                                                              "},
                  {"        public Table<$0> $0                                                                   "},
                  {"        {                                                                                     "},
                  {"            get { return this.GetTable<$0>(); }                                               "},
                  {"        }                                                                                     "},
               }, t => new[] { t.Name })

            .WriteLines(new ScriptLines()
               {
                  {"    }                                                                                         "},
               });
      }

      public static void WriteEntities(ScriptTarget target, IEnumerable<TableMap> tables)
      {
         foreach (var table in tables)
         {
            WriteEntity(target, table);
         }
      }

      public static void WriteEntity(ScriptTarget target, TableMap table)
      {         
         target.WriteKeyClassAndContinue(table)
            .WriteClassOpenAndContinue(table)
            .WriteConstructorAndContinue(table)
            .WriteFieldsAndContinue(table)
            .WriteKeyPropertyAndContinue(table)
            .WritePropertiesAndContinue(table)
            .WritePartialsAndContinue(table)
            .WritePropertyChangingAndContinue(table)
            .WritePropertyChangedAndContinue(table)
            .WriteValidationCheckAndContinue(table)
            .WriteClassClose(table);

         //Tuple.Create(target, table)
         //   .WriteKeyClassAndContinue()
         //   .WriteClassOpenAndContinue()
         //   .WriteConstructorAndContinue()
         //   .WriteFieldsAndContinue()
         //   .WriteKeyPropertyAndContinue()
         //   .WritePropertiesAndContinue()
         //   .WritePartialsAndContinue()
         //   .WritePropertyChangingAndContinue()
         //   .WritePropertyChangedAndContinue()
         //   .WriteValidationCheckAndContinue()
         //   .WriteClassClose();
      }

      public static void WriteKeyClass(ScriptTarget target, TableMap table)
      {
         var primaries = table.GetPrimaryKeyColumns()
            .ToArray();

         primaries.GuardAgainstKeyClassOverflow();

         if (primaries.Any())
         {
            target
               .WriteLinesAndContinue(new ScriptLines()
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

               .WriteTemplateAndContinue(primaries, new ScriptLines()
                  {
                     {"            $0 = $1;                                     "},
                  }, p => new[]
                     {
                        p.Detail.Name, p.ScriptAsParameterName()
                     })

               .WriteLinesAndContinue(new ScriptLines()
                  {
                     {"        }                                                 "},
                     {"                                                          "},
                  })

               .WriteTemplateAndContinue(primaries, new ScriptLines()
                  {
                     {"        public readonly $0 $1;                            "},
                  }, p => new[]
                     {
                        p.ScriptAsCsType(), p.Detail.Name
                     })

               .WriteLines(new ScriptLines()
                  {
                     {"   }                                                      "},
                  });
         }         
      }

      public static void WriteClassOpen(ScriptTarget target, TableDetail table)
      {
         target.WriteLines(new ScriptLines()
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
            , table.Schema
            , table.Name
            , target.Options.ScriptAsDelimitedImpliedInterfaces()
            );
      }
      
      public static void WriteConstructor(ScriptTarget target, TableMap table)
      {
         var defaults = table.GetColumnsWithDefaultDefinitions()
            .ToArray();

         target
            .WriteLinesAndContinue(new ScriptLines()
               {
                  {"                                                                "},
                  {"        public $0()                                             "},
                  {"        {                                                       "},
               }, table.Detail.Name)

            .WriteTemplateAndContinue(defaults, new ScriptLines()
               {
                  {"            _$0 = ($1) $2;                                      ", ScriptingOptions.ImplementColumnDefaults},
                  {"            _entity.Change($0, _$0);                            ", ScriptingOptions.ImplementColumnDefaults | ScriptingOptions.ImplementStateTracking},
               }, d => new[]
                  {
                       d.Detail.Name
                     , d.ScriptAsCsType()
                     , CsValueScriptingServices.ToCsScriptedDefaultValue(target.Options, d.Detail) //d.ScriptAsDefaultValue(target.Options)
                  })

            .WriteLines(new ScriptLines()
               {
                  {"        }                                                       "},
                  {"                                                                ", ScriptingOptions.ImplementStateTracking},
                  {"        public IDbEntity Entity                                 ", ScriptingOptions.ImplementStateTracking},
                  {"        {                                                       ", ScriptingOptions.ImplementStateTracking},
                  {"            get { return _entity; }                             ", ScriptingOptions.ImplementStateTracking},
                  {"        }                                                       ", ScriptingOptions.ImplementStateTracking},
               });
      }
      
      public static void WriteFields(ScriptTarget target, IEnumerable<ColumnMap> columns)
      {
         foreach (var column in columns)
         {
            WriteField(target, column);
         }         
      }

      public static void WriteField(ScriptTarget target, ColumnMap column)
      {
         target.WriteLines(new ScriptLines()
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

      public static void WritePartials(ScriptTarget target, IEnumerable<ColumnMap> columns)
      {
         foreach (var column in columns)
         {
            WritePartial(target, column);
         }         
      }

      public static void WritePartial(ScriptTarget target, ColumnMap column)
      {
         target.WriteLines(new ScriptLines()
            {
               {"                                                                ", ScriptingOptions.ImplementPartialPropertyChanges},
               {"        partial void On$0Changing(ref $1 value);                ", ScriptingOptions.ImplementPartialPropertyChanges},
               {"        partial void On$0Changed();                             ", ScriptingOptions.ImplementPartialPropertyChanges},
            }
            , column.Detail.Name
            , column.ScriptAsCsType());
      }

      public static void WriteKeyProperty(ScriptTarget target, TableMap table)
      {
         var primaries = table.GetPrimaryKeyColumns()
            .ToArray();

         if (primaries.Length > 0)
         {
            target.WriteLines(new ScriptLines()
               {
                  {"                                                             "},
                  {"        public $0Key PrimaryKey                              "},
                  {"        {                                                    "},
                  {"            get { return new $0Key($1); }                    "},
                  {"        }                                                    "},
               }
               , table.Detail.Name
               , primaries.ScriptAsDelimitedPrivateMemberNames());
         }         
      }

      public static void WriteProperties(ScriptTarget target, IEnumerable<ColumnMap> columns)
      {
         foreach (var column in columns)
         {
            WriteProperty(target, column);
         }         
      }
      
      public static void WriteProperty(ScriptTarget target, ColumnMap column)
      {
         var csType = column.ScriptAsCsType();

         target.WriteLines(new ScriptLines()
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
            target.WriteLinesAndContinue(new ScriptLines()
               {
                  {"                    if (!Equals(value, null))             ", ScriptingOptions.UnspecifiedDateTimes},
                  {"                    {                                     ", ScriptingOptions.UnspecifiedDateTimes},
               })
               .WriteLine(
                  csType == "DateTime?"
                     ? "                        value = new DateTime(value.Value.Ticks, DateTimeKind.Unspecified);"
                     : "                        value = new DateTime(value.Ticks, DateTimeKind.Unspecified);      ", ScriptingOptions.UnspecifiedDateTimes);

            if (column.Detail.SqlType.IsDateOnly() && target.Options.HasFlag(ScriptingOptions.TruncateTimeForDateColumns))
            {
               target.WriteLine(
                  target.Options.HasFlag(ScriptingOptions.UnspecifiedDateTimes)
                     ? "                        value = new DateTime(value.Date.Ticks, DateTimeKind.Unspecified);"
                     : "                        value = value.Date;");
            }
            
            target.WriteLines(new ScriptLines()
               {
                  {"                    }                                        ", ScriptingOptions.UnspecifiedDateTimes},
                  {"                                                             ", ScriptingOptions.UnspecifiedDateTimes},
               });
         }

         target.WriteLines(new ScriptLines()
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

      public static void WritePropertyChanging(ScriptTarget target)
      {
         target.WriteLines(new ScriptLines()
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

      public static void WritePropertyChanged(ScriptTarget target)
      {
         target.WriteLines(new ScriptLines()
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
      
      public static void WriteValidationCheck(ScriptTarget target, TableMap table) //, ScriptingOptions options)
      {
         if (!target.Options.HasFlag(ScriptingOptions.ImplementStateTracking))
         {
            throw new Exception("Cannot implement validation check without state tracking enabled");
         }

         /*
          * state == current -> true
          * state == deleted -> true
          * state == updated -> if all non-nullable/non-generated string columns have a value then true
          * state == new -> if all non-nullable/non-generated columns have a change recorded
          */
         target
            .WriteLinesAndContinue(new ScriptLines()
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

            .WriteTemplateAndContinue(table.GetRequiredStringColumns(), new ScriptLines()
               {
                  {"                if (_$0 == default(string))                                                                     "},
                  {"                {                                                                                               "},
                  {"                    failures.Add(new RequirementFailure(\"$0\", \"Null value not allowed\", default(string)));  "},
                  {"                }                                                                                               "},
               }, (c) => new[] {c.Detail.Name})

            .WriteLinesAndContinue(new ScriptLines()
               {
                  {"            }                                                                                                   "},
                  {"                                                                                                                "},
                  {"            if (_entity.State == EntityState.New)                                                               "},
                  {"            {                                                                                                   "},
               })

            .WriteTemplateAndContinue(table.GetRequiredColumns(), new ScriptLines()
               {
                  {"                if (!_entity.Changes.Any(c => c.ColumnName == \"$0\"))                                          "},
                  {"                {                                                                                               "},
                  {"                    failures.Add(new RequirementFailure(\"$0\", \"Required value not set\", default(string)));  "},
                  {"                }                                                                                               "},
               }, (c) => new[] {c.Detail.Name})

            .WriteLines(new ScriptLines()
               {
                  {"            }                                                                                                   "},
                  {"                                                                                                                "},
                  {"            HasExtendedRequirementsMet(failures);                                                               "},
                  {"                                                                                                                "},
                  {"            return failures;                                                                                    "},
                  {"        }                                                                                                       "},
               });
      }
      
      public static void WriteClassClose(ScriptTarget target)
      {
         target.WriteLineAndContinue("    }");
      }

      public static void WriteNamespaceClose(ScriptTarget target)
      {
         target.WriteLine("}");
      }
   }
}
