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

namespace CommonContracts.WsEventing
{
    /// <summary>
    /// The request message for the <see cref="IEventSource.Subscribe"/> operation.
    /// </summary>
    [MessageContract(IsWrapped = false)]
    public class SubscribeRequestMessage
    {
        #region Fields

        private SubscribeRequestMessageBody body;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscribeRequestMessage"/> class.
        /// </summary>
        /// <remarks>
        /// Generally used in deserialization routines.
        /// </remarks>
        public SubscribeRequestMessage()
        {
#pragma warning disable 618
            this.body = new SubscribeRequestMessageBody();
#pragma warning restore 618
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscribeRequestMessage"/> class with
        /// the <see cref="EndpointAddress"/> containing the location to receive event notifications
        /// at for the subscriber.
        /// </summary>
        /// <remarks>
        /// This is a convienence overload allowing the caller to quickly craft a request to subscribe
        /// using the basic most common use case. The default <see cref="Constants.WsEventing.DeliverModes">Delivery Mode</see>
        /// of Push will be created.
        /// </remarks>
        public SubscribeRequestMessage(String notifyTo, Uri mode = null) : this(new EndpointAddress(notifyTo), mode)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscribeRequestMessage"/> class with
        /// the <see cref="EndpointAddress"/> containing the location to receive event notifications
        /// at for the subscriber.
        /// </summary>
        /// <remarks>
        /// This is a convienence overload allowing the caller to quickly craft a request to subscribe
        /// using the basic most common use case. The default <see cref="Constants.WsEventing.DeliverModes">Delivery Mode</see>
        /// of Push will be created.
        /// </remarks>
        public SubscribeRequestMessage(EndpointAddress notifyTo, Uri mode = null) : this()
        {
            if (mode == null) mode = new Uri(Constants.WsEventing.DeliverModes.Push);
            this.body.Delivery = new Delivery(mode, notifyTo);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscribeRequestMessage"/> class with the supplied <see cref="Delivery"/> value.
        /// </summary>
        /// <remarks>
        /// Generally used by client subscribers.
        /// </remarks>
        public SubscribeRequestMessage(Delivery delivery) : this(delivery, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscribeRequestMessage"/> class with
        /// the supplied <see cref="Delivery"/> value and optional <see cref="EndpointAddress"/>
        /// containing the endpoint to contact when a subscription is ended by the
        /// <see cref="IEventSource">event source</see>.
        /// </summary>
        /// <remarks>
        /// Generally used by client subscribers.
        /// </remarks>
        public SubscribeRequestMessage(Delivery delivery, EndpointAddress endTo)
        {
            Contract.Requires<ArgumentNullException>(delivery != null, "delivery");

            this.body = new SubscribeRequestMessageBody(delivery, endTo);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the optional <see cref="SubscriptionTopic"/> for the subscription.
        /// </summary>
        /// <remarks>
        /// This property represents a proprietary extension to the protocol. It is helpful
        /// in a wide range of eventing scenarios and so is defined by default in this
        /// message contract type. To alter (or remove it) simply subclass and override the
        /// property and redefine the header or omitting the <see cref="MessageHeaderAttribute"/>
        /// for removal.
        /// </remarks>
        /// <value>The optional <see cref="Uri"/> for the subscription topic.</value>
        [MessageHeader(Name = "SubscriptionTopic", Namespace = Constants.WsEventing.Extension.ExtensionNamespace)]
        public virtual SubscriptionTopic SubscriptionTopic { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="SubscribeRequestMessageBody"/> content contained in the
        /// received SOAP message.
        /// </summary>
        /// <remarks>This type does not enforce validation of the structure.</remarks>
        /// <value>The <see cref="SubscribeRequestMessageBody"/> content contained in the received SOAP message.</value>
        [MessageBodyMember(Name = "Subscribe", Namespace = Constants.WsEventing.Namespace, Order = 0)]
        public virtual SubscribeRequestMessageBody Body
        {
            get { return this.body; }
            set { this.body = value; }
        }

        #endregion
    }
}
