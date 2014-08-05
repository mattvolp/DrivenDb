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
    internal class EntityComparer<T> : IComparer<T>
        where T : class, IDbEntity<T>
    {
        private readonly Func<T, T, int> m_Comparer;

        public EntityComparer(Func<T, T, int> comparer)
        {
            m_Comparer = comparer;
        }

        public int Compare(T x, T y)
        {
            if (m_Comparer == null)
                return x.CompareTo(y);
            else
                return m_Comparer(x, y);
        }
    }
}
