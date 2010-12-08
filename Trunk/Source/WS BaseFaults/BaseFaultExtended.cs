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
using System.IO;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace CommonContracts.WsBaseFaults
{
    /// <summary>
    /// Provides extended support for the full members that are possible for WS-BaseFaults type.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <note type="inheritinfo">Inheritors should only use <see cref="DataMemberAttribute.Order"/> values greater than 4.</note>
    /// </para>
    /// <para>
    /// In general, when leveraging the base fault contract types, favor use of the <see cref="BaseFault"/>
    /// type and not the <see cref="BaseFaultExtended"/> type.
    /// </para>
    /// <para>
    /// Due to the schema of the BaseFaultType, the <see cref="DataContractSerializer"/> requires use of the
    /// of the <see cref="XElement"/> on the fault contract. It is intended however that direct use of the
    /// properties is avoided where possible and the use the strongly typed constructors to support initializing
    /// the needed xml values. This type performs NO XML Schema validation on any directly supplied XML content.
    /// </para>
    /// <para>
    /// Due to way the <seealso cref="DataContractSerializer"/> operates, only a single <see cref="CommonContracts.WsBaseFaults.Description"/>
    /// can be handled with this type. In addition, any <see cref="CommonContracts.WsBaseFaults.Description.Language"/> value will be ignored.
    /// If you find that you need to support sequences, then it is suggested that a custom <see cref="Exception"/> type is defined containing
    /// the contract types. This exception should be thrown instead of <see cref="FaultException"/> based types. Leverage a custom
    /// <see cref="IErrorHandler"/> that understands your custom exception and can provide a fault return message based on the custom exception.
    /// </para>
    /// </remarks>
    public abstract class BaseFaultExtended : BaseFault
    {
        #region Properties

        private XElement originator;
        private XElement description;
        private XElement errorCode;

        #endregion
        
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseFaultExtended"/> class with the current <see cref="DateTime.UtcNow"/> value.
        /// </summary>
        protected BaseFaultExtended()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseFaultExtended"/> class with the supplied optional contract elements.
        /// </summary>
        /// <param name="originator">The optional <see cref="Originator"/> value.</param>
        /// <param name="description">The optional <see cref="Description"/> value.</param>
        /// <param name="errorCode">The optional <see cref="ErrorCode"/> value.</param>
        protected BaseFaultExtended(EndpointAddress originator, Description description, ErrorCode errorCode) : this(null, DateTime.UtcNow, originator, description, errorCode)
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseFaultExtended"/> class with the full supplied contract elements.
        /// </summary>
        /// <remarks>If the supplied date is not in UTC, the value will be coerced.</remarks>
        /// <param name="faultCause">The optional cause of this fault.</param>
        /// <param name="utc">The <see cref="BaseFault.Timestamp"/> value.</param>
        /// <param name="originator">The optional <see cref="Originator"/> value.</param>
        /// <param name="description">The optional <see cref="Description"/> value.</param>
        /// <param name="errorCode">The optional <see cref="ErrorCode"/> value.</param>
        protected BaseFaultExtended(BaseFault faultCause, DateTime utc, EndpointAddress originator, Description description, ErrorCode errorCode) : base(faultCause, utc)
        {
            if (originator != null)
            {
                using (var stream = new MemoryStream())
                {
                    var writer = new XmlTextWriter(stream, null);
                    originator.WriteTo(AddressingVersion.WSAddressing10, writer);
                    writer.Flush();
                    stream.Position = 0;
                    this.originator = XElement.Load(stream);
                }
            }

            if (description != null)
            {
                using (var stream = new MemoryStream())
                {
                    var writer = new XmlTextWriter(stream, null);
                    ((IXmlSerializable)description).WriteXml(writer);
                    writer.Flush();
                    stream.Position = 0;
                    this.description = XElement.Load(stream);
                }
            }

            if (errorCode != null)
            {
                using (var stream = new MemoryStream())
                {
                    var writer = new XmlTextWriter(stream, null);
                    ((IXmlSerializable)errorCode).WriteXml(writer);
                    writer.Flush();
                    stream.Position = 0;
                    this.errorCode = XElement.Load(stream);
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseFaultExtended"/> class with the current <see cref="DateTime.UtcNow"/> value and the supplied <paramref name="faultCause"/>.
        /// </summary>
        /// <param name="faultCause">The optional cause of this fault.</param>
        protected BaseFaultExtended(BaseFault faultCause) : base(faultCause)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseFaultExtended"/> class with the supplied <paramref name="utc"/> value.
        /// </summary>
        /// <remarks>If the supplied date is not in UTC, the value will be coerced.</remarks>
        /// <param name="utc">The <see cref="BaseFault.Timestamp"/> value.</param>
        protected BaseFaultExtended(DateTime utc) : base(utc)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseFaultExtended"/> class with the supplied <paramref name="utc"/> and <paramref name="faultCause"/> values.
        /// </summary>
        /// <remarks>If the supplied date is not in UTC, the value will be coerced.</remarks>
        /// <param name="utc">The <see cref="BaseFault.Timestamp"/> value.</param>
        /// <param name="faultCause">The optional cause of this fault.</param>
        protected BaseFaultExtended(BaseFault faultCause, DateTime utc) : base(faultCause, utc)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets an <see cref="XElement"/> containing a WS-Addressing EPR for the source of the fault.
        /// </summary>
        /// <value>The <see cref="XElement"/> containing a WS-Addressing EPR for the source of the fault.</value>
        [DataMember(IsRequired = false, Name = "Originator", Order = 1)]
        public virtual XElement Originator
        {
            get { return originator; }
            set { originator = value; }
        }

        /// <summary>
        /// Gets or sets an <see cref="XElement"/> containing an <see cref="ErrorCode"/> for the fault.
        /// </summary>
        /// <value>The <see cref="XElement"/> containing an <see cref="ErrorCode"/> for the fault.</value>
        [DataMember(IsRequired = false, Name = "ErrorCode", Order = 2)]
        public virtual XElement ErrorCode
        {
            get { return errorCode; }
            set { errorCode = value; }
        }

        /// <summary>
        /// Gets or sets an <see cref="XElement"/> containing a <see cref="Description"/> for the fault.
        /// </summary>
        /// <remarks>
        /// Due to technical limitations of the <see cref="DataContractSerializer"/>, only a single description can be provided.
        /// See the remarks section on the <see cref="BaseFaultExtended"/> documentation for one possible workaround.
        /// </remarks>
        /// <value>The <see cref="XElement"/> containing a <see cref="Description"/> for the fault.</value>
        [DataMember(IsRequired = false, Name = "Description", Order = 3)]
        public virtual XElement Description
        {
            get { return description; }
            set { description = value; }
        }

        #endregion
    }
}
