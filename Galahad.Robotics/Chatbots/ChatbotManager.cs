using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galahad.Robotics
{
    using Galatea;
    using Galatea.AI.Abstract;
    using Galatea.Runtime;
    using Properties;

    class ChatbotManager : Collection<IChatbot>, IChatbotManager
    {
        public ChatbotManager(IUser user, UWPSettings settings)
        {
            alice = new Gala.Dolly.Chatbots.Alice(user, settings.ChatbotName, settings.ChatbotAliceConfigFolder, settings.ChatbotResourcesFolder);
        }

        /*
        private static bool TestDirectoryExists(string directory)
        {
            return Task.Run(() =>
            {
                return System.IO.Directory.Exists(directory);
            }).Result;
        }
         */

        IChatbot IChatbotManager.this[int index]
        {
            get { return alice; }
        }
        IChatbot IChatbotManager.this[string name]
        {
            get { return alice; }
        }

        /// <summary>
        /// Gets or sets the <see cref="IChatbot"/> contained in the collection that is actively 
        /// responding to User text.
        /// </summary>
        IChatbot IChatbotManager.Current
        {
            get { return alice; }
            set { throw new NotImplementedException(); }
        }

        #region IComponent

        ISite IComponent.Site
        {
            get { return site; }
            set { site = value; }
        }

        /// <summary>
        /// Represents the method that handles the <see cref="IComponent.Disposed"/>
        /// event of a <see cref="ChatbotManager"/> component.
        /// </summary>
        public event EventHandler Disposed;
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting
        /// unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the System.ComponentModel.Component
        /// and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">
        /// true to release both managed and unmanaged resources; false to release only unmanaged
        /// resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            alice.Dispose();
            Disposed?.Invoke(this, EventArgs.Empty);
        }
        /// <summary>
        ///  Gets or sets the <see cref="ISite"/> associated with the 
        ///  <see cref="ChatbotManager"/> component.
        /// </summary>
        protected virtual ISite Site
        {
            get { return site; }
            set { site = value; }
        }

        #endregion

        private Gala.Dolly.Chatbots.Alice alice;
        private ISite site;
    }
}