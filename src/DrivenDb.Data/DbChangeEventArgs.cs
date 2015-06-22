using System;
using System.Collections.Generic;

namespace DrivenDb.Data
{
   // TODO: test
   public class DbChangeEventArgs : EventArgs
   {
      public DbChangeEventArgs(IEnumerable<DbChange> changes)
      {
         Changes = changes;
      }

      public IEnumerable<DbChange> Changes
      {
         get;
         private set;
      }
   }
}
