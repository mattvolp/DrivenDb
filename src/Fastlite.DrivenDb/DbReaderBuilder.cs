using System;
using System.Linq;

namespace Fastlite.DrivenDb
{
   internal sealed class DbReaderBuilder : IDbReaderBuilder
   {
      private readonly IDbMapperLoader _mappers;
      private readonly DbQuery _query;

      public DbReaderBuilder(IDbMapperLoader mappers, DbQuery query)
      {
         if (mappers == null)
            throw new ArgumentNullException("mappers");

         if (query == null)
            throw new ArgumentNullException("query");

         _mappers = mappers;
         _query = query;
      }

      public DbResultList<T> As<T>()
         where T : new()
      {         
         var recordset = _query.Execute<T>();
         var mapper = _mappers.Load<T>(recordset.Records);
         
         mapper.Map(recordset.Records);
         
         var entities = recordset.Records
            .Select(r => r.Entity)
            .ToList();

         return new DbResultList<T>(entities);
      }
   }
}   