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
using Fastlite.DrivenDb.Core.Contracts.Interfaces;

namespace Fastlite.DrivenDb.Data.Access.Interfaces
{
   public interface ISqlBuilder
   {
      string Schema
      {
         get;
         set;
      }

      string Table
      {
         get;
         set;
      }
      
      void AddColumn(string name);
      void AddSetter(string column, int parameter);
      void AddWhere(string column, int parameter);
      void GroupWhere();

      string ToInsert<T>(T entity, int index, bool returnId) where T : IDbEntity;
      string ToSelect();
      string ToUpdate();
      string ToDelete();

      DateTime CorrectDateTime(DateTime dateTime);
   }
}
