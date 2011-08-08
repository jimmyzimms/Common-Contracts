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
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace CommonContracts.WsBaseFaults.Extensions
{
    /// <summary>
    /// The fault detail message that indicates an Service Level Agreement issue for a request.
    /// </summary>
    /// <remarks>
    /// This fault is usually applied to service calls that have a hard execution time limit (N seconds or fault),
    /// data access services that can encounter a query timeout as normal course of operation (e.g. the SqlClient
    /// provider), or that perform hueristic evaluation of recieved requests for the caller (such as a service that
    /// combines access for premium and basic customers that limits basic callers to a single request per a time
    /// period). This fault contract can easily be used to support this common operational requirement in a standardized
    /// manner.
    /// </remarks>
    [XmlRoot("SLAViolation", Namespace = Constants.Namespace, DataType = Constants.Namespace + ":SLAViolationType")]
    [XmlSchemaProvider("AcquireSchema")]
    public class SlaViolationFault : BaseFaultFull
    {
        #region Overrides

        protected override void WriteStartElement(XmlWriter writer)
        {
        }

        protected override void WriteEndElement(XmlWriter writer)
        {
        }

        #endregion
        
        #region Methods

        /// <summary>
        /// Factory used to create the expected <see cref="FaultReason"/> to be used when returning this fault type.
        /// </summary>
        /// <returns>The default <see cref="FaultReason"/> to be used with the <see cref="SlaViolationFault"/> type.</returns>
        public static FaultReason CreateStandardReason()
        {
            var faultReason = new FaultReason("The recieved request to the service is unable to be processed in time or has encountered an issue regarding the scope of the request made by the caller.");
            return faultReason;
        }

        /// <summary>
        /// Factory used to create the expected <see cref="FaultCode"/> to be used when returning this fault type.
        /// </summary>
        /// <returns>The default <see cref="FaultCode"/> to be used with the <see cref="SlaViolationFault"/> type.</returns>
        public static FaultCode CreateStandardCode()
        {
            var faultCode = FaultCode.CreateSenderFaultCode(Constants.Faults.SlaViolationFault.FaultCode, Constants.Namespace);
            return faultCode;
        }

        /// <summary>
        /// Factory used to create the expected <see cref="MessageFault"/> to be used when returning this fault type.
        /// </summary>
        /// <param name="fault">The <see cref="SlaViolationFault"/> to create a <see cref="MessageFault"/> from.</param>
        /// <returns>The appropriate <see cref="MessageFault"/> for the supplied <see cref="SlaViolationFault"/>.</returns>
        public static MessageFault CreateStandardFault(SlaViolationFault fault)
        {
            var faultCode = CreateStandardCode();
            var faultReason = CreateStandardReason();

            return MessageFault.CreateFault(faultCode, faultReason, fault);
        }

        #endregion

        #region Schema

        /// <summary>
        /// Acquires the XML Schema for the current type.
        /// </summary>
        /// <param name="xs">The <see cref="XmlSchemaSet"/> to add the schema for this type to.</param>
        /// <returns>The <see cref="XmlQualifiedName"/> for this type.</returns>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "This parameter is validated via Code Contracts")]
        [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is a parameter name used in Code Contracts")]
        public new static XmlQualifiedName AcquireSchema(XmlSchemaSet xs)
        {
            Contract.Requires<ArgumentNullException>(xs != null, "xs");
            Debug.Assert(xs != null);

            BaseFaultFull.AcquireSchema(xs);

            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("CommonContracts.WsBaseFaults.Extensions.SlaViolationFault Schema.xsd"))
            {
                Debug.Assert(stream != null, "Resource Stream 'CommonContracts.WsBaseFaults.Extensions.SlaViolationFault Schema' was not able to be opened");

                var schema = XmlSchema.Read(stream, null);
                xs.Add(schema);    
            }

            return new XmlQualifiedName("SLAViolationType", Constants.Namespace);
        }

        #endregion
    }
}
