﻿#region Legal

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
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace CommonContracts.WsEventing
{
    /// <summary>
    /// Represents the "http://schemas.xmlsoap.org/ws/2004/08/eventing:GetStatusType" type.
    /// </summary>
    [XmlSchemaProvider("AcquireSchema")]
    [XmlRoot(DataType = Constants.WsEventing.Namespace + ":RenewRequestType", ElementName = "Renew", Namespace = Constants.WsEventing.Namespace)]
    public class RenewRequestMessageBody : IXmlSerializable
    {
        #region Fields

        private Expires expires;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the requested <see cref="Expires"/> time for the new subscription expiration. This value may be null.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The event source defines the actual expiration and is not constrained to use a time less or greater
        /// than the requested expiration. The expiration time may be a specific time or a duration value.
        /// If the requested expiration is a duration, then the implied start of that duration is the time when 
        /// the subscription manager starts processing the Renew request. Both specific times and durations are
        /// interpreted based on the event source's clock.
        /// </para>
        /// <para>
        /// If this value is null, then the request is for a subscription that will not expire. That is, the subscriber
        /// is requesting the event source to renew a subscription with an indefinite lifetime. If the event source
        /// grants such a subscription, it may be terminated by the subscriber using an <see cref="UnsubscribeRequestMessage"/>, 
        /// or it may be terminated by the event source at any time for reasons such as connection termination, resource
        /// constraints, or system shut-down.
        /// </para>
        /// <para>
        /// If the subscription manager chooses not to renew this subscription, the request MUST fail, and the subscription 
        /// manager MAY generate a wse:UnableToRenew fault indicating that the renewal was not accepted.
        /// </para>
        /// </remarks>
        /// <value>The requested <see cref="Expires"/> time for the subscription.</value>
        public virtual Expires Expires
        {
            get { return this.expires; }
            set { this.expires = value; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RenewRequestMessageBody"/> class.
        /// </summary>
        /// <remarks>
        /// This overload does not request a specific expiration time (relying on the event source default)
        /// to be applied to the subscription.
        /// </remarks>
        public RenewRequestMessageBody()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RenewRequestMessageBody"/> class with the indicated <see cref="WsEventing.Expires">expiration</see>.
        /// </summary>
        /// <param name="expires">The <see cref="WsEventing.Expires"/> value containing the details of the requested new expiration for the subscription.</param>
        public RenewRequestMessageBody(Expires expires)
        {
            Contract.Requires<ArgumentNullException>(expires != null, "expires");

            this.expires = expires;
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

            reader.ReadStartElement("Renew", Constants.WsEventing.Namespace);
            reader.Read();

            if (reader.IsStartElement("Expires", Constants.WsEventing.Namespace))
            {
                this.Expires = new Expires(reader);
            }

            if (reader.LocalName == "Expires") reader.ReadEndElement();

            reader.ReadEndElement();
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {

            var prefix = writer.LookupPrefix(Constants.WsEventing.Namespace);
            if (String.IsNullOrEmpty(prefix)) prefix = "wse";

            writer.WriteStartElement(prefix, "Renew", Constants.WsEventing.Namespace);
            if (this.Expires != null) ((IXmlSerializable) this.Expires).WriteXml(writer);
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
            if (xs == null) throw new ArgumentNullException("xs");

            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("CommonContracts.WsEventing.Renew.xsd"))
            {
                Debug.Assert(stream != null, "Resource Stream 'CommonContracts.WsEventing.Renew.xsd' was not able to be opened");

                var schema = XmlSchema.Read(stream, null);

                var imports = new XmlSchemaSet();
                Expires.AcquireSchema(imports);
                SubscriptionManager.AcquireSchema(imports);

                foreach (var includeSchema in imports.Schemas().Cast<XmlSchema>())
                {
                    if (includeSchema.TargetNamespace == Constants.WsEventing.Namespace)
                    {
                        XmlSchemaInclude include = new XmlSchemaInclude();
                        include.Schema = includeSchema;
                        schema.Includes.Add(include);
                    }
                }

                xs.Add(schema);
            }

            return new XmlQualifiedName("RenewType", Constants.WsEventing.Namespace);
        }

        #endregion
    }
}
