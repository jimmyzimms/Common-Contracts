using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Xml;
using System.Xml.Serialization;

namespace CommonContracts.WsBaseFaults
{
    /// <summary>
    /// An extension of <see cref="ErrorCode{T}"/> that allows additional elements to be added to the content of the "http://docs.oasis-open.org/wsrf/bf-2:BaseFaultType/ErrorCode" element.
    /// </summary>
    /// <typeparam name="T">The type representing the internal content of <see cref="ErrorCode{T}"/>.</typeparam>
    [XmlRoot("ErrorCode", Namespace = Constants.WsBaseFaultsNamespace)]
    public class ErrorCode<T> : ErrorCode where T : IXmlSerializable, new()
    {
        #region Fields

        private T content;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorCode{T}"/> class.
        /// </summary>
        /// <param name="dialect">The <see cref="ErrorCode.Dialect"/> value.</param>
        /// <param name="content">The <see cref="Content"/> value.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", Justification = "This is the parameter name of the code and globalization is not needed.")]
        public ErrorCode(Uri dialect, T content)
            : base(dialect)
        {
            Contract.Requires<ArgumentNullException>(dialect != null, "dialect");

            this.content = content;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the additional content for the <see cref="ErrorCode{T}"/>.
        /// </summary>
        /// <value>The additional content for the <see cref="ErrorCode{T}"/>.</value>
        public virtual T Content
        {
            get { return this.content; }
            set { this.content = value; }
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Extension point for additional xml content to be handled.
        /// </summary>
        /// <param name="reader">The <see cref="XmlReader"/> containing the XML to process.</param>
        protected override void ProcessAdditionalElements(XmlReader reader)
        {
            this.content = new T();
            this.content.ReadXml(reader);
        }

        /// <summary>
        /// Extension point for additional xml content to be handled.
        /// </summary>
        /// <param name="writer">The <see cref="XmlWriter"/> to write any additional content XML.</param>
        protected override void ProcessAdditionalElements(XmlWriter writer)
        {
            if (this.content == null) return;

            this.content.WriteXml(writer);
        }

        #endregion
    }
}
