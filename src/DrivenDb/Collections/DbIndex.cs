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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DrivenDb.Utility;

namespace DrivenDb.Collections
{
    internal class DbIndex<K, T> : IDbIndex<K, T>
       where T : class, IDbEntity<T>
    {
        private readonly WeakEventManager<DbIndexChangeEventArgs> m_IndexChangedManager = new WeakEventManager<DbIndexChangeEventArgs>();
        private readonly WeakEventManager<EventArgs> m_IndexClearedManager = new WeakEventManager<EventArgs>();
        private readonly Dictionary<K, T> m_Dictionary = new Dictionary<K, T>();
        private readonly IDbIndexCore<K, T> m_Core;

        public DbIndex(IDbIndexCore<K, T> core)
        {
            m_Core = core;
        }

        public IDbIndex<K, T> Source
        {
            get { return this; }
        }

        public T this[K key]
        {
            get 
            {
                lock (m_Dictionary)
                {
                    return m_Dictionary[key];
                }
            }
        }

        public int Count
        {
            get 
            {
                lock (m_Dictionary)
                {
                    return m_Dictionary.Count;
                }
            }
        }

        public event EventHandler<DbIndexChangeEventArgs> IndexChanged
        {
            add { m_IndexChangedManager.Add(value); }
            remove { m_IndexChangedManager.Remove(value); }
        }

        public event EventHandler<EventArgs> IndexCleared
        {
            add { m_IndexClearedManager.Add(value); }
            remove { m_IndexClearedManager.Remove(value); }
        }

        public void AddOrFail(T item)
        {
            AddOrFail(new[] { item });
        }

        public void AddOrFail(IEnumerable<T> items)
        {
            lock (m_Dictionary)
            {
                foreach (var item in items)
                {
                    var key = m_Core.ExtractKey(item);

                    m_Dictionary.Add(key, item);

                    var insert = new DbChange(DbChangeType.Inserted, item.Table.Name, null, item);

                    m_IndexChangedManager.Invoke(this, new DbIndexChangeEventArgs(new[] { insert }));
                }
            }
        }

        public void AddOrMerge(T item)
        {
            AddOrMerge(new[] { item });
        }

        public void AddOrMerge(IEnumerable<T> items)
        {
            lock (m_Dictionary)
            {
                foreach (var item in items)
                {
                    var key = m_Core.ExtractKey(item);

                    T existing;

                    if (m_Dictionary.TryGetValue(key, out existing))
                    {
                        existing.Merge(item);
                    }
                    else
                    {
                        m_Dictionary.Add(key, item);

                        var insert = new DbChange(DbChangeType.Inserted, item.Table.Name, null, item);

                        m_IndexChangedManager.Invoke(this, new DbIndexChangeEventArgs(new[] { insert }));
                    }
                }
            }
        }

        public void AddOrReplace(T item)
        {
            AddOrReplace(new[] { item });
        }

        public void AddOrReplace(IEnumerable<T> items)
        {
            lock (m_Dictionary)
            {
                foreach (var item in items)
                {
                    var key = m_Core.ExtractKey(item);

                    T existing;

                    if (m_Dictionary.TryGetValue(key, out existing))
                    {
                        m_Dictionary[key] = item;

                        var update = new DbChange(DbChangeType.Updated, item.Table.Name, null, item);

                        m_IndexChangedManager.Invoke(this, new DbIndexChangeEventArgs(new[] { update }));
                    }
                    else
                    {
                        m_Dictionary.Add(key, item);

                        var insert = new DbChange(DbChangeType.Inserted, item.Table.Name, null, item);

                        m_IndexChangedManager.Invoke(this, new DbIndexChangeEventArgs(new[] { insert }));
                    }
                }
            }
        }

        public void AddOrIgnore(T item)
        {
            AddOrIgnore(new[] { item });
        }

        public void AddOrIgnore(IEnumerable<T> items)
        {
            lock (m_Dictionary)
            {
                foreach (var item in items)
                {
                    var key = m_Core.ExtractKey(item);

                    if (!m_Dictionary.ContainsKey(key))
                    {
                        m_Dictionary.Add(key, item);

                        var insert = new DbChange(DbChangeType.Inserted, item.Table.Name, null, item);

                        m_IndexChangedManager.Invoke(this, new DbIndexChangeEventArgs(new[] { insert }));
                    }
                }
            }
        }

        public void Clear()
        {
            lock (m_Dictionary)
            {
                m_Dictionary.Clear();
                m_IndexClearedManager.Invoke(this, new EventArgs());
            }
        }

        public bool Contains(K key)
        {
            lock (m_Dictionary)
            {
                return m_Dictionary.ContainsKey(key);
            }
        }

        public bool Contains(T item)
        {
            lock (m_Dictionary)
            {
                return m_Dictionary.ContainsKey(m_Core.ExtractKey(item));
            }
        }

        public bool Remove(K key)
        {
            lock (m_Dictionary)
            {
                T existing;

                if (m_Dictionary.TryGetValue(key, out existing))
                {
                    var delete = new DbChange(DbChangeType.Deleted, existing.Table.Name, null, existing);
                    m_IndexChangedManager.Invoke(this, new DbIndexChangeEventArgs(new[] { delete }));
                }

                return true;
            }
        }

        public bool Remove(T item)
        {
            lock (m_Dictionary)
            {
                var key = m_Core.ExtractKey(item);
                var result = m_Dictionary.Remove(key);

                if (result)
                {
                    var delete = new DbChange(DbChangeType.Deleted, item.Table.Name, null, item);
                    m_IndexChangedManager.Invoke(this, new DbIndexChangeEventArgs(new[] { delete }));
                }
                return result;
            }
        }

        public IDbView<K, T> DefaultView
        {
            get { return this; }
        }

        public INotifyingDbView<K, T> ViewAs(Func<T, bool> filter)
        {
            lock (m_Dictionary)
            {
                return new DbView<K, T>(m_Core, this, filter);
            }
        }

        public INotifyingDbView<K, T> ViewAs(Func<T, T, int> comparer)
        {
            lock (m_Dictionary)
            {
                return new DbView<K, T>(m_Core, this, comparer);
            }
        }

        public INotifyingDbView<K, T> ViewAs(Func<T, bool> filter, Func<T, T, int> comparer)
        {
            lock (m_Dictionary)
            {
                return new DbView<K, T>(m_Core, this, filter, comparer);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            lock (m_Dictionary)
            {
                return m_Dictionary.Values.ToList().GetEnumerator();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
