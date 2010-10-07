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
using System.Xml.Serialization;

namespace CommonContracts.WsBaseFaults
{
    /// <summary>
    /// An extension of <see cref="ErrorCode{T}"/> that allows additional elements to be added to the content of the "http://docs.oasis-open.org/wsrf/bf-2:BaseFaultType/ErrorCode" element.
    /// </summary>
    /// <typeparam name="T">The type representing the internal content of <see cref="ErrorCode{T}"/>.</typeparam>
    [XmlRoot("ErrorCode", Namespace = Constants.WsBaseFaultsNamespace)]
    public class ErrorCode<T> : ErrorCode where T : IXmlSerializable, new()
    {
        #region Fields

        private T content;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorCode{T}"/> class.
        /// </summary>
        /// <param name="dialect">The <see cref="ErrorCode.Dialect"/> value.</param>
        /// <param name="content">The <see cref="Content"/> value.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", Justification = "This is the parameter name of the code and globalization is not needed.")]
        public ErrorCode(Uri dialect, T content)
            : base(dialect)
        {
            Contract.Requires<ArgumentNullException>(dialect != null, "dialect");

            this.content = content;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the additional content for the <see cref="ErrorCode{T}"/>.
        /// </summary>
        /// <value>The additional content for the <see cref="ErrorCode{T}"/>.</value>
        public virtual T Content
        {
            get { return this.content; }
            set { this.content = value; }
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Extension point for additional xml content to be handled.
        /// </summary>
        /// <param name="reader">The <see cref="XmlReader"/> containing the XML to process.</param>
        protected override void ProcessAdditionalElements(XmlReader reader)
        {
            this.content = new T();
            this.content.ReadXml(reader);
        }

        /// <summary>
        /// Extension point for additional xml content to be handled.
        /// </summary>
        /// <param name="writer">The <see cref="XmlWriter"/> to write any additional content XML.</param>
        protected override void ProcessAdditionalElements(XmlWriter writer)
        {
            if (this.content == null) return;

            this.content.WriteXml(writer);
        }

        #endregion
    }
}
