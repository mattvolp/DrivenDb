using System;
using System.Data;

namespace Fastlite.DrivenDb
{
   public class DbAccessor : IDbAccessor
   {
      private readonly IDbMapperLoader _mappers;
      private readonly IDbConnectionFactory _connections;

      protected DbAccessor(IDbMapperLoader mappers, Func<IDbConnection> connections)
      {
         if (mappers == null) 
            throw new ArgumentNullException("mappers");

         if (connections == null) 
            throw new ArgumentNullException("connections");

         _mappers = mappers;
         _connections = new DbConnectionFactory(connections);
      }

      public IDbReaderBuilder Read(string sql, params object[] arguments)
      {
         var query = new DbQuery(_connections, sql, arguments);

         return new DbReaderBuilder(_mappers, query);
      }
   }
}
