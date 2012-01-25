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
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace CommonContracts.WsEventing
{
    [XmlSchemaProvider("AcquireSchema")]
    [XmlRoot(DataType = Constants.WsEventing.Extension.ExtensionNamespace + ":SubscriptionTopic", ElementName = "SubscriptionTopic", Namespace = Constants.WsEventing.Extension.ExtensionNamespace)]
    public class SubscriptionTopic : IXmlSerializable
    {
        #region Fields

        private Uri topic;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the <see cref="Uri"/> containing the topic identifier value.
        /// </summary>
        /// <value>The <see cref="Uri"/> containing the topic identifier value.</value>
        public virtual Uri Topic
        {
            get { return this.topic; }
            set { this.topic = value; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionTopic"/> class.
        /// </summary>
        public SubscriptionTopic()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionTopic"/> class with the supplied <paramref name="topic"/> value.
        /// </summary>
        /// <param name="topic">The topic the subscription is for.</param>
        public SubscriptionTopic(Uri topic)
        {
            Contract.Assert(topic != null, "topic");

            this.topic = topic;
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

            if (reader.IsStartElement("SubscriptionTopic", Constants.WsEventing.Extension.ExtensionNamespace))
            {
                this.topic = new Uri(reader.ReadElementString());
            }
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            var prefix = writer.LookupPrefix(Constants.WsEventing.Extension.ExtensionNamespace);
            if (String.IsNullOrEmpty(prefix)) prefix = "wsee";

            if (this.topic != null)
            {
                writer.WriteElementString(prefix, "SubscriptionTopic", Constants.WsEventing.Extension.ExtensionNamespace, this.topic.AbsoluteUri);
            }
        }

        #endregion

        #region Schema

        /// <summary>
        /// Adds an <see cref="XmlSchema"/> instance for this type to the supplied <see cref="XmlSchemaSet"/>.
        /// </summary>
        /// <param name="xs">The <see cref="XmlSchemaSet"/> to add an <see cref="XmlSchema"/> to.</param>
        /// <returns>An <see cref="XmlQualifiedName"/> for the current object.</returns>
        public static XmlSchemaType AcquireSchema(XmlSchemaSet xs)
        {
            if (xs == null) throw new ArgumentNullException("xs");

            var reader = new StringReader(@"<xs:schema targetNamespace='" + Constants.WsEventing.Extension.ExtensionNamespace + "' xmlns:xs='http://www.w3.org/2001/XMLSchema'><xs:element name='SubscriptionTopic' type='xs:anyURI'/></xs:schema>");

            var schema = XmlSchema.Read(reader, null);
            xs.Add(schema);
            var type = schema.Items.OfType<XmlSchemaElement>().First(element => element.Name == "SubscriptionTopic");

            return type.SchemaType;
        }

        #endregion
    }
}
