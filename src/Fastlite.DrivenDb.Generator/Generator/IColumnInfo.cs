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

namespace Fastlite.DrivenDb.Generator.Generator
{
   internal interface IColumnInfo
   {
      string Name
      {
         get;
      }

      string SqlType
      {
         get;
      }

      string ClrType
      {
         get;
      }

      bool IsNullable
      {
         get;
      }

      bool IsPrimaryKey
      {
         get;
      }

      bool IsIdentity
      {
         get;
      }

      bool HasDefault
      {
         get;
      }

      string DefaultValue
      {
         get;
      }

      int OrdinalPosition
      {
         get;
      }

      bool IsReadonly
      {
         get;
         set;
      }
   }
}