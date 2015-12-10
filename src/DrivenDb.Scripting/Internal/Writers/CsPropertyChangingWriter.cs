namespace DrivenDb.Scripting.Internal.Writers
{
   internal class CsPropertyChangingWriter
   {
      public void Write(ScriptTarget target)
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
   }
}
