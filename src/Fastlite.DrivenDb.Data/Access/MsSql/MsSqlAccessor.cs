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
using System.Linq;
using Fastlite.DrivenDb.Core.Contracts.Interfaces;
using Fastlite.DrivenDb.Data.Access.Base;
using Fastlite.DrivenDb.Data.Access.Interfaces;
using Fastlite.DrivenDb.Data.Access.MsSql.Interfaces;

namespace Fastlite.DrivenDb.Data.Access.MsSql
{
   internal class MsSqlAccessor : DbAccessor, IMsSqlAccessor
   {
      private readonly IMsSqlScripter _scripter;
      private readonly IDb _db;

      public MsSqlAccessor(IMsSqlScripter scripter, IDbMapper mapper, IDb db)
         : base(scripter, mapper, db)
      {
         _scripter = scripter;
         _db = db;
      }

      public void WriteEntityUsingScopeIdentity<T>(T entity)
         where T : IDbEntity, new()
      {
         WriteEntitiesUsingScopeIdentity(new[] { entity });
      }

      public void WriteEntitiesUsingScopeIdentity<T>(IEnumerable<T> entities)
         where T : IDbEntity, new()
      {
         WriteEntities(null, null, entities,
                       (c, i, e) => _scripter.ScriptInsertWithScopeIdentity(c, e, i, true)
                       , null
                       , null
                       , null
                       , true
            );
      }

      public Tuple<T, D> WriteEntityAndOutputDeleted<T, D>(T entity, D deleted)
         where T : IDbEntity, new()
         where D : class
      {
         return WriteEntitiesAndOutputDeleted(new[] { entity }, deleted).First();
      }

      public IEnumerable<Tuple<T, D>> WriteEntitiesAndOutputDeleted<T, D>(IEnumerable<T> entities, D deleted)
         where T : IDbEntity, new()
         where D : class
      {
         if (!typeof(D).FullName.Contains("AnonymousType"))
         {
            throw new NotSupportedException("Only anonymous types can be used to specify deleted output");
         }

         var columns = typeof(D).GetProperties().Select(p => p.Name).ToArray();
         var output = new List<Tuple<T, D>>();
         var constructor = typeof(D).GetConstructors()[0];

         WriteEntities(null, null, entities,
                       (c, i, e) => _scripter.ScriptInsert(c, i, e, true),
                       (c, i, e) => _scripter.ScriptUpdateOutputDeleted(c, i, e, columns),
                       (c, i, e) => _scripter.ScriptDeleteOutputDeleted(c, i, e, columns),
                       (i, e, a) => output.Add(new Tuple<T, D>(e, (D) constructor.Invoke(a.Skip(a.Length - columns.Length).ToArray())))
                       , true
            );

         return output;
      }

      IMsSqlScope IMsSqlAccessor.CreateScope()
      {
         return new MsSqlScope(_db, this, _scripter);
      }
   }
}