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
using DrivenDb.Base;

namespace DrivenDb.SqLite
{
   class SqLiteBuilder : SqlBuilder
   {
      public SqLiteBuilder() 
         : base("[", "]", "@", ";")
      {
      }

      public override string ToInsert<T>(T entity, int index, bool returnId)
      {
         var identityColumn = returnId && entity.IdentityColumn != null
            ? entity.IdentityColumn.Name + ", "
            : null;

         var identityValue = identityColumn != null
            ? "null, "
            : "";

         var identitySelect = identityColumn != null
            ? String.Format("SELECT {0}, last_insert_rowid();", index)
            : null;

         return String.Format("INSERT INTO {0} ({1}{2}) VALUES ({3}{4}); {5}", GetFormattedTable(), identityColumn, GetFormattedSetterColumns(), identityValue, GetFormattedSetterValues(), identitySelect);
      }
   }
}
