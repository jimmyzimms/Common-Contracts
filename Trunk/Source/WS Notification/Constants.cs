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

namespace CommonContracts.WsNotification
{
    /// <summary>
    /// Provides compile time constants for the WS-Notification contracts.
    /// </summary>
    /// <remarks>This covers the WS-BaseNotification, WS-BrokeredNotification, and WS-Topics support.</remarks>
    public static class Constants
    {
        /// <summary>
        /// Holds the value of the root namespace used for all WS-Notification services. Contains the value 'http://docs.oasis-open.org/wsn/'.
        /// </summary>
        public const String RootNamespace = "http://docs.oasis-open.org/wsn/";

        /// <summary>
        /// Provides compile time constants for the WS-BaseNotification 1.3 specification.
        /// </summary>
        public static class WsBaseNotification
        {
            /// <summary>
            /// Holds the value of the WS-BaseNotification namespace. Contains the value http://docs.oasis-open.org/wsn/b-2'.
            /// </summary>
            public const String Namespace = RootNamespace + "b-2";

            /// <summary>
            /// Holds the value of the WS-BaseNotification WSDL namespace. Contains the value http://docs.oasis-open.org/wsn/bw-2'.
            /// </summary>
            public const String Wsdl = RootNamespace + "bw-2";
        }

        /// <summary>
        /// Provides compile time constants for the WS-BrokeredNotification 1.3 specification.
        /// </summary>
        public static class WsBrokeredNotification
        {
            /// <summary>
            /// Holds the value of the WS-BrokeredNotification namespace. Contains the value http://docs.oasis-open.org/wsn/br-2'.
            /// </summary>
            public const String Namespace = RootNamespace + "br-2";

            /// <summary>
            /// Holds the value of the WS-BrokeredNotification WSDL namespace. Contains the value http://docs.oasis-open.org/wsn/brw-2'.
            /// </summary>
            public const String Wsdl = RootNamespace + "brw-2";
        }

        /// <summary>
        /// Provides compile time constants for the WS-Topics 1.3 specification.
        /// </summary>
        public static class WsTopics
        {
            /// <summary>
            /// Holds the value of the WS-Topics namespace. Contains the value http://docs.oasis-open.org/wsn/t-1'.
            /// </summary>
            public const String Namespace = RootNamespace + "t-1";
        }
    }
}
