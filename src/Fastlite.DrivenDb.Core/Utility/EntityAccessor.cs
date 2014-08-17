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
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Fastlite.DrivenDb.Core.Utility.Interfaces;

namespace Fastlite.DrivenDb.Core.Utility
{
   public class EntityAccessor<T> : IEntityAccessor
   {
      private readonly Type _genericType;
      private readonly Type _objectType;
      private readonly Dictionary<string, PropertyInfo> _properties;  
      private readonly Dictionary<string, Func<T, object>> _getters;
      private readonly Dictionary<string, Action<T, object>> _setters;          

      public EntityAccessor(bool caseSensitive = true)
      {
         _genericType = typeof (T);
         _objectType = typeof (object);

         _getters = new Dictionary<string, Func<T, object>>(caseSensitive ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase);
         _setters = new Dictionary<string, Action<T, object>>(caseSensitive ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase);
         _properties = _genericType.GetProperties().ToDictionary(p => p.Name, caseSensitive ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase);

         CompileGetters();
         CompileSetters();
      }

      public bool HasProperty(string name)
      {
         return _getters.ContainsKey(name) || _setters.ContainsKey(name);
      }

      public bool CanReadProperty(string name)
      {
         return _getters.ContainsKey(name);
      }

      public bool CanWriteProperty(string name)
      {
         return _setters.ContainsKey(name);
      }

      public new Type GetType()
      {
         return typeof (T);
      }

      public IEnumerable<PropertyInfo> GetProperties()
      {
         return _properties.Values;
      }

      public PropertyInfo GetPropertyInfo(string name)
      {
         PropertyInfo property;

         if (!_properties.TryGetValue(name, out property))
         {
            throw new InvalidDataException(String.Format(@"Field {0} is missing", _genericType.Name));
         }

         return property;
      }

      public TValue GetPropertyValue<TValue>(T instance, string name)
      {
         Func<T, object> getter;

         if (!_getters.TryGetValue(name, out getter))
         {
            throw new InvalidDataException(String.Format(@"Field {0} is missing", _genericType.Name));
         }

         return (TValue) getter(instance);
      }

      public void SetPropertyValue<TValue>(T instance, string name, TValue value)
      {
         Action<T, object> setter;

         if (!_setters.TryGetValue(name, out setter))
         {
            throw new InvalidDataException(String.Format(@"Field {0} is missing", _genericType.Name));
         }

         setter(instance, value);
      }

      private void CompileGetters()
      {
         foreach (var property in GetProperties().Where(p => p.CanRead))
         {
            var parameter = Expression.Parameter(_genericType);
            var compiled = Expression.Lambda<Func<T, object>>(
               Expression.Convert(
                  Expression.Property(
                     parameter, property.Name
                     ), _objectType
                  ), parameter
               ).Compile();

            _getters.Add(property.Name, compiled);
         }
      }

      private void CompileSetters()
      {
         foreach (var property in GetProperties().Where(p => p.CanWrite))
         {
            var setter = property.GetSetMethod();

            if (setter != null)
            {
               var parameter1 = Expression.Parameter(_genericType);
               var parameter2 = Expression.Parameter(_objectType);
               var compiled = Expression.Lambda<Action<T, object>>(
                  Expression.Call(parameter1, setter, Expression.Convert(parameter2, property.PropertyType)
                     ), parameter1, parameter2
                  ).Compile();

               _setters.Add(property.Name, compiled);
            }
         }
      }
   }
}