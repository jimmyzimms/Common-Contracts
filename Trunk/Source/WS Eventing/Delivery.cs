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
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace CommonContracts.WsEventing
{
    /// <summary>
    /// Represents the "http://schemas.xmlsoap.org/ws/2004/08/eventing:Delivery" element.
    /// </summary>
    [XmlSchemaProvider("AcquireSchema")]
    [XmlRoot(DataType = Constants.WsEventing.Namespace + ":Delivery", ElementName = "Delivery", Namespace = Constants.WsEventing.Namespace)]
    public class Delivery : HeaderCollection, IXmlSerializable
    {
        #region Fields

        private Uri mode;
        private EndpointAddressAugust2004 notifyTo;
        
        #endregion

        #region Properties
        
        public virtual Uri DeliveryMode
        {
            get { return this.mode; }
            set { this.mode = value; }
        }

        public virtual EndpointAddressAugust2004 NotifyTo
        {
            get { return this.notifyTo; }
            set { this.notifyTo = value; }
        }

        #endregion

        #region Constructors
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Delivery"/> class with the default <see cref="Constants.WsEventing.DeliverModes.Push"/> delivery mode.
        /// </summary>
        public Delivery() : this(new Uri(Constants.WsEventing.DeliverModes.Push), null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Delivery"/> class with the suppleid <paramref name="mode"/> and <paramref name="notifyTo"/> <see cref="Uri"/> parameter values.
        /// </summary>
        /// <param name="mode">A <see cref="Uri"/> containing the <see cref="DeliveryMode"/> value.</param>
        /// <param name="notifyTo">An <see cref="EndpointAddress"/> containing the location to notify an event message to.</param>
        public Delivery(Uri mode, EndpointAddress notifyTo)
        {
            Contract.Requires<ArgumentNullException>(mode != null, "mode");

            this.mode = mode;
            if (notifyTo == null) return;

            this.notifyTo = EndpointAddressAugust2004.FromEndpointAddress(notifyTo);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Delivery"/> class from the supplied <paramref name="reader"/>.
        /// </summary>
        /// <param name="reader">An <see cref="XmlReader"/> to create an instance of the <see cref="Delivery"/> class from.</param>
        public Delivery(XmlReader reader)
        {
            Contract.Requires<ArgumentNullException>(reader != null, "reader");
            Contract.Requires<ArgumentException>(reader.ReadState == ReadState.Interactive, "The XmlReader must be in an interactive state to be read from");

            ((IXmlSerializable)this).ReadXml(reader);
        }

        #endregion

        #region IXmlSerializable Members

        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            Contract.Requires<ArgumentNullException>(reader != null);
            //Contract.Requires<ArgumentException>(reader.ReadState == ReadState.Interactive, String.Format(null, "The supplied XmlReader must be in the 'Interactive' state. The current state is '{0}'", reader.ReadState));

            if (reader.IsStartElement("Delivery", Constants.WsEventing.Namespace) == false)
            {
                throw new XmlException("Invalid Element, it must be 'Delivery'");
            }

            String modeAsString = reader.GetAttribute("Mode", Constants.WsEventing.Namespace);
            this.DeliveryMode = String.IsNullOrEmpty(modeAsString) ? DeliveryMode = new Uri(Constants.WsEventing.DeliverModes.Push) : new Uri(modeAsString);

            reader.ReadToDescendant("NotifyTo", Constants.WsEventing.Namespace);
            this.NotifyTo = EndpointAddressAugust2004.FromEndpointAddress(EndpointAddress.ReadFrom(AddressingVersion.WSAddressingAugust2004, reader));
            if (this.NotifyTo == null) throw new XmlException("Missing element 'NotifyTo'");

            // option: additional headers
            while (reader.NodeType != XmlNodeType.EndElement)
            {
                AddressHeader ah = AddressHeader.CreateAddressHeader(reader.Name, reader.NamespaceURI, reader.ReadElementContentAsObject());
                this.Add(ah);
                reader.MoveToContent();
            }
            reader.ReadEndElement();
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            if (this.NotifyTo == null) throw new InvalidOperationException("Cannot serialize Delivery contract as there is no NotifyTo value");

            var prefix = writer.LookupPrefix(Constants.WsEventing.Namespace);
            if (String.IsNullOrEmpty(prefix)) prefix = "wse";

            writer.WriteStartElement(prefix, "Delivery", Constants.WsEventing.Namespace);
            if (!this.DeliveryMode.Equals(Constants.WsEventing.DeliverModes.Push))
            {
                writer.WriteAttributeString(prefix, "Mode", Constants.WsEventing.Namespace, DeliveryMode.ToString());
            }
            writer.WriteStartElement(prefix, "NotifyTo", Constants.WsEventing.Namespace);

            ((IXmlSerializable)this.NotifyTo).WriteXml(writer);
            //this.NotifyTo.ToEndpointAddress().WriteTo(AddressingVersion.WSAddressingAugust2004, writer, "NotifyTo", Constants.WsEventing.Namespace);

            writer.WriteEndElement();

            // option: additional headers
            for (int ii = 0; ii < base.Count; ii++)
            {
                base[ii].WriteAddressHeader(writer);
            }
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

            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("CommonContracts.WsEventing.DeliveryType.xsd"))
            {
                Debug.Assert(stream != null, "Resource Stream 'CommonContracts.WsEventing.DeliveryType.xsd' was not able to be opened");

                var schema = XmlSchema.Read(stream, null);
                xs.Add(schema);
            }
            EndpointAddressAugust2004.GetSchema(xs);
            return new XmlQualifiedName("DeliveryType", Constants.WsEventing.Namespace);
        }

        #endregion
    }
}
