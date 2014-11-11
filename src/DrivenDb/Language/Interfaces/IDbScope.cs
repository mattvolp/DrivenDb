using System;
using System.Collections.Generic;

namespace DrivenDb
{
   public interface IDbScope : IDisposable
   {
      void WriteEntity<T>(T entity)
         where T : IDbEntity;

      void WriteEntities<T>(IEnumerable<T> entities)
         where T : IDbEntity;

      void Commit();
      void Execute(string query, params object[] parameters);
   }
}