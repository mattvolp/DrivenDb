using CommandLine;
using Fastlite.DrivenDb.Data.Access;

namespace Fastlite.DrivenDb.Console
{
   internal class Options
   {
      [Option('t', "type", Required = true, HelpText = "Connection Type: 0 = MsSql, 1 = SqLite, 2 = MySql, 3 = Oracle")]
      public DbAccessorType DbAccessorType
      {
         get; set;
      }
   }
}
