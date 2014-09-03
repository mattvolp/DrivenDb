using System;
using System.Data;

namespace Fastlite.DrivenDb
{
   internal sealed class DbConnectionFactory : IDbConnectionFactory
   {
      private readonly Func<IDbConnection> _factory;

      public DbConnectionFactory(Func<IDbConnection> factory)
      {
         if (factory == null)
            throw new ArgumentNullException("factory");

         _factory = factory;
      }

      public IDbConnection Create()
      {
         return _factory();
      }
   }
}
