using System.Collections.Generic;

namespace DrivenDb
{
   public interface IDbAggregator
   {
      void WriteAggregate(IDbAccessor accessor, IDbAggregate aggregate);
      void WriteAggregates(IDbAccessor accessor, IEnumerable<IDbAggregate> aggregates);
   }
}