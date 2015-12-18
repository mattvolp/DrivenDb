using DrivenDb.Core.Extensions;
using DrivenDb.Scripting.Internal.Interfaces;

namespace DrivenDb.Scripting.Internal.Writers
{
   internal class CsPropertyChangingWriter
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
                  {"        public event PropertyChangingEventHandler PropertyChanging = delegate {};             "},
                  {"                                                                                              "},
                  {"        protected virtual void OnPropertyChanging([CallerMemberName] string property = null)  "},
                  {"        {                                                                                     "},
                  {"            PropertyChanging(this, new PropertyChangingEventArgs(property));                  "},
                  {"        }                                                                                     "},
               })
            .Ignore();
      }      
   }
}
