using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.ServiceModel;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace CommonContracts.WsBaseFaults
{
    /// <summary>
    /// A custom <see cref="BaseFaultFull"/> type that is usuful when the deserialization occurs of a base fault extension that you do not have a class representation of.
    /// </summary>
    /// <remarks>Due to the schema of the BaseFaultType, WCF requires use of the <see cref="XmlSerializerFormatAttribute"/> on your service contracts.</remarks>
    [XmlRoot("BaseFault", Namespace = Constants.WsBaseFaultsNamespace, DataType = Constants.WsBaseFaultsNamespace + ":BaseFaultType")]
    [XmlSchemaProvider("AcquireSchema")]
    public class UnknownBaseFault : BaseFaultFull
    {
        #region Fields
        
        private String namespaceUri;
        private String localName;
        private String xmlType;
        private readonly ICollection<XElement> additionalContent = new Collection<XElement>();
        private readonly ICollection<XAttribute> additionalAttributes = new Collection<XAttribute>();

        #endregion

        #region Constructor
        
        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownBaseFault"/> class from the supplied <paramref name="reader"/>.
        /// </summary>
        /// <param name="reader">The <see cref="XmlReader"/> to create an instance from.</param>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "This is checked via Code Contracts. CA Engine does not yet understand how to deal with contracts.")]
        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", Justification = "This is the parameter name of the code and globalization is not needed.")]
        public UnknownBaseFault(XmlReader reader)
        {
            Contract.Requires<ArgumentNullException>(reader != null, "reader");
            Contract.Requires<ArgumentException>(reader.ReadState == ReadState.Interactive, "reader");

            ((IXmlSerializable)this).ReadXml(reader);
        }

        #endregion

        #region Overrides
        
        /// <summary>
        /// Extension point for reading the start element for an unknown <see cref="BaseFaultFull"/> type.
        /// </summary>
        /// <remarks>
        /// This method ignore the root element and namespace and will begin processing the reader. If does not call the base method.
        /// </remarks>
        /// <param name="reader">The <see cref="XmlReader"/> to read the XML from.</param>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "This is checked via Code Contracts. CA Engine does not yet understand how to deal with contracts.")]
        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", Justification = "This is the parameter name of the code and globalization is not needed.")]
        protected override void ReadStartElement(XmlReader reader)
        {
            this.namespaceUri = reader.NamespaceURI;
            this.localName = reader.LocalName;
            var typeAttribute = reader.GetAttribute("type", Constants.XmlSchemaTypeNamespace);
            this.xmlType = typeAttribute ?? String.Empty;

            if (reader.HasAttributes)
            {
                while (reader.MoveToNextAttribute())
                {
                    if (reader.NamespaceURI == "http://www.w3.org/2000/xmlns/" || reader.NamespaceURI == Constants.XmlSchemaTypeNamespace) continue;

                    var attribute = new XAttribute(XName.Get(reader.LocalName, reader.NamespaceURI), reader.Value);
                    this.additionalAttributes.Add(attribute);
                }
            }
            reader.ReadStartElement();
        }

        /// <summary>
        /// Extension point for additional xml content to be handled. This method simply creates a new element available via the <see cref="AdditionalContent"/> property containing the xml content of the current node.
        /// </summary>
        /// <param name="reader">The <see cref="XmlReader"/> containing the XML to process.</param>
        protected override void ProcessAdditionalElements(XmlReader reader)
        {
            this.additionalContent.Add(XElement.Parse(reader.ReadOuterXml()));
        }

        #endregion

        #region Properties
        
        /// <summary>
        /// Gets the xsi:type value, if any, for the fault.
        /// </summary>
        /// <value>The xsi:type value, if any, for the fault.</value>
        public String XmlType
        {
            get
            {
                Contract.Ensures(Contract.Result<String>() != null);
                return this.xmlType;
            }
        }

        /// <summary>
        /// Gets the unknown base fault element namespace uri.
        /// </summary>
        /// <value>The unknown base fault element namespace uri.</value>
        public String NamespaceUri
        {
            get
            {
                Contract.Ensures(Contract.Result<String>() != null);
                return this.namespaceUri;
            }
        }

        /// <summary>
        /// Gets the unknown base fault element local name.
        /// </summary>
        /// <value>The unknown base fault element local name.</value>
        public String LocalName
        {
            get
            {
                Contract.Ensures(!String.IsNullOrEmpty(Contract.Result<String>()));
                return this.localName;
            }
        }

        /// <summary>
        /// Gets any additional xml content that may be part of the <see cref="UnknownBaseFault"/>.
        /// </summary>
        /// <value>Any additional xml content that may be part of the <see cref="UnknownBaseFault"/>.</value>
        public IEnumerable<XElement> AdditionalContent
        {
            get
            {
                Contract.Ensures(Contract.Result<IEnumerable<XElement>>() != null);
                Contract.Ensures(Contract.ForAll(Contract.Result<IEnumerable<XElement>>(), item => item != null));
                return this.additionalContent;
            }
        }

        /// <summary>
        /// Gets any additional xml attributes that may be part of the root fault element.
        /// </summary>
        /// <value>Any additional xml attributes that may be part of the root fault element.</value>
        public IEnumerable<XAttribute> AdditionalAttributes
        {
            get
            {
                Contract.Ensures(Contract.Result<IEnumerable<XAttribute>>() != null);
                Contract.Ensures(Contract.ForAll(Contract.Result<IEnumerable<XAttribute>>(), item => item != null));

                return this.additionalAttributes;
            }
        }

        #endregion
    }
}
