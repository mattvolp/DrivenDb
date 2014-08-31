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
using System.Runtime.Serialization;
using Fastlite.DrivenDb.Core.Contracts.Arguments;
using Fastlite.DrivenDb.Core.Contracts.Interfaces;

namespace Fastlite.DrivenDb.Core.Contracts
{
   [DataContract]
   public abstract class DbEntity<T> 
      : DbRecord<T>
      , IDbEntity<T> where T : DbEntity<T>
      , IDbEntity
      , INotifyPropertyChanged
      , new()
   {
      // ReSharper disable InconsistentNaming
      [DataMember]
      protected EntityState __state;

      [DataMember]
      protected HashSet<string> __changes = new HashSet<string>();

      [DataMember]
      protected DateTime? __lastModified;

      [DataMember]
      protected DateTime? __lastUpdated;
      // ReSharper restore InconsistentNaming

      protected DbEntity()
      {
         Initialize();
      }

      public event StateChangedEvent StateChanged;

      public IDbEntity<T> Entity
      {
         get { return this; }
      }
      EntityState IDbEntity.State
      {
         get { return __state; }
      }
      
      DateTime? IDbEntity.LastUpdated
      {
         get { return __lastUpdated; }
      }

      DateTime? IDbEntity.LastModified
      {
         get { return __lastModified; }
      }

      IEnumerable<string> IDbEntity.Changes
      {
         get { return __changes; }
      }

      // TODO: unnecessary, if not inherited
      protected virtual void BeforeSerialization()
      {
      }

      // TODO: unnecessary, if not inherited
      [OnSerializing]
      private void OnSerializing(StreamingContext context)
      {
         BeforeSerialization();
      }

      // TODO: unnecessary, if not inherited
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
         __instance.PropertyChanged += __instance_PropertyChanged;
      }

      void __instance_PropertyChanged(object sender, PropertyChangedEventArgs e)
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
      }

      void IDbEntity.Reset()
      {
         ChangeState(EntityState.Current);

         __lastModified = null;
         __lastUpdated = DateTime.Now;
         __changes.Clear();
      }

      void IDbEntity.Delete()
      {
         if (__state != EntityState.Deleted)
         {
            //__preDeletedState = __state;

            ChangeState(EntityState.Deleted);
         }
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

      protected void ChangeState(EntityState state)
      {
         var previous = __state;

         __state = state;

         if (previous != state && StateChanged != null)
         {
            StateChanged(this, new StateChangedEventArgs(previous, state));
         }
      }
   }
}