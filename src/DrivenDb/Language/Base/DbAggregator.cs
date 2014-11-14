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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DrivenDb.Base
{
   internal class DbAggregator : IDbAggregator
   {
      public void WriteAggregate(IDbAccessor accessor, IDbAggregate aggregate)
      {
         if (aggregate == null)
         {
            return;
         }

         WriteAggregates(accessor, new[] {aggregate});
      }

      public void WriteAggregates(IDbAccessor accessor, IEnumerable<IDbAggregate> aggregates)
      {
         if (aggregates == null || !aggregates.Any())
         {
            return;
         }

         var aggregateTypes = aggregates
            .Where(a => a != null)
            .ToLookup(a => a.GetType());

         foreach (var aggregateType in aggregateTypes)
         {
            WriteAggregatesOfSameType(accessor, aggregateType);
         }
      }

      private static void WriteAggregatesOfSameType(IDbAccessorSlim accessor, IEnumerable<IDbAggregate> aggregates)
      {
         var primaryGetter = UpdatePrimaries(accessor, aggregates);

         UpdateForeigns(accessor, aggregates, primaryGetter);
         UpdateAggregates(accessor, aggregates);
      }

      private static void UpdateAggregates(IDbAccessorSlim accessor, IEnumerable<IDbAggregate> aggregates)
      {
         //var aggregateProperties = GetAggregateProperties(aggregates.First().GetType());
         //var aggregateTypes = aggregates
         //   .SelectMany(a => aggregateProperties.Select(p => p.Item2.Invoke(a, null)))
         //   .ToLookup(a => a.GetType());

         //foreach (var aggregateType in aggregateTypes)
         //{
         //   var primaryGetter = GetPrimaryGetter(aggregateType.First().GetType());

         //   foreach (var aggregate in aggregates)
         //   {
         //      var foreignPrimary = primaryGetter.Invoke(aggregate, null);
         //      var mappings = GetForeignMappings()

         //      foreach (var aggregateProperty in aggregateProperties)
         //      {
         //         UpdateForeign(aggregate, foreignPrimary, mappings);
         //      }
         //   }
         //}
      }

      private static MethodInfo UpdatePrimaries(IDbAccessorSlim accessor, IEnumerable<IDbAggregate> aggregates)
      {
         var primaryGetter = GetPrimaryGetter(aggregates.First().GetType());
         var primaries = aggregates.Select(a => primaryGetter.Invoke(a, null))
            .Where(p => p != null)
            .Cast<IDbEntity>()
            .ToArray();

         accessor.WriteEntities(primaries);

         return primaryGetter;
      }

      private static void UpdateForeigns(IDbAccessorSlim accessor, IEnumerable<IDbAggregate> aggregates, MethodInfo primaryGetter)
      {         
         var foreignProperties = GetForeignProperties(aggregates.First().GetType());
         var foreigns = new List<IDbEntity>();

         foreach (var aggregate in aggregates)
         {
            var primary = primaryGetter.Invoke(aggregate, null);

            foreach (var foreignProperty in foreignProperties)
            {
               var mappings = GetForeignMappings(foreignProperty.Item1);
               var foreignObject = foreignProperty.Item2.Invoke(aggregate, null);
               var foreignEnumerable = foreignObject as IEnumerable;

               if (foreignEnumerable != null)
               {
                  foreach (var foreign in foreignEnumerable)
                  {
                     UpdateForeign(primary, (IDbEntity) foreign, mappings);

                     foreigns.Add((IDbEntity) foreign);
                  }
               }
               else
               {
                  var foreign = (IDbEntity) foreignObject;

                  UpdateForeign(primary, foreign, mappings);

                  foreigns.Add(foreign);
               }
            }
         }

         accessor.WriteEntities(foreigns);
      }

      private static void UpdateForeign(object primary, IDbEntity foreign, IEnumerable<DbForeignAttribute> mappings)
      {
         foreach (var mapping in mappings)
         {
            var propertyGetter = GetPropertyGetter(primary, mapping.PrimaryProperty);
            var propertySetter = GetPropertySetter(foreign, mapping.ForeignProperty);

            var value = propertyGetter.Invoke(primary, null);

            propertySetter.Invoke(foreign, new[] {value});
         }
      }

      private static MethodInfo GetPropertyGetter(object primary, string primaryProperty)
      {
         return primary.GetType().GetProperty(primaryProperty).GetGetMethod();
      }

      private static MethodInfo GetPropertySetter(IDbEntity foreign, string foreignProperty)
      {
         return foreign.GetType().GetProperty(foreignProperty).GetSetMethod();
      }

      private static IEnumerable<DbForeignAttribute> GetForeignMappings(PropertyInfo property)
      {
         return property.GetCustomAttributes(typeof (DbForeignAttribute), true)
            .Cast<DbForeignAttribute>()
            .ToArray();
      }

      private static MethodInfo GetPrimaryGetter(IReflect type)
      {
         var getters = type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(p => p.GetCustomAttributes(typeof (DbPrimaryAttribute), true).Any())
            .Select(p => p.GetGetMethod())
            .ToArray();

         if (getters.Count() != 1)
         {
            throw new InvalidAggregateStructure();
         }

         return getters.Single();
      }

      private static Tuple<PropertyInfo, MethodInfo>[] GetForeignProperties(IReflect type)
      {
         return type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(p =>    p.PropertyType.GetInterface(typeof (IDbAggregate).Name) == null
                        && p.PropertyType.GetInterface(typeof(IEnumerable<IDbAggregate>).Name) == null
                        && p.GetCustomAttributes(typeof (DbForeignAttribute), true).Any())
            .Select(p => new Tuple<PropertyInfo, MethodInfo>(p, p.GetGetMethod()))
            .ToArray();
      }

      private static Tuple<PropertyInfo, MethodInfo>[] GetAggregateProperties(Type type)
      {
         return type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(p =>    p.PropertyType.GetInterface(typeof (IDbAggregate).Name) != null
                        || p.PropertyType.GetInterface(typeof (IEnumerable<IDbAggregate>).Name) != null)
            .Select(p => new Tuple<PropertyInfo, MethodInfo>(p, p.GetGetMethod()))
            .ToArray();
      }
   }
}
