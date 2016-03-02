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
using System.Data;
using DrivenDb.Base;
using DrivenDb.MsSql;
using DrivenDb.SqLite;
using DrivenDb.MySql;
using DrivenDb.Collections;
using DrivenDb.Oracle;

namespace DrivenDb
{
   // TODO: review tests to make sure they are universal and clean up after themselves tryf
   public static class DbFactory
   {
      public static IDbAccessor CreateAccessor(DbAccessorType type, IDb db)
      {
         var setup = GetSetup(type, db);

         if (type == DbAccessorType.MsSql)
         {
            return new MsSqlAccessor((IMsSqlScripter)setup.Scripter, setup.Mapper, db, setup.Aggregator);
         }

         return new DbAccessor(setup.Scripter, setup.Mapper, db, setup.Aggregator);
      }

      public static IDbAccessor CreateAccessor(DbAccessorType type, Func<IDbConnection> connections)
      {
         return CreateAccessor(type, new Db(type, AccessorExtension.None, connections));
      }

      public static IDbAccessor CreateAccessor(DbAccessorType type, AccessorExtension extensions, Func<IDbConnection> connections)
      {
         return CreateAccessor(type, new Db(type, extensions, connections));
      }

      public static IDbAccessor CreateAccessor(Func<ISqlBuilder> builder, AccessorExtension extensions, Func<IDbConnection> connections)
      {

         var db = new Db(extensions, connections);
         var aggregator = new DbAggregator();

         return new DbAccessor(new DbScripter(db, new ValueJoiner(), builder), new DbMapper(db), db, aggregator);
      }

      public static IDbAccessorSlim CreateSlimAccessor(DbAccessorType type, IDb db)
      {
         return CreateAccessor(type, db);
      }

      public static IDbAccessorSlim CreateSlimAccessor(DbAccessorType type, Func<IDbConnection> connections)
      {
         return CreateAccessor(type, new Db(type, AccessorExtension.None, connections));
      }

      public static IDbAccessorSlim CreateSlimAccessor(DbAccessorType type, AccessorExtension extensions, Func<IDbConnection> connections)
      {
         return CreateAccessor(type, new Db(type, extensions, connections));
      }

      public static IDbAccessorSlim CreateSlimAccessor(Func<ISqlBuilder> builder, AccessorExtension extensions, Func<IDbConnection> connections)
      {
         var db = new Db(extensions, connections);
         var aggregator = new DbAggregator();

         return new DbAccessor(new DbScripter(db, new ValueJoiner(), builder), new DbMapper(db), new Db(extensions, connections), aggregator);
      }

      public static IDbIndex<K, T> CreateIndex<K, T>(IDbIndexCore<K, T> core)
          where T : class, IDbEntity<T>
      {
         return new DbIndex<K, T>(core);
      }

      public static IDbIndex<K, T> CreateIndex<K, T>(Func<T, K> extractor)
          where T : class, IDbEntity<T>
      {
         return new DbIndex<K, T>(new DbIndexCore<K, T>(extractor));
      }

      public static IDbCache<K, T, I> CreateCache<K, T, I>(IDbCacheCore<K, T, I> core)
          where T : class, IDbEntity<T>
      {
         return new DbCache<K, T, I>(core);
      }

      internal static AccessorSetup GetSetup(DbAccessorType type, IDb db)
      {
         var aggregator = new DbAggregator();
         var mapper = new DbMapper(db);

         Func<ISqlBuilder> builders;
         IValueJoiner joiner;

         switch (type)
         {
            case DbAccessorType.MsSql:
               {
                  Func<IMsSqlBuilder> msbuilders = () => new MsSqlBuilder();

                  var msjoiner = new MsSqlValueJoiner();
                  var msscripter = new MsSqlScripter(db, msjoiner, msbuilders);

                  return new AccessorSetup(msscripter, mapper, aggregator);
               }
            case DbAccessorType.SqLite:
               {
                  builders = () => new SqLiteBuilder();
                  joiner = new SqLiteValueJoiner();
               }
               break;
            case DbAccessorType.MySql:
               {
                  builders = () => new MySqlBuilder();
                  joiner = new MySqlValueJoiner();
               }
               break;
            case DbAccessorType.Oracle:
               {
                  builders = () => new OracleBuilder();
                  joiner = new OracleValueJoiner();
               }
               break;
            default:
               throw new InvalidOperationException(string.Format("Unsupported DbAccessorType value of '{0}'", type));
         }

         var scripter = new DbScripter(db, joiner, builders);

         return new AccessorSetup(scripter, mapper, aggregator);
      }

      internal sealed class AccessorSetup
      {
         private readonly IDbScripter _scripter;
         private readonly IDbMapper _mapper;
         private readonly IDbAggregator _aggregator;

         public AccessorSetup(IDbScripter scripter, IDbMapper mapper, IDbAggregator aggregator)
         {
            _scripter = scripter;
            _mapper = mapper;
            _aggregator = aggregator;
         }

         public IDbScripter Scripter
         {
            get { return _scripter;}
         }

         public IDbMapper Mapper
         {
            get { return _mapper; }
         }

         public IDbAggregator Aggregator
         {
            get { return _aggregator; }
         }
      }
   }
}