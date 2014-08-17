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
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization;
using Fastlite.DrivenDb.Core.Contracts.Interfaces;

namespace Fastlite.DrivenDb.Core.Contracts
{
   [DataContract]
   public abstract class DbEntity<T> 
      : DbRecord<T>
      , IDbEntity<T> where T : DbEntity<T>, IDbEntity, INotifyPropertyChanged, new()
   {
      protected DbEntity()
      {
         Initialize();
      }

      public IDbEntity<T> Entity
      {
         get { return this; }
      }

      protected virtual void BeforeSerialization()
      {
      }

      [OnSerializing]
      private void OnSerializing(StreamingContext context)
      {
         BeforeSerialization();
      }

      protected virtual void AfterDeserialization()
      {
      }

      [OnDeserialized]
      private void OnDeserialized(StreamingContext context)
      {
         Initialize();
         AfterDeserialization();
      }

      private void Initialize()
      {
         __instance.PropertyChanged += (s, e) =>
            {
               if (__columns.ContainsKey(e.PropertyName))
               {
                  __changes.Add(e.PropertyName);
                  __lastModified = DateTime.Now;

                  if (__state != EntityState.New)
                  {
                     ChangeState(EntityState.Modified);
                  }
               }
            };
      }

      void IDbEntity.Delete()
      {
         if (__state != EntityState.Deleted)
         {
            __preDeletedState = __state;

            ChangeState(EntityState.Deleted);
         }
      }

      void IDbEntity.Undelete()
      {
         if (__state == EntityState.Deleted)
         {
            __state = __preDeletedState;

            ChangeState(__preDeletedState);
         }
      }

      public T ToNew()
      {
         var result = new T();

         result.Update(this.__instance, false);

         if (__identityColumn.Value != null)
         {
            var property = __entityAccessor.GetPropertyInfo(__identityColumn.Key);

            var value = property.PropertyType.IsValueType
               ? Activator.CreateInstance(property.PropertyType)
               : null;

            result.Entity.SetProperty(__identityColumn.Key, value);
            result.__changes.Remove(__identityColumn.Key);
         }

         result.__state = EntityState.New;

         return result;
      }

      public T ToUpdate()
      {
         var result = new T();

         result.Update(this.__instance, false);

         if (__identityColumn.Value != null)
         {
            result.__changes.Remove(__identityColumn.Key);
         }

         result.__state = EntityState.Modified;

         return result;
      }

      public TOther ToMapped<TOther>()
         where TOther : new()
      {
         var result = new TOther();

         MapTo(result);

         return result;
      }

      public void MapTo<TOther>(TOther other)
      {
         var targets = other.GetType()
            .GetProperties();

         foreach (var target in targets)
         {
            if (target.CanWrite && __entityAccessor.CanReadProperty(target.Name))
            {
               target.SetValue(other, __entityAccessor.GetPropertyValue<object>(this.__instance, target.Name), null);
            }
         }
      }

      public void Update(T other, bool checkIdentity = true)
      {
         if (checkIdentity && !this.Entity.SameAs(other))
         {
            throw new InvalidDataException("Cannot update mismatched records");
         }

         var properties = __entityAccessor.GetProperties();

         foreach (var property in properties)
         {
            if (__entityAccessor.CanReadProperty(property.Name)
               && __entityAccessor.CanWriteProperty(property.Name)
               )
            {
               Entity.SetProperty(property.Name, other.GetProperty(property.Name));
            }
         }

         __lastModified = DateTime.Now;
      }

      public T Clone()
      {
         var result = new T();
         var properties = __entityAccessor.GetProperties();

         foreach (var property in properties)
         {
            if (__entityAccessor.CanReadProperty(property.Name)
               && __entityAccessor.CanWriteProperty(property.Name)
               )
            {
               var value = __entityAccessor.GetPropertyValue<object>(__instance, property.Name);

               __entityAccessor.SetPropertyValue(result, property.Name, value);
            }
         }

         result.Reset();

         foreach (var change in __changes)
         {
            result.__changes.Add(change);
         }

         result.__lastModified = this.__lastModified;
         result.__lastUpdated = this.__lastUpdated;
         result.__state = this.__state;

         return result;
      }

      void IDbEntity<T>.Merge(T other, bool checkIdentity)
      {
         if (checkIdentity && !this.Entity.SameAs(other))
         {
            throw new InvalidDataException("Cannot update mismatched records");
         }

         var lastModified = __lastModified;
         var lastUpdated = __lastUpdated;
         var changes = new HashSet<string>(__changes);

         if (__lastUpdated < other.LastUpdated)
         {
            var properties = __entityAccessor.GetProperties();

            foreach (var property in properties)
            {
               if (!changes.Contains(property.Name))
               {
                  Entity.SetProperty(property.Name, other.GetProperty(property.Name));
               }
            }
         }

         foreach (var change in other.Changes)
         {
            if (!changes.Contains(change))
            {
               Entity.SetProperty(change, other.GetProperty(change));
               changes.Add(change);
            }
         }

         __changes = new HashSet<string>(changes);

         var state = other.State == EntityState.Deleted || other.State == EntityState.Modified
                      ? other.State
                      : __state;

         ChangeState(state);

         __lastModified = lastModified.HasValue && lastModified > other.LastModified
                             ? lastModified
                             : other.LastModified;

         __lastUpdated = lastUpdated > other.LastUpdated
                            ? lastUpdated
                            : other.LastUpdated;
      }
   }
}