using Fastlite.DrivenDb.Data.Access;
using Fastlite.DrivenDb.Data.Access.Interfaces;

namespace Fastlite.DrivenDb.Data.Tests.Base.Infrastructure
{
   public sealed class DbAccessorBuilder
   {
      private readonly IDbAccessorFactory _accessorFactory;
      private readonly string _database;
      private AccessorExtension _accessorExtensions;

      public DbAccessorBuilder(string database, IDbAccessorFactory accessorFactory)
      {
         _database = database;
         _accessorFactory = accessorFactory;
         _accessorExtensions = AccessorExtension.None;
      }

      public DbAccessorBuilder WithAllExtensions()
      {
         return WithExtensions(AccessorExtension.All);
      }

      public DbAccessorBuilder WithNoExtensions()
      {
         return WithExtensions(AccessorExtension.All);
      }

      public DbAccessorBuilder WithExtensions(AccessorExtension extensions)
      {
         _accessorExtensions = extensions;

         return this;
      }

      public IDbAccessor Build()
      {
         return _accessorFactory.Create(_database, _accessorExtensions);
      }
   }
}
