using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Xml.Linq;

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
    [DataContract(Name = "International", Namespace = "http://www.w3.org/2005/09/ws-i18n")]
    public class International
    {
        #region Fields
        
        private String locale;
        private String timeZone;
        private XElement[] preferences;

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
        public String Locale
        {
            get { return this.locale; }
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
        /// whether an RFC 822-formatted zone offset or an OLson ID is given, and how a choice between these two kinds
        /// of values is indicated.
        /// </remarks>
        [DataMember(Name = "TZ", Order = 2)]
        public String Timezone
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
        /// element information item be sent in order to operate correctly.
        /// </remarks>
        /// <value>The information, if any, that defines the locale preferences. This value will not be null but may be empty.</value>
        [DataMember(Name = "Preferences", Order = 3)]
        public XElement[] Preferences
        {
            get { return this.preferences; }
            set
            {
                value = value ?? new XElement[0];

                this.preferences = value;
            }
        }

        #endregion
    }
}
