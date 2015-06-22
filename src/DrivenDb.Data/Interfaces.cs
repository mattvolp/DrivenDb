using System;
using System.Collections.Generic;
using System.Linq;
using DrivenDb.Core;

namespace DrivenDb.Data
{
   public interface IDbAccessor
      : IDbAccessorSlim
      , IDbExecutor
   {      
   }

   public interface IDbAccessorSlim
      : IDbMonitor
      , IDbWriter
      , IDbReader
   {
      IDbScope CreateScope();
   }

   // hold events for IDbScope until commit???
   // logs sql and parameters that were executed
   public interface IDbScope
      : IDbReader
      , IDbWriter
      , IDbExecutor
      , IDisposable
   {
   }

   public interface IDbReader
   {
      DbResult<T> ReadSingle<T>(string query, params object[] parameters);
      DbResult<T> ReadSingle<T>(IQueryable<T> query);
      DbResultSet<T> Read<T>(string query, params object[] parameters);
      DbResultSet<T> Read<T>(IQueryable<T> query);
   }

   public interface IDbWriter
   {
      int Write(string query, params object[] parameters);
      int Write(IDbEntity entity);
      int Write(IEnumerable<IDbEntity> entities);
   }

   public interface IDbExecutor
   {
      IDbExecution Execute(string query, params object[] parameters);
      IDbExecution Execute(IQueryable query);
   }

   public interface IDbExecution
      : IDisposable
   {
      DbResult<T> ReadSingle<T>();
      DbResultSet<T> Read<T>();
   }

   public interface IDbMonitor
   {
      // more of an event type thing yo
      //TextWriter Log
      //{
      //   get;
      //   set;
      //}

      event EventHandler<DbChangeEventArgs> Inserted;
      event EventHandler<DbChangeEventArgs> Updated;
      event EventHandler<DbChangeEventArgs> Deleted;
   }
}