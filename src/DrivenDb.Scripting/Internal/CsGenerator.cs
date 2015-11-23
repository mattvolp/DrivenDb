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
          _writer.WriteLine("");
          _writer.WriteLine("namespace $0", @namespace);
          _writer.WriteLine("{");
          _writer.WriteLine("    using DrivenDb.Core;");
          _writer.WriteLine("    using System;");
          _writer.WriteLine("    using System.Linq;");
          _writer.WriteLine("    using System.Collections.Generic;");
          _writer.WriteLine("    using System.Runtime.CompilerServices;");

          if (_options.HasFlag(ScriptingOptions.ImplementNotifyPropertyChanged) || _options.HasFlag(ScriptingOptions.ImplementNotifyPropertyChanging))
          {
             _writer.WriteLine("    using System.ComponentModel;");
          }

          _writer.WriteLine("    using System.Data.Linq;"              , ScriptingOptions.ImplementLinqContext);
          _writer.WriteLine("    using System.Data.Linq.Mapping;"      , ScriptingOptions.ImplementLinqContext);
          _writer.WriteLine("    using System.Runtime.Serialization;"  , ScriptingOptions.Serializable);
       }

       protected void WriteContext(string contextName, IEnumerable<TableMap> tables)
       {
          if (_options.HasFlag(ScriptingOptions.ImplementLinqContext))
          {
             _writer.WriteLine("");
             _writer.WriteLine("    public class $0 : DataContext", contextName);
             _writer.WriteLine("    {");
             _writer.WriteLine("        private static readonly MappingSource _mappingSource = new AttributeMappingSource();");
             _writer.WriteLine("");
             _writer.WriteLine("        public $0() : base(\"_\", _mappingSource)", contextName);
             _writer.WriteLine("        {");
             _writer.WriteLine("        }");             

             foreach (var table in tables)
             {
                _writer.WriteLine("");
                _writer.WriteLine("        public Table<$0> $0", table.Detail.Name);
                _writer.WriteLine("        {");
                _writer.WriteLine("            get { return this.GetTable<$0>(); }", table.Detail.Name);
                _writer.WriteLine("        }");                
             }

             _writer.WriteLine("    }");
          }
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

          _writer.WriteLine("");
          _writer.WriteLine("    public class $0Key"        , table.Detail.Name);
          _writer.WriteLine("        : Tuple<$0>"           , types);
          _writer.WriteLine("    {");
          _writer.WriteLine("        public $0Key($1)"      , table.Detail.Name, typedParameters);
          _writer.WriteLine("            : base($0)"        , baseParameters);
          _writer.WriteLine("        {");

          foreach (var column in primaries)
          {
             _writer.WriteLine("            $0 = @$1;", column.Detail.Name, column.Detail.Name.ToLower());
          }

          _writer.WriteLine("        }");
          _writer.WriteLine("");

          foreach (var column in primaries)
          {
             _writer.WriteLine("        public readonly $0 $1;", column.Detail.SqlType.ToCsString(), column.Detail.Name);             
          }

          _writer.WriteLine("    }");
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
             ? String.Format(" : {0}", String.Join(", ", interfaces))
             : "";

          _writer.WriteLine("");
          _writer.WriteLine("    [DataContract]"                                                , ScriptingOptions.Serializable);
          _writer.WriteLine("    [Table(Name = \"$0.$1\")]"                                     , ScriptingOptions.ImplementLinqContext, table.Detail.Schema, table.Detail.Name);
          _writer.WriteLine("    [DbTable(Schema=\"$0\", Name=\"$1\")]"                         , table.Detail.Schema, table.Detail.Name);
          _writer.WriteLine("    public partial class $0 $1"                                    , table.Detail.Name, inheritance);
          _writer.WriteLine("    {");
          _writer.WriteLine("        [DataMember]"                                              , ScriptingOptions.Serializable | ScriptingOptions.ImplementStateTracking);
          _writer.WriteLine("        private readonly DbEntity _entity = new DbEntity();", ScriptingOptions.ImplementStateTracking);
       }

       protected void WriteConstructor(TableMap table)
       {
          if (_options.HasFlag(ScriptingOptions.ImplementColumnDefaults) && table.Columns.Any(c => c.Detail.HasDefault))
          {
             _writer.WriteLine("");
             _writer.WriteLine("        public $0()", table.Detail.Name);
             _writer.WriteLine("        {");
             
             foreach (var column in table.Columns.Where(c => c.Detail.HasDefault))
             {
                var defaultValue = column.Detail.SqlType.ToCsDefault(_options, column.Detail);
                var csType = column.HasCustomType
                   ? column.CustomType
                   : column.Detail.SqlType.ToCsString();

                _writer.WriteLine("            _$0 = ($1) $2;"                                  , column.Detail.Name, csType, defaultValue);
                _writer.WriteLine("            _entity.Change($0, _$0);"                        , ScriptingOptions.ImplementStateTracking, column.Detail.Name);
             }

             _writer.WriteLine("        }");
          }

          _writer.WriteLine(""                                                                  , ScriptingOptions.ImplementStateTracking);
          _writer.WriteLine("        public IDbEntity Entity"                                   , ScriptingOptions.ImplementStateTracking);
          _writer.WriteLine("        {"                                                         , ScriptingOptions.ImplementStateTracking);
          _writer.WriteLine("            get { return _entity; }"                               , ScriptingOptions.ImplementStateTracking);
          _writer.WriteLine("        }"                                                         , ScriptingOptions.ImplementStateTracking);
       }

       protected void WriteField(ColumnMap column)
       {
          var csType = column.HasCustomType
             ? column.CustomType
             : column.Detail.SqlType.ToCsString();

          var isPrimary = column.Detail.IsPrimary ? "true" : "false";
          var isGenerated = column.Detail.IsGenerated ? "true" : "false";
          
          _writer.WriteLine("");
          _writer.WriteLine("        [DataMember]"                                             , ScriptingOptions.Serializable);
          _writer.WriteLine("        [DbColumn(Name=\"$0\", IsPrimary=$1, IsGenerated=$2)]"    , column.Detail.Name, isPrimary, isGenerated);
          _writer.WriteLine("        private $0 _$1;"                                          , csType, column.Detail.Name);
       }

       protected void WritePartial(ColumnMap column)
       {
          var csType = column.Detail.SqlType.ToCsString();

          _writer.WriteLine(""                                                                 , ScriptingOptions.ImplementPartialPropertyChanges);
          _writer.WriteLine("        partial void On$0Changing(ref $1 value);"                 , ScriptingOptions.ImplementPartialPropertyChanges, column.Detail.Name, csType);
          _writer.WriteLine("        partial void On$0Changed();"                              , ScriptingOptions.ImplementPartialPropertyChanges, column.Detail.Name);
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

          var fields = String.Join(", ", primaries.Select(p => "_" + p.Detail.Name));

          _writer.WriteLine("");
          _writer.WriteLine("        public $0Key PrimaryKey", table.Detail.Name);
          _writer.WriteLine("        {");
          _writer.WriteLine("            get { return new $0Key($1); }", table.Detail.Name, fields);
          _writer.WriteLine("        }");          
       }

       protected void WriteProperty(ColumnMap column)
       {
          var csType = column.HasCustomType
             ? column.CustomType
             : column.Detail.SqlType.ToCsString();
          
          _writer.WriteLine("");
          _writer.WriteLine("        [Column]"                                                 , ScriptingOptions.ImplementLinqContext);
          _writer.WriteLine("        public $0 $1"                                             , csType, column.Detail.Name);
          _writer.WriteLine("        {");
          _writer.WriteLine("            get { return _$0; }"                                  , column.Detail.Name);
          _writer.WriteLine("            set");
          _writer.WriteLine("            {");
          _writer.WriteLine("                if (_$0 != value)"                                , ScriptingOptions.MinimizePropertyChanges, column.Detail.Name);
          _writer.WriteLine("                {"                                                , ScriptingOptions.MinimizePropertyChanges);

          if (csType == "DateTime" || csType == "DateTime?")
          {
             _writer.WriteLine("                    if (!Equals(value, null))"                 , ScriptingOptions.UnspecifiedDateTimes);
             _writer.WriteLine("                    {"                                         , ScriptingOptions.UnspecifiedDateTimes);
             
             if (csType == "DateTime?")
             {
                _writer.WriteLine("                        value = new DateTime(value.Value.Ticks, DateTimeKind.Unspecified);", ScriptingOptions.UnspecifiedDateTimes);
             }
             else
             {
                _writer.WriteLine("                        value = new DateTime(value.Ticks, DateTimeKind.Unspecified);", ScriptingOptions.UnspecifiedDateTimes);
             }
             
             if (column.Detail.SqlType.IsDateOnly() && _options.HasFlag(ScriptingOptions.TruncateTimeForDateColumns))
             {
                if (_options.HasFlag(ScriptingOptions.UnspecifiedDateTimes))
                   _writer.WriteLine("                        value = new DateTime(value.Date.Ticks, DateTimeKind.Unspecified);");
                else
                   _writer.WriteLine("                        value = value.Date;");
             }

             _writer.WriteLine("                    }"                                         , ScriptingOptions.UnspecifiedDateTimes);
             _writer.WriteLine(""                                                              , ScriptingOptions.UnspecifiedDateTimes);
          }

          _writer.WriteLine("                    On$0Changing(ref value);"                     , ScriptingOptions.ImplementPartialPropertyChanges, column.Detail.Name);
          _writer.WriteLine("                    OnPropertyChanging();"                        , ScriptingOptions.ImplementNotifyPropertyChanging);
          _writer.WriteLine("                    _$0 = value;"                                 , column.Detail.Name);
          _writer.WriteLine("                    _entity.Change(\"$0\", value);"               , ScriptingOptions.ImplementStateTracking, column.Detail.Name);
          _writer.WriteLine("                    OnPropertyChanged();"                         , ScriptingOptions.ImplementNotifyPropertyChanged);
          _writer.WriteLine("                    On$0Changed();"                               , ScriptingOptions.ImplementPartialPropertyChanges, column.Detail.Name);
          _writer.WriteLine("                }"                                                , ScriptingOptions.MinimizePropertyChanges, column.Detail.Name);
          _writer.WriteLine("            }");
          _writer.WriteLine("        }");
       }

       protected void WritePropertyChanging()
       {
          _writer.WriteLine("");
          _writer.WriteLine("        public event PropertyChangingEventHandler PropertyChanging = delegate {};");
          _writer.WriteLine("");
          _writer.WriteLine("        protected virtual void OnPropertyChanging([CallerMemberName] string property = null)");
          _writer.WriteLine("        {");
          _writer.WriteLine("            PropertyChanging(this, new PropertyChangingEventArgs(property));");
          _writer.WriteLine("        }");
       }

       protected void WritePropertyChanged()
       {
          _writer.WriteLine("");
          _writer.WriteLine("        public event PropertyChangedEventHandler PropertyChanged = delegate {};");
          _writer.WriteLine("");
          _writer.WriteLine("        protected virtual void OnPropertyChanged([CallerMemberName] string property = null)");
          _writer.WriteLine("        {");
          _writer.WriteLine("            PropertyChanged(this, new PropertyChangedEventArgs(property));");
          _writer.WriteLine("        }");
       }

       // TODO: consider testing custom mapped types in validation check
       private void WriteValidationCheck(TableMap table)
       {
          if (!_options.HasFlag(ScriptingOptions.ImplementStateTracking))
          {
             throw new Exception("Cannot implement validation check without state tracking enabled");
          }

          _writer.WriteLine("");
          _writer.WriteLine("        partial void HasExtendedRequirementsMet(IList<RequirementFailure> failures);");
          _writer.WriteLine("");
          _writer.WriteLine("        public IEnumerable<RequirementFailure> GetRequirementsFailures()");
          _writer.WriteLine("        {");
          _writer.WriteLine("            var failures = new List<RequirementFailure>();");

          /*
           * state == current -> true
           * state == deleted -> true
           * state == updated -> if all non-nullable/non-generated string columns have a value then true
           * state == new -> if all non-nullable/non-generated columns have a change recorded
           */
          _writer.WriteLine("            if (_entity.State == EntityState.Current || _entity.State == EntityState.Deleted)");
          _writer.WriteLine("            {");
          _writer.WriteLine("                return failures;");
          _writer.WriteLine("            }");

          _writer.WriteLine("            if (_entity.State == EntityState.Updated || _entity.State == EntityState.New)");
          _writer.WriteLine("            {");

          foreach (var column in table.Columns.Where(c => !c.Detail.IsGenerated && !c.Detail.IsNullable && c.Detail.SqlType.ToCsString() == "string"))
          {
             _writer.WriteLine("                if (_$0 == default(string))", column.Detail.Name);
             _writer.WriteLine("                {");
             _writer.WriteLine("                    failures.Add(new RequirementFailure(\"$0\", \"Null value not allowed\", default(string)));", column.Detail.Name);
             _writer.WriteLine("                }");
          }

          _writer.WriteLine("            }");

          _writer.WriteLine("            if (_entity.State == EntityState.New)");
          _writer.WriteLine("            {");

          foreach (var column in table.Columns.Where(c => !c.Detail.IsGenerated && !c.Detail.IsNullable))
          {
             _writer.WriteLine("                if (!_entity.Changes.Any(c => c.ColumnName == \"$0\"))", column.Detail.Name);
             _writer.WriteLine("                {");
             _writer.WriteLine("                    failures.Add(new RequirementFailure(\"$0\", \"Required value not set\", default(string)));", column.Detail.Name);
             _writer.WriteLine("                }");
          }

          _writer.WriteLine("            }");

          _writer.WriteLine("            HasExtendedRequirementsMet(failures);");
          _writer.WriteLine("");
          _writer.WriteLine("            return failures;");
          _writer.WriteLine("        }");
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
