using System;
using System.Data.SQLite;
using Fastlite.DrivenDb.Data.Access;
using Fastlite.DrivenDb.Data.Access.Interfaces;
using Fastlite.DrivenDb.Data.Tests.Base.Infrastructure;

namespace Fastlite.DrivenDb.Data.Tests.SqLite.Infrastructure
{
   internal sealed class SqLiteAccessorFactory : IDbAccessorFactory
   {
      public IDbAccessor Create(string database, AccessorExtension extensions)
      {
         return DbFactory.CreateAccessor(
            DbAccessorType.SqLite, extensions,
            () => new SQLiteConnection(String.Format("Data Source={0};Version=3;New=True", database))
            );
      }
   }
}
