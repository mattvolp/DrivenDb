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

namespace DrivenDb
{
   [AttributeUsage(AttributeTargets.Property)]
   public class DbColumnAttribute : Attribute
   {
      public bool IsDbGenerated
      {
         get;
         set;
      }

      public bool IsPrimaryKey
      {
         get;
         set;
      }

      public string Name
      {
         get;
         set;
      }
   }
}