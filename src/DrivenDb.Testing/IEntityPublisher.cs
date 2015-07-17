using System.Collections.Generic;

namespace DrivenDb.Testing
{
   internal interface IEntityPublisher
   {
      void Publish<T>(T item);
      void Publish<T>(params T[] items);
      void Publish<T>(IEnumerable<T> items);
   }
}
