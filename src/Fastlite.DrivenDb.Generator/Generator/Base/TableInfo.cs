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

using System.Collections.Generic;

namespace Fastlite.DrivenDb.Generator.Generator.Base
{
   class TableInfo : ITableInfo
   {
      public TableInfo(string schema, string name, IEnumerable<IColumnInfo> fields)
      {
         Schema = schema;
         Name = name;
         Fields = fields;
      }

      public string Schema
      {
         get;
         protected set;
      }

      public string Name
      {
         get;
         protected set;
      }

      public IEnumerable<IColumnInfo> Fields
      {
         get;
         protected set;
      }
   }
}
