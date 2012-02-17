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

namespace CommonContracts.WsEventing
{
    /// <summary>
    /// Provides compile time constants for the WS-Eventing contracts.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Provides compile time constants for the WS-Eventing 1.2 service
        /// </summary>
        public static class WsEventing
        {
            /// <summary>
            /// Provides the WSDL portType namespace value for the <see cref="IEventSource"/>, <see cref="ISubscriptionManager"/> APIs.
            /// </summary>
            public const String Namespace = "http://schemas.xmlsoap.org/ws/2004/08/eventing";

            /// <summary>
            /// Provides compile time constants for the WS-Eventing 1.2 dialects.
            /// </summary>
            public static class Dialects
            {
                /// <summary>
                /// The XPath dialect.
                /// </summary>
                public const String XPath = "http://www.w3.org/TR/1999/REC-xpath-19991116";
            }

            /// <summary>
            /// Provides compile time constants for the WS-Eventing 1.2 delivery modes.
            /// </summary>
            public static class DeliverModes
            {
                /// <summary>
                /// The push delivery mode.
                /// </summary>
                public const String Push = Namespace + "/DeliveryModes/Push";

                /// <summary>
                /// The wrap delivery mode.
                /// </summary>
                public const String Wrapped = Namespace + "/DeliveryModes/Wrap";
            }

            /// <summary>
            /// Provides compile time constants for the custom extensions to the WS-Eventing 1.2 service actions.
            /// </summary>
            public static class Extension
            {
                /// <summary>
                /// Provides the namespace value for the <see cref="IEventSource"/> API extensions.
                /// </summary>
                public const String ExtensionNamespace = "urn:commoncontracts/ws-eventing/extensions";
            }

            /// <summary>
            /// Provides compile time constants for the WS-Eventing 1.2 service actions.
            /// </summary>
            public static class Actions
            {
                /// <summary>
                /// Holds the SOAP action for the <see cref="IEventSource.Subscribe"/> operation.
                /// </summary>
                public const String Subscribe = Namespace + "/Subscribe";

                /// <summary>
                /// Holds the SOAP reply action for the <see cref="IEventSource.Subscribe"/> operation.
                /// </summary>
                public const String SubscribeReply = Namespace + "/SubscribeResponse";

                /// <summary>
                /// Holds the SOAP action for the <see cref="IEventSourceCallback.SubscriptionEnd"/> operation.
                /// </summary>
                public const String SubscriptionEnd = Namespace + "/SubscriptionEnd";

                /// <summary>
                /// Holds the SOAP action for the <see cref="ISubscriptionManager.GetStatus"/> operation.
                /// </summary>
                public const String GetStatus = Namespace + "/GetStatus";

                /// <summary>
                /// Holds the SOAP reply action for the <see cref="ISubscriptionManager.GetStatus"/> operation.
                /// </summary>
                public const String GetStatusReply = Namespace + "/GetStatusResponse";

                /// <summary>
                /// Holds the SOAP action for the <see cref="ISubscriptionManager.Renew"/> operation.
                /// </summary>
                public const String Renew = Namespace + "/Renew";

                /// <summary>
                /// Holds the SOAP reply action for the <see cref="ISubscriptionManager.Renew"/> operation.
                /// </summary>
                public const String RenewReply = Namespace + "/RenewResponse";

                /// <summary>
                /// Holds the SOAP action for the <see cref="ISubscriptionManager.Unsubscribe"/> operation.
                /// </summary>
                public const String Unsubscribe = Namespace + "/Unsubscribe";

                /// <summary>
                /// Holds the SOAP reply action for the <see cref="ISubscriptionManager.Unsubscribe"/> operation.
                /// </summary>
                public const String UnsubscribeReply = Namespace + "/UnsubscribeResponse";
            }
        }

        /// <summary>
        /// Provides compile time constants for the WS-Addressing spec.
        /// </summary>
        public static class WsAddressing
        {
            /// <summary>
            /// Provides the namespace value for the WS-Addressing spec.
            /// </summary>
            public const String Namespace = "http://schemas.xmlsoap.org/ws/2004/08/addressing";

            /// <summary>
            /// A well known EndPointReference whose address with this value MUST be discarded (i.e. not sent). This URI is typically used in EPRs that designate the concept of a "No Endpoint" Endpoint.
            /// </summary>
            public const String NoAddress = "http://www.w3.org/2005/08/addressing/none";
        }
    }
}
