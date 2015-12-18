using DrivenDb.Core.Extensions;
using DrivenDb.Scripting.Internal.Interfaces;

namespace DrivenDb.Scripting.Internal.Writers
{
   internal class CsPropertyChangedWriter
      : ITableWriter
   {
      public TableTarget Write(TableTarget target)
      {
         return target.Chain(WriteHandler);
      }
      
      public void WriteHandler(TableTarget target)
      {
         target.Writer
            .WriteLines(new ScriptLines()
               {
                  {"                                                                                              "},
                  {"        public event PropertyChangedEventHandler PropertyChanged = delegate {};               "},
                  {"                                                                                              "},
                  {"        protected virtual void OnPropertyChanged([CallerMemberName] string property = null)   "},
                  {"        {                                                                                     "},
                  {"            PropertyChanged(this, new PropertyChangedEventArgs(property));                    "},
                  {"        }                                                                                     "},
               })
            .Ignore();
      }      
   }
}
