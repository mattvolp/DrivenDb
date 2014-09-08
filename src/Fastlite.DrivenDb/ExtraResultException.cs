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

namespace Fastlite.DrivenDb
{
    public class ExtraResultException : Exception
    {
        public ExtraResultException(int expected, int found)
            : base(String.Format("'{0}' result sets where expected but '{1}' were found", expected, found))
        {
            ExpectedResultCount = expected;
            FoundResultCount = found;
        }

        public int ExpectedResultCount
        {
            get;
            private set;
        }

        public int FoundResultCount
        {
            get;
            private set;
        }
    }
}
