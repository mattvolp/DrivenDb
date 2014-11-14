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

namespace DrivenDb.Collections
{
    public class DbIndexCore<K, T> : IDbIndexCore<K, T>
    {
        private readonly Func<T,K> m_Extractor;

        public DbIndexCore(Func<T, K> extractor)
        {
            m_Extractor = extractor;
        }

        public K ExtractKey(T item)
        {
            return m_Extractor(item);
        }
    }
}
