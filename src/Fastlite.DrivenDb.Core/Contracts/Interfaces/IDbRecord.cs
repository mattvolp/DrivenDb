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
using Fastlite.DrivenDb.Core.Contracts.Attributes;

namespace Fastlite.DrivenDb.Core.Contracts.Interfaces
{   
   public interface IDbRecord
   {      
      object[] PrimaryKey
      {
         get;
      }

      EntityState State
      {
         get;
      }

      string Schema
      {
         get;
      }

      DbTableAttribute Table
      {
         get;
      }

      DbSequenceAttribute Sequence
      {
         get;
      }
      
      DbColumnAttribute IdentityColumn
      {
         get;
      }

      DbColumnAttribute[] PrimaryColumns
      {
         get;
      }

      IDictionary<string, DbColumnAttribute> Columns
      {
         get;
      }

      IEnumerable<string> Changes
      {
         get;
      }

      DateTime? LastModified
      {
         get;
      }

      DateTime? LastUpdated
      {
         get;
      }

      void SetIdentity(long identity);

      object GetProperty(string property);

      void SetProperty(string property, object value);

      void Reset();
   }
}