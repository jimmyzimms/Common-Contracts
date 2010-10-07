using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace CommonContracts.WsBaseFaults
{
    /// <summary>
    /// Represents the "http://docs.oasis-open.org/wsrf/bf-2:BaseFaultType/Description" element.
    /// </summary>
    [XmlRoot("Description", Namespace = Constants.WsBaseFaultsNamespace)]
    public class Description : IXmlSerializable
    {
        #region Fields

        private String descriptionValue;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Description"/> class from the supplied <paramref name="reader"/>.
        /// </summary>
        /// <param name="reader">The <see cref="XmlReader"/> to create an instance from.</param>
        public Description(XmlReader reader)
        {
            Contract.Requires<ArgumentNullException>(reader != null, "reader");
            Contract.Requires<ArgumentException>(reader.ReadState == ReadState.Interactive, "reader");

            ((IXmlSerializable)this).ReadXml(reader);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Description"/> class.
        /// </summary>
        /// <param name="value">The <see cref="Value"/> property value.</param>
        public Description(String value)
        {
            Contract.Requires<ArgumentNullException>(!String.IsNullOrWhiteSpace(value), "value");

            this.descriptionValue = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Description"/> class.
        /// </summary>
        /// <param name="value">The <see cref="Value"/> property value.</param>
        /// <param name="culture">An optional <see cref="CultureInfo"/> value.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1304:SpecifyCultureInfo", Justification = "This type is a XSD contract element and the intended use is for a CultureInfo used to indicate how the supplied string should be evaluated")]
        public Description(String value, CultureInfo culture)
            : this(value)
        {
            this.Language = culture;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the value of the <see cref="Description"/>.
        /// </summary>
        /// <value>The value of the <see cref="Description"/>.</value>
        public virtual String Value
        {
            get { return this.descriptionValue; }
            set
            {              
                this.descriptionValue = value;
            }
        }

        /// <summary>
        /// Gets or sets the optional <see cref="CultureInfo"/> value for the <see cref="Description"/>.
        /// </summary>
        /// <value>The optional <see cref="CultureInfo"/> value for the <see cref="Description"/>.</value>
        public CultureInfo Language { get; set; }

        #endregion

        #region IXmlSerializable Members

        [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "IXmlSerializable.GetSchema() must always return null and should never be called application code")]
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "This is checked via Code Contracts. CA Engine does not yet understand how to deal with contracts.")]
        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            Contract.Requires<ArgumentNullException>(reader != null, "reader");
            Contract.Requires<ArgumentException>(reader.ReadState == ReadState.Interactive, "reader");

            if (!reader.IsStartElement("Description", Constants.WsBaseFaultsNamespace))
            {
                throw new XmlException("Invalid Element, it must be 'Description'");
            }

            var cultureInfoValue = reader.GetAttribute("lang", "http://www.w3.org/XML/1998/namespace");
            if (!String.IsNullOrWhiteSpace(cultureInfoValue)) this.Language = CultureInfo.GetCultureInfo(cultureInfoValue);

            this.Value = reader.ReadElementContentAsString();
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            if (writer == null) throw new ArgumentNullException("writer");

            var prefix = writer.LookupPrefix(Constants.WsBaseFaultsNamespace);
            if (String.IsNullOrEmpty(prefix)) prefix = "wsbf";

            writer.WriteStartElement(prefix, "Description", Constants.WsBaseFaultsNamespace);
            if (this.Language != null)
            {
                writer.WriteAttributeString("xml", "lang", "http://www.w3.org/XML/1998/namespace", this.Language.Name);
            }
            writer.WriteString(this.Value);
            writer.WriteEndElement();
        }

        #endregion
    }
}