/**************************************************************************************
 * Original Author : Anthony Leatherwood (fastlite@outlook.com)                              
 * Source Location : https://github.com/Fastlite/DrivenDb     
 *  
 * This source is subject to the Mozilla Public License, version 2.0.
 * Link: https://github.com/Fastlite/DrivenDb/blob/master/LICENSE
 *  
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
 * EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE. 
 **************************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;

namespace Fastlite.DrivenDb.Data.Access.Interfaces
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