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
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace CommonContracts.WsBaseFaults
{
    /// <summary>
    /// Represents the "http://docs.oasis-open.org/wsrf/bf-2:BaseFaultType/Description" element.
    /// </summary>
    [XmlRoot("Description", Namespace = Constants.WsBaseFaultsNamespace)]
    public class Description : IXmlSerializable
    {
        #region Fields

        private String descriptionValue;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Description"/> class from the supplied <paramref name="reader"/>.
        /// </summary>
        /// <param name="reader">The <see cref="XmlReader"/> to create an instance from.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is a parameter name used in Code Contracts")]
        public Description(XmlReader reader)
        {
            Contract.Requires<ArgumentNullException>(reader != null, "reader");
            Contract.Requires<ArgumentException>(reader.ReadState == ReadState.Interactive, "reader");

            ((IXmlSerializable)this).ReadXml(reader);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Description"/> class.
        /// </summary>
        /// <param name="value">The <see cref="Value"/> property value.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is a parameter name used in Code Contracts")]
        public Description(String value)
        {
            Contract.Requires<ArgumentNullException>(!String.IsNullOrWhiteSpace(value), "value");

            this.descriptionValue = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Description"/> class.
        /// </summary>
        /// <param name="value">The <see cref="Value"/> property value.</param>
        /// <param name="culture">An optional <see cref="CultureInfo"/> value.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1304:SpecifyCultureInfo", Justification = "This type is a XSD contract element and the intended use is for a CultureInfo used to indicate how the supplied string should be evaluated")]
        public Description(String value, CultureInfo culture)
            : this(value)
        {
            this.Language = culture;
        }

        #endregion
        
        #region Properties

        /// <summary>
        /// Gets or sets the value of the <see cref="Description"/>.
        /// </summary>
        /// <value>The value of the <see cref="Description"/>.</value>
        public virtual String Value
        {
            get { return this.descriptionValue; }
            set
            {              
                this.descriptionValue = value;
            }
        }

        /// <summary>
        /// Gets or sets the optional <see cref="CultureInfo"/> value for the <see cref="Description"/>.
        /// </summary>
        /// <value>The optional <see cref="CultureInfo"/> value for the <see cref="Description"/>.</value>
        public CultureInfo Language { get; set; }

        #endregion

        #region IXmlSerializable Members

        [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "IXmlSerializable.GetSchema() must always return null and should never be called application code")]
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "This is validated via Code Contracts")]
        [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is a parameter name used in Code Contracts")]
        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            Contract.Requires<ArgumentNullException>(reader != null, "reader");
            Contract.Requires<ArgumentException>(reader.ReadState == ReadState.Interactive, "reader");

            if (!reader.IsStartElement("Description", Constants.WsBaseFaultsNamespace))
            {
                throw new XmlException("Invalid Element, it must be 'Description'");
            }

            var cultureInfoValue = reader.GetAttribute("lang", "http://www.w3.org/XML/1998/namespace");
            if (!String.IsNullOrWhiteSpace(cultureInfoValue)) this.Language = CultureInfo.GetCultureInfo(cultureInfoValue);

            this.Value = reader.ReadElementContentAsString();
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            if (writer == null) throw new ArgumentNullException("writer");

            var prefix = writer.LookupPrefix(Constants.WsBaseFaultsNamespace);
            if (String.IsNullOrEmpty(prefix)) prefix = "wsbf";

            writer.WriteStartElement(prefix, "Description", Constants.WsBaseFaultsNamespace);
            if (this.Language != null)
            {
                writer.WriteAttributeString("xml", "lang", "http://www.w3.org/XML/1998/namespace", this.Language.Name);
            }
            writer.WriteString(this.Value);
            writer.WriteEndElement();
        }

        #endregion
    }
}