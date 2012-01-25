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
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace CommonContracts.WsEventing.Faults
{
    /// <summary>
    /// Fault contract used when a <see cref="SubscribeRequestMessageBody.FilterDialect"/> value is not supported by an Event Source.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This fault is sent when a <see cref="SubscribeRequestMessage"/> request specifies a filter dialect that the event source
    /// does not support.  Optionally, this fault may contain a list of supported filter dialect URIs. The supported dialects MAY
    /// be omiited when it there is other documentation or information that should be evident to the caller making the subscription 
    /// (such as an Event Source that is documented to only support XPath filtering).
    /// </para>
    /// <para>
    /// This should NOT be confused with an Event Source that does not support filtering. Instead use the <see cref="FilteringNotSupportedFault"/> type.
    /// </para>
    /// </remarks>
    [XmlSchemaProvider("AcquireSchema")]
    [XmlRoot(ElementName = "SupportedDialect", Namespace = Constants.WsEventing.Namespace)]
    public class SupportedDialectFault : IXmlSerializable
    {
        #region Fields

        private readonly static SupportedDialectFault Instance = new SupportedDialectFault();

        private IList<String> dialects;

        #endregion
        
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SupportedDialectFault"/> class.
        /// </summary>
        public SupportedDialectFault()
        {
            this.dialects = new List<String>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SupportedDialectFault"/> class.
        /// </summary>
        /// <param name="dialects">The sequence of supported dialects.</param>
        public SupportedDialectFault(params String[] dialects)
        {
            Contract.Requires<ArgumentNullException>(dialects != null, "dialects");
            
            dialects = dialects.Where(item => !String.IsNullOrWhiteSpace(item)).ToArray();

            this.dialects = dialects.ToList();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the list of the supported dialects.
        /// </summary>
        /// <value>The list of the supported dialects.</value>
        public virtual IList<String> Dialects
        {
            get { return this.dialects; }
            set
            {
                Contract.Requires(value != null, "Dialects");
                Contract.Requires(Contract.ForAll(value, item => !String.IsNullOrWhiteSpace(item)), "Dialects");

                this.dialects = value;
            }
        }

        /// <summary>
        /// Gets the default <see cref="SupportedDialectFault"/> singleton instance that can
        /// be used in any fault situation.
        /// </summary>
        /// <remarks>
        /// Use this property value to return an empty fault detail to a client. It is most beneficial
        /// when the service only supports a well known and documented set of filter dialects.
        /// </remarks>
        /// <value>The default <see cref="SupportedDialectFault"/> instance.</value>
        public static SupportedDialectFault Default
        {
            get
            {
                Contract.Ensures(Contract.Result<SupportedDialectFault>() != null);

                return Instance;
            }
        }

        #endregion

        #region IXmlSerializable Members

        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            if (reader == null) throw new ArgumentNullException("reader");

            if (!(reader.LocalName == "SupportedDialect" && reader.NamespaceURI == Constants.WsEventing.Namespace)) return;

            do
            {
                var mode = reader.ReadElementContentAsString();
                if (!String.IsNullOrWhiteSpace(mode)) this.Dialects.Add(mode);
            }
            while (reader.LocalName == "SupportedDialect" && reader.NamespaceURI == Constants.WsEventing.Namespace);
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            var prefix = writer.LookupPrefix(Constants.WsEventing.Namespace);
            if (String.IsNullOrEmpty(prefix)) prefix = "wse";

            if (!this.Dialects.Any()) return;

            foreach (var dialect in this.Dialects)
            {
                writer.WriteStartElement(prefix, "SupportedDialect", Constants.WsEventing.Namespace);
                writer.WriteValue(dialect);
                writer.WriteEndElement();
            }
        }

        #endregion

        #region Factory Methods

        /// <summary>
        /// Creates the apporiate standard <see cref="FaultReason"/> that should be used for the <see cref="SupportedDialectFault"/>.
        /// </summary>
        /// <returns>The standard <see cref="FaultReason"/>.</returns>
        public static FaultReason CreateFaultReason()
        {
            return new FaultReason("The requested filter dialect is not supported.");
        }

        /// <summary>
        /// Creates the apporiate standard <see cref="FaultCode"/> that should be used for the <see cref="SupportedDialectFault"/>.
        /// </summary>
        /// <returns>The standard <see cref="FaultCode"/>.</returns>
        public static FaultCode CreateFaultCode()
        {
            return FaultCode.CreateSenderFaultCode("FilteringRequestedUnavailable", Constants.WsEventing.Namespace);
        }

        #endregion

        #region Schema

        /// <summary>
        /// Adds an <see cref="XmlSchema"/> instance for this type to the supplied <see cref="XmlSchemaSet"/>.
        /// </summary>
        /// <param name="xs">The <see cref="XmlSchemaSet"/> to add an <see cref="XmlSchema"/> to.</param>
        /// <returns>An <see cref="XmlQualifiedName"/> for the current object.</returns>
        public static XmlSchemaType AcquireSchema(XmlSchemaSet xs)
        {
            if (xs == null) throw new ArgumentNullException("xs");

            var schema = XmlSchema.Read(new StringReader(@"<xs:schema xmlns:xs='http://www.w3.org/2001/XMLSchema' targetNamespace='http://schemas.xmlsoap.org/ws/2004/08/eventing'><xs:element name='SupportedDialect' type='xs:anyURI'/></xs:schema>"), null);
            xs.Add(schema);
            var type = schema.Items.OfType<XmlSchemaElement>().First(element => element.Name == "SupportedDialect");

            return type.SchemaType;
        }

        #endregion
    }
}
