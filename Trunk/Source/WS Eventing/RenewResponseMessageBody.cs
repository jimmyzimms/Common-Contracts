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
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace CommonContracts.WsEventing
{
    /// <summary>
    /// Represents the "http://schemas.xmlsoap.org/ws/2004/08/eventing:RenewResponseType" type.
    /// </summary>
    [XmlSchemaProvider("AcquireSchema")]
    [XmlRoot(DataType = Constants.WsEventing.Namespace + ":RenewResponseType", ElementName = "RenewResponse", Namespace = Constants.WsEventing.Namespace)]
    public class RenewResponseMessageBody : IXmlSerializable
    {
        #region Fields

        private Expires expires;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the <see cref="Expires"/> used by the event sink to determine when the renewed subscription will expire.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If this value is null, the subscription will not expire. That is, the subscription has an indefinite lifetime. It may be
        /// terminated by the subscriber using an <see cref="ISubscriptionManager.Unsubscribe"/> request, or it may be terminated by
        /// the event source at any time for reasons such as connection termination, resource constraints, or system shut-down.
        /// </para>
        /// <para>
        /// There is no requirement that this value have any correlation to the requested expiration.
        /// </para>
        /// </remarks>
        /// <value>The <see cref="Expires"/> used by the event sink to determine when the created subscription will expire or a null value.</value>
        public virtual Expires Expires
        {
            get { return this.expires; }
            set
            {
                Contract.Requires<ArgumentNullException>(value != null, "Expires");

                this.expires = value;
            }
        }

        #endregion

        #region Constructors
        
        /// <summary>
        /// Initializes a new instance of the <see cref="RenewResponseMessageBody"/> class.
        /// </summary>
        /// <remarks>This constructor will not initialize any values.</remarks>
        [Obsolete("This method is required for the XmlSerializer and not to be directly called")]
        public RenewResponseMessageBody()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RenewResponseMessageBody"/> class with the supplied <see cref="WsEventing.Expires">expiration date</see>.
        /// </summary>
        /// <param name="expires">The <see cref="WsEventing.Expires"/> indicating the new expiration date.</param>
        public RenewResponseMessageBody(Expires expires)
        {
            Contract.Requires<ArgumentNullException>(expires != null, "expires");

            this.expires = expires;
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

            reader.ReadStartElement("RenewResponse", Constants.WsEventing.Namespace);

            if (reader.IsStartElement("Expires", Constants.WsEventing.Namespace)) this.Expires = new Expires(reader);
            
            reader.ReadEndElement();
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            if (Expires == null) throw new XmlException("Expires cannot be null");

            ((IXmlSerializable)this.Expires).WriteXml(writer);
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
            if (xs == null) throw new ArgumentNullException("xs");

            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("CommonContracts.WsEventing.RenewResponse.xsd"))
            {
                Debug.Assert(stream != null, "Resource Stream 'CommonContracts.WsEventing.RenewResponse.xsd' was not able to be opened");

                var schema = XmlSchema.Read(stream, null);

                var imports = new XmlSchemaSet();
                Expires.AcquireSchema(imports);
                SubscriptionManager.AcquireSchema(imports);

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

            return new XmlQualifiedName("RenewResponseType", Constants.WsEventing.Namespace);
        }

        #endregion
    }
}
