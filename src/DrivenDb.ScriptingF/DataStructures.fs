module DataStructures

open System

[<Flags>]
type ScriptingOptions = 
   | None = 0
   | ImplementNotifyPropertyChanging = 1
   | ImplementNotifyPropertyChanged = 2
   | ImplementPartialPropertyChanges = 4
   | ImplementLinqContext = 8
   | ImplementPrimaryKey = 16
   | ImplementColumnDefaults = 32
   | MinimizePropertyChanges = 64
   | Serializable = 128
   | ImplementStateTracking = 256
   | UnspecifiedDateTimes = 512
   | TruncateTimeForDateColumns = 1024
   | ImplementValidationCheck = 2048   
//  |   All = ImplementNotifyPropertyChanging
//            | ImplementNotifyPropertyChanged
//            | ImplementPartialPropertyChanges
//            | ImplementLinqContext
//            | ImplementPrimaryKey
//            | ImplementColumnDefaults
//            | MinimizePropertyChanges
//            | Serializable
//            | ImplementStateTracking
//            | UnspecifiedDateTimes
//            | TruncateTimeForDateColumns
//            | ImplementValidationCheck

type ColumnDetail = 
   {
      SqlType : string // DbType TODO
      Name : string
      IsNullable : bool
      IsPrimary : bool
      IsGenerated : bool
      IsReadonly : bool
      HasDefault : bool
      DefaultValue : string
      ColumnPosition : int
   }

type ColumnMap = 
   {
      Detail : ColumnDetail
      CustomType : string
   }

type TableDetail =
   {
      Schema : string
      Name : string
      Columns : ColumnDetail list
   }

type TableMap = 
   {
      Detail : TableDetail
      Columns : ColumnMap list
   }

let IsDateOnly (column : ColumnDetail) : bool =
   true