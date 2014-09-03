using System;
using System.Data;

namespace Fastlite.DrivenDb
{
   public class DbAccessor2 : IDbAccessor2
   {
      private readonly DbAccessorType2 _accessorType;
      private readonly IDbConnectionFactory _connections;


      protected DbAccessor2(DbAccessorType2 accessorType, Func<IDbConnection> connections)
      {
         _accessorType = accessorType;
         _connections = new DbConnectionFactory(connections);
      }

      public IDbReaderBuilder Read(string sql, params object[] arguments)
      {
         var query = new DbQuery(_connections, sql, arguments);

         return new DbReaderBuilder(null, query);
      }
   }
}
