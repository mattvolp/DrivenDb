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
using System.Collections.Generic;
using System.Linq;

namespace Fastlite.DrivenDb.Core.Utility
{
    internal class WeakEventManager<T>        
        where T : EventArgs
    {
        private readonly HashSet<Weak<EventHandler<T>>> _handlers = new HashSet<Weak<EventHandler<T>>>(new WeakEqualityComparer<EventHandler<T>>());

        public void Add(EventHandler<T> handler)
        {
            _handlers.Add(new Weak<EventHandler<T>>(handler));
        }

        public void Remove(EventHandler<T> handler)
        {
            _handlers.Remove(new Weak<EventHandler<T>>(handler));
        }

        public void Invoke(object sender, T args)
        {
            foreach (var handler in _handlers.ToArray())
            {
                if (!handler.IsAlive)
                {
                    _handlers.Remove(handler);
                }
                else
                {
                    handler.Target.Invoke(sender, args);
                }
            }
        }
    }
}
