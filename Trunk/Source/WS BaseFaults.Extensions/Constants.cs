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

namespace CommonContracts.WsBaseFaults.Extensions
{
    /// <summary>
    /// Provides compile time constants for the library.
    /// </summary>
    internal static class Constants
    {
        /// <summary>
        /// Used to hold the namespace that extension faults should use. Contains the string 'urn:common-contracts/basefaults/extensions'.
        /// </summary>
        internal const String Namespace = "urn:common-contracts/basefaults/extensions";

        /// <summary>
        /// Provides compile time constants used for base faults.
        /// </summary>
        internal static class Faults
        {

            /// <summary>
            /// Provides compile time constants for the <see cref="Extensions.MessageValidationFault"/> type.
            /// </summary>
            internal static class MessageValidationFault
            {
                /// <summary>
                /// Holds the name of the fault code to be used with the <see cref="Extensions.MessageValidationFault"/> message.
                /// </summary>
                internal const String FaultCode = "MessageValidation";

                /// <summary>
                /// Holds the soap action that should be used in any soap operation that returns a <see cref="Extensions.MessageValidationFault"/> message.
                /// </summary>
                internal const String Action = Namespace + "/messagevalidationfault";
            }

            /// <summary>
            /// Provides compile time constants for the <see cref="Extensions.SlaViolationFault"/> type.
            /// </summary>
            internal static class SlaViolationFault
            {
                /// <summary>
                /// Holds the name of the fault code to be used with the <see cref="Extensions.SlaViolationFault"/> message.
                /// </summary>
                internal const String FaultCode = "SLAViolation";

                /// <summary>
                /// Holds the soap action that should be used in any soap operation that returns a <see cref="Extensions.SlaViolationFault"/> message.
                /// </summary>
                internal const String Action = Namespace + "/slaviolationfault";
            }

            /// <summary>
            /// Provides compile time constants for the <see cref="Extensions.ServiceUnavailableFault"/> type.
            /// </summary>
            internal static class ServiceUnavailableFault
            {
                /// <summary>
                /// Holds the name of the fault code to be used with the <see cref="Extensions.ServiceUnavailableFault"/> message.
                /// </summary>
                internal const String FaultCode = "ServiceUnavailable";

                /// <summary>
                /// Holds the soap action that should be used in any soap operation that returns a <see cref="Extensions.ServiceUnavailableFault"/> message.
                /// </summary>
                internal const String Action = Namespace + "/serviceunavailablefault";
            }
        }
    }
}
