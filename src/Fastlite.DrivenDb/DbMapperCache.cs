using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Fastlite.DrivenDb
{
   internal sealed class DbMapperCache : IDbMapperLoader
   {
      private readonly ConcurrentDictionary<string, object> _cached = new ConcurrentDictionary<string, object>();
      private readonly IDbMapperLoader _loader;

      public DbMapperCache(IDbMapperLoader loader)
      {
         if (loader == null) 
            throw new ArgumentNullException("loader");

         _loader = loader;
      }

      public IDbMapper<T> Load<T>(DbRecordList<T> records)
      {
         if (!records.Any())
         {
            throw new InvalidOperationException("Empty record list cannot be cached");
         }

         var key = DbSignature.Create(records[0]);

         return (IDbMapper<T>) _cached.GetOrAdd(key, (sql) => (object) _loader.Load<T>(records));
      }
   }
}
