using System.Linq;
using DrivenDb.Data;

namespace DrivenDb.Scripting.Internal.Writers
{
   internal class CsKeyClassWriter
   {
      public static void Write(ScriptTarget target, TableMap table)
      {
         var primaries = table.GetPrimaryKeyColumns()
            .ToArray();

         primaries.GuardAgainstKeyClassOverflow();

         if (primaries.Any())
         {
            target
               .WriteLinesAndContinue(new ScriptLines()
                  {
                     {"                                                          "},
                     {"    public class $0Key                                    "},
                     {"        : Tuple<$1>                                       "},
                     {"    {                                                     "},
                     {"        public $0Key($2)                                  "},
                     {"            : base($3)                                    "},
                     {"        {                                                 "},
                  }
                  , table.Detail.Name
                  , primaries.ScriptAsDelimitedCsTypes()
                  , primaries.ScriptAsDelimitedCsTypedParameterNames()
                  , primaries.ScriptAsDelimitedParameterNames())

               .WriteTemplateAndContinue(primaries, new ScriptLines()
                  {
                     {"            $0 = $1;                                     "},
                  }, p => new[]
                     {
                        p.Detail.Name, p.ScriptAsParameterName()
                     })

               .WriteLinesAndContinue(new ScriptLines()
                  {
                     {"        }                                                 "},
                     {"                                                          "},
                  })

               .WriteTemplateAndContinue(primaries, new ScriptLines()
                  {
                     {"        public readonly $0 $1;                            "},
                  }, p => new[]
                     {
                        p.ScriptAsCsType(), p.Detail.Name
                     })

               .WriteLines(new ScriptLines()
                  {
                     {"   }                                                      "},
                  });
         }
      }
   }
}
