using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace CommonContracts.WsBaseFaults
{
    /// <summary>
    /// Represents the "http://docs.oasis-open.org/wsrf/bf-2:BaseFaultType/ErrorCode" element.
    /// </summary>
    [XmlRoot("ErrorCode", Namespace = Constants.WsBaseFaultsNamespace)]
    public class ErrorCode : IXmlSerializable
    {
        #region Fields

        private Uri dialect;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorCode"/> class from the supplied <paramref name="reader"/>.
        /// </summary>
        /// <param name="reader">The <see cref="XmlReader"/> to create an instance from.</param>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "This is checked via Code Contracts. CA Engine does not yet understand how to deal with contracts.")]
        public ErrorCode(XmlReader reader)
        {
            Contract.Requires<ArgumentNullException>(reader != null, "reader");
            Contract.Requires<ArgumentException>(reader.ReadState == ReadState.Interactive, "reader");

            ((IXmlSerializable)this).ReadXml(reader);
        }

        /// <summary>
        /// Intializes a new instance of the <see cref="ErrorCode"/> class with the supplied <paramref name="dialect"/> value.
        /// </summary>
        /// <param name="dialect">The <see cref="Dialect"/> value.</param>
        public ErrorCode(Uri dialect)
        {
            Contract.Requires<ArgumentNullException>(dialect != null, "dialect");

            this.dialect = dialect;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the <see cref="Uri"/> value for the <see cref="ErrorCode"/>.
        /// </summary>
        /// <value>The <see cref="Uri"/> value for the <see cref="ErrorCode"/>.</value>
        public virtual Uri Dialect
        {
            get { return this.dialect; }
            protected set
            {
                Contract.Requires<ArgumentNullException>(value != null, "Dialect");
                this.dialect = value;
            }
        }

        #endregion

        #region IXmlSerializable Members

        /// <summary>
        /// Extension point for additional xml content to be handled. This method ignore all additional information when
        /// executed and will invoke <see cref="XmlReader.Read()"/> until no more elements exist and the reader is finished.
        /// </summary>
        /// <remarks>
        /// This base method is safe to not call provided you manually move the reader to the end element. If calling this
        /// method on the base type, perform any work with the supplied <paramref name="reader"/> prior to calling the base
        /// implementation. Inheritors should be aware that a child reader is created specifically for any inner XML content
        /// and supplied to this method. The reader will always be positioned on the initial element.
        /// </remarks>
        /// <param name="reader">The <see cref="XmlReader"/> containing the XML to process.</param>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "This is checked via Code Contracts. CA Engine does not yet understand how to deal with contracts.")]
        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", Justification = "This is the parameter name of the code and globalization is not needed.")]
        protected virtual void ProcessAdditionalElements(XmlReader reader)
        {
            Contract.Requires<ArgumentNullException>(reader != null);
            Contract.Requires<ArgumentException>(reader.ReadState == ReadState.Interactive, "reader");

            while (reader.Read())
            {
                // do nothing
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "IXmlSerializable.GetSchema() must always return null and should never be called application code")]
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "This is checked via Code Contracts. CA Engine does not yet understand how to deal with contracts.")]
        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", Justification = "This is the parameter name of the code and globalization is not needed.")]
        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            Contract.Requires<ArgumentNullException>(reader != null, "reader");
            Contract.Requires<ArgumentException>(reader.ReadState == ReadState.Interactive, "reader");

            if (!reader.IsStartElement("ErrorCode", Constants.WsBaseFaultsNamespace))
            {
                throw new XmlException("Invalid Element, it must be 'ErrorCode'");
            }

            var attributeValue = reader.GetAttribute("dialect");
            if (String.IsNullOrWhiteSpace(attributeValue)) throw new XmlException("Invalid Element, 'ErrorCode' requires the 'dialect' attribute value to be supplied");
            this.dialect = new Uri(attributeValue);

            if (reader.IsEmptyElement)
            {
                reader.Read();
            }
            else
            {
                var innerReader = reader.ReadSubtree();
                innerReader.Read();
                this.ProcessAdditionalElements(innerReader);

                reader.ReadEndElement();
            }
        }

        /// <summary>
        /// Extension point for additional xml content to be handled. This method ignore all additional information when executed.
        /// </summary>
        /// <remarks>This method performs no work and is safe to not be called by inheritors.</remarks>
        /// <param name="writer">The <see cref="XmlWriter"/> to write any additional content XML.</param>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "This is a no-op method.")]
        protected virtual void ProcessAdditionalElements(XmlWriter writer)
        {
        }

        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "This is checked via Code Contracts. CA Engine does not yet understand how to deal with contracts.")]
        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", Justification = "This is the parameter name of the code and globalization is not needed.")]
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            Contract.Requires<ArgumentNullException>(writer != null, "writer");

            var prefix = writer.LookupPrefix(Constants.WsBaseFaultsNamespace);
            if (String.IsNullOrEmpty(prefix)) prefix = "wsbf";

            writer.WriteStartElement(prefix, "ErrorCode", Constants.WsBaseFaultsNamespace);
            writer.WriteAttributeString("dialect", this.Dialect.ToString());

            this.ProcessAdditionalElements(writer);

            writer.WriteEndElement();
        }

        #endregion
    }
}