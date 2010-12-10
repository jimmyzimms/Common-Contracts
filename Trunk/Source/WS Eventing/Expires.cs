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
        
        public virtual DateTime Value
        {
            get
            {
                if (expires.StartsWith("P"))
                {
                    DateTime dt = DateTime.Now.Add(XmlConvert.ToTimeSpan(expires)).ToUniversalTime();
                    expires = XmlConvert.ToString(dt, XmlDateTimeSerializationMode.Utc);
                    return dt;
                }
                return XmlConvert.ToDateTime(expires, XmlDateTimeSerializationMode.Utc);
            }
            set
            {
                expires = XmlConvert.ToString(value, XmlDateTimeSerializationMode.Utc);
            }
        }
        
        #endregion

        #region Constructors
        
        [Obsolete("This constructor is required for by the XmlSerializer")]
        public Expires()
        {
        }

        public Expires(DateTime dt)
        {
            expires = XmlConvert.ToString(dt, XmlDateTimeSerializationMode.Utc);
        }
        
        public Expires(TimeSpan duration)
        {
            expires = XmlConvert.ToString(duration);
        }

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

        public static Boolean IsValidTime(String timeToParse)
        {
            Contract.Requires<ArgumentNullException>(!String.IsNullOrWhiteSpace(timeToParse), "timeToParse");

            return !timeToParse.StartsWith("P") && DateTime.Parse(timeToParse) < DateTime.Now ? false : true;
        }

        public static Boolean IsDurationTime(String timeToParse)
        {
            Contract.Requires<ArgumentNullException>(!String.IsNullOrWhiteSpace(timeToParse), "timeToParse");

            return timeToParse.StartsWith("P") ? true : false;
        }

        #endregion
    }
}
