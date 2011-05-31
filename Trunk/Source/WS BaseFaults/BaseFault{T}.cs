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
using System.Runtime.Serialization;
using System.ServiceModel;

namespace CommonContracts.WsBaseFaults
{
    /// <summary>
    /// Provides the ability for <see cref="DataContractSerializer"/> based faults to enlist in the ability
    /// to specify the <see cref="FaultCause"/> value at runtime with a root fault instance.
    /// </summary>
    /// <remarks>
    /// <para>
    /// One of the powerful features of the WSRF-BF specification is the ability to perform fault chaining
    /// back to an original root fault. Due to the parculiarities of the way the <see cref="DataContractSerializer"/>
    /// actually works a generic type definition indicating the concrete type (or type hierarchy) of the fault cause
    /// is required to be declared/used at runtime. Simply declare a new subclass of this type indicating
    /// the type for <typeparamref name="T"/> of the actual data contract type that is being chained and a 
    /// <see cref="DataContractAttribute"/> with the appropriate name and namespace value for your custom fault type. 
    /// If the generic type definition is to be used with a type hierarchy (represented as an xsi:type attribute in 
    /// the xml), then the <typeparam name="T"/> type <b>MUST</b> use the <see cref="KnownTypeAttribute"/> to indicate
    /// the possible concrete types that will used at runtime on the server side or a serialier error will occur and 
    /// no content will be sent to the caller (in .Net clients this will appear as a <see cref="CommunicationException"/> 
    /// with the message value, "The underlying connection was closed: The connection was closed unexpectedly".
    /// </para>
    /// <para>Inheritors should only use <see cref="DataMemberAttribute.Order"/> values greater than 1.</para>
    /// </remarks>
    [DataContract(IsReference = false, Name = "BaseFault", Namespace = Constants.WsBaseFaults.Namespace)]
    public abstract class BaseFault<T> : BaseFault
    {
        #region Fields

        private T faultCause;

        #endregion

        #region Constructors
        
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseFault{T}"/> class.
        /// </summary>
        protected BaseFault() {}

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseFault{T}"/> class with the supplied <paramref name="faultCause"/>.
        /// </summary>
        /// <param name="faultCause">The root fault cause.</param>
        protected BaseFault(T faultCause)
        {
            this.faultCause = faultCause;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseFault{T}"/> class with the supplied <paramref name="utc"/> value.
        /// </summary>
        /// <remarks>If the supplied date is not in UTC, the value will be coerced.</remarks>
        /// <param name="utc">The <see cref="BaseFault.Timestamp"/> value.</param>
        protected BaseFault(DateTime utc) : base(utc) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseFault{T}"/> class with the supplied <paramref name="utc"/> and
        /// <paramref name="faultCause"/> values.
        /// </summary>
        /// <param name="utc">The <see cref="BaseFault.Timestamp"/> value.</param>
        /// <param name="faultCause">The root fault cause.</param>
        protected BaseFault(DateTime utc, T faultCause) : this(utc)
        {
            this.faultCause = faultCause;
        }

        #endregion

        #region Properties
        
        /// <summary>
        /// Gets or sets the cause of this fault.
        /// </summary>
        /// <remarks>
        /// If being used on the client side for supporting nested faults, you should subclass this type and use a generic type
        /// that indicates the set of possible FaultCause types via the <see cref="KnownTypeAttribute"/>.
        /// </remarks>
        /// <value>The cause of this fault.</value>
        /// <exception cref="ArgumentException">The supplied value is the same reference as the current <see cref="BaseFault"/>.</exception>
        [DataMember(EmitDefaultValue = false, IsRequired = false, Name = "FaultCause", Order = 1)]
        public virtual T FaultCause
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
