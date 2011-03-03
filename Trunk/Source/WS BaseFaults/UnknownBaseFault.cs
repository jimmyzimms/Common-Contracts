#region Legal

// Jimmy Zimmerman
// Team Mongoose
//
// END USER LICENSE AGREEMENT
// IMPORTANT - READ THIS CAREFULLY:  This End User License Agreement is a legal agreement
// between you (either an individual, business entity, currently active identity of an
// individual with Multiple Personality Disorder, octopus overlord, or otherworldly entity),
// and Team Mongoose, for the enclosed, components.
//
// By reading this document and/or installing this product, you agree to be bound by the
// terms of this EULA.
//
// Team Mongoose owns all copyright, trade secret, trademark, trade wars,
// patent, portent, and potential rights to these components.  Team Mongoose
// grants you the right to deploy the enclosed components.
//
// If you agree to the terms of this EULA, a license to use these components is granted to you.
//
// If you should happen to benefit from the enclosed components, then you are legally and
// contractually bound to thank us for it. Send your regards to jimmyzimms@hotmail.com.
//
// OTHER RIGHTS AND LIMITATONS
// You may not reverse-engineer, decompile, decode, disassemble, psycho-analyze, or otherwise
// attempt to find hidden meanings between the lines of this EULA; unless, of course, you should
// happen to find some, and they are funny.
//
// You may not translate this EULA or any part of the components into Ancient Sumerian.
//
// THESE COMPONENTS ARE PROVIDED “AS-IS” WITHOUT WARRANTY OF ANY KIND. ANY USE OF THE COMPONENTS
// CONTAINED IS AT YOUR OWN RISK. TEAM MONGOOSE DISCLAIM ALL WARRANTIES, EITHER
// EXPRESS OR IMPLIED, WITH RESPECT TO THE ACCURRACY AND CORRECTNESS OF THE COMPONENTS CONTAINED
// HEREIN. TEAM MONGOOSE DOES NOT WARRANT THAT THE COMPONENTS ARE FLAWLESS.
//
// REDISTRIBUTION AND USE IN SOURCE AND BINARY FORMS, WITH OR WITHOUT MODIFICATION, ARE PERMITTED
// PROVIDED THAT THE FOLLOWING CONDITIONS ARE MET:
// * REDISTRIBUTIONS OF SOURCE CODE MUST RETAIN THE ABOVE COPYRIGHT NOTICE
// * REDISTRIBUTIONS IN BINARY FORM MUST NOTE THE USE OF THE COMPONENT IN DOCUMENTATION AND/OR
//   OTHER MATERIALS PROVIDED WITH THE DISTRIBUTION.
// * NEITHER THE NAME OF TEAM MONGOOSE MAY BE USED TO ENDORES OR PROMOTE PRODUCTS
//   DERIVED FROM THIS SOFTWARE WITHOUT SPECIFIC PRIOR WRITTEN PERMISSION.
//
// IN NO EVENT SHALL TEAM MONGOOSE BE HELD LIABLE FOR INCIDENTAL, SPECIAL, INDIRECT,
// INCONSEQUENTIAL, UNBELIEVABLE, EXAGGERATED, VERBOSE, OR TYPICAL DAMAGES INCURRED WHILE USING
// THE ENCLOSED COMPONENTS.
//
// OUR STUFF ALWAYS WORKS - SOMETIMES.

#endregion

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.ServiceModel.Channels;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace CommonContracts.WsBaseFaults
{
    /// <summary>
    /// A custom <see cref="BaseFaultFull"/> type that is useful when the deserialization occurs of a base fault extension that you do not have a class representation of.
    /// </summary>
    /// <remarks>
    /// This type can be used whenever you have untyped <see cref="Message"/> based service client proxies and need to represent a unknown (or any derivation thereof)
    /// of a WS-BaseFaults compliant fault. While not a firm requirement, this class is generally used by service clients.
    /// </remarks>
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
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "This type is validated via Code Contracts")]
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
        [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "Due to the way this type is used as a class for serialization of unknown types this string value makes sense")]
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
