using System;
using System.Collections.Generic;
using System.Linq;
using DrivenDb.Data;
using DrivenDb.Data.Internal;

namespace DrivenDb.Scripting.Internal
{
   internal static class OptionWriterExtensions
   {
      //
      // HELPER 
      //

      public delegate OptionWriter TableMapWriterContinuation(OptionWriter writer, TableMap table);
      public delegate OptionWriter TableDetailWriterContinuation(OptionWriter writer, TableDetail table);
      public delegate OptionWriter ColumnMapsWriterContinuation(OptionWriter writer, IEnumerable<ColumnMap> column);
      public delegate OptionWriter OptionWriterContinuation(OptionWriter writer);

      public static OptionWriter PenUpIf(this OptionWriter writer, bool condition)
      {
         return condition
            ? new OptionWriter(writer._options, writer._writer, true)
            : writer;
      }

      public static OptionWriter PenDownIf(this OptionWriter writer, bool condition)
      {
         return condition
            ? new OptionWriter(writer._options, writer._writer, false)
            : writer;
      }

      public static OptionWriter PenDownAnd(this OptionWriter writer, bool condition)
      {
         return !writer._penUp 
            ? new OptionWriter(writer._options, writer._writer, !condition)
            : writer;
      }

      public static OptionWriter PenDown(this OptionWriter writer)
      {
         return new OptionWriter(writer._options, writer._writer, false);
      }
      
      public static OptionWriter WriteSection(this OptionWriter writer, TableMapWriterContinuation continuation, TableMap table, ScriptingOptions options)
      {
         if (writer._options.HasFlag(options))
            writer.WriteSection(continuation, table);
            
         return writer;
      }

      public static OptionWriter WriteSection(this OptionWriter writer, TableDetailWriterContinuation continuation, TableDetail table, ScriptingOptions options)
      {
         if (writer._options.HasFlag(options))
            writer.WriteSection(continuation, table);

         return writer;
      }

      public static OptionWriter WriteSection(this OptionWriter writer, ColumnMapsWriterContinuation continuation, IEnumerable<ColumnMap> columns, ScriptingOptions options)
      {
         if (writer._options.HasFlag(options))
            writer.WriteSection(continuation, columns);

         return writer;
      }
      
      public static OptionWriter WriteSection(this OptionWriter writer, TableMapWriterContinuation continuation, TableMap table)
      {
         continuation(writer, table);

         return writer;
      }

      public static OptionWriter WriteSection(this OptionWriter writer, TableDetailWriterContinuation continuation, TableDetail table)
      {
         continuation(writer, table);

         return writer;
      }

      public static OptionWriter WriteSection(this OptionWriter writer, ColumnMapsWriterContinuation continuation, IEnumerable<ColumnMap> columns)
      {
         continuation(writer, columns);
         
         return writer;
      }

      public static OptionWriter WriteSection(this OptionWriter writer, OptionWriterContinuation continuation, ScriptingOptions options)
      {
         if (writer._options.HasFlag(options))
            continuation(writer);

         return writer;
      }

      public static OptionWriter WriteSection(this OptionWriter writer, OptionWriterContinuation continuation)
      {
         continuation(writer);

         return writer;
      }

      //
      // BEHAVIOR
      //

      public static OptionWriter WriteNamespaceOpen(this OptionWriter writer, string @namespace)
      {
         return writer.WriteLines(new OptionLines()
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

      public static OptionWriter WriteContext(this OptionWriter writer, string contextName, IEnumerable<TableDetail> tables)
      {
         return writer
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
               }, t => new[] { t.Name })

            .WriteLines(new OptionLines()
               {
                  {"    }                                                                                         "},
               });
      }

      public static OptionWriter ScriptEntity(this OptionWriter writer, TableMap table, ScriptingOptions options)
      {
         return writer
            .WriteSection(WriteKeyClass, table, ScriptingOptions.ImplementPrimaryKey)
            .WriteSection(WriteClassOpen, table.Detail)
            .WriteSection(WriteConstructor, table)
            .WriteSection(WriteFields, table.Columns)
            .WriteSection(WriteKeyProperty, table, ScriptingOptions.ImplementPrimaryKey)
            .WriteSection(WriteProperties, table.Columns, options)
            .WriteSection(WritePartials, table.Columns)
            .WriteSection(WritePropertyChanging, ScriptingOptions.ImplementNotifyPropertyChanging)
            .WriteSection(WritePropertyChanged, ScriptingOptions.ImplementNotifyPropertyChanged)
            .WriteSection(WriteValidationCheck, table, ScriptingOptions.ImplementValidationCheck)
            .WriteSection(WriteClassClose);
      }

      public static OptionWriter WriteKeyClass(this OptionWriter writer, TableMap table)
      {
         var primaries = table.GetPrimaryKeyColumns()
            .ToArray();

         primaries.GuardAgainstKeyClassOverflow();

         if (primaries.Any())
         {
            writer
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

         return writer;
      }

      public static OptionWriter WriteClassOpen(this OptionWriter writer, TableDetail table)
      {
         return writer.WriteLines(new OptionLines()
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
            , writer._options.ScriptAsDelimitedImpliedInterfaces()
            );
      }

      public static OptionWriter WriteConstructor(this OptionWriter writer, TableMap table)
      {
         var defaults = table.GetColumnsWithDefaultDefinitions()
            .ToArray();
         
         return writer
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
                    d.Detail.Name
                  , d.ScriptAsCsType()
                  //, d.ScriptAsDefaultValue(writer._options)
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

      public static OptionWriter WriteFields(this OptionWriter writer, IEnumerable<ColumnMap> columns)
      {
         foreach (var column in columns)
         {
            writer.WriteField(column);
         }

         return writer;
      }

      public static OptionWriter WriteField(this OptionWriter writer, ColumnMap column)
      {
         return writer.WriteLines(new OptionLines()
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

      public static OptionWriter WritePartials(this OptionWriter writer, IEnumerable<ColumnMap> columns)
      {
         foreach (var column in columns)
         {
            writer.WritePartial(column);
         }

         return writer;
      }

      public static OptionWriter WritePartial(this OptionWriter writer, ColumnMap column)
      {
         return writer.WriteLines(new OptionLines()
            {
               {"                                                                ", ScriptingOptions.ImplementPartialPropertyChanges},
               {"        partial void On$0Changing(ref $1 value);                ", ScriptingOptions.ImplementPartialPropertyChanges},
               {"        partial void On$0Changed();                             ", ScriptingOptions.ImplementPartialPropertyChanges},
            }
            , column.Detail.Name
            , column.ScriptAsCsType());
      }

      public static OptionWriter WriteKeyProperty(this OptionWriter writer, TableMap table)
      {
         var primaries = table.GetPrimaryKeyColumns()
            .ToArray();

         if (primaries.Length > 0)
         {
            writer.WriteLines(new OptionLines()
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

         return writer;
      }

      public static OptionWriter WriteProperties(this OptionWriter writer, IEnumerable<ColumnMap> columns)
      {
         foreach (var column in columns)
         {
            writer.WriteProperty(column); 
         }

         return writer;
      }

      public static OptionWriter WriteProperty(this OptionWriter writer, ColumnMap column) 
      {
         var csType = column.ScriptAsCsType();

         return writer.WriteLines(new OptionLines()
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
            }, csType, column.Detail.Name)

            .PenDownIf(csType == "DateTime" || csType == "DateTime?")
            .WriteLines(new OptionLines()
               {
                  {"                    if (!Equals(value, null))             ", ScriptingOptions.UnspecifiedDateTimes},
                  {"                    {                                     ", ScriptingOptions.UnspecifiedDateTimes},
               })
            .WriteLine(
               csType == "DateTime?"
                  ? "                        value = new DateTime(value.Value.Ticks, DateTimeKind.Unspecified);"
                  : "                        value = new DateTime(value.Ticks, DateTimeKind.Unspecified);      ", ScriptingOptions.UnspecifiedDateTimes)
                  
            .PenDownAnd(column.Detail.SqlType.IsDateOnly() && writer._options.HasFlag(ScriptingOptions.TruncateTimeForDateColumns))
            .WriteLine(
               writer._options.HasFlag(ScriptingOptions.UnspecifiedDateTimes)
                  ? "                        value = new DateTime(value.Date.Ticks, DateTimeKind.Unspecified);"
                  : "                        value = value.Date;")

            .PenDownIf(csType == "DateTime" || csType == "DateTime?")
            .WriteLines(new OptionLines()
               {
                  {"                    }                                        ", ScriptingOptions.UnspecifiedDateTimes},
                  {"                                                             ", ScriptingOptions.UnspecifiedDateTimes},
               })

            .PenDown()
            .WriteLines(new OptionLines()
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

      public static OptionWriter WritePropertyChanging(this OptionWriter writer)
      {
         return writer.WriteLines(new OptionLines()
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

      public static OptionWriter WritePropertyChanged(this OptionWriter writer)
      {
         return writer.WriteLines(new OptionLines()
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

      public static OptionWriter WriteValidationCheck(this OptionWriter writer, TableMap table) //, ScriptingOptions options)
      {
         if (!writer._options.HasFlag(ScriptingOptions.ImplementStateTracking))
         {
            throw new Exception("Cannot implement validation check without state tracking enabled");
         }

         /*
          * state == current -> true
          * state == deleted -> true
          * state == updated -> if all non-nullable/non-generated string columns have a value then true
          * state == new -> if all non-nullable/non-generated columns have a change recorded
          */
         return writer
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
               }, c => new[] { c.Detail.Name })

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
               }, c => new[] { c.Detail.Name })

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

      public static OptionWriter WriteClassClose(this OptionWriter writer)
      {
         return writer.WriteLine("    }");
      }

      public static OptionWriter WriteNamespaceClose(this OptionWriter writer)
      {
         return writer.WriteLine("}");
      }
   }
}
