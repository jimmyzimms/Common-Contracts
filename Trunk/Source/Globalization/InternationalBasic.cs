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
using System.Diagnostics;
using System.Globalization;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace CommonContracts.Globalization
{
    /// <summary>
    /// A data contract used to represent the WS-I18N specification (see http://www.w3.org/TR/ws-i18n/ for more information).
    /// </summary>
    /// <remarks>
    /// <para>When used in example documentation in or XSD, this type should try to be QName referenced as "i18n:International".</para>
    /// <para>This type accelerates contract development of <seealso cref="DataContractSerializer"/> based globalization features that
    /// implement the minimum level of WS-Internationalization specification. Simply use this type in your <see cref="MessageContractAttribute"/>
    /// WCF messages (usually as a supplied <see cref="MessageHeaderAttribute">SOAP Message Header</see>).</para>
    /// </remarks>
    [DebuggerDisplay("Locale = '{Locale}', Timezone = '{Timezone}', Preferences.Count = '{Preferences.Content.Count}'")]
    [DataContract(Name = "International", Namespace = "http://www.w3.org/2005/09/ws-i18n")]
    public class International
    {
        #region Fields
        
        private String locale;
        private String timeZone;
        private Preferences preferences = new Preferences();

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="International"/> class.
        /// </summary>
        public International()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="International"/> class.
        /// </summary>
        /// <param name="cultureInfo">The <see cref="CultureInfo"/> that should be used to represent the <see cref="Locale"/> value.</param>
        public International(CultureInfo cultureInfo)
        {
            cultureInfo = cultureInfo ?? CultureInfo.CurrentUICulture;

            this.locale = cultureInfo.IetfLanguageTag;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="International"/> class.
        /// </summary>
        /// <param name="timeZone">The value that should be used to represent the <see cref="Timezone"/> value.</param>
        public International(String timeZone)
        {
            this.timeZone = timeZone;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the value that defines the locale.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Its value MUST be either a valid LDML locale identifier (See http://unicode.org/reports/tr35/tr35-8.html)
        /// or one of the values "$neutral" or "$default".
        /// </para>
        /// <para>The value $default indicates that the service or provider should use its own runtime locale as the
        /// base setting. The value $neutral represents the base or root locale on the receiving system. Different
        /// systems and environments identify this setting in different ways.
        /// </para>
        /// </remarks>
        /// <value>The value that defines the locale. The value will default to the string '$default'.</value>
        [DataMember(Name = "Locale", Order = 1)]
        public virtual String Locale
        {
            get { return this.locale ?? "$default"; }
            set
            {
                value = (value ?? "$default").Trim();
                this.locale = value;
            }
        }

        /// <summary>
        /// Gets or sets the optional value that defines the timezone.
        /// </summary>
        /// <remarks>
        /// The i18n:tz element information item represents the local time zone of the requester or provider. The value 
        /// of the element MUST be either RFC 822-formatted zone offset (see http://www.ietf.org/rfc/rfc822.txt) or an
        /// Olson ID (see http://www.twinsun.com/tz/tz-link.htm) from the 'olsonid' database. Note that RFC 822 zone offsets
        /// are not complete time zone identifiers and Olson identifiers are preferred. It is implementation-defined
        /// whether an RFC 822-formatted zone offset or an Oson ID is given, and how a choice between these two kinds
        /// of values is indicated.
        /// </remarks>
        /// <value></value>
        [DataMember(Name = "TZ", Order = 2, IsRequired = false)]
        public virtual String Timezone
        {
            get { return this.timeZone; }
            set
            {
                value = (value ?? String.Empty).Trim();
                this.timeZone = value;
            }
        }

        /// <summary>
        /// Gets or sets the information, if any, that defines the locale preferences.
        /// </summary>
        /// <remarks>
        /// The i18n:preferences element information item represents a way to construct specific locale preferences and overrides. 
        /// Support for the i18n:preferences element is OPTIONAL and specific behavior in relation to any particular preference
        /// is implementation dependent. Implementations of this specification are not required to recognize, support, or acknowledge
        /// the i18n:preferences element information item or any of its sub-elements. Services MUST NOT require a i18n:preferences
        /// element information item be sent in order to operate correctly. When serializing, this element will always be present
        /// in the output stream though it may contain no elements.
        /// </remarks>
        /// <value>The information, if any, that defines the locale preferences. This value will not be null but may be empty.</value>
        [DataMember(Name = "Preferences", Order = 3, IsRequired = false)]
        public virtual Preferences Preferences
        {
            get { return this.preferences; }
            set
            {
                value = value ?? new Preferences();

                this.preferences = value;
            }
        }

        #endregion
    }
}
