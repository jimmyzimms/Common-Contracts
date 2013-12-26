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
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.MsmqIntegration;
using CommonContracts.WsEventing;

namespace Producer
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class Service : IEventSource
    {
        public static List<Uri> Subscribers = new List<Uri>();

        public SubscribeResponseMessage Subscribe(SubscribeRequestMessage request)
        {
            var deliverTo = request.Body.Delivery.NotifyTo.ToEndpointAddress().Uri;
            Subscribers.Add(deliverTo);

            Console.WriteLine("\tSubscription request for " + deliverTo);

            // We're not going to run an actual Subscription Manager endpoint NOR
            //  going to support identification of specific subscribers (via the
            //  wse:Identifier header element) in this example hence the use of
            //  http://tempuri endpoint address.
            var response = new SubscribeResponseMessage(new SubscribeResponseMessageBody(new SubscriptionManager(new Uri("http://tempuri")), new Expires(DateTime.MaxValue)));
            return response;
        }

        public static void RaiseEvent()
        {
            var id = Guid.NewGuid();

            Console.WriteLine("Raising event with id = " + id + " ...");

            foreach (var subscriber in Subscribers)
            {
                Console.WriteLine("\tSending event to-> " + subscriber);

                var binding = new MsmqIntegrationBinding();
                binding.Durable = false;
                binding.ExactlyOnce = false;
                binding.Security.Mode = MsmqIntegrationSecurityMode.None;
                binding.SerializationFormat = MsmqMessageSerializationFormat.Xml;
                binding.UseMsmqTracing = true;

                var channel = ChannelFactory<IEventCallback>.CreateChannel(binding, new EndpointAddress(subscriber));
                channel.Raise(new MsmqMessage<Guid>(id));

                Console.WriteLine("\tEvent sent!");
            }
        }

        public Message Subscribe(Message request)
        {
            var xml = request.GetReaderAtBodyContents().ReadOuterXml();
            return null;
        }
    }
}
