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
    /// Represents the /wse:SubscribeResponse/SubscriptionManager element.
    /// </summary>
    [XmlSchemaProvider("AcquireSchema")]
    [XmlRoot(DataType = Constants.WsEventing.Namespace + ":SubscribeResponseType", ElementName = "SubscribeResponse", Namespace = Constants.WsEventing.Namespace)]
    public class SubscriptionManager : IXmlSerializable
    {
        #region Fields

        private EndpointAddress epa;
        
        #endregion

        #region Properties
        
        public virtual EndpointAddress EndpointAddress
        {
            get { return this.epa; }
            set { this.epa = value; }
        }
        
        public Identifier Identifier
        {
            get { return new Identifier(EndpointAddress); }
        }
        
        #endregion

        #region Constructors

        [Obsolete("This method is required for the XmlSerializer and not not be directly called")]
        public SubscriptionManager()
        {
        }
        
        public SubscriptionManager(Uri address, Identifier id)
        {
            this.epa= new EndpointAddress(address, new [] { id.AddressHeader });
        }
        
        public SubscriptionManager(Uri address, AddressHeader[] headers)
        {
            this.epa = new EndpointAddress(address, headers);
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

            this.EndpointAddress = EndpointAddress.ReadFrom(AddressingVersion.WSAddressing10, reader, "SubscriptionManager", Constants.WsEventing.Namespace);
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            if (this.EndpointAddress != null)
            {
                this.EndpointAddress.WriteTo(AddressingVersion.WSAddressing10, writer, "SubscriptionManager", Constants.WsEventing.Namespace);
            }
        }

        #endregion

        #region Schema

        public static XmlQualifiedName AcquireSchema(XmlSchemaSet xs)
        {
            Contract.Requires<ArgumentNullException>(xs != null, "xs");

            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ProvPubs.Soupbone.Contracts.WsEventing.WsAddressing.xsd"))
            {
                Debug.Assert(stream != null, "Resource Stream 'ProvPubs.Soupbone.Contracts.WsEventing.WsAddressing.xsd' was not able to be opened");

                var schema = XmlSchema.Read(stream, null);
                xs.Add(schema);
            }

            return new XmlQualifiedName("EndpointReferenceType", Constants.WsAddressing.Namespace);
        }

        #endregion
    }
}
