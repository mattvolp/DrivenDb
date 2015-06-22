using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Text;
using DrivenDb.Data;
using DrivenDb.Data.Internal;
using DrivenDb.VisualStudio.Generator.Internal;
using Microsoft.CSharp;

namespace DrivenDb.Testing
{
   internal class EntityCompiler
   {      
      public EntityFactory CompileFactory(ScriptingOptions options, TableMap[] tables)
      {
         using (var buffer = new StringWriter())
         {
            var scripter = new CsGenerator(options, buffer);
            
            scripter.Write("TestNamespace", "TestContext", tables);

            var code = buffer.ToString();

            var provider = new CSharpCodeProvider();
            var parameters = new CompilerParameters();

            parameters.ReferencedAssemblies.Add("Resources\\DrivenDb.Core.dll");
            parameters.ReferencedAssemblies.Add("System.dll");
            parameters.ReferencedAssemblies.Add("System.Core.dll");
            parameters.ReferencedAssemblies.Add("System.Runtime.dll");
            parameters.ReferencedAssemblies.Add("System.Data.dll");
            parameters.ReferencedAssemblies.Add("System.Data.Linq.dll");
            parameters.ReferencedAssemblies.Add("System.Runtime.Serialization.dll");
            parameters.GenerateInMemory = true;
            parameters.GenerateExecutable = false;

            var results = provider.CompileAssemblyFromSource(parameters, code);

            if (results.Errors.HasErrors)
            {
               var builder = new StringBuilder();

               foreach (CompilerError error in results.Errors)
               {
                  builder.AppendLine(String.Format("Error ({0}): {1}", error.ErrorNumber, error.ErrorText));
               }

               throw new InvalidOperationException(builder.ToString());
            }

            return new EntityFactory(results.CompiledAssembly);
         }
      }
   }
}
