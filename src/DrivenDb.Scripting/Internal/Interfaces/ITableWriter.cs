using DrivenDb.Data;

namespace DrivenDb.Scripting.Internal.Interfaces
{
   internal interface ITableWriter
   {
      void Write(ScriptTarget target, TableMap table);
   }
}