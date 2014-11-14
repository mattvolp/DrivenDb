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

using System.Collections.Generic;

namespace DrivenDbConsole.Contracts.Base
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
