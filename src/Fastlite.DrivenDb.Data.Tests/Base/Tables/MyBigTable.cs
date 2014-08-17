using System.ComponentModel;
using Fastlite.DrivenDb.Core.Contracts;
using Fastlite.DrivenDb.Core.Contracts.Attributes;

namespace Fastlite.DrivenDb.Data.Tests.Base.Tables
{
   [DbTable(Name = "MyBigTable")]
   internal partial class MyBigTable : DbEntity<MyBigTable>, INotifyPropertyChanged
   {
      private long m_Id;
      private string m_Property1;
      private string m_Property2;
      private string m_Property3;
      private string m_Property4;
      private string m_Property5;
      private string m_Property6;
      private string m_Property7;
      private string m_Property8;
      private string m_Property9;
      private string m_Property10;
      private string m_Property11;
      private string m_Property12;

      [DbColumn(IsDbGenerated = true, IsPrimaryKey = true, Name = "Id")]
      public long Id
      {
         get { return m_Id; }
         set
         {
            m_Id = value;
            PropertyChanged(this, new PropertyChangedEventArgs("Id"));
         }
      }

      [DbColumn(IsDbGenerated = false, IsPrimaryKey = false, Name = "Property1")]
      public string Property1
      {
         get { return m_Property1; }
         set
         {
            m_Property1 = value;
            PropertyChanged(this, new PropertyChangedEventArgs("Property1"));
         }
      }

      [DbColumn(IsDbGenerated = false, IsPrimaryKey = false, Name = "Property2")]
      public string Property2
      {
         get { return m_Property2; }
         set
         {
            m_Property2 = value;
            PropertyChanged(this, new PropertyChangedEventArgs("Property2"));
         }
      }

      [DbColumn(IsDbGenerated = false, IsPrimaryKey = false, Name = "Property3")]
      public string Property3
      {
         get { return m_Property3; }
         set
         {
            m_Property3 = value;
            PropertyChanged(this, new PropertyChangedEventArgs("Property3"));
         }
      }

      [DbColumn(IsDbGenerated = false, IsPrimaryKey = false, Name = "Property4")]
      public string Property4
      {
         get { return m_Property4; }
         set
         {
            m_Property4 = value;
            PropertyChanged(this, new PropertyChangedEventArgs("Property4"));
         }
      }

      [DbColumn(IsDbGenerated = false, IsPrimaryKey = false, Name = "Property5")]
      public string Property5
      {
         get { return m_Property5; }
         set
         {
            m_Property5 = value;
            PropertyChanged(this, new PropertyChangedEventArgs("Property5"));
         }
      }

      [DbColumn(IsDbGenerated = false, IsPrimaryKey = false, Name = "Property6")]
      public string Property6
      {
         get { return m_Property6; }
         set
         {
            m_Property6 = value;
            PropertyChanged(this, new PropertyChangedEventArgs("Property6"));
         }
      }

      [DbColumn(IsDbGenerated = false, IsPrimaryKey = false, Name = "Property7")]
      public string Property7
      {
         get { return m_Property7; }
         set
         {
            m_Property7 = value;
            PropertyChanged(this, new PropertyChangedEventArgs("Property7"));
         }
      }

      [DbColumn(IsDbGenerated = false, IsPrimaryKey = false, Name = "Property8")]
      public string Property8
      {
         get { return m_Property8; }
         set
         {
            m_Property8 = value;
            PropertyChanged(this, new PropertyChangedEventArgs("Property8"));
         }
      }

      [DbColumn(IsDbGenerated = false, IsPrimaryKey = false, Name = "Property9")]
      public string Property9
      {
         get { return m_Property9; }
         set
         {
            m_Property9 = value;
            PropertyChanged(this, new PropertyChangedEventArgs("Property9"));
         }
      }

      [DbColumn(IsDbGenerated = false, IsPrimaryKey = false, Name = "Property10")]
      public string Property10
      {
         get { return m_Property10; }
         set
         {
            m_Property10 = value;
            PropertyChanged(this, new PropertyChangedEventArgs("Property10"));
         }
      }

      [DbColumn(IsDbGenerated = false, IsPrimaryKey = false, Name = "Property11")]
      public string Property11
      {
         get { return m_Property11; }
         set
         {
            m_Property11 = value;
            PropertyChanged(this, new PropertyChangedEventArgs("Property11"));
         }
      }

      [DbColumn(IsDbGenerated = false, IsPrimaryKey = false, Name = "Property12")]
      public string Property12
      {
         get { return m_Property12; }
         set
         {
            m_Property12 = value;
            PropertyChanged(this, new PropertyChangedEventArgs("Property12"));
         }
      }

      public event PropertyChangedEventHandler PropertyChanged;
   }
}
