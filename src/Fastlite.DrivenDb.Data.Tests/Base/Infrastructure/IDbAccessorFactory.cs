using Fastlite.DrivenDb.Data.Access;
using Fastlite.DrivenDb.Data.Access.Interfaces;

namespace Fastlite.DrivenDb.Data.Tests.Base.Infrastructure
{
   public interface IDbAccessorFactory
   {
      IDbAccessor Create(string database, AccessorOptions options);
   }
}