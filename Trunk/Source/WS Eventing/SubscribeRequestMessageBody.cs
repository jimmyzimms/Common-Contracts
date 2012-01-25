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
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace CommonContracts.WsEventing
{
    /// <summary>
    /// Represents the "http://schemas.xmlsoap.org/ws/2004/08/eventing:SubscribeType" type.
    /// </summary>
    /// <remarks>This message body element only supports XPath message filtering.</remarks>
    [XmlSchemaProvider("AcquireSchema")]
    [XmlRoot(DataType = Constants.WsEventing.Namespace + ":SubscribeType", ElementName = "Subscribe", Namespace = Constants.WsEventing.Namespace)]
    public class SubscribeRequestMessageBody : IXmlSerializable
    {
        #region Fields

        private EndpointAddressAugust2004 endTo;
        private Delivery delivery;
        private Expires expires;
        private XPathMessageFilter filter;
        private String filterDialect;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the <see cref="EndpointAddressAugust2004"/> that a notification should be sent to if a subscription is terminated. This value may be null.
        /// </summary>
        /// <remarks>
        /// A subscriber can indicated where to send a <see cref="SubscriptionEndMessage"/> if the subscription is terminated unexpectedly. The default behavior is
        /// not to send this message. An event source is not required to respect this parameter if it is supplied. It is assumed that binding configuration is well
        /// known or documented / agreed upon through another mechanism. This contract performs no validation of endpoint addresses.
        /// </remarks>
        /// <value>The <see cref="EndpointAddressAugust2004"/> that a notification should be sent to if a subscription is terminated.</value>
        public virtual EndpointAddressAugust2004 EndTo
        {
            get { return this.endTo; }
            set { this.endTo = value; }
        }

        /// <summary>
        /// Gets or sets the <see cref="Delivery"/> destination for notification messages. This value is required.
        /// </summary>
        /// <value>The <see cref="Delivery"/> destination for notification messages, using some delivery mode.</value>
        public virtual Delivery Delivery
        {
            get
            {
                Contract.Ensures(Contract.Result<Delivery>() != null);

                return this.delivery;
            }
            set
            {
                Contract.Requires<ArgumentNullException>(value != null, "Delivery");

                this.delivery = value;
            }
        }

        /// <summary>
        /// Gets or sets the requested <see cref="Expires"/> time for the subscription. This value may be null.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The event source defines the actual expiration and is not constrained to use a time less or greater
        /// than the requested expiration. The expiration time may be a specific time or a duration from the
        /// subscription's creation time. Both specific times and durations are interpreted based on the event
        /// source's clock.
        /// </para>
        /// <para>
        /// If this value is null, then the request is for a subscription that will not expire. That is, the subscriber
        /// is requesting the event source to create a subscription with an indefinite lifetime. If the event source
        /// grants such a subscription, it may be terminated by the subscriber using an <see cref="UnsubscribeRequestMessage"/>, 
        /// or it may be terminated by the event source at any time for reasons such as connection termination, resource
        /// constraints, or system shut-down.
        /// </para>
        /// <para>
        /// If the expiration time is either a zero duration or a specific time that occurs in the past according to the
        /// event source, then the request MUST fail, and the event source MAY generate a wse:InvalidExpirationTime fault
        /// indicating that an invalid expiration time was requested.
        /// </para>
        /// <para>
        /// Some event sources may not have a "wall time" clock available, and so are only able to accept durations as
        /// expirations. If such a source receives a <see cref="SubscribeRequestMessage"/> containing a specific time
        /// expiration, then the request MAY fail; if so, the event source MAY generate a wse:UnsupportedExpirationType
        /// fault indicating that an unsupported expiration type was requested.
        /// </para>
        /// </remarks>
        /// <value>The requested <see cref="Expires"/> time for the subscription.</value>
        public virtual Expires Expires
        {
            get { return this.expires; }
            set { this.expires = value; }
        }

        /// <summary>
        /// Gets or sets an <see cref="XPathMessageFilter"/>
        /// </summary>
        /// <remarks>
        /// <para>
        /// The XPath 1.0 expression that evaluates to a boolean value for a notification. If the evaluation is false,
        /// the notification MUST NOT be sent to the event sink specified in the <see cref="Delivery"/> property. If null,
        /// the implied value is an expression that always returns true. If the event source does not support filtering,
        /// then a request that specifies a filter MUST fail, and the event source MAY generate a wse:FilteringNotSupported
        /// fault indicating that filtering is not supported.
        /// </para>
        /// </remarks>
        public virtual XPathMessageFilter Filter
        {
            get { return this.filter; }
            set
            {
                this.filter = value;
                this.filterDialect = Constants.WsEventing.Dialects.XPath;
            }
        }

        /// <summary>
        /// Gets a value indicating the dialect of the filter to be used by the event source to determine if a subscriber is
        /// interested in a notification.
        /// </summary>
        /// <remarks>
        /// <para>The default value is assumed to be XPath 1.0 as specified in <see cref="Constants.WsEventing.Dialects.XPath"/>.</para>
        /// <para>
        /// If the event source supports filtering but cannot honor the requested filtering, the request MUST fail, and the
        /// event source MAY generate a wse:FilteringRequestedUnavailable fault indicating that the requested filter dialect
        /// is not supported.
        /// </para>
        /// </remarks>
        /// <value>Gets a value indicating the dialect of the filter to be used by the event source to determine if a subscriber is interested in a notification.</value>
        public virtual String FilterDialect
        {
            get
            {
                Contract.Ensures(!String.IsNullOrWhiteSpace(Contract.Result<String>()));

                return this.filterDialect;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscribeRequestMessageBody"/> class.
        /// </summary>
        [Obsolete("This method is required for the XmlSerializer and not to be directly called")]
        public SubscribeRequestMessageBody() : this(new Delivery())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscribeRequestMessageBody"/> class with the supplied <paramref name="delivery"/>.
        /// </summary>
        /// <param name="delivery">The <see cref="Delivery"/> value.</param>
        public SubscribeRequestMessageBody(Delivery delivery) : this(delivery, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscribeRequestMessageBody"/> class with the supplied <paramref name="delivery"/> and optional <see cref="EndpointAddress"/> to send a notice if a subscription is terminated.
        /// </summary>
        /// <param name="delivery">The <see cref="Delivery"/> value.</param>
        /// <param name="endTo">An optional <see cref="EndpointAddress"/> containing the EPR that should be used when to send a notice if a subscription is terminated.</param>
        public SubscribeRequestMessageBody(Delivery delivery, EndpointAddress endTo = null)
        {
            Contract.Requires<ArgumentNullException>(delivery != null, "delivery");

            this.delivery = delivery;
            if (endTo == null) return;
            this.endTo = EndpointAddressAugust2004.FromEndpointAddress(endTo);
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

            if (reader.IsStartElement("Subscribe", Constants.WsEventing.Namespace) == false)
            {
                throw new XmlException("Invalid Element, it must be 'Subscribe'");
            }

            reader.ReadStartElement("Subscribe", Constants.WsEventing.Namespace);
            while (reader.NodeType != XmlNodeType.EndElement)
            {
                if (reader.IsStartElement("EndTo", Constants.WsEventing.Namespace))
                {
                    this.EndTo = EndpointAddressAugust2004.FromEndpointAddress(EndpointAddress.ReadFrom(AddressingVersion.WSAddressingAugust2004, reader));
                }
                else if (reader.IsStartElement("Delivery", Constants.WsEventing.Namespace))
                {
                    this.Delivery = new Delivery(reader);
                }
                else if (reader.IsStartElement("Expires", Constants.WsEventing.Namespace))
                {
                    this.Expires = new Expires(reader);
                }
                else if (reader.IsStartElement("Filter", Constants.WsEventing.Namespace))
                {
                    String dialect = reader.GetAttribute("Dialect");
                    if (String.IsNullOrEmpty(dialect) || dialect == Constants.WsEventing.Dialects.XPath)
                    {
                        this.Filter = new XPathMessageFilter(reader);
                    }
                    else
                    {
                        this.filterDialect = dialect;
                        reader.Skip();
                    }
                }
                reader.MoveToContent();
            }
            reader.ReadEndElement();
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            var prefix = writer.LookupPrefix(Constants.WsEventing.Namespace);
            if (String.IsNullOrEmpty(prefix)) prefix = "wse";

            writer.WriteStartElement(prefix, "Subscribe", Constants.WsEventing.Namespace);
            if (this.EndTo != null)
            {
                this.EndTo.ToEndpointAddress().WriteTo(AddressingVersion.WSAddressingAugust2004, writer, "EndTo", Constants.WsEventing.Namespace);
            }
            if (this.Delivery != null)
            {
                ((IXmlSerializable)this.Delivery).WriteXml(writer);
            }
            if (this.Expires != null)
            {
                ((IXmlSerializable)this.Expires).WriteXml(writer);
            }
            if (this.Filter != null)
            {
                this.Filter.WriteXPathTo(writer, prefix, "Filter", Constants.WsEventing.Namespace, true);
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

            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("CommonContracts.WsEventing.Subscribe.xsd"))
            {
                Debug.Assert(stream != null, "Resource Stream 'CommonContracts.WsEventing.Subscribe.xsd' was not able to be opened");

                var schema = XmlSchema.Read(stream, null);

                var imports = new XmlSchemaSet();
                Delivery.AcquireSchema(imports);
                Expires.AcquireSchema(imports);

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

            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("CommonContracts.WsEventing.WsAddressing.xsd"))
            {
                Debug.Assert(stream != null, "Resource Stream 'CommonContracts.WsEventing.WsAddressing.xsd' was not able to be opened");

                var schema = XmlSchema.Read(stream, null);
                xs.Add(schema);
            }


            return new XmlQualifiedName("SubscribeType", Constants.WsEventing.Namespace);
        }

        #endregion
    }
}
