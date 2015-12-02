module CsScriptingServices

open System
open System.IO
open DataStructures
open DbTypes

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
   |> WriteTemplate defaults (fun c -> [c.Detail.Name; (ScriptAsCsType c); (ScriptAsDefaultValue c target.Options)]) [
      Optional("            _$0 = ($1) $2;                                      ", ScriptingOptions.ImplementColumnDefaults)
      Optional("            _entity.Change($0, _$0);                            ", ScriptingOptions.ImplementColumnDefaults ||| ScriptingOptions.ImplementStateTracking)
      ]
   |> WriteLines [] [
      Required("        }                                                       ")
      Optional("                                                                ", ScriptingOptions.ImplementStateTracking)
      Optional("        public IDbEntity Entity                                 ", ScriptingOptions.ImplementStateTracking)
      Optional("        {                                                       ", ScriptingOptions.ImplementStateTracking)
      Optional("            get { return _entity; }                             ", ScriptingOptions.ImplementStateTracking)
      Optional("        }                                                       ", ScriptingOptions.ImplementStateTracking)
      ]
   

let WriteField (target : ScriptTarget) (column : ColumnMap) : unit =         
   target |> WriteLines [
      column.Detail.Name 
      (ScriptAsCsBoolean column.Detail.IsPrimary)
      (ScriptAsCsBoolean column.Detail.IsGenerated)
      (ScriptAsCsType column)
      ] [
      Required("                                                                ")
      Optional("        [DataMember]                                            ", ScriptingOptions.Serializable)
      Required("        [DbColumn(Name=\"$0\", IsPrimary=$1, IsGenerated=$2)]   ")
      Required("        private $3 _$0;                                         ")
      ]  |> ignore


let WriteFields (target : ScriptTarget) (columns : ColumnMap array) : ScriptTarget =
      columns |> Array.iter (WriteField target)
      target 

let WritePartial (target : ScriptTarget) (column : ColumnMap) : unit = //ScriptTarget =   
   target |> WriteLines [column.Detail.Name; (ScriptAsCsType column)] [
      Optional("                                                                ", ScriptingOptions.ImplementPartialPropertyChanges)
      Optional("        partial void On$0Changing(ref $1 value);                ", ScriptingOptions.ImplementPartialPropertyChanges)
      Optional("        partial void On$0Changed();                             ", ScriptingOptions.ImplementPartialPropertyChanges)
      ] |> ignore
      

let WritePartials (columns : ColumnMap list)  (target : ScriptTarget) : ScriptTarget =   
   columns |> List.iter (WritePartial target) 
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

//let WriteProperty (column : ColumnMap) (target : ScriptTarget) : ScriptTarget =
//   let csType = (ScriptAsCsType column);
//
//   target |> WriteLines [csType; column.Detail.Name] [
//      Required("                                                                ")
//      Optional("        [Column]                                                ", ScriptingOptions.ImplementLinqContext)
//      Required("        public $0 $1                                            ")
//      Required("        {                                                       ")
//      Required("            get { return _$1; }                                 ")
//      Required("            set                                                 ")
//      Required("            {                                                   ")
//      Optional("                if (_$1 != value)                               ", ScriptingOptions.MinimizePropertyChanges)
//      Optional("                {                                               ", ScriptingOptions.MinimizePropertyChanges)
//      ] |> ignore
//
//   if (csType = "DateTime" || csType = "DateTime?") then
//      target |> WriteLines [] [
//         Optional("                    if (!Equals(value, null))             ", ScriptingOptions.UnspecifiedDateTimes)
//         Optional("                    {                                     ", ScriptingOptions.UnspecifiedDateTimes)
//         ] |> ignore
//
//      if (csType = "DateTime?") then
//         target |> WriteLines [] [
//            Required("                        value = new DateTime(value.Value.Ticks, DateTimeKind.Unspecified);")
//            ] |> ignore
//      else
//         target |> WriteLines [] [
//            Optional("                        value = new DateTime(value.Ticks, DateTimeKind.Unspecified);      ", ScriptingOptions.UnspecifiedDateTimes)
//            ] |> ignore
//
//         if ((IsDateOnly column.Detail.SqlType) && _options.HasFlag(ScriptingOptions.TruncateTimeForDateColumns)) then
//            if (target.Options.HasFlag(ScriptingOptions.UnspecifiedDateTimes)) then
//               target |> WriteLines [] [            
//                  Required("                        value = new DateTime(value.Date.Ticks, DateTimeKind.Unspecified);")
//                  ] |> ignore
//            else
//               target |> WriteLines [] [
//                  Required("                        value = value.Date;")
//                  ] |> ignore
//
//      target |> WriteLines [] [
//         Optional("                    }                                        ", ScriptingOptions.UnspecifiedDateTimes)
//         Optional("                                                             ", ScriptingOptions.UnspecifiedDateTimes)
//         ] |> ignore
//   
//   target |> WriteLines [column.Detail.Name] [
//      Optional("                    On$0Changing(ref value);                    ", ScriptingOptions.ImplementPartialPropertyChanges)
//      Optional("                    OnPropertyChanging();                       ", ScriptingOptions.ImplementNotifyPropertyChanging)
//      Required("                    _$0 = value;                                ")
//      Optional("                    _entity.Change(\"$0\", value);              ", ScriptingOptions.ImplementStateTracking)
//      Optional("                    OnPropertyChanged();                        ", ScriptingOptions.ImplementNotifyPropertyChanged)
//      Optional("                    On$0Changed();                              ", ScriptingOptions.ImplementPartialPropertyChanges)
//      Optional("                }                                               ", ScriptingOptions.MinimizePropertyChanges)
//      Required("            }                                                   ")
//      Required("        }                                                       ")
//      ]


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
