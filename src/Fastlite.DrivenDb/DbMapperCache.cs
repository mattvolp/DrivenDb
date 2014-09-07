using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Fastlite.DrivenDb
{
   internal sealed class DbMapperCache : IDbMapperLoader
   {
      private readonly ConcurrentDictionary<string, IDbMapper> _cached = new ConcurrentDictionary<string, IDbMapper>();
      private readonly IDbMapperLoader _loader;

      public DbMapperCache(IDbMapperLoader loader)
      {
         if (loader == null) 
            throw new ArgumentNullException("loader");

         _loader = loader;
      }

      public IDbMapper Load<T>(DbRecordSet<T> recordset)
      {
         if (recordset.Records.Any())
         {
            throw new InvalidOperationException("Empty record list cannot be cached");
         }

         var key = DbSignature.Create(recordset.Records[0]);

         return _cached.GetOrAdd(key, (sql) => _loader.Load<T>(recordset));
      }
   }
}
