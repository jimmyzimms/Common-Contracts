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
using CommonContracts.WsEventing;

namespace Source
{
    public partial class Service : ISubscriptionManager
    {
        /// <summary>
        /// Operation to get the status of a subscription. The subscriber sends a request to the subscription manager
        /// and if the subscription is valid and has not expired the status will be returned.
        /// </summary>
        /// <param name="request">The <see cref="GetStatusRequestMessage">request message</see> containing the subscription status request details.</param>
        /// <returns>The <see cref="GetStatusResponseMessage">GetStatusResponseMessage</see> containing the subscription status details.</returns>
        public GetStatusResponseMessage GetStatus(GetStatusRequestMessage request)
        {
            Guid id;
            request.Identifier.Value.TryGetGuid(out id);
            var exists = Subscribers.ContainsKey(id);
            if (exists)
            {
                // Normally we'd return the saved expires dated but as in our example we're not worrying about this
                //  so we'll just return max date if it exists.
                return new GetStatusResponseMessage(new GetStatusResponseMessageBody(new Expires(DateTime.MaxValue)));
            }
            return new GetStatusResponseMessage(new GetStatusResponseMessageBody());
        }

        /// <summary>
        /// Operation to update the expiration for a subscription. The subscriber sends a request to the subscription manager
        /// and if the subscription manager accepts a request to renew a subscription the new expiration details will be returned.
        /// </summary>
        /// <param name="request">The <see cref="RenewRequestMessage">request message</see> containing the renewal request details.</param>
        /// <returns>The <see cref="RenewResponseMessage">RenewResponseMessage</see> containing the new subscription expiration details.</returns>
        public RenewResponseMessage Renew(RenewRequestMessage request)
        {
            Guid id;
            request.Identifier.Value.TryGetGuid(out id);
            var exists = Subscribers.ContainsKey(id);
            if (exists)
            {
                // Normally we'd reset the expires but as in our example we're not worrying about this
                //  we'll just return max date.
                return new RenewResponseMessage(new RenewResponseMessageBody(new Expires(DateTime.MaxValue)));
            }
            return new RenewResponseMessage();
        }

        /// <summary>
        /// Operation to unsubscribe an event sink for an existing subscription.
        /// </summary>
        /// <param name="request">The <see cref="UnsubscribeRequestMessage">request message</see> containing the unsubscription request details.</param>
        public void Unsubscribe(UnsubscribeRequestMessage request)
        {
            Guid id;
            request.Identifier.Value.TryGetGuid(out id);
            var exists = Subscribers.ContainsKey(id);
            if (exists)
            {
                Subscribers.Remove(id);
            }
        }
    }
}
