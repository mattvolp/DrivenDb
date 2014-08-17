using System.Collections.Generic;
using System.Data;
using Fastlite.DrivenDb.Core.Contracts.Interfaces;
using Fastlite.DrivenDb.Data.Access.Interfaces;

namespace Fastlite.DrivenDb.Data.Access.Base
{
   internal class DbScope : IDbScope
   {
      protected readonly IDbConnection _connection;
      protected readonly DbAccessor _accessor;
      protected readonly IDbTransaction _transaction;

      internal DbScope(IDb db, DbAccessor accessor)
      {
         _connection = db.CreateConnection();
         _accessor = accessor;

         _connection.Open();
         _transaction = _connection.BeginTransaction();
      }

      public void WriteEntity<T>(T entity)
         where T : IDbEntity
      {
         _accessor.TransactEntity(_connection, _transaction, entity, true);
      }

      public void WriteEntity<T>(T entity, bool returnId)
         where T : IDbEntity
      {
         _accessor.TransactEntity(_connection, _transaction, entity, returnId);
      }

      public void WriteEntities<T>(IEnumerable<T> entities)
         where T : IDbEntity
      {
         _accessor.TransactEntities(_connection, _transaction, entities, true);
      }

      public void WriteEntities<T>(IEnumerable<T> entities, bool returnIds)
         where T : IDbEntity
      {
         _accessor.TransactEntities(_connection, _transaction, entities, returnIds);
      }

      public void Execute(string query, params object[] parameters)
      {
         _accessor.Execute(_connection, _transaction, query, parameters);
      }

      public void Commit()
      {
         _transaction.Commit();
      }

      public void Dispose()
      {
         _transaction.Dispose();
         _connection.Dispose();
      }
   }
}