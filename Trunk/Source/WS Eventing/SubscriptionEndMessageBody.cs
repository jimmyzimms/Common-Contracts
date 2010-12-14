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
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace CommonContracts.WsEventing
{
    /// <summary>
    /// Represents the "http://schemas.xmlsoap.org/ws/2004/08/eventing:SubscribeType" type.
    /// </summary>
    [XmlSchemaProvider("AcquireSchema")]
    [XmlRoot(DataType = Constants.WsEventing.Namespace + ":SubscriptionEndType", ElementName = "SubscriptionEnd", Namespace = Constants.WsEventing.Namespace)]
    public class SubscriptionEndMessageBody : IXmlSerializable
    {
        #region Fields

        private SubscriptionManager manager;
        private SubscriptionEndCode status;
        private String reason;

        #endregion

        #region Constructors

        [Obsolete("This method is required for the XmlSerializer and not to be directly called")]
        public SubscriptionEndMessageBody()
        {
        }

        public SubscriptionEndMessageBody(SubscriptionManager manager, SubscriptionEndCode status, String reason = null)
        {
            Contract.Requires<ArgumentNullException>(manager != null, "manager");

            this.manager = manager;
            this.status = status;
            this.reason = reason;
        }

        #endregion

        #region Properties

        public virtual SubscriptionManager SubscriptionManager
        {
            get { return this.manager; }
            set
            {
                Contract.Requires<ArgumentNullException>(value != null, "SubscriptionManager");

                this.manager = value;
            }
        }

        public virtual SubscriptionEndCode Status
        {
            get { return this.status; }
            set { this.status = value; }
        }

        public virtual String Reason { get { return this.reason; } set { this.reason = value; } }

        #endregion

        #region IXmlSerializable Members

        /// <summary>
        /// This method is reserved and should not be used. When implementing the IXmlSerializable interface, you should return null (Nothing in Visual Basic) from this method, and instead, if specifying a custom schema is required, apply the <see cref="T:System.Xml.Serialization.XmlSchemaProviderAttribute"/> to the class.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Xml.Schema.XmlSchema"/> that describes the XML representation of the object that is produced by the <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)"/> method and consumed by the <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)"/> method.
        /// </returns>
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Generates an object from its XML representation.
        /// </summary>
        /// <param name="reader">The <see cref="T:System.Xml.XmlReader"/> stream from which the object is deserialized. </param>
        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            Contract.Requires<ArgumentNullException>(reader != null);
            //Contract.Requires<ArgumentException>(reader.ReadState == ReadState.Interactive, String.Format(null, "The supplied XmlReader must be in the 'Interactive' state. The current state is '{0}'", reader.ReadState));

            if (reader.IsStartElement("SubscriptionEnd", Constants.WsEventing.Namespace) == false)
            {
                throw new XmlException("Invalid Element, it must be 'SubscriptionEnd'");
            }

            reader.ReadStartElement("SubscriptionEnd", Constants.WsEventing.Namespace);
            this.SubscriptionManager = new SubscriptionManager(reader);
            
            reader.ReadStartElement("Status", Constants.WsEventing.Namespace);
            var statusString = reader.ReadString();
            if (!String.IsNullOrWhiteSpace(statusString))
            {
                this.Status = SubscriptionEndCodeExtensions.Parse(statusString);
            }
            reader.ReadEndElement();

            reader.ReadStartElement("Reason", Constants.WsEventing.Namespace);
            this.reason = reader.ReadString();
            reader.ReadEndElement();
            
            reader.ReadEndElement();
        }

        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter"/> stream to which the object is serialized. </param>
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            var prefix = writer.LookupPrefix(Constants.WsEventing.Namespace);
            if (String.IsNullOrEmpty(prefix)) prefix = "wse";

            writer.WriteStartElement(prefix, "SubscriptionEnd", Constants.WsEventing.Namespace);
            
            if (this.SubscriptionManager != null)
            {
                ((IXmlSerializable)this.SubscriptionManager).WriteXml(writer);
            }

            writer.WriteStartElement("Status", Constants.WsEventing.Namespace);
            this.Status.WriteXml(writer);
            writer.WriteEndElement();

            if (!String.IsNullOrEmpty(this.Reason)) writer.WriteElementString(prefix, "Reason", Constants.WsEventing.Namespace, this.Reason);

            writer.WriteEndElement();
        }

        #endregion
    }
}
