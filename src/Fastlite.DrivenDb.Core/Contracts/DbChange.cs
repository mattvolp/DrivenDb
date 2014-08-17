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
using System.Runtime.Serialization;
using Fastlite.DrivenDb.Core.Contracts.Interfaces;

namespace Fastlite.DrivenDb.Core.Contracts
{
   [DataContract]
   public class DbChange
   {
      public DbChange(DbChangeType changeType, string affectedTable, IEnumerable<string> affectedColumns, IDbEntity entity)
      {
         ChangeType = changeType;
         AffectedTable = affectedTable;
         AffectedColumns = affectedColumns;
         Entity = entity;
      }

      [DataMember]
      public DbChangeType ChangeType
      {
         get;
         private set;
      }

      [DataMember]
      public string AffectedTable
      {
         get;
         private set;
      }

      [DataMember]
      public IEnumerable<string> AffectedColumns
      {
         get;
         private set;
      }

      public IDbEntity Entity
      {
         get;
         private set;
      }
   }
}