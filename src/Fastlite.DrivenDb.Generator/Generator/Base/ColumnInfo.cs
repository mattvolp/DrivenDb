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

namespace Fastlite.DrivenDb.Generator.Generator.Base
{
   internal class ColumnInfo : IColumnInfo
   {
      public ColumnInfo(string name, string sqlType, string clrType, string defaultValue, bool isNullable, bool isPrimaryKey, bool isIdentity, int ordinalPosition, bool isReadonly)
      {
         Name = name;
         SqlType = sqlType;
         ClrType = clrType;
         IsNullable = isNullable;
         IsPrimaryKey = isPrimaryKey;
         IsIdentity = isIdentity;
         OrdinalPosition = ordinalPosition;
         IsReadonly = isReadonly;
      }

      public string Name
      {
         get;
         protected set;
      }

      public string SqlType
      {
         get;
         protected set;
      }

      public string ClrType
      {
         get;
         protected set;
      }

      public bool IsNullable
      {
         get;
         protected set;
      }

      public bool IsPrimaryKey
      {
         get;
         protected set;
      }

      public bool IsIdentity
      {
         get;
         protected set;
      }

      public bool HasDefault
      {
         get { return !String.IsNullOrWhiteSpace(DefaultValue); }
      }

      public string DefaultValue
      {
         get;
         protected set;
      }

      public int OrdinalPosition
      {
         get;
         protected set;
      }

      public bool IsReadonly
      {
         get;
         set;
      }
   }
}