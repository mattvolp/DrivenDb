using Fastlite.DrivenDb.Data.Access;
using Fastlite.DrivenDb.Data.Access.Interfaces;

namespace Fastlite.DrivenDb.Data.Tests.Base.Infrastructure
{
   public sealed class DbAccessorBuilder
   {
      private readonly IDbAccessorFactory _accessorFactory;
      private readonly string _database;
      private AccessorOptions _accessorOptions;

      public DbAccessorBuilder(string database, IDbAccessorFactory accessorFactory)
      {
         _database = database;
         _accessorFactory = accessorFactory;
         _accessorOptions = AccessorOptions.None;
      }

      public DbAccessorBuilder WithAllExtensions()
      {
         return WithExtensions(AccessorOptions.All);
      }

      public DbAccessorBuilder WithNoExtensions()
      {
         return WithExtensions(AccessorOptions.All);
      }

      public DbAccessorBuilder WithExtensions(AccessorOptions options)
      {
         _accessorOptions = options;

         return this;
      }

      public IDbAccessor Build()
      {
         return _accessorFactory.Create(_database, _accessorOptions);
      }
   }
}
