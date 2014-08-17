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

using Fastlite.DrivenDb.Generator.Generator.Base;

namespace Fastlite.DrivenDb.Generator.Generator.MsSql
{
   internal class MsSqlColumnInfo : ColumnInfo
   {
      public MsSqlColumnInfo(string name, string sqlType, string clrType, string defaultValue, bool isNullable, bool isPrimaryKey, bool isIdentity, int ordinalPosition, bool isReadonly)
         : base(name, sqlType, clrType, defaultValue, isNullable, isPrimaryKey, isIdentity, ordinalPosition, isReadonly)
      {
         switch ((defaultValue ?? "").ToLower())
         {
            case "(newid())":
               DefaultValue = "Guid.NewGuid()";
               break;

            case "(getdate())":
               DefaultValue = "DateTime.Now";
               break;

            case "(null)":
               DefaultValue = "null";
               break;

            default:
               DefaultValue = defaultValue;
               break;
         }

         if (DefaultValue != null)
         {
            if (DefaultValue.Contains("'"))
            {
               DefaultValue = DefaultValue.Replace("'", "\"");
            }

            if (sqlType.ToLower() == "bit")
            {
               if (DefaultValue == "null")
                  DefaultValue = "null";
               else if (DefaultValue == "((0))")
                  DefaultValue = "false";
               else
                  DefaultValue = "true";
            }

            if (DefaultValue.Contains("(("))
            {
               DefaultValue = DefaultValue.Replace("((", "");
               DefaultValue = DefaultValue.Replace("))", "");
            }

            if (sqlType.ToLower() == "money")
            {
               DefaultValue += "m";
            }
         }
      }
   }
}