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
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace CommonContracts.WsBaseFaults
{
    /// <summary>
    /// Represents the "http://docs.oasis-open.org/wsrf/bf-2:BaseFaultType/ErrorCode" element.
    /// </summary>
    [XmlRoot("ErrorCode", Namespace = Constants.WsBaseFaults.Namespace)]
    public class ErrorCode : IXmlSerializable
    {
        #region Fields

        private Uri dialect;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorCode"/> class from the supplied <paramref name="reader"/>.
        /// </summary>
        /// <param name="reader">The <see cref="XmlReader"/> to create an instance from.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is a parameter name used in Code Contracts")]
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "This is validated via Code Contracts")]
        public ErrorCode(XmlReader reader)
        {
            Contract.Requires<ArgumentNullException>(reader != null, "reader");
            Contract.Requires<ArgumentException>(reader.ReadState == ReadState.Interactive, "reader");

            ((IXmlSerializable)this).ReadXml(reader);
        }

        /// <summary>
        /// Intializes a new instance of the <see cref="ErrorCode"/> class with the supplied <paramref name="dialect"/> value.
        /// </summary>
        /// <param name="dialect">The <see cref="Dialect"/> value.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is a parameter name used in Code Contracts")]
        public ErrorCode(Uri dialect)
        {
            Contract.Requires<ArgumentNullException>(dialect != null, "dialect");

            this.dialect = dialect;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the <see cref="Uri"/> value for the <see cref="ErrorCode"/>.
        /// </summary>
        /// <value>The <see cref="Uri"/> value for the <see cref="ErrorCode"/>.</value>
        [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is a parameter name used in Code Contracts")]
        public virtual Uri Dialect
        {
            get { return this.dialect; }
            protected set
            {
                Contract.Requires<ArgumentNullException>(value != null, "Dialect");
                this.dialect = value;
            }
        }

        #endregion

        #region IXmlSerializable Members

        /// <summary>
        /// Extension point for additional xml content to be handled. This method ignore all additional information when
        /// executed and will invoke <see cref="XmlReader.Read()"/> until no more elements exist and the reader is finished.
        /// </summary>
        /// <remarks>
        /// This base method is safe to not call provided you manually move the reader to the end element. If calling this
        /// method on the base type, perform any work with the supplied <paramref name="reader"/> prior to calling the base
        /// implementation. Inheritors should be aware that a child reader is created specifically for any inner XML content
        /// and supplied to this method. The reader will always be positioned on the initial element.
        /// </remarks>
        /// <param name="reader">The <see cref="XmlReader"/> containing the XML to process.</param>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "This is validated via Code Contracts")]
        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", Justification = "This is the parameter name of the code and globalization is not needed.")]
        protected virtual void ProcessAdditionalElements(XmlReader reader)
        {
            Contract.Requires<ArgumentNullException>(reader != null);
            Contract.Requires<ArgumentException>(reader.ReadState == ReadState.Interactive, "reader");

            while (reader.Read())
            {
                // do nothing
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "IXmlSerializable.GetSchema() must always return null and should never be called application code")]
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "This is validated via Code Contracts")]
        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", Justification = "This is the parameter name of the code and globalization is not needed.")]
        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            Contract.Requires<ArgumentNullException>(reader != null, "reader");
            Contract.Requires<ArgumentException>(reader.ReadState == ReadState.Interactive, "reader");

            if (!reader.IsStartElement("ErrorCode", Constants.WsBaseFaults.Namespace))
            {
                throw new XmlException("Invalid Element, it must be 'ErrorCode'");
            }

            var attributeValue = reader.GetAttribute("dialect");
            if (String.IsNullOrWhiteSpace(attributeValue)) throw new XmlException("Invalid Element, 'ErrorCode' requires the 'dialect' attribute value to be supplied");
            this.dialect = new Uri(attributeValue);

            if (reader.IsEmptyElement)
            {
                reader.Read();
            }
            else
            {
                var innerReader = reader.ReadSubtree();
                innerReader.Read();
                this.ProcessAdditionalElements(innerReader);

                reader.ReadEndElement();
            }
        }

        /// <summary>
        /// Extension point for additional xml content to be handled. This method ignore all additional information when executed.
        /// </summary>
        /// <remarks>This method performs no work and is safe to not be called by inheritors.</remarks>
        /// <param name="writer">The <see cref="XmlWriter"/> to write any additional content XML.</param>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "This is a no-op method.")]
        protected virtual void ProcessAdditionalElements(XmlWriter writer)
        {
        }

        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "This is validated via Code Contracts")]
        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", Justification = "This is the parameter name of the code and globalization is not needed.")]
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            Contract.Requires<ArgumentNullException>(writer != null, "writer");

            var prefix = writer.LookupPrefix(Constants.WsBaseFaults.Namespace);
            if (String.IsNullOrEmpty(prefix)) prefix = "wsbf";

            writer.WriteStartElement(prefix, "ErrorCode", Constants.WsBaseFaults.Namespace);
            writer.WriteAttributeString("dialect", this.Dialect.ToString());

            this.ProcessAdditionalElements(writer);

            writer.WriteEndElement();
        }

        #endregion
    }
}