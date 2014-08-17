using System;
using System.Collections.Generic;
using System.Text;

namespace Fastlite.DrivenDb.Console
{
   class Program
   {
      static void Main(string[] args)
      {
         var options = new Options();
         if (CommandLine.Parser.Default.ParseArguments(args, options))
         {
            // consume Options instance properties
            //if (options.Verbose)
            //{
            //   Console.WriteLine(options.InputFile);
            //   Console.WriteLine(options.MaximumLength);
            //}
            //else
            //   Console.WriteLine("working ...");
         }
         else
         {
         }
      }
   }
}
