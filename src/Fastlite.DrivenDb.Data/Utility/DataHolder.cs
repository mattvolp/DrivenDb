using System.Data;

namespace Fastlite.DrivenDb.Data.Utility
{
   class DataHolder<T>
   {
      public IDataRecord DataRecord;
      public T Entity;
   }
}
