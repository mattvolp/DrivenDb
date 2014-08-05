/**************************************************************************************
 * Original Author : Anthony Leatherwood (adleatherwood@gmail.com)                              
 * Source Location : http://drivendb.codeplex.com     
 *  
 * This source is subject to the Microsoft Public License.
 * Link: http://drivendb.codeplex.com/license
 *  
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
 * EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE. 
 **************************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DrivenDb.Collections
{
    public class DbCache<K, T, I> : IDbCache<K, T, I>
        where T : class, IDbEntity<T>
    {
        private readonly IDbCacheCore<K, T, I> m_Core;
        private readonly IDbIndex<K, T> m_Index;
        private readonly IDictionary<K, I> m_Info;

        public DbCache(IDbCacheCore<K, T, I> core)
        {
            m_Core = core;
            m_Core.FlushEntities += (s, e) => Flush();

            m_Index = new DbIndex<K, T>(core);
            m_Info = new Dictionary<K, I>();

            Add(m_Core.RetrieveInitialEntities());            
        }

        public IDbIndex<K, T> Source
        {
            get { return m_Index; }
        }

        public bool Contains(K key)
        {
            return m_Index.Contains(key);
        }

        public bool Contains(T item)
        {
            return m_Index.Contains(item);
        }

        public INotifyingDbView<K, T> ViewAs(Func<T, bool> filter)
        {
            return m_Index.ViewAs(filter);
        }

        public INotifyingDbView<K, T> ViewAs(Func<T, T, int> comparer)
        {
            return m_Index.ViewAs(comparer);
        }

        public INotifyingDbView<K, T> ViewAs(Func<T, bool> filter, Func<T, T, int> comparer)
        {
            return m_Index.ViewAs(filter, comparer);
        }

        public T this[K key]
        {
            get 
            {
                lock (m_Info)
                {
                    if (!m_Index.Contains(key))
                    {
                        var entity = m_Core.RetrieveEntity(key);
                        Add(entity);
                    }

                    return m_Index[key];
                }
            }
        }

        public int Count
        {
            get { return m_Index.Count; }
        }

        public IDbView<K, T> DefaultView
        {
            get { return m_Index.DefaultView; }
        }

        public event EventHandler<DbIndexChangeEventArgs> IndexChanged
        {
            add { m_Index.IndexChanged += value; }
            remove { m_Index.IndexChanged -= value; }
        }

        public event EventHandler<EventArgs> IndexCleared
        {
            add { m_Index.IndexCleared += value; }
            remove { m_Index.IndexCleared -= value; }
        }

        public bool TryGetEntity(K key, out T entity)
        {
            lock(m_Info)
            {
                if (!m_Index.Contains(key))
                {
                    entity = m_Core.RetrieveEntity(key);

                    if (entity != null)
                    {
                        Add(entity);
                        return true;
                    }

                    return false;
                }

                entity = m_Index[key];
                return true;
            }
        }

        public void Add(T item)
        {
            Add(new[] { item });
        }

        public void Add(IEnumerable<T> items)
        {
            lock (m_Info)
            {
                foreach (var item in items)
                {
                    var info = m_Core.CreateInfo(item);

                    IndexAdditionMethod method;

                    if (m_Core.CacheEntity(item, info, out method))
                    {
                        switch (method)
                        {
                            case IndexAdditionMethod.Fail:
                                m_Index.AddOrFail(item);
                                break;
                            case IndexAdditionMethod.Ignore:
                                m_Index.AddOrIgnore(item);
                                break;
                            case IndexAdditionMethod.Merge:
                                m_Index.AddOrMerge(item);
                                break;
                            case IndexAdditionMethod.Replace:
                                m_Index.AddOrReplace(item);
                                break;
                            default:
                                throw new InvalidOperationException(String.Format("Invalid IndexAdditionMethod value of '{0}'", method));
                        }
                    }
                }
            }
        }

        public void Clear()
        {
            m_Index.Clear();
        }

        public bool Remove(K key)
        {
            lock (m_Info)
            {                
                I info;

                if (m_Info.TryGetValue(key, out info))
                {
                    var item = m_Index[key];

                    if (m_Core.FlushEntity(item, info))
                    {
                        m_Info.Remove(key);
                        return m_Index.Remove(key);
                    }
                }

                return false;
            }
        }

        public bool Remove(T item)
        {
            return Remove(m_Core.ExtractKey(item));
        }

        public void Flush()
        {
            lock (m_Info)
            {
                foreach (var item in m_Index.ToArray())
                {
                    var key = m_Core.ExtractKey(item);

                    I info;

                    if (m_Info.TryGetValue(key, out info))
                    {
                        if (m_Core.FlushEntity(item, info))
                        {
                            m_Info.Remove(key);
                            m_Index.Remove(key);
                        }
                    }
                }
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return m_Index.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
