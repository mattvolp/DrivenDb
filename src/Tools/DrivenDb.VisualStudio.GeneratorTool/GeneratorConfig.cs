/**************************************************************************************
 * Original Author : Anthony Leatherwood (adleatherwood@gmail.com)
 * Source Location : https://github.com/Fastlite/DrivenDb
 *
 * This source is subject to the Microsoft Public License.
 * Link: http://www.microsoft.com/en-us/openness/licenses.aspx
 *
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
 * EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
 **************************************************************************************/

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

      [XmlElement(IsNullable = true)]
      public string ReadOnlyTableFilter
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
