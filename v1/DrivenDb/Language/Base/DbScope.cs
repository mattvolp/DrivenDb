using System.Collections.Generic;
using System.Data;

namespace DrivenDb.Base
{
   internal class DbScope : IDbScope
   {
      protected readonly IDbConnection m_Connection;
      protected readonly DbAccessor m_Accessor;
      protected readonly IDbTransaction m_Transaction;

      internal DbScope(IDb db, DbAccessor accessor)
      {
         m_Connection = db.CreateConnection();
         m_Accessor = accessor;

         m_Connection.Open();
         m_Transaction = m_Connection.BeginTransaction();
      }

      public void WriteEntity<T>(T entity)
         where T : IDbEntity
      {
         m_Accessor.TransactEntity(m_Connection, m_Transaction, entity, true);
      }

      public void WriteEntity<T>(T entity, bool returnId)
         where T : IDbEntity
      {
         m_Accessor.TransactEntity(m_Connection, m_Transaction, entity, returnId);
      }

      public void WriteEntities<T>(IEnumerable<T> entities)
         where T : IDbEntity
      {
         m_Accessor.TransactEntities(m_Connection, m_Transaction, entities, true);
      }

      public void WriteEntities<T>(IEnumerable<T> entities, bool returnIds)
         where T : IDbEntity
      {
         m_Accessor.TransactEntities(m_Connection, m_Transaction, entities, returnIds);
      }

      public void Execute(string query, params object[] parameters)
      {
         m_Accessor.Execute(m_Connection, m_Transaction, query, parameters);
      }

      public void Commit()
      {
         m_Transaction.Commit();
      }

      public void Dispose()
      {
         m_Transaction.Dispose();
         m_Connection.Dispose();
      }
   }
}