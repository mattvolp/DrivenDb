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

namespace DrivenDb.Collections
{
    public interface IDbCache<K,T,I> : IDbView<K, T>
        where T : class, IDbEntity<T>
    {
        T this[K key]
        {
            get;
        }

        int Count
        {
            get;
        }

        IDbView<K, T> DefaultView
        {
            get;
        }

        event EventHandler<DbIndexChangeEventArgs> IndexChanged;
        event EventHandler<EventArgs> IndexCleared;

        bool TryGetEntity(K key, out T entity);
        void Add(T item);
        void Add(IEnumerable<T> items);
        void Clear();
        bool Remove(K key);
        bool Remove(T item);
        void Flush();
    }
}
