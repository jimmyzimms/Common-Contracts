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
using System.Diagnostics.Contracts;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace CommonContracts.WsEventing
{
    /// <summary>
    /// Represents the "http://schemas.xmlsoap.org/ws/2004/08/eventing:ExpirationType" XML datatype.
    /// </summary>
    [XmlSchemaProvider("AcquireSchema")]
    [XmlRoot(DataType = Constants.WsEventing.Namespace + ":Expires", ElementName = "Expires", Namespace = Constants.WsEventing.Namespace)]
    public class Expires : IXmlSerializable
    {
        #region Fields

        private String expires;
        
        #endregion

        #region Properties
        
        /// <summary>
        /// Gets or sets the <see cref="DateTime"/> that the subscription is requested to end.
        /// </summary>
        /// <remarks>
        /// This type will always deserialize the recieved value into a <see cref="DateTime"/> 
        /// value based on the current machine clock and coerced into <see cref="XmlDateTimeSerializationMode">UTC</see>
        /// regardless if the original value was a timespan.
        /// </remarks>
        /// <value>The <see cref="DateTime"/> that the subscription is requested to end.</value>
        public virtual DateTime Value
        {
            get
            {
                if (this.expires.StartsWith("P"))
                {
                    DateTime dt = DateTime.Now.Add(XmlConvert.ToTimeSpan(expires)).ToUniversalTime();
                    this.expires = XmlConvert.ToString(dt, XmlDateTimeSerializationMode.Utc);
                    return dt;
                }
                return XmlConvert.ToDateTime(expires, XmlDateTimeSerializationMode.Utc);
            }
            set
            {
                this.expires = XmlConvert.ToString(value, XmlDateTimeSerializationMode.Utc);
            }
        }
        
        #endregion

        #region Constructors
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Expires"/> class with the default values. This constructor should only be used for deserialization.
        /// </summary>
        [Obsolete("This method is required for the XmlSerializer and not to be directly called")]
        public Expires()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Expires"/> class based on the supplied <paramref name="expirationDate"/>.
        /// </summary>
        /// <param name="expirationDate">The <see cref="DateTime"/> when the subscription is requested to expire.</param>
        public Expires(DateTime expirationDate)
        {
            this.expires = XmlConvert.ToString(expirationDate, XmlDateTimeSerializationMode.Utc);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Expires"/> class based on the supplied <paramref name="duration"/>.
        /// </summary>
        /// <param name="duration">A <see cref="TimeSpan"/> is requested to expire.</param>
        public Expires(TimeSpan duration)
        {
            this.expires = XmlConvert.ToString(duration);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Expires"/> class from the supplied <paramref name="reader"/>.
        /// </summary>
        /// <param name="reader">The <see cref="XmlReader"/> to construct an instance of the <see cref="Expires"/> class from.</param>
        public Expires(XmlReader reader)
        {
            Contract.Requires<ArgumentNullException>(reader != null, "reader");
            Contract.Requires<ArgumentException>(reader.ReadState == ReadState.Interactive, "The XmlReader must be in an interactive state to be read from");
 
            ((IXmlSerializable)this).ReadXml(reader);
        }

        #endregion

        #region IXmlSerializable Members

        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            Contract.Requires<ArgumentNullException>(reader != null);
            //Contract.Requires<ArgumentException>(reader.ReadState == ReadState.Interactive, String.Format(null, "The supplied XmlReader must be in the 'Interactive' state. The current state is '{0}'", reader.ReadState));

            if (reader.IsStartElement("Expires", Constants.WsEventing.Namespace))
            {
                expires = reader.ReadElementString();
            }
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            var prefix = writer.LookupPrefix(Constants.WsEventing.Namespace);
            if (String.IsNullOrEmpty(prefix)) prefix = "wse";

            if (expires != null)
            {
                writer.WriteElementString(prefix, "Expires", Constants.WsEventing.Namespace, expires);
            }
        }

        #endregion

        #region Schema

        /// <summary>
        /// Adds an <see cref="XmlSchema"/> instance for this type to the supplied <see cref="XmlSchemaSet"/>.
        /// </summary>
        /// <param name="xs">The <see cref="XmlSchemaSet"/> to add an <see cref="XmlSchema"/> to.</param>
        /// <returns>An <see cref="XmlQualifiedName"/> for the current object.</returns>
        public static XmlQualifiedName AcquireSchema(XmlSchemaSet xs)
        {
            Contract.Requires<ArgumentNullException>(xs != null, "xs");

            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("CommonContracts.WsEventing.ExpirationType.xsd"))
            {
                Debug.Assert(stream != null, "Resource Stream 'CommonContracts.WsEventing.ExpirationType.xsd' was not able to be opened");

                var schema = XmlSchema.Read(stream, null);
                xs.Add(schema);
            }

            return new XmlQualifiedName("ExpirationType", Constants.WsEventing.Namespace);
        }

        #endregion

        #region Validations

        /// <summary>
        /// Indicates if the supplied <paramref name="timeToParse"/> is able to be parsed into an expiration value.
        /// </summary>
        /// <remarks>This does not confirm if the value is an acceptable expiration value, only that it is an acceptable format.</remarks>
        /// <param name="timeToParse">The string value to parse.</param>
        /// <returns>True if the supplied <paramref name="timeToParse"/> value can be parsed into an expiration value; otherwise false.</returns>
        public static Boolean IsValidTime(String timeToParse)
        {
            Contract.Requires<ArgumentNullException>(!String.IsNullOrWhiteSpace(timeToParse), "timeToParse");

            return !timeToParse.StartsWith("P") && DateTime.Parse(timeToParse) < DateTime.Now ? false : true;
        }

        /// <summary>
        /// Indicates is the supplied <paramref name="timeToParse"/> is in duration format (AKA <see cref="TimeSpan"/>).
        /// </summary>
        /// <remarks>This does not confirm if the value is an acceptable expiration value, only that it is an acceptable format.</remarks>
        /// <param name="timeToParse">The string value to parse.</param>
        /// <returns>True if the supplied <paramref name="timeToParse"/> value is in duration format; otherwise false.</returns>
        public static Boolean IsDurationTime(String timeToParse)
        {
            Contract.Requires<ArgumentNullException>(!String.IsNullOrWhiteSpace(timeToParse), "timeToParse");

            return timeToParse.StartsWith("P") ? true : false;
        }

        #endregion
    }
}
