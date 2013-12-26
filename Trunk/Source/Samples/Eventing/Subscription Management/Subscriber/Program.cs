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
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Xml;
using CommonContracts.WsEventing;

namespace Subscriber
{
    public class Program
    {
        public const String EventSinkUri = @"http://localhost:8081/eventsink";

        public static void Main(String[] args)
        {
            Console.WriteLine("Press any key to subscribe...");
            Console.ReadKey(true);

            var manager = Subscribe();
            
            Console.WriteLine("Press any key to Check Status...");
            Console.ReadKey(true);

            CheckStatus(manager);

            Console.WriteLine("Press any key to Renew...");
            Console.ReadKey(true);

            Renew(manager);

            Console.WriteLine("Press any key to Unsubscribe...");
            Console.ReadKey(true);

            Unsubscribe(manager);

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey(true);
        }

        public static SubscriptionManager Subscribe()
        {
            var request = new SubscribeRequestMessage(EventSinkUri);
            var channel = ChannelFactory<IEventSource>.CreateChannel(new BasicHttpBinding(), new EndpointAddress("http://localhost:8080/eventsource"));
            var response = channel.Subscribe(request);

            var expirationDate = response.Body.Expires.Value;
            var id = response.Identifier;
            var uri = response.Body.SubscriptionManager.EndpointAddress.ToEndpointAddress().ToString();

            Console.WriteLine("Event {0} subscribed till {1}. Manager @ {2}", id, expirationDate, uri);

            return response.Body.SubscriptionManager;
        }

        public static void CheckStatus(SubscriptionManager manager)
        {
            var epa = manager.EndpointAddress.ToEndpointAddress();
            var request = new GetStatusRequestMessage(manager.Identifier);
            var channel = ChannelFactory<ISubscriptionManager>.CreateChannel(new BasicHttpBinding(), epa);
            var response = channel.GetStatus(request);

            var expiration = response.Body.Expires;
            if (expiration == null)
            {
                Console.WriteLine("Subscription {0} is already expired", manager.Identifier.Value);
            }
            else
            {
                Console.WriteLine("Status for {0} expires at {1}", manager.Identifier.Value, expiration.Value);
            }
        }

        public static void Renew(SubscriptionManager manager)
        {
            var request = new RenewRequestMessage(manager.Identifier); // Request the default expiration extension from the service
            var channel = ChannelFactory<ISubscriptionManager>.CreateChannel(new BasicHttpBinding(), manager.EndpointAddress.ToEndpointAddress());
            var response = channel.Renew(request);

            Console.WriteLine("Status for {0} now expires at {1}", manager.Identifier.Value, response.Body.Expires.Value);
        }

        public static void Unsubscribe(SubscriptionManager manager)
        {
            var request = new UnsubscribeRequestMessage(manager.Identifier); // Perform the default unsubscribe request from the service
            var channel = ChannelFactory<ISubscriptionManager>.CreateChannel(new BasicHttpBinding(), manager.EndpointAddress.ToEndpointAddress());
            channel.Unsubscribe(request);

            Console.WriteLine("Event unsubscribed!");
        }
    }
}
