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
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace CommonContracts.WsEventing
{
    /// <summary>
    /// Represents the "http://schemas.xmlsoap.org/ws/2004/08/eventing:SubscribeType" type.
    /// </summary>
    [XmlSchemaProvider("AcquireSchema")]
    [XmlRoot(DataType = Constants.WsEventing.Namespace + ":SubscribeType", ElementName = "Subscribe", Namespace = Constants.WsEventing.Namespace)]
    public class SubscribeRequestMessageBody : IXmlSerializable
    {
        #region Fields

        private EndpointAddress endTo;
        private Delivery delivery;
        private Expires expires;
        private XPathMessageFilter filter;
        private String filterDialect;

        #endregion

        #region Properties

        public virtual EndpointAddress EndTo
        {
            get { return this.endTo; }
            set { this.endTo = value; }
        }

        public virtual Delivery Delivery
        {
            get { return this.delivery; }
            set { this.delivery = value; }
        }

        public virtual Expires Expires
        {
            get { return this.expires; }
            set { this.expires = value; }
        }

        public virtual XPathMessageFilter Filter
        {
            get { return this.filter; }
            set
            {
                this.filter = value;
                this.filterDialect = Constants.WsEventing.Dialects.XPath;
            }
        }

        public virtual String FilterDialect
        {
            get { return filterDialect; }
        }

        #endregion

        #region Constructors

        [Obsolete("This method is required for the XmlSerializer and not not be directly called")]
        public SubscribeRequestMessageBody() : this(new Delivery(), null)
        {
        }

        public SubscribeRequestMessageBody(Delivery delivery, EndpointAddress endTo)
        {
            Contract.Requires<ArgumentNullException>(delivery != null, "delivery");

            this.delivery = delivery;
            this.endTo = endTo;
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

            if (reader.IsStartElement("Subscribe", Constants.WsEventing.Namespace) == false)
            {
                throw new XmlException("Invalid Element, it must be 'Subscribe'");
            }

            reader.ReadStartElement("Subscribe", Constants.WsEventing.Namespace);
            while (reader.NodeType != XmlNodeType.EndElement)
            {
                if (reader.IsStartElement("EndTo", Constants.WsEventing.Namespace))
                {
                    this.EndTo = EndpointAddress.ReadFrom(AddressingVersion.WSAddressingAugust2004, reader);
                }
                else if (reader.IsStartElement("Delivery", Constants.WsEventing.Namespace))
                {
                    this.Delivery = new Delivery(reader);
                }
                else if (reader.IsStartElement("Expires", Constants.WsEventing.Namespace))
                {
                    this.Expires = new Expires(reader);
                }
                else if (reader.IsStartElement("Filter", Constants.WsEventing.Namespace))
                {
                    String dialect = reader.GetAttribute("Dialect");
                    if (String.IsNullOrEmpty(dialect) || dialect == Constants.WsEventing.Dialects.XPath)
                    {
                        this.Filter = new XPathMessageFilter(reader);
                    }
                    else
                    {
                        this.filterDialect = dialect;
                        reader.Skip();
                    }
                }
                reader.MoveToContent();
            }
            reader.ReadEndElement();
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            var prefix = writer.LookupPrefix(Constants.WsEventing.Namespace);
            if (String.IsNullOrEmpty(prefix)) prefix = "wse";

            writer.WriteStartElement(prefix, "Subscribe", Constants.WsEventing.Namespace);
            if (this.EndTo != null)
            {
                this.EndTo.WriteTo(AddressingVersion.WSAddressingAugust2004, writer, "EndTo", Constants.WsEventing.Namespace);
            }
            if (Delivery != null)
            {
                ((IXmlSerializable)this.Delivery).WriteXml(writer);
            }
            if (this.Expires != null)
            {
                ((IXmlSerializable)this.Expires).WriteXml(writer);
            }
            if (Filter != null)
            {
                this.Filter.WriteXPathTo(writer, "wse", "Filter", Constants.WsEventing.Namespace, true);
            }
            writer.WriteEndElement();
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

            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("CommonContracts.WsEventing.Subscribe.xsd"))
            {
                Debug.Assert(stream != null, "Resource Stream 'CommonContracts.WsEventing.Subscribe.xsd' was not able to be opened");

                var schema = XmlSchema.Read(stream, null);

                var imports = new XmlSchemaSet();
                Delivery.AcquireSchema(imports);
                Expires.AcquireSchema(imports);

                foreach (var includeSchema in imports.Schemas().Cast<XmlSchema>())
                {
                    if (includeSchema.TargetNamespace == Constants.WsEventing.Namespace)
                    {
                        XmlSchemaInclude include = new XmlSchemaInclude();
                        include.Schema = includeSchema;
                        schema.Includes.Add(include);
                    }
                }

               xs.Add(schema);
            }

            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("CommonContracts.WsEventing.WsAddressing.xsd"))
            {
                Debug.Assert(stream != null, "Resource Stream 'CommonContracts.WsEventing.WsAddressing.xsd' was not able to be opened");

                var schema = XmlSchema.Read(stream, null);
                xs.Add(schema);
            }


            return new XmlQualifiedName("SubscribeType", Constants.WsEventing.Namespace);
        }

        #endregion
    }
}
