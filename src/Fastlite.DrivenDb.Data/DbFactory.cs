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
using System.Data;
using Fastlite.DrivenDb.Data.Access;
using Fastlite.DrivenDb.Data.Access.Base;
using Fastlite.DrivenDb.Data.Access.Interfaces;
using Fastlite.DrivenDb.Data.Access.MsSql;
using Fastlite.DrivenDb.Data.Access.MySql;
using Fastlite.DrivenDb.Data.Access.Oracle;
using Fastlite.DrivenDb.Data.Access.SqLite;

namespace Fastlite.DrivenDb.Data
{   
   public static class DbFactory
   {
      public static IDbAccessor CreateAccessor(DbAccessorType type, IDb db)
      {         
         var mapper = new DbMapper(db);

         Func<ISqlBuilder> builders;
         
         var joiner = new ValueJoiner();

         switch (type)
         {
            case DbAccessorType.MsSql:
               {                  
                  var msscripter = new MsSqlScripter(db, joiner, () => new MsSqlBuilder());

                  return new MsSqlAccessor(msscripter, mapper, db);
               }
            case DbAccessorType.SqLite:
               builders = () => new SqLiteBuilder();
               break;
            case DbAccessorType.MySql:
               builders = () => new MySqlBuilder();
               break;
            case DbAccessorType.Oracle:
               builders = () => new OracleBuilder();
               break;
            default:
               throw new InvalidOperationException(String.Format("Unsupported DbAccessorType value of '{0}'", type));
         }

         var scripter = new DbScripter(db, joiner, builders);

         return new DbAccessor(scripter, mapper, db);
      }

      public static IDbAccessor CreateAccessor(DbAccessorType type, Func<IDbConnection> connections)
      {
         return CreateAccessor(type, new Db(type, AccessorOptions.All, connections));
      }

      public static IDbAccessor CreateAccessor(DbAccessorType type, AccessorOptions options, Func<IDbConnection> connections)
      {
         return CreateAccessor(type, new Db(type, options, connections));
      }

      public static IDbAccessorSlim CreateSlimAccessor(DbAccessorType type, IDb db)
      {
         return CreateAccessor(type, db);
      }

      public static IDbAccessorSlim CreateSlimAccessor(DbAccessorType type, Func<IDbConnection> connections)
      {
         return CreateAccessor(type, new Db(type, AccessorOptions.All, connections));
      }

      public static IDbAccessorSlim CreateSlimAccessor(DbAccessorType type, AccessorOptions options, Func<IDbConnection> connections)
      {
         return CreateAccessor(type, new Db(type, options, connections));
      }
   }
}