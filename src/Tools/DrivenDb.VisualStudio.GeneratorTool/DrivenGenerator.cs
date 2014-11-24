using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Serialization;
using DrivenDbConsole;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace DrivenDb.VisualStudio.GeneratorTool
{
   [ComVisible(true)]
   [Guid("38d11784-61bd-4afe-86e1-c8f97a20cf70")]
   public class DrivenGenerator : IVsSingleFileGenerator
   {
      public int DefaultExtension(out string pbstrDefaultExtension)
      {
         pbstrDefaultExtension = GetDefaultExtension();

         return VSConstants.S_OK;
      }

      public int Generate(string wszInputFilePath, string bstrInputFileContents, string wszDefaultNamespace, IntPtr[] rgbOutputFileContents, out uint pcbOutput, IVsGeneratorProgress pGenerateProgress)
      {
         var generated = GenerateCode(wszInputFilePath, bstrInputFileContents, wszDefaultNamespace);

         rgbOutputFileContents[0] = Marshal.AllocCoTaskMem(generated.Length);

         Marshal.Copy(generated, 0, rgbOutputFileContents[0], generated.Length);

         pcbOutput = (uint) generated.Length;

         return VSConstants.S_OK;
      }

      public virtual string GetDefaultExtension()
      {
         Log("GetDefaultExtension() called");
         return ".Generated.cs";
      }

      protected virtual byte[] GenerateCode(string inputFileName, string inputFileContent, string defaultNamespace)
      {
         try
         {
            Log("GenerateCode() called - {0}, {1}", inputFileName, inputFileContent);

            if (String.IsNullOrWhiteSpace(inputFileContent))
            {
               GenerateDefaultContent(inputFileName, defaultNamespace);
               return new byte[0];
            }

            var config = DeserializeConfig(inputFileContent);
            var viewModel = new MainWindowViewModel();

            viewModel.AppName = "not used";
            viewModel.SelectedAccessorType = config.AccessorType.ToString();

            var cstring = config.DatabaseConfigs
               .OrderByDescending(c => c.MachineName)
               .Where(c => c.MachineName == Environment.MachineName || c.MachineName == "")
               .Select(c => c.ConnectionString)
               .FirstOrDefault();

            viewModel.CString = cstring;
            viewModel.Namespace = !String.IsNullOrWhiteSpace(config.Namespace) 
               ? config.Namespace
               : defaultNamespace;

            viewModel.TableFilter = config.TableFilter;
            viewModel.ReadOnlyTableFilter = config.ReadOnlyTableFilter;

            var canExecute = viewModel.Generate.CanExecute(null);

            Log("Generate.CanExecute() - {0}", canExecute);

            if (canExecute)
            {
               viewModel.Generate.Execute(null);
            }

            var output = viewModel.Output;

            Log("Generate.Output - {0}", output);

            return Encoding.UTF8.GetBytes(output);
         }
         catch (Exception e)
         {
            var error = e.Message + Environment.NewLine + e.StackTrace;
            return Encoding.UTF8.GetBytes(error);
         }
      }

      [Conditional("DEBUG")]
      private static void Log(string format, params object[] parameters)
      {
         var header = String.Format("{0} : {1}{2}", DateTime.Now, format, Environment.NewLine);

         File.AppendAllText(@"d:\DrivenGenerator.log", string.Format(header, parameters));
      }

      private static void GenerateDefaultContent(string filename, string defaultNamespace)
      {
         var serializer = new XmlSerializer(typeof(GeneratorConfig));

         using (var stream = new FileStream(filename, FileMode.Truncate))
         {
            var defaultConfig = new GeneratorConfig()
               {
                  AccessorType = DbAccessorType.MsSql,
                  Namespace = defaultNamespace ?? "YourNamespace",
                  TableFilter = "%",
                  DatabaseConfigs = new DatabaseConfig[]
                     {
                        new DatabaseConfig()
                           {
                              MachineName = Environment.MachineName,
                              ConnectionString = "Integrated Security=SSPI;Initial Catalog=YourDb;Data Source=."
                           },
                     }
               };

            serializer.Serialize(stream, defaultConfig);
            stream.Flush();
         }
      }

      private static GeneratorConfig DeserializeConfig(string input)
      {
         var serializer = new XmlSerializer(typeof (GeneratorConfig));

         using (var stream = new MemoryStream())
         using (var writer = new StreamWriter(stream))
         {
            writer.Write(input);
            writer.Flush();

            stream.Position = 0;

            var config = (GeneratorConfig) serializer.Deserialize(stream);

            return config;
         }
      }
   }
}
