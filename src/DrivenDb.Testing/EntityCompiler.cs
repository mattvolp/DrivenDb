using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Text;
using DrivenDb.Data;
using DrivenDb.Data.Internal;
using DrivenDb.Scripting.Internal;
using DrivenDb.Scripting.Internal.Collections;
using DrivenDb.Scripting.Internal.Interfaces;
using DrivenDb.Scripting.Internal.Writers;
using Microsoft.CSharp;

namespace DrivenDb.Testing
{
   internal class EntityCompiler
   {      
      public EntityFactory CompileFactory(ScriptingOptions options, TableMap[] tables)
      {
         using (var buffer = new StringWriter())
         {
            var target = new ScriptTarget(options, buffer, "TestNamespace", "TestContext");
            var scripter = new CsGenerator(target,
               new TablesWriterCollection(
                  new CsContextScripter(),
                  new CsClassScripter(new TableWriterCollection(
                     new CsConstructorWriter(new CsDefaultTranslator(target.Options)),
                     new CsFieldWriter(),
                     new CsKeyPropertyWriter(),
                     new CsPartialWriter(),
                     new CsPropertyChangingWriter(),
                     new CsPropertyChangedWriter(),
                     new CsValidationWriter()
                     )
                     )));
            
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
                  builder.AppendLine($"Error ({error.ErrorNumber}): {error.ErrorText}");
               }

               throw new InvalidOperationException(builder.ToString());
            }

            return new EntityFactory(results.CompiledAssembly);
         }
      }
   }
}
