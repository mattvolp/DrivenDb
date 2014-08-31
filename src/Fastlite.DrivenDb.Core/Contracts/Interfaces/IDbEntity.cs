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

namespace Fastlite.DrivenDb.Core.Contracts.Interfaces
{
   public interface IDbEntity : IDbRecord
   {
      EntityState State
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

      void Reset();

      void Delete();
   }

   public interface IDbEntity<T> : IDbEntity
      where T : IDbEntity
   {
      T Clone();
   }
}