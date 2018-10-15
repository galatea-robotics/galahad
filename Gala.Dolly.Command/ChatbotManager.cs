using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Galatea.Runtime;

namespace Gala.Dolly.Test
{
    /// <summary>
    /// Contains a collection of hard-coded defined Chatbots downloaded from the internet.
    /// </summary>
    internal sealed class ChatbotManager : KeyedCollection<string, IChatbot>, IChatbotManager
    {
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

        private IChatbot _chatbot;
        private ISite _site;
    }
}
