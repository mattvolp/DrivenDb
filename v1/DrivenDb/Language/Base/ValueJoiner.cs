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

namespace DrivenDb.Base
{
   internal class ValueJoiner : IValueJoiner
   {
      public string Join(IEnumerable values)
      {
         var ints = values as IEnumerable<int>;

         if (ints != null)
         {
            return Join(ints);
         }

         var nints = values as IEnumerable<int?>;

         if (nints != null)
         {
            return Join(nints);
         }

         var longs = values as IEnumerable<long>;

         if (longs != null)
         {
            return Join(longs);
         }

         var nlongs = values as IEnumerable<long?>;

         if (nlongs != null)
         {
            return Join(nlongs);
         }

         var decimals = values as IEnumerable<decimal>;

         if (decimals != null)
         {
            return Join(decimals);
         }

         var ndecimals = values as IEnumerable<decimal?>;

         if (ndecimals != null)
         {
            return Join(ndecimals);
         }

         var guids = values as IEnumerable<Guid>;

         if (guids != null)
         {
            return Join(guids);
         }

         var nguids = values as IEnumerable<Guid?>;

         if (nguids != null)
         {
            return Join(nguids);
         }

         var strings = values as IEnumerable<string>;

         if (strings != null)
         {
            return Join(strings);
         }

         throw new NotSupportedException(String.Format("Could not cast IEnumerable to a known specific type"));
      }

      public string Join(IEnumerable<string> values)
      {
         var distinct = values.Distinct().Select(v => v == null ? "NULL" : String.Format("'{0}'", v));
         return String.Join(",", distinct);
      }

      public string Join(IEnumerable<decimal> values)
      {
         var distinct = values.Distinct().Select(v => v.ToString());
         return String.Join(",", distinct);
      }

      public string Join(IEnumerable<decimal?> values)
      {
         var distinct = values.Distinct().Select(v => !v.HasValue ? "NULL" : v.Value.ToString());
         return String.Join(",", distinct);
      }

      public string Join(IEnumerable<long> values)
      {
         var distinct = values.Distinct().Select(v => v.ToString());
         return String.Join(",", distinct);
      }

      public string Join(IEnumerable<long?> values)
      {
         var distinct = values.Distinct().Select(v => !v.HasValue ? "NULL" : v.Value.ToString());
         return String.Join(",", distinct);
      }

      public string Join(IEnumerable<int> values)
      {
         var distinct = values.Distinct().Select(v => v.ToString());
         return String.Join(",", distinct);
      }

      public string Join(IEnumerable<int?> values)
      {
         var distinct = values.Distinct().Select(v => !v.HasValue ? "NULL" : v.Value.ToString());
         return String.Join(",", distinct);
      }

      public string Join(IEnumerable<Guid> values)
      {
         var distinct = values.Distinct().Select(v => String.Format("'{0}'", v));
         return String.Join(",", distinct);
      }

      public string Join(IEnumerable<Guid?> values)
      {
         var distinct = values.Distinct().Select(v => v == null ? "NULL" : String.Format("'{0}'", v.Value));
         return String.Join(",", distinct);
      }
   }
}