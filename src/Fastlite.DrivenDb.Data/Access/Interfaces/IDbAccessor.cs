﻿/**************************************************************************************
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

using Fastlite.DrivenDb.Core.Contracts.Interfaces;

namespace Fastlite.DrivenDb.Data.Access.Interfaces
{
   public interface IDbAccessor : IDbAccessorSlim
   {      
      DbSet<T1, T2> ReadEntities<T1, T2>(string query, params object[] parameters)
         where T1 : IDbRecord, new()
         where T2 : IDbRecord, new();

      DbSet<T1, T2, T3> ReadEntities<T1, T2, T3>(string query, params object[] parameters)
         where T1 : IDbRecord, new()
         where T2 : IDbRecord, new()
         where T3 : IDbRecord, new();

      DbSet<T1, T2, T3, T4> ReadEntities<T1, T2, T3, T4>(string query, params object[] parameters)
         where T1 : IDbRecord, new()
         where T2 : IDbRecord, new()
         where T3 : IDbRecord, new()
         where T4 : IDbRecord, new();

      DbSet<T1, T2, T3, T4, T5> ReadEntities<T1, T2, T3, T4, T5>(string query, params object[] parameters)
         where T1 : IDbRecord, new()
         where T2 : IDbRecord, new()
         where T3 : IDbRecord, new()
         where T4 : IDbRecord, new()
         where T5 : IDbRecord, new();

      DbSet<T1, T2, T3, T4, T5, T6> ReadEntities<T1, T2, T3, T4, T5, T6>(string query, params object[] parameters)
         where T1 : IDbRecord, new()
         where T2 : IDbRecord, new()
         where T3 : IDbRecord, new()
         where T4 : IDbRecord, new()
         where T5 : IDbRecord, new()
         where T6 : IDbRecord, new();

      DbSet<T1, T2, T3, T4, T5, T6, T7> ReadEntities<T1, T2, T3, T4, T5, T6, T7>(string query, params object[] parameters)
         where T1 : IDbRecord, new()
         where T2 : IDbRecord, new()
         where T3 : IDbRecord, new()
         where T4 : IDbRecord, new()
         where T5 : IDbRecord, new()
         where T6 : IDbRecord, new()
         where T7 : IDbRecord, new();

      DbSet<T1, T2, T3, T4, T5, T6, T7, T8> ReadEntities<T1, T2, T3, T4, T5, T6, T7, T8>(string query, params object[] parameters)
         where T1 : IDbRecord, new()
         where T2 : IDbRecord, new()
         where T3 : IDbRecord, new()
         where T4 : IDbRecord, new()
         where T5 : IDbRecord, new()
         where T6 : IDbRecord, new()
         where T7 : IDbRecord, new()
         where T8 : IDbRecord, new();

      DbSet<T1, T2, T3, T4, T5, T6, T7, T8, T9> ReadEntities<T1, T2, T3, T4, T5, T6, T7, T8, T9>(string query, params object[] parameters)
         where T1 : IDbRecord, new()
         where T2 : IDbRecord, new()
         where T3 : IDbRecord, new()
         where T4 : IDbRecord, new()
         where T5 : IDbRecord, new()
         where T6 : IDbRecord, new()
         where T7 : IDbRecord, new()
         where T8 : IDbRecord, new()
         where T9 : IDbRecord, new();

      DbSet<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> ReadEntities<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(string query, params object[] parameters)
         where T1 : IDbRecord, new()
         where T2 : IDbRecord, new()
         where T3 : IDbRecord, new()
         where T4 : IDbRecord, new()
         where T5 : IDbRecord, new()
         where T6 : IDbRecord, new()
         where T7 : IDbRecord, new()
         where T8 : IDbRecord, new()
         where T9 : IDbRecord, new()
         where T10 : IDbRecord, new();
   }
}