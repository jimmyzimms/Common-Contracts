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
using System.Diagnostics.Contracts;
using System.Runtime.Serialization;

namespace CommonContracts.WsBaseFaults
{
    /// <summary>
    /// Provides the minimum base common members that all <see cref="DataContractSerializer"/> based faults should provide.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This type accelerates contract development of <seealso cref="DataContractSerializer"/> based faults that implement
    /// the minimum level of WS-BaseFaults 1.2 specification. Simply subclass this type, supplying a new <see cref="DataContractAttribute"/>
    /// with the appropriate name and namespace value for your custom fault type.
    /// </para>
    /// Inheritors should only use <see cref="DataMemberAttribute.Order"/> values greater than 4.</remarks>
    [DataContract(IsReference = false, Name = "BaseFault", Namespace = Constants.WsBaseFaults.Namespace)]
    public abstract class BaseFault : IExtensibleDataObject
    {
        #region Fields
        
        private DateTime timestamp;
        private BaseFault faultCause;

        #endregion

        #region Constructors
        
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseFault"/> class with the current <see cref="DateTime.UtcNow"/> value.
        /// </summary>
        protected BaseFault() : this(DateTime.UtcNow) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseFault"/> class with the current <see cref="DateTime.UtcNow"/> value and the supplied <paramref name="faultCause"/>.
        /// </summary>
        /// <param name="faultCause">The optional cause of this fault.</param>
        protected BaseFault(BaseFault faultCause) : this(faultCause, DateTime.UtcNow)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseFault"/> class with the supplied <paramref name="utc"/> value.
        /// </summary>
        /// <remarks>If the supplied date is not in UTC, the value will be coerced.</remarks>
        /// <param name="utc">The <see cref="Timestamp"/> value.</param>
        protected BaseFault(DateTime utc)
        {
            if (utc.Kind != DateTimeKind.Utc) utc = utc.ToUniversalTime();
            this.timestamp = utc;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseFault"/> class with the supplied <paramref name="utc"/> and <paramref name="faultCause"/> values.
        /// </summary>
        /// <remarks>If the supplied date is not in UTC, the value will be coerced.</remarks>
        /// <param name="utc">The <see cref="Timestamp"/> value.</param>
        /// <param name="faultCause">The optional cause of this fault.</param>
        protected BaseFault(BaseFault faultCause, DateTime utc) : this(utc)
        {
            this.faultCause = faultCause;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the structure that contains extra data.
        /// </summary>
        /// <value>An <see cref="ExtensionDataObject"/> that contains data that is not recognized as belonging to the data contract.</value>
        public ExtensionDataObject ExtensionData { get; set; }

        /// <summary>
        /// Gets the time, in UTC, of the fault instance.
        /// </summary>
        /// <value>The time, in UTC, of the fault instance.</value>
        [DataMember(EmitDefaultValue = true, IsRequired = true, Name = "Timestamp", Order = 0)]
        public virtual DateTime Timestamp
        {
            get
            {
                Contract.Ensures(Contract.Result<DateTime>().Kind == DateTimeKind.Utc);
                return this.timestamp;
            }
            protected set
            {
                if (value.Kind != DateTimeKind.Utc) value = value.ToUniversalTime();
                this.timestamp = value;
            }
        }

        /// <summary>
        /// Gets or sets the cause of this fault.
        /// </summary>
        /// <remarks>
        /// If being used on the client side for supporting nested faults, you should subclass this type to represent
        /// the top level fault and indicate the set of possible FaultCause types via the <see cref="KnownTypeAttribute"/>.
        /// </remarks>
        /// <value>The cause of this fault.</value>
        /// <exception cref="ArgumentException">The supplied value is the same reference as the current <see cref="BaseFault"/>.</exception>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "FaultCause", Order = 4)]
        public virtual BaseFault FaultCause
        {
            get { return this.faultCause; }
            set
            {
                if (ReferenceEquals(this, value)) throw new ArgumentException("You cannot nest a BaseFault with the same reference as itself as this would cause a cirular reference in the FaultCause chain.", "value");
                this.faultCause = value;
            }
        }

        #endregion
    }
}