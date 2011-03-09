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

namespace CommonContracts.WsBaseFaults
{
    /// <summary>
    /// Provides compile time constants for the WS-BaseFaults specification.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Provides compile time constants for the WS-Addressing spec.
        /// </summary>
        public static class WsAddressing
        {
            /// <summary>
            /// Holds the soap action for the base SOAP Fault message action.
            /// </summary>
            public const String BaseFaultAction = "http://schemas.xmlsoap.org/ws/2004/08/addressing/fault/";

            /// <summary>
            /// Holds the namespace for the WS-Addressing types.
            /// </summary>
            internal const String Namespace = "http://www.w3.org/2005/08/addressing";
        }

        /// <summary>
        /// Provides compile time constants for the XSI types.
        /// </summary>
        internal static class XmlSchemaInfo
        {
            /// <summary>
            /// Holds the namespace for the XSI types.
            /// </summary>
            internal const String Namespace = "http://www.w3.org/2001/XMLSchema-instance";
        }

        /// <summary>
        /// Provides compile time constants for the WS-BaseFaults spec.
        /// </summary>
        public static class WsBaseFaults
        {
            /// <summary>
            /// Holds the namespace for the WS-BaseFaults types.
            /// </summary>
            public const String Namespace = "http://docs.oasis-open.org/wsrf/bf-2";
        }
    }
}