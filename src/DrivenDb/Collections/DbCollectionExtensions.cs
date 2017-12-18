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
   public static class DbCollectionExtensions
    {
        public static IDbIndex<K, T> ToIndexWithFail<K, T>(this IEnumerable<T> enumerable, Func<T, K> extractor)
           where T : class, IDbEntity<T>
        {
            var core = new DbIndexCore<K, T>(extractor);
            var index = new DbIndex<K, T>(core);

            index.AddOrFail(enumerable);

            return index;
        }

        public static IDbIndex<K, T> ToIndexWithMerge<K, T>(this IEnumerable<T> enumerable, Func<T, K> extractor)
           where T : class, IDbEntity<T>
        {
            var core = new DbIndexCore<K, T>(extractor);
            var index = new DbIndex<K, T>(core);

            index.AddOrMerge(enumerable);

            return index;
        }

        public static IDbIndex<K, T> ToIndexWithReplace<K, T>(this IEnumerable<T> enumerable, Func<T, K> extractor)
           where T : class, IDbEntity<T>
        {
            var core = new DbIndexCore<K, T>(extractor);
            var index = new DbIndex<K, T>(core);

            index.AddOrReplace(enumerable);

            return index;
        }

        public static IDbIndex<K, T> ToIndexWithIgnore<K, T>(this IEnumerable<T> enumerable, Func<T, K> extractor)
           where T : class, IDbEntity<T>
        {
            var core = new DbIndexCore<K, T>(extractor);
            var index = new DbIndex<K, T>(core);

            index.AddOrIgnore(enumerable);

            return index;
        }
    }
}
