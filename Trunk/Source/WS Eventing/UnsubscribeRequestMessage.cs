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
using System.ServiceModel;

namespace CommonContracts.WsEventing
{
    /// <summary>
    /// The request message for the <see cref="ISubscriptionManager.Unsubscribe"/> operation.
    /// </summary>
    [MessageContract(IsWrapped = false)]
    public class UnsubscribeRequestMessage
    {
        #region Fields

        private Identifier identifier;
        private UnsubscribeRequestMessageBody body;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UnsubscribeRequestMessage"/> class.
        /// </summary>
        public UnsubscribeRequestMessage()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnsubscribeRequestMessage"/> class.
        /// </summary>
        /// <param name="identifier">The <see cref="Identifier"/> containing the subscription information.</param>
        public UnsubscribeRequestMessage(Identifier identifier)
        {
            Contract.Requires<ArgumentNullException>(identifier != null, "identifier");

            this.identifier = identifier;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnsubscribeRequestMessage"/> class with the supplied body value.
        /// </summary>
        /// <remarks>
        /// Generally used with custom <see cref="UnsubscribeRequestMessageBody"/> content types.
        /// </remarks>
        /// <param name="body">The body content of the unsubscribe request.</param>
        public UnsubscribeRequestMessage(UnsubscribeRequestMessageBody body)
        {
            Contract.Requires<ArgumentNullException>(body != null, "body");

            this.body = body;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the <see cref="Identifier"/> value for the subscription end request.
        /// </summary>
        /// <value>The <see cref="Identifier"/> value for the subscription end request.</value>
        [MessageHeader(Name = "Identifier", Namespace = Constants.WsEventing.Namespace)]
        public virtual Identifier Identifier
        {
            get { return this.identifier; }
            set { this.identifier = value; }
        }

        /// <summary>
        /// Gets or sets the <see cref="UnsubscribeRequestMessageBody"/> content contained in the
        /// received SOAP message.
        /// </summary>
        /// <value>The <see cref="UnsubscribeRequestMessageBody"/> content contained in the received SOAP message.</value>
        [MessageBodyMember(Name = "Unsubscribe", Namespace = Constants.WsEventing.Namespace, Order = 0)]
        public virtual UnsubscribeRequestMessageBody Body
        {
            get { return this.body; }
            set
            {
                this.body = value;
            }
        }

        #endregion
    }
}