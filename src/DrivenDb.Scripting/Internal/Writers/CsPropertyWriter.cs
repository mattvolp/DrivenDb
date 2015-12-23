using DrivenDb.Core.Extensions;
using DrivenDb.Data.Internal;
using DrivenDb.Scripting.Internal.Interfaces;

namespace DrivenDb.Scripting.Internal.Writers
{
   internal class CsPropertyWriter
      : IClassContentScripter
   {
      public TableTarget Write(TableTarget target)
      {
         foreach (var column in target)
         {
            column
               .Chain(OpenProperty)
               .Chain(ScriptDateTimeSpecificLogic)
               .Chain(CloseProperty);
         }

         return target;
      }

      private static void OpenProperty(ColumnTarget target)
      {         
         target.Writer
            .WriteLines(new ScriptLines()
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
               }, target.Column.ScriptAsCsType(), target.Column.Detail.Name)
            .Ignore();
      }

      private static void ScriptDateTimeSpecificLogic(ColumnTarget target)
      {
         var csType = target.Column.ScriptAsCsType();

         if (csType == "DateTime" || csType == "DateTime?")
         {
            target
               .Chain(OpenNullCheck)
               .Chain(ScriptUnspecifiedValueAssignment)
               .Chain(ScriptTimeCropAssignment)
               .Chain(CloseNullCheck)
               .Ignore();                        
         }
      }

      private static void OpenNullCheck(ColumnTarget target)
      {
         target.Writer
            .WriteLines(new ScriptLines()
               {
                  {"                    if (!Equals(value, null))             ", ScriptingOptions.UnspecifiedDateTimes},
                  {"                    {                                     ", ScriptingOptions.UnspecifiedDateTimes},
               })
            .Ignore();
      }

      private static void ScriptUnspecifiedValueAssignment(ColumnTarget target)
      {
         target.Writer
            .WriteLines(target.Column.ScriptAsCsType() == "DateTime?"
               ? new ScriptLines() {{"                        value = new DateTime(value.Value.Ticks, DateTimeKind.Unspecified);"}}
               : new ScriptLines() {{"                        value = new DateTime(value.Ticks, DateTimeKind.Unspecified);      ", ScriptingOptions.UnspecifiedDateTimes}})
            .Ignore();
      }

      private static void ScriptTimeCropAssignment(ColumnTarget target)
      {
         if (target.Column.Detail.SqlType.IsDateOnly() && target.Target.Options.HasFlag(ScriptingOptions.TruncateTimeForDateColumns))
         {
            target.Writer
               .WriteLines(
                  target.Target.Options.HasFlag(ScriptingOptions.UnspecifiedDateTimes)
                     ? new ScriptLines() {"                        value = new DateTime(value.Date.Ticks, DateTimeKind.Unspecified).Date;"}
                     : new ScriptLines() {"                        value = value.Date;"})
               .Ignore();
         }
      }

      private static void CloseNullCheck(ColumnTarget target)
      {
         target.Writer
            .WriteLines(new ScriptLines()
               {
                  {"                    }                                        ", ScriptingOptions.UnspecifiedDateTimes},
                  {"                                                             ", ScriptingOptions.UnspecifiedDateTimes},
               })
            .Ignore();
      }
      
      private static void CloseProperty(ColumnTarget target)
      {
         target.Writer
            .WriteLines(new ScriptLines()
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
               }, target.Column.Detail.Name)
            .Ignore();
      }    
   }
}
