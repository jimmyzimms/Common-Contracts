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
using System.Linq;
using System.ServiceModel;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace CommonContracts.WsEventing
{
    [XmlSchemaProvider(null, IsAny = true)]
    public class SupportedDeliveryModeFault : IXmlSerializable
    {
        #region Fields
        
        private IList<String> modes;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SupportedDeliveryModeFault"/> class.
        /// </summary>
        public SupportedDeliveryModeFault()
        {
            this.modes = new List<String>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SupportedDeliveryModeFault"/> class.
        /// </summary>
        /// <param name="modes">The sequence of supported modes.</param>
        public SupportedDeliveryModeFault(IEnumerable<String> modes)
        {
            Contract.Requires<ArgumentNullException>(modes != null, "modes");
            Contract.Requires<ArgumentNullException>(Contract.ForAll(modes, item => !String.IsNullOrWhiteSpace(item)), "modes");

            this.modes = modes.ToList();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the list of supported modes.
        /// </summary>
        /// <value>The list of supported modes.</value>
        public virtual IList<String> Modes
        {
            get { return this.modes; }
            set
            {
                Contract.Requires<ArgumentNullException>(value != null, "Modes");
                Contract.Requires<ArgumentNullException>(Contract.ForAll(value, item => !String.IsNullOrWhiteSpace(item)), "Modes");

                this.modes = value;
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
            reader.ReadStartElement("SupportedDeliveryMode", Constants.WsEventing.Namespace);
            while (reader.NodeType != XmlNodeType.EndElement)
            {
                this.Modes.Add(reader.ReadContentAsString());
                reader.MoveToContent();
            }
            reader.ReadEndElement();
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("wse", "SupportedDeliveryMode", Constants.WsEventing.Namespace);
            if (this.Modes != null && this.Modes.Any())
            {
                foreach (var mode in this.Modes)
                {
                    writer.WriteElementString("Mode", mode);
                }
            }
            writer.WriteEndElement();
        }

        #endregion

        #region Factory Methods
        
        /// <summary>
        /// Creates the apporiate standard <see cref="FaultReason"/> that should be used for the <see cref="SupportedDeliveryModeFault"/>.
        /// </summary>
        /// <returns>The standard <see cref="FaultReason"/>.</returns>
        public static FaultReason CreateFaultReason()
        {
            return new FaultReason("The requested delivery mode is not supported.");
        }

        /// <summary>
        /// Creates the apporiate standard <see cref="FaultCode"/> that should be used for the <see cref="SupportedDeliveryModeFault"/>.
        /// </summary>
        /// <returns>The standard <see cref="FaultCode"/>.</returns>
        public static FaultCode CreateFaultCode()
        {
            return FaultCode.CreateSenderFaultCode("DeliveryModeRequestedUnvailable", Constants.WsEventing.Namespace);
        }

        #endregion
    }
}
