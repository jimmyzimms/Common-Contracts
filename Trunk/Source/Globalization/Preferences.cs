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
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace CommonContracts.Globalization
{
    /// <summary>
    /// A data contract used for untyped and unbounded extension of the i18n:international/i18n:preferences
    /// element. Since this element represents an xs:any type in XML, the presence of any preferences are
    /// handled as raw XML. The suggested use it to provide a strongly typed class that implements type coersion
    /// or conversion to validate the supplied XML content. It is not intended that this type will perform any
    /// validation on read or written XML.
    /// </summary>
    [XmlSchemaProvider("AcquireSchema")]
    [DebuggerDisplay("Content = '{Content.Count}'")]
    [XmlRoot("preferences", Namespace = Constants.Namespace)]
    public sealed class Preferences : IXmlSerializable
    {
        #region Fields
        
        private readonly ICollection<XElement> content = new Collection<XElement>();

        #endregion

        #region Properties
        
        /// <summary>
        /// Gets the <see cref="ICollection{T}"/> of <see cref="XElement"/> content found in an i18n:preferences
        /// element. It may be empty but will never be null.
        /// </summary>
        /// <value>The <see cref="ICollection{T}"/> of xml content in an i18n:preferences element.</value>
        public ICollection<XElement> Content
        {
            get { return this.content; }
        }

        #endregion

        #region Schema

        /// <summary>
        /// Adds an <see cref="XmlSchema"/> instance for this type to the supplied <see cref="XmlSchemaSet"/>.
        /// </summary>
        /// <param name="xs">The <see cref="XmlSchemaSet"/> to add an <see cref="XmlSchema"/> to.</param>
        /// <returns>An <see cref="XmlQualifiedName"/> for the current object.</returns>
        [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is a parameter name used in Code Contracts")]
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "This is validated via Code Contracts")]
        public static XmlSchemaType AcquireSchema(XmlSchemaSet xs)
        {
            Contract.Requires<ArgumentNullException>(xs != null, "xs");

            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("CommonContracts.Globalization.ws-i18n Schema.xsd"))
            {
                Debug.Assert(stream != null, "Resource Stream 'CommonContracts.Globalization.ws-i18n Schema.xsd' was not able to be opened");

                var schema = XmlSchema.Read(stream, null);
                var type = schema.Items.OfType<XmlSchemaElement>().First(element => element.Name == "preferences");
                xs.Add(schema);

                return type.SchemaType;
            }
        }

        #endregion

        #region IXmlSerializable Members

        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            if (reader == null) throw new ArgumentNullException("reader");
            if (reader.ReadState != ReadState.Interactive) throw new ArgumentException("The supplied Xmlreader must be in ReadState == Interactive", "reader");

            if (reader.IsStartElement("preferences", Constants.Namespace) == false)
            {
                throw new XmlException("Invalid Element, it must be 'preferences'");
            }

            var xml = XElement.Parse(reader.ReadOuterXml());

            foreach (var node in xml.Descendants())
            {
                this.Content.Add(node);
            }
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            if (writer == null) throw new ArgumentNullException("writer");

            foreach (var element in this.Content)
            {
                element.WriteTo(writer);
            }
        }

        #endregion
    }
}
