namespace DrivenDb.Scripting.Internal.Writers
{
   internal class CsPropertyChangedWriter
   {
      public void Write(ScriptTarget target)
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
   }
}
