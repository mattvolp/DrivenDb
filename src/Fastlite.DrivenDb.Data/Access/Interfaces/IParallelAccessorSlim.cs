using System.Collections.Generic;
using Fastlite.DrivenDb.Core.Contracts.Interfaces;

namespace Fastlite.DrivenDb.Data.Access.Interfaces
{
   public interface IParallelAccessorSlim
   {
      IEnumerable<T> ReadEntities<T>(string query, params object[] parameters)
        where T : IDbRecord, new();
   }
}