using System;

namespace Galatea.AI.Characterization
{
    using AI.Abstract;

    /// <summary>
    /// Represents a pattern specifying that a user response is 
    /// positive/negative, and to what degree.
    /// </summary>
    /// <remarks>
    /// UserFeedback inherits from the Template model, as it should
    /// factor in the Artificial Intelligence system.
    /// </remarks>
    public class UserFeedback : BaseTemplate, IUserFeedback
    {
        /// <summary>
        /// Creates a new instance of the <see cref="UserFeedback"/> class.
        /// </summary>
        public UserFeedback(int id, FeedbackType feedbackType, int level, string name)
            : base(id, name, TemplateType.Custom, name)
        {
            if (level < 0 || level > 100)
                throw new TeaArgumentException(Globalization.DiagnosticResources.Integer_Expected_0_100);

            _level = level;
            _feedbackType = feedbackType;

            //// Synonyms (for Speech Recognition)
            //_relationships = new TemplateRelationshipCollection<UserFeedback>();
        }
        /// <summary>
        /// Gets a value specifying if the feedback is Positive, Negative, or neutral.
        /// </summary>
        public FeedbackType FeedbackType { get { return _feedbackType; } }
        /// <summary>
        /// Gets an integer specifying the degree by which the feedback is Positive or Negative.
        /// </summary>
        public int Level { get { return _level; } }
        /// <summary>
        /// Gets a value that is used to compare two <see cref="UserFeedback"/> template instances.
        /// </summary>
        public override int CompareValue { get { return _level * (int)_feedbackType; } }
        //public TemplateRelationshipCollection<UserFeedback> Relationships { get { return _relationships; } }


        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }


        private readonly FeedbackType _feedbackType;
        private readonly int _level;
    }
}
