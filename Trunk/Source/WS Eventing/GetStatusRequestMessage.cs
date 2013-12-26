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
    /// The request message for the <see cref="ISubscriptionManager.GetStatus"/> operation.
    /// </summary>
    [MessageContract(IsWrapped = false)]
    public class GetStatusRequestMessage
    {
        #region Fields

        private Identifier identifier;
        private GetStatusRequestMessageBody body;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GetStatusRequestMessage"/> with the default values. This constructor should only be used for deserialization.
        /// </summary>
        public GetStatusRequestMessage() : this(new GetStatusRequestMessageBody())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GetStatusRequestMessage"/> with the default values. This constructor should only be used for deserialization.
        /// </summary>
        /// <param name="identifier">The <see cref="Identifier"/> containing the subscription information.</param>
        public GetStatusRequestMessage(Identifier identifier) : this()
        {
            Contract.Requires<ArgumentNullException>(identifier != null, "identifier");

            this.identifier = identifier;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GetStatusRequestMessage"/> with the supplied <paramref name="id"/> and <paramref name="body"/> values.
        /// </summary>
        /// <remarks>
        /// Generally used with custom <see cref="GetStatusRequestMessageBody"/> content types.
        /// </remarks>
        /// <param name="body">The body of the subscription status request.</param>
        public GetStatusRequestMessage(GetStatusRequestMessageBody body)
        {
            Contract.Requires<ArgumentNullException>(body != null, "body");

            this.body = body;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GetStatusRequestMessage"/> with the supplied <paramref name="id"/> and <paramref name="body"/> values.
        /// </summary>
        /// <remarks>
        /// Generally used with custom <see cref="GetStatusRequestMessageBody"/> content types.
        /// </remarks>
        /// <param name="identifier">The <see cref="Identifier"/> containing the subscription information.</param>
        /// <param name="body">The body of the subscription status request.</param>
        public GetStatusRequestMessage(Identifier identifier, GetStatusRequestMessageBody body) : this(body)
        {
            Contract.Requires<ArgumentNullException>(identifier != null, "identifier");

            this.identifier = identifier;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the <see cref="Identifier"/> value for the subscription status request.
        /// </summary>
        /// <value>The <see cref="Identifier"/> value for the subscription status request.</value>
        [MessageHeader(Name = "Identifier", Namespace = Constants.WsEventing.Namespace)]
        public virtual Identifier Identifier
        {
            get { return this.identifier; }
            set { this.identifier = value; }
        }

        /// <summary>
        /// Gets or sets the <see cref="GetStatusRequestMessageBody"/> content for the subscription request.
        /// </summary>
        /// <value>The <see cref="GetStatusRequestMessageBody"/> content for the subscription request.</value>
        [MessageBodyMember(Name = "GetStatus", Namespace = Constants.WsEventing.Namespace, Order = 0)]
        public virtual GetStatusRequestMessageBody Body
        {
            get
            {
                Contract.Ensures(Contract.Result<GetStatusRequestMessageBody>() != null);

                return this.body;
            }
            set
            {
                Contract.Requires<ArgumentNullException>(value != null, "Body");

                this.body = value;
            }
        }

        #endregion
    }
}
