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
using System.Diagnostics.Contracts;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace CommonContracts.WsEventing
{
    /// <summary>
    /// Represents the "http://schemas.xmlsoap.org/ws/2004/08/eventing:Identifier" element.
    /// </summary>
    /// <remarks>
    /// In some cases, it is convenient for all EPRs issued by a single event source to address a
    /// single Web service and use a reference parameter to distinguish among the active subscriptions.
    /// For convenience in this common situation, this specification defines a global element,
    /// wsa:Identifier of type xs:anyURI, that MAY be used as a distinguishing reference parameter
    /// if desired by the event source. 
    /// </remarks>
    [XmlSchemaProvider("AcquireSchema")]
    [XmlRoot(DataType = "http://www.w3.org/2001/XMLSchema:anyURI", ElementName = "Identifier", Namespace = Constants.WsEventing.Namespace)]
    public class Identifier : IXmlSerializable
    {
        #region Fields

        private UniqueId identifier;
        
        #endregion

        #region Properties
        
        /// <summary>
        /// Gets or sets the <see cref="UniqueId"/> for the identifier.
        /// </summary>
        /// <value>The <see cref="UniqueId"/> for the identifier.</value>
        public virtual UniqueId Value
        {
            get { return identifier; }
            set
            {
                Contract.Requires<ArgumentNullException>(value != null, "Value");
                identifier = value;
            }
        }
        
        public AddressHeader AddressHeader
        {
            get { return AddressHeader.CreateAddressHeader("Identifier", Constants.WsEventing.Namespace, Value.ToString()); }
        }

        #endregion

        #region Constructors

        [Obsolete("This method is required for the XmlSerializer and not to be directly called")]
        public Identifier() : this(Guid.NewGuid())
        {
        }
        
        public Identifier(UniqueId id)
        {
            Contract.Requires<ArgumentNullException>(id != null, "id");

            this.identifier = id;
        }
        
        public Identifier(String id)
        {
            Contract.Requires<ArgumentNullException>(!String.IsNullOrWhiteSpace(id), "id");

            this.identifier = new UniqueId(id);
        }
        
        public Identifier(Identifier id)
        {
            Contract.Requires<ArgumentNullException>(id != null, "id");
            Contract.Assume(id.Value != null);

            this.identifier = id.Value;
        }
        
        public Identifier(Guid id) : this(String.Concat("uuid:", id))
        {
        }

        public Identifier(XmlReader reader)
        {
            ((IXmlSerializable)this).ReadXml(reader);
        }
        
        public Identifier(EndpointAddress epa)
        {
            Contract.Requires<ArgumentNullException>(epa != null, "epa");
            
            AddressHeader header = epa.Headers.FindHeader("Identifier", Constants.WsEventing.Namespace);
            if (header == null) throw new ArgumentException("No AddressHeader was found in the supplied EndpointAddress for the " + Constants.WsEventing.Namespace + ":Identifier QName");

            ((IXmlSerializable)this).ReadXml(header.GetAddressHeaderReader());
        }

        #endregion

        #region Factory Methods

        /// <summary>
        /// Factory method used to create an <see cref="Identifier"/> instance from the supplied endpoint address.
        /// </summary>
        /// <param name="epa">The <see cref="EndpointAddress"/> to attempt to locate and create an <see cref="Identifier"/> instance from the address headers collection.</param>
        /// <returns>If <paramref name="epa"/> contains an identifier value, the created instance; otherwise null.</returns>
        /// <exception cref="XmlException">The identifier xml is invalid to the defined schema.</exception>
        public static Identifier CreateFrom(EndpointAddress epa)
        {
            Contract.Requires<ArgumentNullException>(epa != null, "epa");

            var header = epa.Headers.FindHeader("Identifier", Constants.WsEventing.Namespace);
            if (header == null) return null;

            return new Identifier(header.GetAddressHeaderReader());
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

            if (reader.IsStartElement("Identifier", Constants.WsEventing.Namespace) == false)
            {
                throw new XmlException("Invalid Element, it must be 'Identifier'");
            }
            Value = new UniqueId(reader.ReadElementContentAsString("Identifier", Constants.WsEventing.Namespace));
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            var prefix = writer.LookupPrefix(Constants.WsEventing.Namespace);
            if (String.IsNullOrEmpty(prefix)) prefix = "wse";

            writer.WriteStartElement(prefix, "Identifier", Constants.WsEventing.Namespace);
            writer.WriteValue(this.Value.ToString());
            writer.WriteEndElement();
        }

        #endregion

        #region Schema

        /// <summary>
        /// Adds an <see cref="XmlSchema"/> instance for this type to the supplied <see cref="XmlSchemaSet"/>.
        /// </summary>
        /// <param name="xs">The <see cref="XmlSchemaSet"/> to add an <see cref="XmlSchema"/> to.</param>
        /// <returns>An <see cref="XmlQualifiedName"/> for the current object.</returns>
        public static XmlQualifiedName AcquireSchema(XmlSchemaSet xs)
        {
            Contract.Requires<ArgumentNullException>(xs != null, "xs");

            return new XmlQualifiedName("anyURI", "http://www.w3.org/2001/XMLSchema");
        }

        #endregion
    }
}
