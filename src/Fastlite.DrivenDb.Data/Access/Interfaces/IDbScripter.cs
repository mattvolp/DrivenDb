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
using System.Data;
using Fastlite.DrivenDb.Core.Contracts.Interfaces;

namespace Fastlite.DrivenDb.Data.Access.Interfaces
{
   public interface IDbScripter
   {
      void ScriptExecute(IDbCommand command, string query, params object[] parameters);

      void ScriptIdentitySelect<T>(IDbCommand command, params object[] parameters)
         where T : IDbRecord, new();

      void ScriptArray<T>(IDbCommand command, string query, params object[] parameters)
         where T : struct;

      void ScriptSelect(IDbCommand command, string query, params object[] parameters);

      void ScriptInsert<T>(IDbCommand command, int index, T entity, bool returnId)
         where T : IDbEntity;

      void ScriptUpdate<T>(IDbCommand command, int index, T entity)
         where T : IDbEntity;

      void ScriptDelete<T>(IDbCommand command, int index, T entity)
         where T : IDbEntity;

      void ScriptRelatedSelect<P, C>(IEnumerable<P> parents, IDbCommand command, string[] pkey, string[] ckey)
         where P : IDbRecord
         where C : IDbRecord, new();
   }
}