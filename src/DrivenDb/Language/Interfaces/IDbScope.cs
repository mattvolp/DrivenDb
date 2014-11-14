/**************************************************************************************
 * Original Author : Anthony Leatherwood (adleatherwood@gmail.com)                              
 * Source Location : https://github.com/Fastlite/DrivenDb    
 *  
 * This source is subject to the Microsoft Public License.
 * Link: http://www.microsoft.com/en-us/openness/licenses.aspx
 *  
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
 * EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE. 
 **************************************************************************************/

using System;
using System.Collections.Generic;

namespace DrivenDb
{
   public interface IDbScope : IDisposable
   {
      void WriteEntity<T>(T entity)
         where T : IDbEntity;

      void WriteEntities<T>(IEnumerable<T> entities)
         where T : IDbEntity;

      void Commit();
      void Execute(string query, params object[] parameters);
   }
}