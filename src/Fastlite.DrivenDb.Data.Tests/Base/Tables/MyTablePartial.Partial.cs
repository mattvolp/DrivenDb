
using System.ComponentModel;

namespace Fastlite.DrivenDb.Data.Tests.Base.Tables
{
   internal partial class MyTablePartial : INotifyPropertyChanged
   {
      public MyTablePartial()
      {
         UnrelatedProperty = "yo";
      }

      public string UnrelatedProperty
      {
         get;
         private set;
      }
   }
}
