using System;
using DrivenDb.Utility;
using Xunit;

namespace DrivenDb.Tests.Utility
{
    public class WeakEventManagerTests
    {
        [Fact]
        public void AddRemoveHandlerTest()
        {
            var manager = new WeakEventManager<EventArgs>();
            var validated = false;

            EventHandler<EventArgs> handler = (s, e) => 
                {
                    Assert.Equal(s, this);
                    validated = true;
                };

            manager.Add(handler);
            manager.Invoke(this, new EventArgs());

            Assert.True(validated);

            validated = false;
            manager.Remove(handler);
            manager.Invoke(this, new EventArgs());

            Assert.False(validated); 
        }

        [Fact]
        public void AddCollectHandlerTest()
        {
            var dispatcher = new TestDispatcher();
            var listener = new TestListener(dispatcher);

            Assert.False(listener.Validated);

            dispatcher.FireTestEvent();

            Assert.True(listener.Validated);

            listener = null;
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            GC.WaitForFullGCComplete();

            Assert.DoesNotThrow(
                () => dispatcher.FireTestEvent()
                );
        }

        private class TestDispatcher
        {
            private readonly WeakEventManager<EventArgs> m_Manager = new WeakEventManager<EventArgs>();

            public event EventHandler<EventArgs> TestEvent
            {
                add { m_Manager.Add(value); }
                remove { m_Manager.Remove(value); }
            }

            public void FireTestEvent()
            {
                m_Manager.Invoke(this, new EventArgs());
            }
        }

        private class TestListener
        {
            public TestListener(TestDispatcher dispatcher)
            {
                dispatcher.TestEvent += OnTestEvent;
            }

            public bool Validated
            {
                get;
                private set;
            }

            public void OnTestEvent(object sender, EventArgs args)
            {
                Validated = true;
            }

            public void Reset()
            {
                Validated = false;
            }
        }
    }
}
