using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Galatea.Runtime;
using Galatea.Runtime.Services;

namespace Gala.Dolly.Test
{
    using Gala.Dolly.Test.Properties;

    /// <summary>
    /// Contains a collection of hard-coded defined Chatbots downloaded from the internet.
    /// </summary>
    internal sealed class ChatbotManager : KeyedCollection<string, IChatbot>, IChatbotManager
    {
        internal ChatbotManager()
        {
            // Initialize Default
            _chatbot = new DefaultChatbot(Settings.Default.DefaultChatbotName);
        }

        IChatbot IChatbotManager.Current
        {
            get { return _chatbot; }
            set { _chatbot = value; }
        }
        
        #region IComponent

        ISite IComponent.Site
        {
            get { return _site; }
            set { _site = value; }
        }
        void IDisposable.Dispose() {
            foreach (IChatbot chatbot in this)
                chatbot.Dispose();

            if (Disposed != null) Disposed(this, EventArgs.Empty);
        }

        public event EventHandler Disposed;

        #endregion

        protected override string GetKeyForItem(IChatbot item)
        {
            return item.Name;
        }

        private class DefaultChatbot : Galatea.Runtime.Services.Chatbot
        {
            public DefaultChatbot(string name) : base(name)
            {
            }

            public override string Greeting { get => Settings.Default.DefaultChatbotResponse; }
        }

        private IChatbot _chatbot;
        private ISite _site;
    }
}
