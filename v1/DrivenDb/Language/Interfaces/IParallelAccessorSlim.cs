using System.Collections.Generic;

namespace DrivenDb
{
   public interface IParallelAccessorSlim
   {
      IEnumerable<T> ReadEntities<T>(string query, params object[] parameters)
        where T : IDbRecord, new();
   }
}