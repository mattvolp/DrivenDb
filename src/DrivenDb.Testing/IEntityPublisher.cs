using System.Collections.Generic;
using DrivenDb.Core;

namespace DrivenDb.Testing
{
   internal interface IEntityPublisher
   {
      void Publish<T>(T item)
         where T : IDbEntityProvider;

      void Publish<T>(params T[] items)
         where T : IDbEntityProvider;

      void Publish<T>(IEnumerable<T> items)
         where T : IDbEntityProvider;
   }
}
