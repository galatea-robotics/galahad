using System;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Galatea.AI.Language
{
    using Galatea;

    /// <summary>
    /// An error that occurs during the <see cref="SubstitutionsManager.LoadSubstitutions(StringCollection)"/> method.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors")]
    [Serializable]
    //[System.Runtime.InteropServices.ComVisible(false)]
    public class SubstitutionsException : TeaException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubstitutionsException"/> class.
        /// </summary>
        public SubstitutionsException(string substitutionArgument)
            : base()
        {
            _substitutionArgument = substitutionArgument;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="SubstitutionsException"/> class.
        /// </summary>
        public SubstitutionsException(string substitutionArgument, string message, Exception innerException)
            : base(message, innerException)
        {
            _substitutionArgument = substitutionArgument;
        }

        /// <summary>
        /// When overridden in a derived class, sets the System.Runtime.Serialization.SerializationInfo
        /// with information about the exception.
        /// </summary>
        /// <param name="info">
        /// The System.Runtime.Serialization.SerializationInfo that holds the serialized
        /// object data about the exception being thrown.
        /// </param>
        /// <param name="context">
        /// The System.Runtime.Serialization.StreamingContext that contains contextual information
        /// about the source or destination.
        /// </param>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
        /// <summary>
        /// A string value that was improperly formatted for Substitutions.
        /// </summary>
        public virtual string SubstitutionArgument { get { return _substitutionArgument; } }

        private string _substitutionArgument;
    }
}
