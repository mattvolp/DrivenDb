using System;
using System.Data;

namespace DrivenDb.Data
{
   public class DbConfig
   {
      // TODO: test
      internal DbConfig(
         Func<IDbConnection> connectionFactory,
         int commandTimeout,         
         bool allowQueryScripting,
         bool autoApplyDateTimeContraints,
         bool allowUnmappedResults,
         bool treatStringsAsAnsiStringsByDefault,
         bool useCaseInsensitiveColumnMapping
         
         )
      {
         ConnectionFactory = connectionFactory;
         CommandTimeout = commandTimeout;
         AllowQueryScripting = allowQueryScripting;
         AutoApplyDateTimeContraints = autoApplyDateTimeContraints;
         AllowUnmappedResults = allowUnmappedResults;
         TreatStringsAsAnsiStrings = treatStringsAsAnsiStringsByDefault;
         UseCaseInsensitiveColumnMapping = useCaseInsensitiveColumnMapping;
      }

      public readonly Func<IDbConnection> ConnectionFactory;
      public readonly int CommandTimeout;
      public readonly bool AllowQueryScripting;
      public readonly bool AutoApplyDateTimeContraints;
      public readonly bool AllowUnmappedResults;
      public readonly bool TreatStringsAsAnsiStrings;
      public readonly bool UseCaseInsensitiveColumnMapping;      
   }
}