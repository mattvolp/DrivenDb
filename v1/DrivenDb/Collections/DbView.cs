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
using System.Collections.Specialized;

namespace DrivenDb.Collections
{
    internal class DbView<K, T> : INotifyingDbView<K, T>
        where T : class, IDbEntity<T>
    {
        private readonly IDbIndexCore<K, T> m_Core;
        private readonly HashSet<K> m_Keys = new HashSet<K>();
        private readonly SortedList<T, T> m_List;
        private readonly IDbIndex<K, T> m_Index;
        private readonly Func<T, bool> m_Filter;

        public DbView(IDbIndexCore<K, T> core, IDbIndex<K, T> index, Func<T, bool> filter)
            : this(core, index, filter, null)
        {
        }

        public DbView(IDbIndexCore<K, T> core, IDbIndex<K, T> index, Func<T, T, int> comparer)
            : this(core, index, null, comparer)
        {
        }

        public DbView(IDbIndexCore<K, T> core, IDbIndex<K, T> index, Func<T, bool> filter, Func<T, T, int> comparer)
        {
            m_Core = core;
            m_Index = index;
            m_Filter = filter;

            m_List = new SortedList<T, T>(new EntityComparer<T>(comparer));

            foreach (T item in index)
            {
                if (m_Filter == null || m_Filter(item))
                {
                    lock (m_List)
                    {
                        m_Keys.Add(m_Core.ExtractKey(item));
                        InsertItem(item);
                    }
                }
            }

            m_Index.IndexChanged += OnSourceIndexChanged;
            m_Index.IndexCleared += OnSourceIndexCleared;
        }

        public IDbIndex<K, T> Source
        {
            get { return m_Index; }
        }

        public bool Contains(K key)
        {
            lock (m_List)
            {
                return m_Keys.Contains(key);
            }
        }

        public bool Contains(T item)
        {
            lock (m_List)
            {
                return m_List.ContainsKey(item);
            }
        }

        public INotifyingDbView<K, T> ViewAs(Func<T, bool> filter)
        {
            return new DbView<K, T>(m_Core, m_Index, (t) => m_Filter(t) && filter(t));
        }

        public INotifyingDbView<K, T> ViewAs(Func<T, T, int> comparer)
        {
            return new DbView<K, T>(m_Core, m_Index, comparer);
        }

        public INotifyingDbView<K, T> ViewAs(Func<T, bool> filter, Func<T, T, int> comparer)
        {
            return new DbView<K, T>(m_Core, m_Index, (t) => m_Filter(t) && filter(t), comparer);
        }

        private void OnSourceIndexChanged(object sender, DbIndexChangeEventArgs args)
        {
            foreach (var change in args.Changes)
            {
                var item = (T)change.Entity;

                switch (change.ChangeType)
                {
                    case DbChangeType.Inserted:
                        {
                            if (m_Filter == null || m_Filter(item))
                            {
                                lock (m_List)
                                {
                                    m_Keys.Add(m_Core.ExtractKey(item));
                                    InsertItem(item);
                                }
                            }
                        }
                        break;
                    case DbChangeType.Updated:
                        {
                            lock (m_List)
                            {
                                var index = m_List.IndexOfKey(item);
                                var filtered = m_Filter == null || m_Filter(item);

                                if (filtered && index < 0)
                                {
                                    lock (m_List)
                                    {
                                        m_Keys.Add(m_Core.ExtractKey(item));
                                        InsertItem(item);
                                    }
                                }
                                else if (index > -1)
                                {
                                    if (filtered)
                                    {
                                        DeleteItem(item, index);
                                        InsertItem(item);
                                    }
                                    else
                                    {
                                        m_Keys.Remove(m_Core.ExtractKey(item));
                                        DeleteItem(item, index);
                                    }
                                }
                            }
                        }
                        break;
                    case DbChangeType.Deleted:
                        {
                            lock (m_List)
                            {
                                var index = m_List.IndexOfKey(item);

                                if (index > -1)
                                {
                                    m_Keys.Remove(m_Core.ExtractKey(item));
                                    DeleteItem(item, index);
                                }
                            }
                        }
                        break;
                }
            }
        }

        private void OnSourceIndexCleared(object sender, EventArgs args)
        {
            CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged = delegate { };

        public IEnumerator<T> GetEnumerator()
        {
            lock (m_List)
            {
                return m_List.Values.GetEnumerator();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void InsertItem(T item)
        {
            m_List.Add(item, item);

            var index = m_List.IndexOfKey(item);

            CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
        }

        private void DeleteItem(T item, int index)
        {
            CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
        }
    }
}
