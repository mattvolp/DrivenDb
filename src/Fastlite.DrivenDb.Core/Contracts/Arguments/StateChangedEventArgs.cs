using System;

namespace Fastlite.DrivenDb.Core.Contracts.Arguments
{
   public class StateChangedEventArgs : EventArgs
   {      
      public StateChangedEventArgs(EntityState previous, EntityState current)
      {
         Previous = previous;
         Current = current;
      }

      public readonly EntityState Previous;
      public readonly EntityState Current;
   }
}
