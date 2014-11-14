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
    public interface IDbView<K, T> : IEnumerable<T>
        where T : class, IDbEntity<T>
    {
        IDbIndex<K, T> Source
        {
            get;
        }

        bool Contains(K key);
        bool Contains(T item);

        INotifyingDbView<K, T> ViewAs(Func<T, bool> filter);
        INotifyingDbView<K, T> ViewAs(Func<T, T, int> comparer);
        INotifyingDbView<K, T> ViewAs(Func<T, bool> filter, Func<T, T, int> comparer);
    }
}
