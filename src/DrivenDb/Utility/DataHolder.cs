using System.Data;

namespace DrivenDb.Utility
{
   class DataHolder<T>
   {
      public IDataRecord DataRecord;
      public T Entity;
   }
}
