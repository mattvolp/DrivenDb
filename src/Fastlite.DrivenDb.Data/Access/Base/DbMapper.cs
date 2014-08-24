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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Fastlite.DrivenDb.Core.Contracts.Attributes;
using Fastlite.DrivenDb.Core.Contracts.Exceptions;
using Fastlite.DrivenDb.Core.Contracts.Interfaces;
using Fastlite.DrivenDb.Data.Access.Interfaces;
using Fastlite.DrivenDb.Data.Utility;

namespace Fastlite.DrivenDb.Data.Access.Base
{
   internal class DbMapper : IDbMapper
   {
      private static readonly MethodInfo _isDbNull = typeof (IDataRecord).GetMethod("IsDBNull");
      private static readonly MethodInfo _getValue = typeof (IDataRecord).GetMethod("GetValue");

      private static readonly Dictionary<RuntimeTypeHandle, MethodInfo> _methods;

      static DbMapper()
      {
         _methods = new Dictionary<RuntimeTypeHandle, MethodInfo>();

         // non-nullable
         _methods[typeof (char).TypeHandle] = typeof (IDataRecord).GetMethod("GetChar");
         _methods[typeof (byte).TypeHandle] = typeof (IDataRecord).GetMethod("GetByte");
         _methods[typeof (byte[]).TypeHandle] = typeof (IDataRecord).GetMethod("GetBytes");
         _methods[typeof (short).TypeHandle] = typeof (IDataRecord).GetMethod("GetInt16");
         _methods[typeof (int).TypeHandle] = typeof (IDataRecord).GetMethod("GetInt32");
         _methods[typeof (long).TypeHandle] = typeof (IDataRecord).GetMethod("GetInt64");
         _methods[typeof (float).TypeHandle] = typeof (IDataRecord).GetMethod("GetFloat");
         _methods[typeof (double).TypeHandle] = typeof (IDataRecord).GetMethod("GetDouble");
         _methods[typeof (decimal).TypeHandle] = typeof (IDataRecord).GetMethod("GetDecimal");
         _methods[typeof (bool).TypeHandle] = typeof (IDataRecord).GetMethod("GetBoolean");
         _methods[typeof (string).TypeHandle] = typeof (IDataRecord).GetMethod("GetString");
         _methods[typeof (Guid).TypeHandle] = typeof (IDataRecord).GetMethod("GetGuid");
         _methods[typeof (DateTime).TypeHandle] = typeof (IDataRecord).GetMethod("GetDateTime");
         _methods[typeof (TimeSpan).TypeHandle] = typeof (IDataRecord).GetMethod("GetDateTime");

         // nullable
         _methods[typeof (byte?).TypeHandle] = typeof (IDataRecord).GetMethod("GetByte");
         _methods[typeof (short?).TypeHandle] = typeof (IDataRecord).GetMethod("GetInt16");
         _methods[typeof (int?).TypeHandle] = typeof (IDataRecord).GetMethod("GetInt32");
         _methods[typeof (long?).TypeHandle] = typeof (IDataRecord).GetMethod("GetInt64");
         _methods[typeof (float?).TypeHandle] = typeof (IDataRecord).GetMethod("GetFloat");
         _methods[typeof (double?).TypeHandle] = typeof (IDataRecord).GetMethod("GetDouble");
         _methods[typeof (decimal?).TypeHandle] = typeof (IDataRecord).GetMethod("GetDecimal");
         _methods[typeof (bool?).TypeHandle] = typeof (IDataRecord).GetMethod("GetBoolean");
         _methods[typeof (char?).TypeHandle] = typeof (IDataRecord).GetMethod("GetChar");
         _methods[typeof (Guid?).TypeHandle] = typeof (IDataRecord).GetMethod("GetGuid");
         _methods[typeof (DateTime?).TypeHandle] = typeof (IDataRecord).GetMethod("GetDateTime");
         _methods[typeof (TimeSpan?).TypeHandle] = typeof (IDataRecord).GetMethod("GetDateTime");
      }

      private readonly ConcurrentDictionary<QueryIdentity, QueryMapper> _cache = new ConcurrentDictionary<QueryIdentity, QueryMapper>();
      private readonly ConcurrentDictionary<Type, AnonFactory> _factories = new ConcurrentDictionary<Type, AnonFactory>();
      private readonly IDb _db;

      public DbMapper(IDb db)
      {
         _db = db;
      }

      public T MapValue<T>(IDataReader reader)
      {
         var isNullable = Equals(default(T), null);

         if (reader.Read())
         {
            if (reader.IsDBNull(0))
            {
               if (!isNullable)
               {
                  throw new InvalidCastException("Unable to convert DbNull value to no-nullable value");
               }

               return default(T);
            }

            return (T) reader.GetValue(0);
         }

         return default(T);
      }

      public string MapValue(IDataReader reader)
      {
         if (reader.Read())
         {
            return reader.GetString(0);
         }

         return null;
      }

      public IEnumerable<T> MapValues<T>(IDataReader reader)
      {
         var result = new List<T>();
         var isNullable = Equals(default(T), null);

         while (reader.Read())
         {
            if (reader.IsDBNull(0))
            {
               if (!isNullable)
               {
                  throw new InvalidCastException("Unable to convert DbNull value to non-nullable value");
               }

               result.Add(default(T));
            }
            else
            {
               result.Add((T) reader.GetValue(0));
            }
         }

         return result;
      }

      public T MapEntity<T>(string query, IDataReader reader)
         where T : IDbRecord, new()
      {
         var mapper = GetMapper<T>(query, reader);
         var result = new List<T>();

         while (reader.Read())
         {
            var gnu = new T();

            mapper(reader, gnu);

            gnu.Reset();

            result.Add(gnu);
         }

         return result.SingleOrDefault();
      }

      public IEnumerable<T> MapEntities<T>(string query, IDataReader reader)
         where T : IDbRecord, new()
      {
         var mapper = GetMapper<T>(query, reader);
         var names = new List<String>();

         for (var i = 0; i < reader.FieldCount; i++)
         {
            names.Add(reader.GetName(i));
         }

         var holders = new List<DataHolder<T>>();

         while (reader.Read())
         {
            var values = new object[reader.FieldCount];

            reader.GetValues(values);
            holders.Add(new DataHolder<T>() {DataRecord = new DataRecord(names, values)});
         }

         Parallel.ForEach(holders, h =>
            {
               var gnu = new T();

               mapper(h.DataRecord, gnu);

               h.Entity = gnu;
               h.Entity.Reset();
            });

         return holders.Select(h => h.Entity).ToArray();
      }

      public IEnumerable<T> MapType<T>(string query, IDataReader reader)
         where T : new()
      {
         return MapType<T>(query, reader, () => new T());
      }

      public IEnumerable<T> MapType<T>(string query, IDataReader reader, Func<T> factory)
      {
         var mapper = GetMapper<T>(query, reader);
         var result = new List<T>();

         while (reader.Read())
         {
            var gnu = factory();

            mapper(reader, gnu);

            result.Add(gnu);
         }

         return result;
      }

      public IEnumerable<T> MapAnonymous<T>(T model, string query, IDataReader reader)
      {
         var type = typeof (T);

         AnonFactory info;

         if (!_factories.TryGetValue(type, out info))
         {
            _factories.AddOrUpdate(type, new AnonFactory(type), (t, a) => new AnonFactory(type));
            info = _factories[type];
         }

         var result = new List<T>();
         var values = new object[info.ParamCount];

         while (reader.Read())
         {
            reader.GetValues(values);

            for (var i = 0; i < values.Length; i++)
            {
               if (values[i] == DBNull.Value)
               {
                  values[i] = null;
               }
            }

            var gnu = info.Create<T>(values);

            result.Add(gnu);
         }

         return result;
      }

      public Action<IDataRecord, T> GetMapper<T>(string query, IDataRecord reader)
      {
         var identity = new QueryIdentity(query, typeof (T));

         QueryMapper info;

         if (!_cache.TryGetValue(identity, out info))
         {
            var mapper = BuildMapper<T>(reader);

            info = new QueryMapper(mapper);

            _cache[identity] = info;
         }

         return (Action<IDataRecord, T>) info.Deserializer;
      }

      private Action<IDataRecord, T> BuildMapper<T>(IDataRecord reader)
      {
         var fields = BuildFieldDictionary<T>();
         var properties = BuildPropertyDictionary<T>();

         //
         // build expression 
         //
         var assignments = new List<Expression>();
         var record = Expression.Parameter(typeof (IDataRecord), "record");
         var entity = Expression.Parameter(typeof (T), "output");

         for (var i = 0; i < reader.FieldCount; i++)
         {
            var columnName = reader.GetName(i);
            var columnType = reader.GetFieldType(i);

            MethodInfo method;

            if (!_methods.TryGetValue(columnType.TypeHandle, out method))
            {
               throw new InvalidDataException("No IDataRecord access method defined for column '" + columnName + "', type '" + columnType.Name + "' in DbMapper.");
            }

            ConditionalExpression assignment = null;
            PropertyInfo property = null;
            FieldInfo field = null;

            if (properties.TryGetValue(columnName, out property))
            {
               var target = Expression.Property(entity, property.Name);
               assignment = BuildAssignmentExpression(record, columnType, method, i, property.PropertyType, target);
            }
            else if (fields.TryGetValue(columnName, out field))
            {
               var target = Expression.Field(entity, field.Name);
               assignment = BuildAssignmentExpression(record, columnType, method, i, field.FieldType, target);
            }

            if (assignment == null && !_db.AllowUnmappedColumns)
            {
               throw new InactiveExtensionException(AccessorOptions.AllowUnmappedColumns.ToString(), "Unable to map column '" + columnName + "', type '" + columnType.Name + "' on target type '" + typeof (T).Name + "'.");
            }
            else if (assignment != null)
            {
               assignments.Add(assignment);
            }
         }

         var block = Expression.Block(assignments.ToArray());

         return Expression.Lambda<Action<IDataRecord, T>>(block, record, entity).Compile();
      }

      private static ConditionalExpression BuildAssignmentExpression(ParameterExpression record, Type columnType, MethodInfo method, int columnIndex, Type targetType, MemberExpression target)
      {
         if (targetType == typeof (TimeSpan) || Nullable.GetUnderlyingType(targetType) == typeof (TimeSpan))
         {
            var value = Expression.Variable(typeof (object));

            return Expression.IfThen(
               Expression.Not(Expression.Call(record, _isDbNull, Expression.Constant(columnIndex, typeof (int)))),
               Expression.Block(new[] {value},
                  Expression.Assign(value, Expression.Call(record, _getValue, Expression.Constant(columnIndex, typeof (int)))),
                  Expression.Assign(target, Expression.Convert(value, targetType))
                  ));
         }
         else if (method.Name == "GetBytes")
         {
            var lzero = Expression.Constant(0L);
            var izero = Expression.Constant(0);
            var imax = Expression.Constant(1024);
            var lmax = Expression.Constant(1024L);
            var lred = Expression.Variable(typeof (long));
            var loffset = Expression.Variable(typeof (long));
            var buffer = Expression.Variable(typeof (byte[]));
            var memory = Expression.Variable(typeof (MemoryStream));
            var breaker = Expression.Label();

            return Expression.IfThen(
               Expression.Not(Expression.Call(record, _isDbNull, Expression.Constant(columnIndex, typeof (int)))),
               Expression.Block(new[] {lred, loffset, buffer, memory},
                  Expression.Assign(memory, Expression.New(typeof (MemoryStream).GetConstructors()[0])),
                  Expression.Assign(buffer, Expression.New(typeof (byte[]).GetConstructors()[0], Expression.Constant(1024))),
                  Expression.Assign(loffset, lzero),
                  Expression.TryFinally(
                     Expression.Block(
                        Expression.Loop(
                           Expression.Block(
                              Expression.Assign(lred, Expression.Call(record, method, Expression.Constant(columnIndex), loffset, buffer, izero, imax)),
                              Expression.Call(memory, typeof (MemoryStream).GetMethod("Write"), buffer, izero, Expression.Convert(lred, typeof (int))),
                              Expression.AddAssign(loffset, lred),
                              Expression.IfThen(Expression.NotEqual(lred, lmax), Expression.Break(breaker))
                              ),
                           breaker
                           ),
                        Expression.Assign(Expression.Property(memory, typeof (MemoryStream).GetProperty("Position")), lzero),
                        Expression.Assign(target, Expression.Call(memory, typeof (MemoryStream).GetMethod("ToArray")))
                        ),
                     Expression.Call(memory, typeof (MemoryStream).GetMethod("Dispose"))
                     )
                  )
               );
         }
         else if (targetType == columnType)
         {
            return Expression.IfThen(
               Expression.Not(Expression.Call(record, _isDbNull, Expression.Constant(columnIndex, typeof (int)))),
               Expression.Assign(target, Expression.Call(record, method, Expression.Constant(columnIndex, typeof (int))))
               );
         }
         else if (targetType.IsEnum || Nullable.GetUnderlyingType(targetType) == columnType)
         {
            return Expression.IfThen(
               Expression.Not(Expression.Call(record, _isDbNull, Expression.Constant(columnIndex, typeof (int)))),
               Expression.Assign(target, Expression.Convert(Expression.Call(record, method, Expression.Constant(columnIndex, typeof (int))), targetType))
               );
         }

         return null;
      }

      private Dictionary<string, FieldInfo> BuildFieldDictionary<T>()
      {
         var fields = _db.CaseInsensitiveColumnMapping
            ? new Dictionary<string, FieldInfo>(StringComparer.CurrentCultureIgnoreCase)
            : new Dictionary<string, FieldInfo>();

         var bindings = _db.PrivateMemberColumnMapping
            ? BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
            : BindingFlags.Public | BindingFlags.Instance;

         foreach (var field in typeof (T).GetFields(bindings))
         {
            var attribute = field.GetCustomAttributes(typeof (DbColumnAttribute), true)
               .Cast<DbColumnAttribute>()
               .SingleOrDefault();

            if (attribute != null)
            {
               fields.Add(attribute.Name ?? field.Name, field);
            }
            else
            {
               fields.Add(field.Name, field);
            }
         }

         return fields;
      }

      private Dictionary<string, PropertyInfo> BuildPropertyDictionary<T>()
      {
         var properties = _db.CaseInsensitiveColumnMapping
            ? new Dictionary<string, PropertyInfo>(StringComparer.CurrentCultureIgnoreCase)
            : new Dictionary<string, PropertyInfo>();

         var bindings = _db.PrivateMemberColumnMapping
            ? BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
            : BindingFlags.Public | BindingFlags.Instance;

         foreach (var property in typeof (T).GetProperties(bindings))
         {
            var attribute = property.GetCustomAttributes(typeof (DbColumnAttribute), true)
               .Cast<DbColumnAttribute>()
               .SingleOrDefault();

            if (attribute != null)
            {
               properties.Add(attribute.Name ?? property.Name, property);
            }
            else
            {
               properties.Add(property.Name, property);
            }
         }

         return properties;
      }
   }
}