using System;
using System.Linq;
using Fastlite.DrivenDb.Core._2._0.Framework;
using Fastlite.DrivenDb.Data._2_0;

namespace Fastlite.DrivenDb.Data._2._0
{
   internal sealed class DbReaderBuilder : IDbReaderBuilder
   {
      private readonly MapperStore _mappers;
      private readonly DbQuery _query;

      public DbReaderBuilder(MapperStore mappers, DbQuery query)
      {
         if (mappers == null)
            throw new ArgumentNullException("mappers");

         if (query == null)
            throw new ArgumentNullException("query");

         _mappers = mappers;
         _query = query;
      }

      public DbResultCollection<T> As<T>()
      {
         var mapper = _mappers.Get<T>(_query);
         var recordset = _query.Execute<T>();
         
         mapper.Map<T>(recordset);
         
         var entities = recordset.Records
            .Select(r => r.Entity)
            .ToList();

         return new DbResultCollection<T>(entities);
      }
   }
}   