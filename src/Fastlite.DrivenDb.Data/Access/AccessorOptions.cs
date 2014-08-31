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

namespace Fastlite.DrivenDb.Data.Access
{
   [Flags]
   public enum AccessorOptions
   {
      None = 0,
      AllowEnumerableParameters = 1,
      LimitDateParameters = 2,
      AllowUnmappedColumns = 4, 
      CaseInsensitiveColumnMapping = 8,
      PrivateMemberColumnMapping = 16,

      Common = AllowEnumerableParameters
         | LimitDateParameters
         | AllowUnmappedColumns
         | CaseInsensitiveColumnMapping,
         
      All = AllowEnumerableParameters
         | LimitDateParameters
         | AllowUnmappedColumns
         | CaseInsensitiveColumnMapping
         | PrivateMemberColumnMapping,
   }
}
