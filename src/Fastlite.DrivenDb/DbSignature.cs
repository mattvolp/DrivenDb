using System;
using System.Linq;

namespace Fastlite.DrivenDb
{
   internal static class DbSignature
   {
      public static string Create<T>(DbRecord<T> record)
      {
         var signature = typeof (T).FullName
                         + "|" + String.Join("|", record.Names)
                         + "|" + String.Join("|", record.Values.Select(v => v.GetType().Name));

         return signature;
      }
   }
}
