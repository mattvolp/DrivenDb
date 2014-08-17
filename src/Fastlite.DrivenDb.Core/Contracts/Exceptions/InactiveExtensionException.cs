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

namespace Fastlite.DrivenDb.Core.Contracts.Exceptions
{
    public class InactiveExtensionException : Exception
    {
        public InactiveExtensionException(string extensionName)
            : base(String.Format("DrivenDb extension '{0}' is inactive.", extensionName))
        {
        }

        public InactiveExtensionException(string extensionName, string message)
           : base(String.Format("DrivenDb extension '{0}' is inactive.  {1}", extensionName, message))
        {
        }
    }
}
