using System;
using System.Collections.Generic;
using System.IO;
using Fastlite.DrivenDb.Core.Contracts;
using Fastlite.DrivenDb.Core.Contracts.Interfaces;
using Fastlite.DrivenDb.Data.Access.Interfaces;

namespace Fastlite.DrivenDb.Data.Access.Wrappers
{
   public class DbWrapper : IDbAccessor
   {
      private readonly IDbAccessor _accessor;

      public DbWrapper(IDbAccessor accessor)
      {
         _accessor = accessor;
      }

      public event EventHandler<DbChangeEventArgs> Inserted
      {
         add { _accessor.Inserted += value; }
         remove { _accessor.Inserted -= value; }
      }

      public event EventHandler<DbChangeEventArgs> Updated
      {
         add { _accessor.Inserted += value; }
         remove { _accessor.Inserted -= value; }
      }

      public event EventHandler<DbChangeEventArgs> Deleted
      {
         add { _accessor.Inserted += value; }
         remove { _accessor.Inserted -= value; }
      }

      public int CommandTimeout
      {
         get { return _accessor.CommandTimeout; }
         set { _accessor.CommandTimeout = value; }
      }

      public T ReadIdentity<T, K>(K key) where T : IDbRecord, new()
      {
         return _accessor.ReadIdentity<T, K>(key);
      }
      
      public IOnJoiner<P, C> ReadRelated<P, C>(P parent)
         where P : IDbRecord, new()
         where C : IDbRecord, new()
      {
         return _accessor.ReadRelated<P, C>(parent);
      }

      public IOnJoiner<P, C> ReadRelated<P, C>(IEnumerable<P> parents)
         where P : IDbRecord, new()
         where C : IDbRecord, new()
      {
         return _accessor.ReadRelated<P, C>(parents);
      }

      public IEnumerable<T> ReadValues<T>(string query, params object[] parameters)
      {
         return _accessor.ReadValues<T>(query, parameters);
      }

      public T ReadValue<T>(string query, params object[] parameters)
      {
         return _accessor.ReadValue<T>(query, parameters);
      }

      public IEnumerable<T> ReadAnonymous<T>(T model, string query, params object[] parameters)
      {
         return _accessor.ReadAnonymous<T>(model, query, parameters);
      }

      public IEnumerable<T> ReadType<T>(string query, params object[] parameters) where T : new()
      {
         return _accessor.ReadType<T>(query, parameters);
      }

      public IEnumerable<T> ReadType<T>(Func<T> factory, string query, params object[] parameters)
      {
         return _accessor.ReadType(factory, query, parameters);
      }

      public T ReadEntity<T>(string query, params object[] parameters) where T : IDbRecord, new()
      {
         return _accessor.ReadEntity<T>(query, parameters);
      }

      public IEnumerable<T> ReadEntities<T>(string query, params object[] parameters) where T : IDbRecord, new()
      {
         return _accessor.ReadEntities<T>(query, parameters);
      }

      public IDbScope CreateScope()
      {
         return _accessor.CreateScope();
      }

      public void WriteEntity(IDbEntity entity)
      {
         _accessor.WriteEntity(entity);
      }

      public void WriteEntities(IEnumerable<IDbEntity> entities)
      {
         _accessor.WriteEntities(entities);
      }

      public void Execute(string query, params object[] parameters)
      {
         _accessor.Execute(query, parameters);
      }

      public TextWriter Log
      {
         get { return _accessor.Log; }
         set { _accessor.Log = value; }
      }

      public DbSet<T1, T2> ReadEntities<T1, T2>(string query, params object[] parameters)
         where T1 : IDbRecord, new()
         where T2 : IDbRecord, new()
      {
         return _accessor.ReadEntities<T1, T2>(query, parameters);
      }

      public DbSet<T1, T2, T3> ReadEntities<T1, T2, T3>(string query, params object[] parameters)
         where T1 : IDbRecord, new()
         where T2 : IDbRecord, new()
         where T3 : IDbRecord, new()
      {
         return _accessor.ReadEntities<T1, T2, T3>(query, parameters);
      }

      public DbSet<T1, T2, T3, T4> ReadEntities<T1, T2, T3, T4>(string query, params object[] parameters)
         where T1 : IDbRecord, new()
         where T2 : IDbRecord, new()
         where T3 : IDbRecord, new()
         where T4 : IDbRecord, new()
      {
         return _accessor.ReadEntities<T1, T2, T3, T4>(query, parameters);
      }

      public DbSet<T1, T2, T3, T4, T5> ReadEntities<T1, T2, T3, T4, T5>(string query, params object[] parameters)
         where T1 : IDbRecord, new()
         where T2 : IDbRecord, new()
         where T3 : IDbRecord, new()
         where T4 : IDbRecord, new()
         where T5 : IDbRecord, new()
      {
         return _accessor.ReadEntities<T1, T2, T3, T4, T5>(query, parameters);
      }

      public DbSet<T1, T2, T3, T4, T5, T6> ReadEntities<T1, T2, T3, T4, T5, T6>(string query, params object[] parameters)
         where T1 : IDbRecord, new()
         where T2 : IDbRecord, new()
         where T3 : IDbRecord, new()
         where T4 : IDbRecord, new()
         where T5 : IDbRecord, new()
         where T6 : IDbRecord, new()
      {
         return _accessor.ReadEntities<T1, T2, T3, T4, T5, T6>(query, parameters);
      }

      public DbSet<T1, T2, T3, T4, T5, T6, T7> ReadEntities<T1, T2, T3, T4, T5, T6, T7>(string query, params object[] parameters)
         where T1 : IDbRecord, new()
         where T2 : IDbRecord, new()
         where T3 : IDbRecord, new()
         where T4 : IDbRecord, new()
         where T5 : IDbRecord, new()
         where T6 : IDbRecord, new()
         where T7 : IDbRecord, new()
      {
         return _accessor.ReadEntities<T1, T2, T3, T4, T5, T6, T7>(query, parameters);
      }

      public DbSet<T1, T2, T3, T4, T5, T6, T7, T8> ReadEntities<T1, T2, T3, T4, T5, T6, T7, T8>(string query, params object[] parameters)
         where T1 : IDbRecord, new()
         where T2 : IDbRecord, new()
         where T3 : IDbRecord, new()
         where T4 : IDbRecord, new()
         where T5 : IDbRecord, new()
         where T6 : IDbRecord, new()
         where T7 : IDbRecord, new()
         where T8 : IDbRecord, new()
      {
         return _accessor.ReadEntities<T1, T2, T3, T4, T5, T6, T7, T8>(query, parameters);
      }

      public DbSet<T1, T2, T3, T4, T5, T6, T7, T8, T9> ReadEntities<T1, T2, T3, T4, T5, T6, T7, T8, T9>(string query, params object[] parameters)
         where T1 : IDbRecord, new()
         where T2 : IDbRecord, new()
         where T3 : IDbRecord, new()
         where T4 : IDbRecord, new()
         where T5 : IDbRecord, new()
         where T6 : IDbRecord, new()
         where T7 : IDbRecord, new()
         where T8 : IDbRecord, new()
         where T9 : IDbRecord, new()
      {
         return _accessor.ReadEntities<T1, T2, T3, T4, T5, T6, T7, T8, T9>(query, parameters);
      }

      public DbSet<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> ReadEntities<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(string query, params object[] parameters)
         where T1 : IDbRecord, new()
         where T2 : IDbRecord, new()
         where T3 : IDbRecord, new()
         where T4 : IDbRecord, new()
         where T5 : IDbRecord, new()
         where T6 : IDbRecord, new()
         where T7 : IDbRecord, new()
         where T8 : IDbRecord, new()
         where T9 : IDbRecord, new()
         where T10 : IDbRecord, new()
      {
         return _accessor.ReadEntities<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(query, parameters);
      }

      public IParallelAccessor Parallel
      {
         get { return _accessor.Parallel; }
         // ReSharper disable ValueParameterNotUsed
         private set { }
         // ReSharper restore ValueParameterNotUsed
      }

      IParallelAccessorSlim IDbAccessorSlim.Parallel
      {
         get { return Parallel; }
      }

      public IFallbackAccessorSlim Fallback
      {
         get { return _accessor.Fallback; }
         // ReSharper disable ValueParameterNotUsed
         private set { }
         // ReSharper restore ValueParameterNotUsed
      }
   }
}