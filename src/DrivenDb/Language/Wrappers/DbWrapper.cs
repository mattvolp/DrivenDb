/**************************************************************************************
 * Original Author : Anthony Leatherwood (adleatherwood@gmail.com)                              
 * Source Location : https://github.com/Fastlite/DrivenDb    
 *  
 * This source is subject to the Microsoft Public License.
 * Link: http://www.microsoft.com/en-us/openness/licenses.aspx
 *  
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
 * EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE. 
 **************************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;

namespace DrivenDb.Wrappers
{
   public class DbWrapper : IDbAccessor
   {
      private readonly IDbAccessor m_Accessor;

      public DbWrapper(IDbAccessor accessor)
      {
         m_Accessor = accessor;
      }

      public event EventHandler<DbChangeEventArgs> Inserted
      {
         add { m_Accessor.Inserted += value; }
         remove { m_Accessor.Inserted -= value; }
      }

      public event EventHandler<DbChangeEventArgs> Updated
      {
         add { m_Accessor.Inserted += value; }
         remove { m_Accessor.Inserted -= value; }
      }

      public event EventHandler<DbChangeEventArgs> Deleted
      {
         add { m_Accessor.Inserted += value; }
         remove { m_Accessor.Inserted -= value; }
      }

      public int CommandTimeout
      {
         get { return m_Accessor.CommandTimeout; }
         set { m_Accessor.CommandTimeout = value; }
      }

      public T ReadIdentity<T, K>(K key) where T : IDbRecord, new()
      {
         return m_Accessor.ReadIdentity<T, K>(key);
      }

      public T ReadIdentity<T, K1, K2>(K1 key1, K2 key2) where T : IDbRecord, new()
      {
         return m_Accessor.ReadIdentity<T, K1, K2>(key1, key2);
      }

      public T ReadIdentity<T, K1, K2, K3>(K1 key1, K2 key2, K3 key3) where T : IDbRecord, new()
      {
         return m_Accessor.ReadIdentity<T, K1, K2, K3>(key1, key2, key3);
      }

      public T ReadIdentity<T, K1, K2, K3, K4>(K1 key1, K2 key2, K3 key3, K4 key4) where T : IDbRecord, new()
      {
         return m_Accessor.ReadIdentity<T, K1, K2, K3, K4>(key1, key2, key3, key4);
      }

      public IOnJoiner<P, C> ReadRelated<P, C>(P parent)
         where P : IDbRecord, new()
         where C : IDbRecord, new()
      {
         return m_Accessor.ReadRelated<P, C>(parent);
      }

      public IOnJoiner<P, C> ReadRelated<P, C>(IEnumerable<P> parents)
         where P : IDbRecord, new()
         where C : IDbRecord, new()
      {
         return m_Accessor.ReadRelated<P, C>(parents);
      }

      public IEnumerable<T> ReadValues<T>(string query, params object[] parameters)
      {
         return m_Accessor.ReadValues<T>(query, parameters);
      }

      public T ReadValue<T>(string query, params object[] parameters)
      {
         return m_Accessor.ReadValue<T>(query, parameters);
      }

      public IEnumerable<T> ReadAnonymous<T>(T model, string query, params object[] parameters)
      {
         return m_Accessor.ReadAnonymous<T>(model, query, parameters);
      }

      public IEnumerable<T> ReadType<T>(string query, params object[] parameters) where T : new()
      {
         return m_Accessor.ReadType<T>(query, parameters);
      }

      public IEnumerable<T> ReadType<T>(Func<T> factory, string query, params object[] parameters)
      {
         return m_Accessor.ReadType(factory, query, parameters);
      }

      public T ReadEntity<T>(string query, params object[] parameters) where T : IDbRecord, new()
      {
         return m_Accessor.ReadEntity<T>(query, parameters);
      }

      public IEnumerable<T> ReadEntities<T>(string query, params object[] parameters) where T : IDbRecord, new()
      {
         return m_Accessor.ReadEntities<T>(query, parameters);
      }

      public IDbScope CreateScope()
      {
         return m_Accessor.CreateScope();
      }

      public void WriteEntity(IDbEntity entity)
      {
         m_Accessor.WriteEntity(entity);
      }

      public void WriteEntities(IEnumerable<IDbEntity> entities)
      {
         m_Accessor.WriteEntities(entities);
      }

      public void Execute(string query, params object[] parameters)
      {
         m_Accessor.Execute(query, parameters);
      }

      public TextWriter Log
      {
         get { return m_Accessor.Log; }
         set { m_Accessor.Log = value; }
      }

      public DbSet<T1, T2> ReadEntities<T1, T2>(string query, params object[] parameters)
         where T1 : IDbRecord, new()
         where T2 : IDbRecord, new()
      {
         return m_Accessor.ReadEntities<T1, T2>(query, parameters);
      }

      public DbSet<T1, T2, T3> ReadEntities<T1, T2, T3>(string query, params object[] parameters)
         where T1 : IDbRecord, new()
         where T2 : IDbRecord, new()
         where T3 : IDbRecord, new()
      {
         return m_Accessor.ReadEntities<T1, T2, T3>(query, parameters);
      }

      public DbSet<T1, T2, T3, T4> ReadEntities<T1, T2, T3, T4>(string query, params object[] parameters)
         where T1 : IDbRecord, new()
         where T2 : IDbRecord, new()
         where T3 : IDbRecord, new()
         where T4 : IDbRecord, new()
      {
         return m_Accessor.ReadEntities<T1, T2, T3, T4>(query, parameters);
      }

      public DbSet<T1, T2, T3, T4, T5> ReadEntities<T1, T2, T3, T4, T5>(string query, params object[] parameters)
         where T1 : IDbRecord, new()
         where T2 : IDbRecord, new()
         where T3 : IDbRecord, new()
         where T4 : IDbRecord, new()
         where T5 : IDbRecord, new()
      {
         return m_Accessor.ReadEntities<T1, T2, T3, T4, T5>(query, parameters);
      }

      public DbSet<T1, T2, T3, T4, T5, T6> ReadEntities<T1, T2, T3, T4, T5, T6>(string query, params object[] parameters)
         where T1 : IDbRecord, new()
         where T2 : IDbRecord, new()
         where T3 : IDbRecord, new()
         where T4 : IDbRecord, new()
         where T5 : IDbRecord, new()
         where T6 : IDbRecord, new()
      {
         return m_Accessor.ReadEntities<T1, T2, T3, T4, T5, T6>(query, parameters);
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
         return m_Accessor.ReadEntities<T1, T2, T3, T4, T5, T6, T7>(query, parameters);
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
         return m_Accessor.ReadEntities<T1, T2, T3, T4, T5, T6, T7, T8>(query, parameters);
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
         return m_Accessor.ReadEntities<T1, T2, T3, T4, T5, T6, T7, T8, T9>(query, parameters);
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
         return m_Accessor.ReadEntities<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(query, parameters);
      }

      public IParallelAccessor Parallel
      {
         get { return m_Accessor.Parallel; }
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
         get { return m_Accessor.Fallback; }
         // ReSharper disable ValueParameterNotUsed
         private set { }
         // ReSharper restore ValueParameterNotUsed
      }
   }
}