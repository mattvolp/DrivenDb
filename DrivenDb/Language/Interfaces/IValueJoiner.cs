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

namespace DrivenDb
{
   public interface IValueJoiner
   {
      string Join(IEnumerable values);
      string Join(IEnumerable<string> values);
      string Join(IEnumerable<long> values);
      string Join(IEnumerable<long?> values);
      string Join(IEnumerable<int> values);
      string Join(IEnumerable<int?> values);
      string Join(IEnumerable<Guid> values);
      string Join(IEnumerable<Guid?> values);
      string Join(IEnumerable<decimal?> values);
      string Join(IEnumerable<decimal> values);
   }
}