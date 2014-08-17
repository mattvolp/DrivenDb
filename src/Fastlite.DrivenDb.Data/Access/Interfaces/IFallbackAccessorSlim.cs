using System;
using System.Collections.Generic;
using Fastlite.DrivenDb.Core.Contracts.Interfaces;

namespace Fastlite.DrivenDb.Data.Access.Interfaces
{
   public interface IFallbackAccessorSlim
   {
      IEnumerable<T> ReadValues<T>(string query, params object[] parameters);

      T ReadValue<T>(string query, params object[] parameters);

      IEnumerable<T> ReadAnonymous<T>(T model, string query, params object[] parameters);

      IEnumerable<T> ReadType<T>(string query, params object[] parameters)
         where T : new();

      IEnumerable<T> ReadType<T>(Func<T> factory, string query, params object[] parameters);

      T ReadEntity<T>(string query, params object[] parameters)
         where T : IDbRecord, new();

      IEnumerable<T> ReadEntities<T>(string query, params object[] parameters)
         where T : IDbRecord, new();
   }
}