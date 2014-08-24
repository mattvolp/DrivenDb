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
      [DataMember]
      private readonly DbChangeType _changeType;

      [DataMember]
      private readonly string _affectedTable;

      [DataMember]
      private readonly IEnumerable<string> _affectedColumns;

      [DataMember]
      private readonly IDbEntity _entity;

      public DbChange(
         DbChangeType changeType, 
         string affectedTable, 
         IEnumerable<string> affectedColumns, 
         IDbEntity entity
         )
      {
         _changeType = changeType;
         _affectedTable = affectedTable;
         _affectedColumns = affectedColumns;
         _entity = entity;         
      }

      public DbChangeType ChangeType
      {
         get { return _changeType; }
      }

      public string AffectedTable
      {
         get { return _affectedTable; }
      }

      public IEnumerable<string> AffectedColumns
      {
         get { return _affectedColumns; }
      }

      public IDbEntity Entity
      {
         get { return _entity; }
      }
   }
}