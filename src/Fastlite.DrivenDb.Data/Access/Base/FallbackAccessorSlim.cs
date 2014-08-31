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
using System.Collections;
using System.Collections.Generic;
using Fastlite.DrivenDb.Core.Contracts.Interfaces;
using Fastlite.DrivenDb.Data.Access.Interfaces;

namespace Fastlite.DrivenDb.Data.Access.Base
{
   internal class FallbackAccessorSlim : IFallbackAccessorSlim
   {
      private readonly IDbAccessorSlim _accessor;

      public FallbackAccessorSlim(IDbAccessorSlim accessor)
      {
         _accessor = accessor;
      }

      public IEnumerable<T> ReadValues<T>(string query, params object[] parameters)
      {
         return !DoFallback(parameters)
            ? _accessor.ReadValues<T>(query, parameters)
            : new T[0];
      }

      public T ReadValue<T>(string query, params object[] parameters)
      {
         return !DoFallback(parameters)
            ? _accessor.ReadValue<T>(query, parameters)
            : default(T);
      }

      public IEnumerable<T> ReadAnonymous<T>(T model, string query, params object[] parameters)
      {
         return !DoFallback(parameters)
            ? _accessor.ReadAnonymous(model, query, parameters)
            : new T[0];
      }

      public IEnumerable<T> ReadType<T>(string query, params object[] parameters) 
         where T : new()
      {
         return !DoFallback(parameters)
            ? _accessor.ReadType<T>(query, parameters)
            : new T[0];
      }

      public IEnumerable<T> ReadType<T>(Func<T> factory, string query, params object[] parameters)
      {
         return !DoFallback(parameters)
            ? _accessor.ReadType(factory, query, parameters)
            : new T[0];
      }

      public T ReadEntity<T>(string query, params object[] parameters) 
         where T : IDbEntity, new()
      {
         return !DoFallback(parameters)
            ? _accessor.ReadEntity<T>(query, parameters)
            : default(T);
      }

      public IEnumerable<T> ReadEntities<T>(string query, params object[] parameters)
         where T : IDbEntity, new()
      {
         return !DoFallback(parameters)
            ? _accessor.ReadEntities<T>(query, parameters)
            : new T[0];
      }

      private bool DoFallback(params object[] parameters)
      {
         if (parameters != null && parameters.Length > 0)
         {
            foreach (var parameter in parameters)
            {
               var enumerable = parameter as IEnumerable;
               var isEnumerable = parameter is IEnumerable;
               var isString = (parameter as string) != null;
               var isBytes = (parameter as byte[]) != null;

               if (isEnumerable && !isString && !isBytes && (enumerable == null || !enumerable.GetEnumerator().MoveNext()))
               {
                  return true;
               }
            }
         }

         return false;
      }
   }
}