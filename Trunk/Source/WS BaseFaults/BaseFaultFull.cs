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
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace CommonContracts.WsBaseFaults
{
    /// <summary>
    /// Provides the full members that are possible for WS-BaseFaults type.
    /// </summary>
    /// <remarks>Due to the schema of the BaseFaultType, WCF requires use of the <see cref="XmlSerializerFormatAttribute"/> on your service contracts.</remarks>
    [XmlRoot("BaseFault", Namespace = Constants.WsBaseFaultsNamespace, DataType = Constants.WsBaseFaultsNamespace + ":BaseFaultType")]
    [XmlSchemaProvider("AcquireSchema")]
    public abstract class BaseFaultFull : IXmlSerializable
    {
        #region Nested Types

        private sealed class DescriptionCollection : Collection<Description>
        {
            internal DescriptionCollection() { }

            internal DescriptionCollection(IEnumerable<Description> descriptions)
                : this()
            {
                if (descriptions == null) return;
                foreach (var description in descriptions.Where(item => item != null))
                {
                    this.Add(description);
                }
            }

            protected override void InsertItem(Int32 index, Description item)
            {
                if (item == null) throw new InvalidOperationException("A null Description cannot be added");
                base.InsertItem(index, item);
            }

            protected override void SetItem(int index, Description item)
            {
                if (item == null) throw new InvalidOperationException("A null Description cannot be added");
                base.SetItem(index, item);
            }
        }

        #endregion

        #region Fields

        private DateTime timestamp;
        private readonly ICollection<Description> descriptions;
        private EndpointAddress10 originator;
        private ErrorCode errorCode;
        private BaseFaultFull faultCause;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseFault"/> class with the current <see cref="DateTime.UtcNow"/> value.
        /// </summary>
        protected BaseFaultFull() : this(DateTime.UtcNow) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseFault"/> class with the current <see cref="DateTime.UtcNow"/> value and supplied <paramref name="originator"/>.
        /// </summary>
        /// <param name="originator">The optional <see cref="Originator"/> value.</param>
        protected BaseFaultFull(EndpointAddress originator)
            : this(DateTime.UtcNow, originator, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseFault"/> class with the current <see cref="DateTime.UtcNow"/> value and supplied <paramref name="descriptions"/>.
        /// </summary>
        /// <param name="descriptions">The optional <see cref="Description"/> elements. Any null items are filtered.</param>
        protected BaseFaultFull(IEnumerable<Description> descriptions)
            : this(DateTime.UtcNow, null, null, descriptions)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseFault"/> class with the current <see cref="DateTime.UtcNow"/> value and supplied <paramref name="errorCode"/>.
        /// </summary>
        /// <param name="errorCode">The optional <see cref="ErrorCode"/> value.</param>
        protected BaseFaultFull(ErrorCode errorCode)
            : this(DateTime.UtcNow, null, errorCode, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseFault"/> class with the supplied <paramref name="utc"/> value.
        /// </summary>
        /// <remarks>If the supplied date is not in UTC, the value will be coerced.</remarks>
        /// <param name="utc">The <see cref="Timestamp"/> value.</param>
        protected BaseFaultFull(DateTime utc)
        {
            if (utc.Kind != DateTimeKind.Utc) utc = utc.ToUniversalTime();
            this.timestamp = utc;
            this.descriptions = new DescriptionCollection();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseFault"/> class with the supplied values.
        /// </summary>
        /// <remarks>If the supplied date is not in UTC, the value will be coerced.</remarks>
        /// <param name="utc">The <see cref="Timestamp"/> value.</param>
        /// <param name="originator">The optional <see cref="Originator"/> value.</param>
        /// <param name="errorCode">The optional <see cref="ErrorCode"/> value.</param>
        /// <param name="descriptions">The optional <see cref="Description"/> elements. Any null items are filtered.</param>
        protected BaseFaultFull(DateTime utc, EndpointAddress originator, ErrorCode errorCode, IEnumerable<Description> descriptions)
            : this(utc)
        {
            this.originator = originator == null ? null : EndpointAddress10.FromEndpointAddress(originator);
            this.errorCode = errorCode;
            this.descriptions = new DescriptionCollection(descriptions);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the time, in UTC, of the fault instance.
        /// </summary>
        /// <value>The time, in UTC, of the fault instance.</value>
        public virtual DateTime Timestamp
        {
            get { return this.timestamp; }
            protected set
            {
                if (value.Kind != DateTimeKind.Utc) value = value.ToUniversalTime();
                this.timestamp = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="EndpointAddress10">WS-Addressing EPR</see> for the source of the fault.
        /// </summary>
        /// <value>The <see cref="EndpointAddress10">WS-Addressing EPR</see> for the source of the fault.</value>
        public virtual EndpointAddress10 Originator
        {
            get { return this.originator; }
            set { this.originator = value; }
        }

        /// <summary>
        /// Gets or sets the <see cref="ErrorCode"/> for the fault.
        /// </summary>
        /// <value>The <see cref="ErrorCode"/> for the fault.</value>
        public virtual ErrorCode ErrorCode
        {
            get { return this.errorCode; }
            set { this.errorCode = value; }
        }

        /// <summary>
        /// Gets the <see cref="ICollection{T}">collection</see> of <see cref="Description"/> instances for the fault.
        /// </summary>
        /// <remarks>Null values are not supported and will cause an <see cref="InvalidOperationException"/> when added to the collection.</remarks>
        /// <value>The <see cref="ICollection{T}">collection</see> of <see cref="Description"/> instances for the fault.</value>
        public virtual ICollection<Description> Descriptions
        {
            get
            {
                Contract.Ensures(Contract.Result<ICollection<Description>>() != null);
                Contract.Ensures(Contract.ForAll(Contract.Result<ICollection<Description>>(), item => item != null));

                return this.descriptions;
            }
        }

        /// <summary>
        /// Gets or sets the cause of this fault.
        /// </summary>
        /// <value>The cause of this fault.</value>
        /// <exception cref="ArgumentException">The supplied value is the same reference as the current <see cref="BaseFault"/>.</exception>
        public virtual BaseFaultFull FaultCause
        {
            get { return this.faultCause; }
            set
            {
                if (ReferenceEquals(this, value)) throw new ArgumentException("You cannot nest a BaseFault with the same reference as itself as this would cause a cirular reference in the FaultCause chain.", "value");
                this.faultCause = value;
            }
        }

        #endregion

        #region IXmlSerializable Members

        [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "IXmlSerializable.GetSchema() must always return null and should never be called application code")]
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        #region Deserialization

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            if (reader == null) throw new ArgumentNullException("reader");

            this.ReadStartElement(reader);

            while (reader.NodeType != XmlNodeType.EndElement)
            {
                if (reader.IsStartElement("Timestamp", Constants.WsBaseFaultsNamespace))
                {
                    this.timestamp = reader.ReadElementContentAsDateTime();
                }
                else if (reader.IsStartElement("Originator", Constants.WsBaseFaultsNamespace))
                {
                    var epa = reader.ReadOuterXml();
                    var innerReader = XmlReader.Create(new StringReader(epa));
                    using (var dictionaryReader = XmlDictionaryReader.CreateDictionaryReader(innerReader))
                    {
                        this.originator = EndpointAddress10.FromEndpointAddress(EndpointAddress.ReadFrom(dictionaryReader));
                    }
                }
                else if (reader.IsStartElement("ErrorCode", Constants.WsBaseFaultsNamespace))
                {
                    this.errorCode = this.CreateErrorCode(reader);
                }
                else if (reader.IsStartElement("Description", Constants.WsBaseFaultsNamespace))
                {
                    var description = new Description(reader);
                    this.descriptions.Add(description);
                }
                else if (reader.IsStartElement("FaultCause", Constants.WsBaseFaultsNamespace))
                {
                    if (reader.IsEmptyElement) continue;

                    var innerReader = XmlReader.Create(new StringReader(reader.ReadInnerXml()));
                    innerReader.Read();
                    this.faultCause = CreateFaultCause(innerReader);
                }
                else
                {
                    this.ProcessAdditionalElements(reader);
                }
            }
            reader.ReadEndElement();
        }

        /// <summary>
        /// Allows customization of the <see cref="ErrorCode"/> value to be created when reading from an XML source.
        /// </summary>
        /// <remarks>
        /// This method only supports creation of <see cref="ErrorCode"/> instances. Any additional information that
        /// may be contained in the element will be ignored. Override this method to support custom logic based.
        /// </remarks>
        /// <param name="reader">The <see cref="XmlReader"/> containing the XML content to process.</param>
        /// <returns>The appropriate <see cref="ErrorCode"/> type.</returns>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "This is checked via Code Contracts. CA Engine does not yet understand how to deal with contracts.")]
        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", Justification = "This is the parameter name of the code and globalization is not needed.")]
        protected virtual ErrorCode CreateErrorCode(XmlReader reader)
        {
            Contract.Requires<ArgumentNullException>(reader != null, "reader");
            Contract.Requires<ArgumentException>(reader.ReadState == ReadState.Interactive, "reader");

            return new ErrorCode(reader);
        }

        /// <summary>
        /// Allows customization of the <see cref="FaultCause"/> value to be created when reading from an XML source.
        /// </summary>
        /// <remarks>This method always create a new instance of the <see cref="UnknownBaseFault"/> type. Override it to provide custom implementation.</remarks>
        /// <param name="reader">The <see cref="XmlReader"/> containing the XML content to process.</param>
        /// <returns>The appropriate <see cref="BaseFaultFull"/> type.</returns>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "This is checked via Code Contracts. CA Engine does not yet understand how to deal with contracts.")]
        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", Justification = "This is the parameter name of the code and globalization is not needed.")]
        protected virtual BaseFaultFull CreateFaultCause(XmlReader reader)
        {
            Contract.Requires<ArgumentNullException>(reader != null, "reader");
            Contract.Requires<ArgumentException>(reader.ReadState == ReadState.Interactive, "reader");

            return new UnknownBaseFault(reader);
        }

        /// <summary>
        /// Extension point for reading the start element. Since WS-BaseFaults supports extending the BaseFault element
        /// or the BaseFault type, support for both approaches is required. This method provides a hook to allow customization
        /// of the start element name and namespace of the fault.
        /// </summary>
        /// <remarks>
        /// This method will look for and confirm that the start element is named "BaseFault" in the WS-BaseFaults namespace. 
        /// If you are extending the BaseFault element, you should override this method, invoke the base version, and then add
        /// the appropriate xsi type attribute to the current element. If you are extending the BaseFault type, then do not call
        /// the base method and instead make a call to <see cref="XmlReader.ReadStartElement(String, String)"/> to read the
        /// appropriate start element.
        /// </remarks>
        /// <param name="reader">The <see cref="XmlReader"/> to read the XML from.</param>
        /// <exception cref="InvalidOperationException">The current start element is not 'http://:BaseFault'. This is usually due to incorrect XML or forgetting to subclass this method to read the expected start element.</exception>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "This is checked via Code Contracts. CA Engine does not yet understand how to deal with contracts.")]
        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", Justification = "This is the parameter name of the code and globalization is not needed.")]
        protected virtual void ReadStartElement(XmlReader reader)
        {
            Contract.Requires<ArgumentNullException>(reader != null, "reader");

            if (reader.IsStartElement("BaseFault", Constants.WsBaseFaultsNamespace) == false)
            {
                throw new XmlException("Invalid Element, it must be 'BaseFault'");
            }

            reader.ReadStartElement("BaseFault", Constants.WsBaseFaultsNamespace);
        }

        /// <summary>
        /// Extension point for additional xml content to be handled. This method does not allow any additional information
        /// to be supplied and will error with a <see cref="NotSupportedException"/>.
        /// </summary>
        /// <remarks>
        /// This base method is safe to not call provided you manually move the reader to the end element.
        /// </remarks>
        /// <param name="reader">The <see cref="XmlReader"/> containing the XML to process.</param>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "This is a no-op method.")]
        protected virtual void ProcessAdditionalElements(XmlReader reader)
        {
            throw new NotSupportedException("Additional BaseFaultType XML content is not supported. In order to process additional elements you must override the ProcessAdditionalElements method and provide your own logic.");
        }

        #endregion

        #region Serialization
        
        /// <summary>
        /// Extension point for writing the start element. Since WS-BaseFaults supports extending the BaseFault element
        /// or the BaseFault type, support for both approaches is required. This method provides a hook to allow customization
        /// of the start element name and namespace of the fault.
        /// </summary>
        /// <remarks>
        /// This method will write a new start element named "BaseFault" in the WS-BaseFaults namespace. If you are extending
        /// the BaseFault element, you should override this method, invoke the base version, and then add the appropriate
        /// xsi type attribute to the current element. If you are extending the BaseFault type, then do not call the base method
        /// and instead make a call to <see cref="XmlWriter.WriteStartElement(String, String, String)"/> to create the appropriate
        /// start element.
        /// </remarks>
        /// <param name="writer">The <see cref="XmlWriter"/> to write the start element XML.</param>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "This is checked via Code Contracts. CA Engine does not yet understand how to deal with contracts.")]
        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", Justification = "This is the parameter name of the code and globalization is not needed.")]
        protected virtual void WriteStartElement(XmlWriter writer)
        {
            Contract.Requires<ArgumentNullException>(writer != null, "writer");

            var prefix = writer.LookupPrefix(Constants.WsBaseFaultsNamespace);
            if (String.IsNullOrEmpty(prefix)) prefix = "wsbf";

            writer.WriteStartElement(prefix, "BaseFault", Constants.WsBaseFaultsNamespace);
        }

        /// <summary>
        /// Extension point for additional xml content to be handled. This method ignore all additional information when executed.
        /// </summary>
        /// <remarks>This method performs no work and is safe to not be called by inheritors.</remarks>
        /// <param name="writer">The <see cref="XmlWriter"/> to write any additional content XML.</param>
        protected virtual void ProcessAdditionalElements(XmlWriter writer)
        {
        }

        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "This is checked via Code Contracts. CA Engine does not yet understand how to deal with contracts.")]
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            Contract.Requires<ArgumentNullException>(writer != null, "writer");

            this.WriteStartElement(writer);

            var prefix = writer.LookupPrefix(Constants.WsBaseFaultsNamespace);
            if (String.IsNullOrEmpty(prefix)) prefix = "wsbf";

            writer.WriteStartElement(prefix, "Timestamp", Constants.WsBaseFaultsNamespace);
            writer.WriteValue(this.Timestamp);
            writer.WriteEndElement();

            if (this.Originator != null)
            {
                var wsaPrefix = writer.LookupPrefix(Constants.WsAddressingNamespace);
                if (String.IsNullOrEmpty(wsaPrefix)) wsaPrefix = "wsa";

                writer.WriteStartElement(prefix, "Originator", Constants.WsBaseFaultsNamespace);
                writer.WriteAttributeString("xmlns", wsaPrefix, null, Constants.WsAddressingNamespace);
                writer.WriteAttributeString("xsi", "type", Constants.XmlSchemaTypeNamespace, Constants.WsAddressingNamespace + ":EndpointReference");

                ((IXmlSerializable)this.Originator).WriteXml(writer);
                writer.WriteEndElement();
            }

            if (this.ErrorCode != null)
            {
                ((IXmlSerializable)this.ErrorCode).WriteXml(writer);
            }

            foreach (var description in this.Descriptions)
            {
                ((IXmlSerializable)description).WriteXml(writer);
            }

            if (this.FaultCause != null)
            {
                writer.WriteStartElement(prefix, "FaultCause", Constants.WsBaseFaultsNamespace);
                ((IXmlSerializable) this.FaultCause).WriteXml(writer);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        #endregion

        #endregion

        #region Schema

        /// <summary>
        /// Acquires the XML Schema for the current type.
        /// </summary>
        /// <param name="xs">The <see cref="XmlSchemaSet"/> to add the schema for this type to.</param>
        /// <returns>The <see cref="XmlQualifiedName"/> for this type.</returns>
        public static XmlQualifiedName AcquireSchema(XmlSchemaSet xs)
        {
            Contract.Requires<ArgumentNullException>(xs != null, "xs");

            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("CommonContracts.WsBaseFaults.Ws-BaseFaults Schema.xsd"))
            {
                Debug.Assert(stream != null, "Resource Stream 'CommonContracts.WsBaseFaults.Ws-BaseFaults Schema.xsd' was not able to be opened");

                var schema = XmlSchema.Read(stream, null);
                xs.Add(schema);
            }

            return new XmlQualifiedName("BaseFaultType", Constants.WsBaseFaultsNamespace);
        }

        #endregion
    }
}