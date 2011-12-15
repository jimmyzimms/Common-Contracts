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
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace CommonContracts.WsEventing
{
    /// <summary>
    /// Represents the /wse:SubscribeResponse/SubscriptionManager element.
    /// </summary>
    [XmlSchemaProvider("AcquireSchema")]
    [XmlRoot(DataType = Constants.WsEventing.Namespace + ":SubscribeResponseType", ElementName = "SubscribeResponse", Namespace = Constants.WsEventing.Namespace)]
    public class SubscriptionManager : IXmlSerializable
    {
        #region Fields

        private EndpointAddressAugust2004 epa;
        
        #endregion

        #region Properties
        
        public virtual EndpointAddressAugust2004 EndpointAddress
        {
            get
            {
                Contract.Ensures(Contract.Result<EndpointAddressAugust2004>() != null);

                return this.epa;
            }
        }
        
        public Identifier Identifier
        {
            get { return new Identifier(this.EndpointAddress.ToEndpointAddress()); }
        }
        
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionManager"/> class with the default values. This constructor should only be used for deserialization.
        /// </summary>
        [Obsolete("This method is required for the XmlSerializer and not to be directly called")]
        public SubscriptionManager()
        {
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionManager"/> class from the supplied <see cref="Uri"/> address.
        /// </summary>
        /// <remarks>
        /// This overload is best used with simple scenarios where the client only needs to understand an address to communicate with the
        /// event source (such as when the event source is self managing and therefore leverages the same binding) or when the binding is
        /// well known / communicated out of band to clients.
        /// </remarks>
        /// <param name="address">The endpoint address of the subscription manager used by the event sink to manage subscriptions for this event source.</param>
        public SubscriptionManager(Uri address)
        {
            Contract.Requires(address != null, "address");

            this.epa = EndpointAddressAugust2004.FromEndpointAddress(new EndpointAddress(address));
        }

        public SubscriptionManager(Uri address, Identifier id)
        {
            Contract.Requires(address != null, "address");
            Contract.Requires(id != null, "id");

            this.epa = EndpointAddressAugust2004.FromEndpointAddress(new EndpointAddress(address, new [] { id.AddressHeader }));
        }
        
        public SubscriptionManager(Uri address, IEnumerable<AddressHeader> headers)
        {
            Contract.Requires(address != null, "address");
            Contract.Requires(headers != null, "headers");
            Contract.Requires(Contract.ForAll(headers, item => item != null));

            this.epa = EndpointAddressAugust2004.FromEndpointAddress( new EndpointAddress(address, headers.ToArray()));
        }
        
        public SubscriptionManager(EndpointAddress address)
        {
            Contract.Requires(address != null, "address");
        }

        public SubscriptionManager(XmlReader reader)
        {
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

            this.epa = EndpointAddressAugust2004.FromEndpointAddress(System.ServiceModel.EndpointAddress.ReadFrom(AddressingVersion.WSAddressingAugust2004, reader, "SubscriptionManager", Constants.WsEventing.Namespace));
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            if (this.EndpointAddress != null)
            {
                this.EndpointAddress.ToEndpointAddress().WriteTo(AddressingVersion.WSAddressingAugust2004, writer, "SubscriptionManager", Constants.WsEventing.Namespace);
            }
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

            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("CommonContracts.WsEventing.WsAddressing.xsd"))
            {
                Debug.Assert(stream != null, "Resource Stream 'CommonContracts.WsEventing.WsAddressing.xsd' was not able to be opened");

                var schema = XmlSchema.Read(stream, null);
                xs.Add(schema);
            }

            return new XmlQualifiedName("EndpointReferenceType", Constants.WsAddressing.Namespace);
        }

        #endregion
    }
}
