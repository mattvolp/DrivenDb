module CsScriptingServices

open System
open System.IO

//
// GLOBAL
//

[<Flags>]
type ScriptingOptions = 
   | None = 0
   | ImplementNotifyPropertyChanging = 1
   | ImplementNotifyPropertyChanged = 2
   | ImplementPartialPropertyChanges = 4
   | ImplementLinqContext = 8
   | ImplementPrimaryKey = 16
   | ImplementColumnDefaults = 32
   | MinimizePropertyChanges = 64
   | Serializable = 128
   | ImplementStateTracking = 256
   | UnspecifiedDateTimes = 512
   | TruncateTimeForDateColumns = 1024
   | ImplementValidationCheck = 2048   
//  |   All = ImplementNotifyPropertyChanging
//            | ImplementNotifyPropertyChanged
//            | ImplementPartialPropertyChanges
//            | ImplementLinqContext
//            | ImplementPrimaryKey
//            | ImplementColumnDefaults
//            | MinimizePropertyChanges
//            | Serializable
//            | ImplementStateTracking
//            | UnspecifiedDateTimes
//            | TruncateTimeForDateColumns
//            | ImplementValidationCheck

type ColumnDetail = 
   {
      SqlType : string // DbType TODO
      Name : string
      IsNullable : bool
      IsPrimary : bool
      IsGenerated : bool
      IsReadonly : bool
      HasDefault : bool
      DefaultValue : string
      ColumnPosition : int
   }

type ColumnMap = 
   {
      Detail : ColumnDetail
      CustomType : string
   }

type TableDetail =
   {
      Schema : string
      Name : string
      Columns : ColumnDetail list
   }

type TableMap = 
   {
      Detail : TableDetail
      Columns : ColumnMap list
   }

//
// data services
//

let GetRequiredStringColumns (table : TableMap) : ColumnMap list =
   []

let GetRequiredColumns (table : TableMap) : ColumnMap list =
   []

let GetPrimaryKeyColumns (table : TableMap) : ColumnMap list =
   []

let GetColumnsWithDefaultDefinitions  (table : TableMap) : ColumnMap list =
   []

//
// SCRIPT WRITING SERVICES
//

type RequiredLine = (string)
type OptionalLine = (string * ScriptingOptions)
type MultiOptionalLine = (string * ScriptingOptions list)

type ScriptLine =    
   | Required of RequiredLine
   | Optional of OptionalLine
   | Multiple of MultiOptionalLine
   
type ScriptTarget = 
   {
      Writer : TextWriter
      Options : ScriptingOptions
   }

let WriteLines (args : string list) (lines : ScriptLine list) (target : ScriptTarget) : ScriptTarget = 
   // write stuff
   target

type ParamExtractor<'a> = 'a -> string list

let WriteTemplate (items : 'a list) (extractor : ParamExtractor<'a>) (lines : ScriptLine list) (target : ScriptTarget) : ScriptTarget =
   // write stuff
   target

//
// SCRIPT BUILDING HELPERS
//

let GuardAgainstOptionNotSet (check : ScriptingOptions) (message : string) (target : ScriptTarget) : ScriptTarget =
   if not (target.Options.HasFlag check) then
      failwith message
   target

//
// SCRIPT BUILDING SERVICES
//

let ScriptNamespaceOpen (namspace : string) (target : ScriptTarget) : ScriptTarget =   
   target |> WriteLines [namspace] [            
      Required("                                            ")
      Required("namespace $0                                ")
      Required("{                                           ")
      Required("    using DrivenDb.Core;                    ")
      Required("    using System;                           ")
      Required("    using System.Linq;                      ")
      Required("    using System.Collections.Generic;       ")
      Required("    using System.Runtime.CompilerServices;  ")
      Optional("    using System.Data.Linq;                 ", ScriptingOptions.ImplementLinqContext)
      Optional("    using System.Data.Linq.Mapping;         ", ScriptingOptions.ImplementLinqContext)
      Optional("    using System.Runtime.Serialization;     ", ScriptingOptions.Serializable)
      Multiple("    using System.ComponentModel;            ", [ ScriptingOptions.ImplementNotifyPropertyChanged 
                                                                 ScriptingOptions.ImplementNotifyPropertyChanging ])
      ]

let ScriptContext (contextName : string) (tables : TableMap list) (target : ScriptTarget) : ScriptTarget =      
   target |> WriteLines [contextName] [ 
      Required("                                                                                              ")
      Required("    public class $0 : DataContext                                                             ")
      Required("    {                                                                                         ")
      Required("        private static readonly MappingSource _mappingSource = new AttributeMappingSource();  ")
      Required("                                                                                              ")
      Required("        public $0() : base(\"_\", _mappingSource)                                             ")
      Required("        {                                                                                     ")
      Required("        }                                                                                     ")
      ]         
   |> WriteTemplate tables (fun t -> [t.Detail.Name]) [
      Required("                                                                                              ")
      Required("        public Table<$0> $0                                                                   ")
      Required("        {                                                                                     ")
      Required("            get { return this.GetTable<$0>(); }                                               ")
      Required("        }                                                                                     ")
      ]         
   |> WriteLines [] [
      Required("    }                                                                                         ")
      ]

//protected void WriteKeyClass(TableMap table)
//      {
//         var primaries = table.GetPrimaryKeyColumns()
//            .ToArray();
//            
//         primaries.GuardAgainstKeyClassOverflow();
//         
//         if (primaries.Any())
//         {
//            _writer
//               .WriteLines(new OptionLines()
//                  {
//                     {"                                                          "},
//                     {"    public class $0Key                                    "},
//                     {"        : Tuple<$1>                                       "},
//                     {"    {                                                     "},
//                     {"        public $0Key($2)                                  "},
//                     {"            : base($3)                                    "},
//                     {"        {                                                 "},
//                  }
//                  , table.Detail.Name
//                  , primaries.ScriptAsDelimitedCsTypes()
//                  , primaries.ScriptAsDelimitedCsTypedParameterNames()
//                  , primaries.ScriptAsDelimitedParameterNames())
//
//               .WriteTemplate(primaries, new OptionLines()
//                  {
//                     {"            $0 = $1;                                     "},
//                  }, p => new[]
//                     {
//                        p.Detail.Name, p.ScriptAsParameterName()
//                     })
//
//               .WriteLines(new OptionLines()
//                  {
//                     {"        }                                                 "},
//                     {"                                                          "},
//                  })
//
//               .WriteTemplate(primaries, new OptionLines()
//                  {
//                     {"        public readonly $0 $1;                            "},
//                  }, p => new[]
//                     {
//                        p.ScriptAsCsType(), p.Detail.Name
//                     })
//
//               .WriteLines(new OptionLines()
//                  {
//                     {"   }                                                      "},
//                  });
//         }
//      }
//      
//      protected void WriteClassOpen(TableMap table)
//      {
//         _writer.WriteLines(new OptionLines()
//            {
//               {"                                                                   "},
//               {"    [DataContract]                                                 ", ScriptingOptions.Serializable},
//               {"    [Table(Name = \"$0.$1\")]                                      ", ScriptingOptions.ImplementLinqContext},
//               {"    [DbTable(Schema=\"$0\", Name=\"$1\")]                          "},
//               {"    public partial class $1 $2                                     "},
//               {"    {                                                              "},
//               {"        [DataMember]                                               ", ScriptingOptions.Serializable | ScriptingOptions.ImplementStateTracking},
//               {"        private readonly DbEntity _entity = new DbEntity();        ", ScriptingOptions.ImplementStateTracking},
//            }
//            , table.Detail.Schema
//            , table.Detail.Name
//            , _options.ScriptAsDelimitedImpliedInterfaces()
//            );
//      }

let WriteConstructor (table : TableMap) (target : ScriptTarget) : ScriptTarget =         
   let defaults = (GetColumnsWithDefaultDefinitions table)
            
   target |> WriteLines [table.Detail.Name] [            
      Required("                                                                ")
      Required("        public $0()                                             ")
      Required("        {                                                       ")
      ]
   |> WriteTemplate defaults (fun c -> [d.Detail.Name, d.ScriptAsCsType(), d.ScriptAsDefaultValue(_options)]) [
      Optional("            _$0 = ($1) $2;                                      ", ScriptingOptions.ImplementColumnDefaults)
      Optional("            _entity.Change($0, _$0);                            ", ScriptingOptions.ImplementColumnDefaults | ScriptingOptions.ImplementStateTracking)
      ]
   |> WriteLines [] [
      Required("        }                                                       ")
      Optional("                                                                ", ScriptingOptions.ImplementStateTracking)
      Optional("        public IDbEntity Entity                                 ", ScriptingOptions.ImplementStateTracking)
      Optional("        {                                                       ", ScriptingOptions.ImplementStateTracking)
      Optional("            get { return _entity; }                             ", ScriptingOptions.ImplementStateTracking)
      Optional("        }                                                       ", ScriptingOptions.ImplementStateTracking)
      ]
   

let WriteField (column : ColumnMap) (target : ScriptTarget) : ScriptTarget =         
   target |> WriteLines [
      column.Detail.Name 
      column.Detail.IsPrimary.ScriptAsCsBoolean()
      column.Detail.IsGenerated.ScriptAsCsBoolean()
      column.ScriptAsCsType()
      ] [
      Required("                                                                ")
      Optional("        [DataMember]                                            ", ScriptingOptions.Serializable)
      Required("        [DbColumn(Name=\"$0\", IsPrimary=$1, IsGenerated=$2)]   ")
      Required("        private $3 _$0;                                         ")
      ]       


//protected void WriteFields(IEnumerable<ColumnMap> columns)
//      {
//         foreach (var column in columns)
//         {
//            WriteField(column);
//         }
//      }

let WritePartial (target : ScriptTarget) (column : ColumnMap) : ScriptTarget =   
   target |> WriteLines [column.Detail.Name (*; column.ScriptAsCsType()*)] [
      Optional("                                                                ", ScriptingOptions.ImplementPartialPropertyChanges)
      Optional("        partial void On$0Changing(ref $1 value);                ", ScriptingOptions.ImplementPartialPropertyChanges)
      Optional("        partial void On$0Changed();                             ", ScriptingOptions.ImplementPartialPropertyChanges)
      ]
      

let WritePartials (columns : ColumnMap list)  (target : ScriptTarget) : ScriptTarget =   
   columns |> List.forall (WritePartial target) |> ignore
   target


let WriteKeyProperty (table : TableMap) (target : ScriptTarget) : ScriptTarget =
   let primaries = (GetPrimaryKeyColumns table)
            
   if (primaries.Length > 0) then
      target |> WriteLines [table.Detail.Name (*; primaries.ScriptAsDelimitedPrivateMemberNames() *)] [
         Required("                                                             ")
         Required("        public $0Key PrimaryKey                              ")
         Required("        {                                                    ")
         Required("            get { return new $0Key($1); }                    ")
         Required("        }                                                    ")
         ]
   else
      target
 

//private void WriteProperties(IEnumerable<ColumnMap> columns)
//      {
//         foreach (var column in columns)
//         {
//            WriteProperty(column);
//         }
//      }      

//protected void WriteProperty(ColumnMap column)
//      {
//         var csType = column.ScriptAsCsType();
//
//         _writer.WriteLines(new OptionLines()
//            {
//               {""},
//               {"        [Column]                                                ", ScriptingOptions.ImplementLinqContext},
//               {"        public $0 $1                                            "},
//               {"        {                                                       "},
//               {"            get { return _$1; }                                 "},
//               {"            set                                                 "},
//               {"            {                                                   "},
//               {"                if (_$1 != value)                               ", ScriptingOptions.MinimizePropertyChanges},
//               {"                {                                               ", ScriptingOptions.MinimizePropertyChanges},
//            }, csType, column.Detail.Name);
//
//         if (csType == "DateTime" || csType == "DateTime?")
//         {
//            _writer
//               .WriteLines(new OptionLines()
//                  {
//                     {"                    if (!Equals(value, null))             ", ScriptingOptions.UnspecifiedDateTimes},
//                     {"                    {                                     ", ScriptingOptions.UnspecifiedDateTimes},
//                  })
//
//               .WriteLine(
//                  csType == "DateTime?"
//                     ? "                        value = new DateTime(value.Value.Ticks, DateTimeKind.Unspecified);"
//                     : "                        value = new DateTime(value.Ticks, DateTimeKind.Unspecified);      ", ScriptingOptions.UnspecifiedDateTimes);
//
//            if (column.Detail.SqlType.IsDateOnly() && _options.HasFlag(ScriptingOptions.TruncateTimeForDateColumns))
//            {
//               _writer.WriteLine(
//                  _options.HasFlag(ScriptingOptions.UnspecifiedDateTimes)
//                     ? "                        value = new DateTime(value.Date.Ticks, DateTimeKind.Unspecified);"
//                     : "                        value = value.Date;");
//            }
//
//            _writer.WriteLines(new OptionLines()
//               {
//                  {"                    }                                        ", ScriptingOptions.UnspecifiedDateTimes},
//                  {"                                                             ", ScriptingOptions.UnspecifiedDateTimes},
//               });
//         }
//
//         _writer.WriteLines(new OptionLines()
//            {
//               {"                    On$0Changing(ref value);                    ", ScriptingOptions.ImplementPartialPropertyChanges},
//               {"                    OnPropertyChanging();                       ", ScriptingOptions.ImplementNotifyPropertyChanging},
//               {"                    _$0 = value;                                "},
//               {"                    _entity.Change(\"$0\", value);              ", ScriptingOptions.ImplementStateTracking},
//               {"                    OnPropertyChanged();                        ", ScriptingOptions.ImplementNotifyPropertyChanged},
//               {"                    On$0Changed();                              ", ScriptingOptions.ImplementPartialPropertyChanges},
//               {"                }                                               ", ScriptingOptions.MinimizePropertyChanges},
//               {"            }                                                   "},
//               {"        }                                                       "},
//            }, column.Detail.Name);
//      }

let WritePropertyChanging (target : ScriptTarget) : ScriptTarget =
   target |> WriteLines [] [
      Required("                                                                                              ")
      Required("        public event PropertyChangingEventHandler PropertyChanging = delegate {};             ")
      Required("                                                                                              ")
      Required("        protected virtual void OnPropertyChanging([CallerMemberName] string property = null)  ")
      Required("        {                                                                                     ")
      Required("            PropertyChanging(this, new PropertyChangingEventArgs(property));                  ")
      Required("        }                                                                                     ")
      ]
      

let WritePropertyChanged (target : ScriptTarget) : ScriptTarget =
   target |> WriteLines [] [
      Required("                                                                                              ")
      Required("        public event PropertyChangedEventHandler PropertyChanged = delegate {};               ")
      Required("                                                                                              ")
      Required("        protected virtual void OnPropertyChanged([CallerMemberName] string property = null)   ")
      Required("        {                                                                                     ")
      Required("            PropertyChanged(this, new PropertyChangedEventArgs(property));                    ")
      Required("        }                                                                                     ")
      ]


let WriteValidationCheck (table : TableMap) (target : ScriptTarget) : ScriptTarget =
   (*
   * state == current -> true
   * state == deleted -> true
   * state == updated -> if all non-nullable/non-generated string columns have a value then true
   * state == new -> if all non-nullable/non-generated columns have a change recorded
   *)
   target |> GuardAgainstOptionNotSet ScriptingOptions.ImplementStateTracking
               "Cannot implement validation check without state tracking enabled"   
   |> WriteLines [] [
      Required("        partial void HasExtendedRequirementsMet(IList<RequirementFailure> failures);                            ")
      Required("                                                                                                                ")
      Required("        public IEnumerable<RequirementFailure> GetRequirementsFailures()                                        ")
      Required("        {                                                                                                       ")
      Required("            var failures = new List<RequirementFailure>();                                                      ")
      Required("                                                                                                                ")
      Required("            if (_entity.State == EntityState.Current || _entity.State == EntityState.Deleted)                   ")
      Required("            {                                                                                                   ")
      Required("                return failures;                                                                                ")
      Required("            }                                                                                                   ")
      Required("                                                                                                                ")
      Required("            if (_entity.State == EntityState.Updated || _entity.State == EntityState.New)                       ")
      Required("            {                                                                                                   ")
      ]
   |> WriteTemplate (GetRequiredStringColumns table) (fun c -> [c.Detail.Name]) [
      Required("                if (_$0 == default(string))                                                                     ")
      Required("                {                                                                                               ")
      Required("                    failures.Add(new RequirementFailure(\"$0\", \"Null value not allowed\", default(string)));  ")
      Required("                }                                                                                               ")
      ]
   |> WriteLines [] [
      Required("            }                                                                                                   ")
      Required("                                                                                                                ")
      Required("            if (_entity.State == EntityState.New)                                                               ")
      Required("            {                                                                                                   ")
      ]
   |> WriteTemplate (GetRequiredColumns table) (fun c -> [c.Detail.Name]) [
      Required("                if (!_entity.Changes.Any(c => c.ColumnName == \"$0\"))                                          ")
      Required("                {                                                                                               ")
      Required("                    failures.Add(new RequirementFailure(\"$0\", \"Required value not set\", default(string)));  ")
      Required("                }                                                                                               ")
      ]
   |> WriteLines [] [
      Required("            }                                                                                                   ")
      Required("                                                                                                                ")
      Required("            HasExtendedRequirementsMet(failures);                                                               ")
      Required("                                                                                                                ")
      Required("            return failures;                                                                                    ")
      Required("        }                                                                                                       ")
      ]
      

let WriteClassClose (target : ScriptTarget) : ScriptTarget =
   target |> WriteLines [] [
      Required("    }")
      ]
      

let WriteNamespaceClose (target : ScriptTarget) : ScriptTarget =      
   target |> WriteLines [] [
      Required("}")
      ]
