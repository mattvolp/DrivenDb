using System.Collections.Generic;
using DrivenDb.Core;
using DrivenDb.Data.Internal;

namespace DrivenDb.Testing.Internal.SqLite
{
   internal class SqLiteFixture
      : EntityScripter      
      
   {
      private readonly IEntityPublisher _publisher;

      public SqLiteFixture()
      {
         var writer = new DbWriter();

         _publisher = new SqLitePublisher(writer);
      }

      public SqLiteFixture Publish<T>(T item) where T : IDbEntityProvider
      {
         _publisher.Publish(item);

         return this;
      }

      public SqLiteFixture Publish<T>(params T[] items) where T : IDbEntityProvider
      {
         _publisher.Publish(items);

         return this;
      }

      public SqLiteFixture Publish<T>(IEnumerable<T> items) where T : IDbEntityProvider
      {
         _publisher.Publish(items);

         return this;
      }
   }
}
