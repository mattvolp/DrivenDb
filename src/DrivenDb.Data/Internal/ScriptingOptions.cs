using System;

namespace DrivenDb.Data.Internal
{
   [Flags]
   internal enum ScriptingOptions
   {
      None = 0,

      ImplementNotifyPropertyChanging = 1,
      ImplementNotifyPropertyChanged = 2,
      ImplementPartialPropertyChanges = 4,
      ImplementLinqContext = 8,
      ImplementPrimaryKey = 16,
      ImplementColumnDefaults = 32,
      MinimizePropertyChanges = 64,
      Serializable = 128,
      ImplementStateTracking = 256,
      UnspecifiedDateTimes = 512,
      TruncateTimeForDateColumns = 1024,
      ImplementValidationCheck = 2048,
      
      All = ImplementNotifyPropertyChanging
            | ImplementNotifyPropertyChanged
            | ImplementPartialPropertyChanges
            | ImplementLinqContext
            | ImplementPrimaryKey
            | ImplementColumnDefaults
            | MinimizePropertyChanges
            | Serializable
            | ImplementStateTracking
            | UnspecifiedDateTimes
            | TruncateTimeForDateColumns
            | ImplementValidationCheck
   }
}