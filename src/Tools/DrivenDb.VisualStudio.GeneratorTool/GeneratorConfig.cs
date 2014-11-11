using System;
using System.Xml.Serialization;

namespace DrivenDb.VisualStudio.GeneratorTool
{
   [Serializable]
   public class GeneratorConfig
   {            
      public DbAccessorType AccessorType
      {
         get;
         set;
      }

      public DatabaseConfig[] DatabaseConfigs
      {
         get;
         set;
      }

      public string Namespace
      {
         get;
         set;
      }

      public string TableFilter
      {
         get;
         set;
      }
   }

   [Serializable]
   public class DatabaseConfig
   {      
      [XmlAttribute]
      public string MachineName
      {
         get;
         set;
      }

      [XmlAttribute]
      public string ConnectionString
      {
         get;
         set;
      }
   }
}
