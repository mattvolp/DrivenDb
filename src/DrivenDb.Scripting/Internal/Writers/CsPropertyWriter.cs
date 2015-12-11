using DrivenDb.Data;
using DrivenDb.Data.Internal;
using DrivenDb.Scripting.Internal.Interfaces;

namespace DrivenDb.Scripting.Internal.Writers
{
   internal class CsPropertyWriter
      : ITableWriter
   {      
      public void Write(ScriptTarget target, TableMap table)
      {
         foreach (var column in table.Columns)
         {
            Write(target, column);
         }
      }

      public void Write(ScriptTarget target, ColumnMap column)
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
            target.WriteLines(new ScriptLines()
               {
                  {"                    if (!Equals(value, null))             ", ScriptingOptions.UnspecifiedDateTimes},
                  {"                    {                                     ", ScriptingOptions.UnspecifiedDateTimes},
               });

            if (csType == "DateTime?")
               target.WriteLines(new ScriptLines() {{"                        value = new DateTime(value.Value.Ticks, DateTimeKind.Unspecified);"}});
            else
               target.WriteLines(new ScriptLines() {{"                        value = new DateTime(value.Ticks, DateTimeKind.Unspecified);      ", ScriptingOptions.UnspecifiedDateTimes}});

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
   }
}
