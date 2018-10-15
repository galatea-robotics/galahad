using System;
using System.Collections.ObjectModel;

namespace Galatea.AI.Characterization
{
    using Galatea.AI.Abstract;
    using Galatea.IO;

    /// <summary>
    /// Some Strongly-typed Feedback Definitions for Robot Fun.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
    public class UserFeedbackTemplates : KeyedCollection<string, UserFeedback>, IDisposable
    {
        private UserFeedbackTemplates() { }
        private static UserFeedbackTemplates GetDefault()
        {
            UserFeedbackTemplates result = new UserFeedbackTemplates();

            try
            {
                // TODO:  Globalize names
                result.Add(new UserFeedback(90, FeedbackType.Neutral, 0, "Neutral"));
                result.Add(new UserFeedback(91, FeedbackType.Positive, 1, "Meh"));
                result.Add(new UserFeedback(92, FeedbackType.Positive, 25, "Yes"));
                result.Add(new UserFeedback(93, FeedbackType.Positive, 50, "Good"));
                result.Add(new UserFeedback(94, FeedbackType.Positive, 75, "Great"));
                result.Add(new UserFeedback(95, FeedbackType.Negative, 1, "MehNo"));
                result.Add(new UserFeedback(96, FeedbackType.Negative, 25, "No"));
                result.Add(new UserFeedback(97, FeedbackType.Negative, 50, "Bad"));
                result.Add(new UserFeedback(98, FeedbackType.Negative, 75, "Awful"));
            }
            catch
            {
                result.Dispose();
                throw;
            }

            return result;
        }
        private static UserFeedbackTemplates _default = GetDefault();

        /// <summary>
        /// A default instance of the <see cref="UserFeedback"/> class.
        /// </summary>
        public static UserFeedbackTemplates Default { get { return _default; } }

        /// <summary>
        /// Extracts the <see cref="BaseTemplate.Name"/> string value from
        /// the specified <see cref="UserFeedback"/> item.
        /// </summary>
        /// <param name="item">
        /// The <see cref="UserFeedback"/> from which to extract the 
        /// <see cref="BaseTemplate.Name"/> key.
        /// </param>
        /// <returns>
        /// The <see cref="BaseTemplate.Name"/> key for the 
        /// specified <see cref="UserFeedback"/>.
        /// </returns>
        [System.Diagnostics.DebuggerStepThrough]
        protected override string GetKeyForItem(UserFeedback item)
        {
            if (item == null) throw new TeaArgumentNullException("item");
            return item.Name;
        }

        #region Strongly-Typed Feedbacks 
        /// <summary>
        /// A <see cref="UserFeedback"/> instance that is neither Positive nor Negative.
        /// </summary>
        public static UserFeedback Neutral { get { return _default["Neutral"]; } }
        /// <summary>
        /// A <see cref="UserFeedback"/> instance that is unenthusiastically positive.
        /// </summary>
        public static UserFeedback Meh { get { return _default["Meh"]; } }
        /// <summary>
        /// A moderately positive <see cref="UserFeedback"/> instance.
        /// </summary>
        public static UserFeedback Yes { get { return _default["Yes"]; } }
        /// <summary>
        /// A very positive <see cref="UserFeedback"/> instance.
        /// </summary>
        public static UserFeedback Good { get { return _default["Good"]; } }
        /// <summary>
        /// A highly positive <see cref="UserFeedback"/> instance.
        /// </summary>
        public static UserFeedback Great { get { return _default["Great"]; } }
        /// <summary>
        /// A <see cref="UserFeedback"/> instance that is apathetically negative.
        /// </summary>
        public static UserFeedback MehNo { get { return _default["MehNo"]; } }
        /// <summary>
        /// A moderately negative <see cref="UserFeedback"/> instance.
        /// </summary>
        public static UserFeedback No { get { return _default["No"]; } }
        /// <summary>
        /// A very negative <see cref="UserFeedback"/> instance.
        /// </summary>
        public static UserFeedback Bad { get { return _default["Bad"]; } }
        /// <summary>
        /// A highly negative <see cref="UserFeedback"/> instance.
        /// </summary>
        public static UserFeedback Awful { get { return _default["Awful"]; } }
        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="IDisposable"/> and optionally 
        /// releases the managed resources.
        /// </summary>
        /// <param name="disposing">
        /// true to release both managed and unmanaged resources; false to release only unmanaged
        /// resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    foreach (UserFeedback fb in this)
                    {
                        fb.Dispose();
                    }
                }

                disposedValue = true;
            }
        }

        /// <summary>
        /// Releases all resources used by the <see cref="IDisposable"/>.
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);

            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
