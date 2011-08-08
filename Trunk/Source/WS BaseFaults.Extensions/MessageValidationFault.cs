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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace CommonContracts.WsBaseFaults.Extensions
{
    /// <summary>
    /// The fault detail message that indicates an validation issue for a request.
    /// </summary>
    [DataContract(Name = "MessageValidation", Namespace = Constants.Namespace)]
    public class MessageValidationFault : BaseFault
    {
        #region Fields

        [DataMember(Name = "Errors")]
        private readonly ValidationErrorCollection errors;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageValidationFault"/>.
        /// </summary>
        /// <remarks>This constructor should only be used by subclasses and on WCF clients for deserialization purposes.</remarks>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "This is checked via Code Contracts. CA Engine does not yet understand how to deal with contracts.")]
        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", Justification = "This is the parameter name of the code and globalization is not needed.")]
        protected MessageValidationFault()
        {
            this.errors = new ValidationErrorCollection();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageValidationFault"/>.
        /// </summary>
        /// <param name="errors">The sequence of validation error messages.</param>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "This is checked via Code Contracts. CA Engine does not yet understand how to deal with contracts.")]
        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", Justification = "This is the parameter name of the code and globalization is not needed.")]
        public MessageValidationFault(IEnumerable<String> errors)
        {
            Contract.Requires<ArgumentNullException>(errors != null, "errors");
            Contract.Requires<ArgumentException>(Contract.ForAll(errors, item => !String.IsNullOrWhiteSpace(item)), "errors");

            this.errors = errors is ValidationErrorCollection ? (ValidationErrorCollection)errors : new ValidationErrorCollection(errors);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageValidationFault"/>.
        /// </summary>
        /// <param name="error">The sequence of validation error messages.</param>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "This is checked via Code Contracts. CA Engine does not yet understand how to deal with contracts.")]
        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", Justification = "This is the parameter name of the code and globalization is not needed.")]
        public MessageValidationFault(String error)
            : this(new[] { error })
        {
            Contract.Requires<ArgumentException>(!String.IsNullOrWhiteSpace(error), "error");
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the collection of validation error messages.
        /// </summary>
        /// <value>The collection of validation error messages.</value>
        public ValidationErrorCollection Errors
        {
            get { return this.errors; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Factory used to create the expected <see cref="FaultReason"/> to be used when returning this fault type.
        /// </summary>
        /// <returns>The default <see cref="FaultReason"/> to be used with the <see cref="MessageValidationFault"/> type.</returns>
        public static FaultReason CreateStandardReason()
        {
            var faultReason = new FaultReason("Message validation failed");
            return faultReason;
        }

        /// <summary>
        /// Factory used to create the expected <see cref="FaultCode"/> to be used when returning this fault type.
        /// </summary>
        /// <returns>The default <see cref="FaultCode"/> to be used with the <see cref="MessageValidationFault"/> type.</returns>
        public static FaultCode CreateStandardCode()
        {
            var faultCode = FaultCode.CreateSenderFaultCode(Constants.Faults.MessageValidationFault.FaultCode, Constants.Namespace);
            return faultCode;
        }

        /// <summary>
        /// Factory used to create the expected <see cref="MessageFault"/> to be used when returning this fault type.
        /// </summary>
        /// <param name="fault">The <see cref="MessageValidationFault"/> to create a <see cref="MessageFault"/> from.</param>
        /// <returns>The appropriate <see cref="MessageFault"/> for the supplied <see cref="MessageValidationFault"/>.</returns>
        public static MessageFault CreateStandardFault(MessageValidationFault fault)
        {
            var faultCode = CreateStandardCode();
            var faultReason = CreateStandardReason();

            return MessageFault.CreateFault(faultCode, faultReason, fault);
        }

        #endregion
    }
}
