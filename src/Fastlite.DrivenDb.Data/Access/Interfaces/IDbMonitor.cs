/**************************************************************************************
 * Original Author : Anthony Leatherwood (fastlite@outlook.com)                              
 * Source Location : https://github.com/Fastlite/DrivenDb     
 *  
 * This source is subject to the Mozilla Public License, version 2.0.
 * Link: https://github.com/Fastlite/DrivenDb/blob/master/LICENSE
 *  
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
 * EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE. 
 **************************************************************************************/

using System;
using Fastlite.DrivenDb.Core.Contracts;

namespace Fastlite.DrivenDb.Data.Access.Interfaces
{
    public interface IDbMonitor
    {
        event EventHandler<DbChangeEventArgs> Inserted;
        event EventHandler<DbChangeEventArgs> Updated;
        event EventHandler<DbChangeEventArgs> Deleted;
    }
}
