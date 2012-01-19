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
using System.Diagnostics.Contracts;
using System.ServiceModel;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace CommonContracts.WsEventing.Faults
{
    /// <summary>
    /// Fault contract used when an Event Source is unable to process the request for any reason.
    /// </summary>
    /// <remarks>
    /// This fault is sent when the event source is not capable of fulfilling a Subscribe request 
    /// for local reasons unrelated to the specific request. Usually this is the catch-all fault
    /// to be returned when a critical runtime failure has occured.
    /// </remarks>
    public sealed class EventSourceUnableToProcessFault : IXmlSerializable
    {
        #region Fields

        private readonly static EventSourceUnableToProcessFault Instance = new EventSourceUnableToProcessFault();

        #endregion

        #region Properties

        /// <summary>
        /// Gets the default singleton instance that should be used when returning this fault.
        /// </summary>
        /// <value>The default <seealso cref="EventSourceUnableToProcessFault"/> instance.</value>
        public static EventSourceUnableToProcessFault Default
        {
            get
            {
                Contract.Ensures(Contract.Result<EventSourceUnableToProcessFault>() != null);

                return Instance;
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
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
        }

        #endregion

        #region Factory Methods

        /// <summary>
        /// Creates the apporiate standard <see cref="FaultReason"/> that should be used for the <see cref="EventSourceUnableToProcessFault"/>.
        /// </summary>
        /// <param name="reasonText">Text explaining the failure.</param>
        /// <returns>The standard <see cref="FaultReason"/>.</returns>
        public static FaultReason CreateFaultReason(String reasonText)
        {
            reasonText = reasonText ?? "The event source is unable to process this request.";

            return new FaultReason(reasonText);
        }

        /// <summary>
        /// Creates the apporiate standard <see cref="FaultCode"/> that should be used for the <see cref="EventSourceUnableToProcessFault"/>.
        /// </summary>
        /// <returns>The standard <see cref="FaultCode"/>.</returns>
        public static FaultCode CreateFaultCode()
        {
            return FaultCode.CreateSenderFaultCode("EventSourceUnableToProcess", Constants.WsEventing.Namespace);
        }

        #endregion
    }
}
