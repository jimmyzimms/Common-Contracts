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
using System.Messaging;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Xml;
using CommonContracts.WsEventing;
using Message = System.ServiceModel.Channels.Message;

namespace Consumer
{
    public class Program
    {
        public const String QueueName = @".\private$\consumer-eventsink";
        public const String EventSinkUri = @"msmq.formatname:DIRECT=OS:" + QueueName;
        public const String EventAction = @"urn:eventsink/notify";
        private static MessageQueue Queue;

        public static void Main(String[] args)
        {
            if (!UacHelper.IsProcessElevated)
            {
                Console.Beep();
                Console.WriteLine("This application requires full administrative privledges to run");
                Console.ReadKey(true);
                return;
            }

            Console.WriteLine("Press any key to subscribe...");
            Console.ReadKey(true);
            
            Subscribe();
            StartListener();

            Console.WriteLine("Press any key to exit...");

            Console.ReadKey(true);

            Unsubscribe();
            EndListener();
        }

        public static void Subscribe()
        {
            var reader = new StringReader("<wse:Subscribe xmlns:wse='http://schemas.xmlsoap.org/ws/2004/08/eventing'><wse:Delivery><wse:NotifyTo><Address xmlns='http://schemas.xmlsoap.org/ws/2004/08/addressing'>" + EventSinkUri + "</Address></wse:NotifyTo></wse:Delivery></wse:Subscribe>");
            var content = XmlReader.Create(reader);
            var request = Message.CreateMessage(MessageVersion.Soap11, Constants.WsEventing.Actions.Subscribe, content);
            var channel = ChannelFactory<IEventSource>.CreateChannel(new BasicHttpBinding(), new EndpointAddress("http://localhost:8080/eventsource"));
            var response = channel.Subscribe(request);
            if (response.IsFault) throw new Exception(response.GetReaderAtBodyContents().ReadOuterXml());

            Console.WriteLine("Event subscribed!");
        }

        public static void Unsubscribe()
        {
            //Since we're not actually running a Subscription Mananger in this example
            //  the code is commented out but is useful to show an example of a non-identified
            //  unsubscribe request.

            //var reader = new StringReader("<UnsubscribeRequestMessage xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" />"");
            //var content = XmlReader.Create(reader);
            //var request = Message.CreateMessage(MessageVersion.Soap11, Constants.WsEventing.Actions.Unsubscribe, content);
            //var channel = ChannelFactory<ISubscriptionManager>.CreateChannel(new BasicHttpBinding(), new EndpointAddress("http://localhost:8080/eventsource"));
            //var response = channel.Unsubscribe(request);
            //if (response.IsFault) throw new Exception(response.GetReaderAtBodyContents().ReadOuterXml());

            Console.WriteLine("Event unsubscribed!");
        }

        public static void StartListener()
        {
            Console.WriteLine("Starting event sink...");

            if (!MessageQueue.Exists(QueueName))
            {
                MessageQueue.Create(QueueName, false);
            }
            Queue = new MessageQueue(QueueName);
            Queue.Formatter = new XmlMessageFormatter(new[] {typeof (Guid)});
            Queue.ReceiveCompleted += (s, a) =>
                                          {
                                              try
                                              {
                                                  Console.WriteLine("Event received: " + a.Message.Body);
                                                  Queue.BeginReceive();
                                              }
                                              catch (MessageQueueException exception)
                                              {
                                              }

                                          };
            Queue.BeginReceive();
        }

        public static void EndListener()
        {
            Console.WriteLine("Ending event sink...");
            Queue.Dispose();
            if (MessageQueue.Exists(QueueName))
            {
                MessageQueue.Delete(QueueName);
            }
        }
    }
}
