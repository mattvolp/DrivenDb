using System;
using System.Data;

namespace DrivenDb.Data
{
   // TODO: test
   public class DbConfigBuilder
   {
      private readonly Func<IDbConnection> _connections;      
      private bool _allowQueryScripting;
      private bool _autoApplyDateTimeContraints;
      private bool _allowUnmappedResults;
      private bool _treatStringsAsAnsiStringsByDefault;
      private bool _useCaseInsensitiveColumnMapping;
      private int _useCommandTimeoutOf = 5 * 60 * 1000;

      public DbConfigBuilder(Func<IDbConnection> connections)
      {
         _connections = connections;
      }

      public DbConfigBuilder AllowQueryScripting(bool value)
      {
         _allowQueryScripting = value;
         return this;
      }

      public DbConfigBuilder AutoApplyDateTimeContraints(bool value)
      {
         _autoApplyDateTimeContraints = value;
         return this;
      }

      public DbConfigBuilder AllowUnmappedResults(bool value)
      {
         _allowUnmappedResults = value;
         return this;
      }

      public DbConfigBuilder TreatStringsAsAnsiStringsByDefault(bool value)
      {
         _treatStringsAsAnsiStringsByDefault = value;
         return this;
      }

      public DbConfigBuilder UseCaseInsensitiveColumnMapping(bool value)
      {
         _useCaseInsensitiveColumnMapping = value;
         return this;
      }

      public DbConfigBuilder UseCommandTimeoutOf(int timeout)
      {
         _useCommandTimeoutOf = timeout;
         return this;
      }

      public DbConfig Build()
      {
         return new DbConfig(
            _connections
            , _useCommandTimeoutOf
            , _allowQueryScripting
            , _autoApplyDateTimeContraints
            , _allowUnmappedResults
            , _treatStringsAsAnsiStringsByDefault
            , _useCaseInsensitiveColumnMapping
            );
      }
   }
}