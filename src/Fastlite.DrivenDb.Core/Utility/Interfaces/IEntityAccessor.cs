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
using System.Collections.Generic;
using System.Reflection;

namespace Fastlite.DrivenDb.Core.Utility.Interfaces
{
   internal interface IEntityAccessor
   {
      bool HasProperty(string name);
      bool CanReadProperty(string name);
      bool CanWriteProperty(string name);
      Type GetType();
      IEnumerable<PropertyInfo> GetProperties();
      PropertyInfo GetPropertyInfo(string name);
   }
}