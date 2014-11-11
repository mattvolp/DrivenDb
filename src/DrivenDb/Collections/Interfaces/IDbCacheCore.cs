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
using System.Collections.Generic;

namespace DrivenDb.Collections
{
    public interface IDbCacheCore<K, T, I> : IDbIndexCore<K, T>
    {        
        T RetrieveEntity(K key);
        IEnumerable<T> RetrieveInitialEntities();

        bool CacheEntity(T entity, I info, out IndexAdditionMethod method);
        bool FlushEntity(T entity, I info);

        event EventHandler FlushEntities;

        I CreateInfo(T entity);
    }
}
