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
using System.Globalization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Xml.Linq;

namespace CommonContracts.Globalization
{
    /// <summary>
    /// A <see cref="IClientMessageInspector"/> to be used on WCF client service proxies
    /// to automatically enable <see cref="CultureInfo"/> to be sent to a service. This
    /// is useful for globalized client applications (especially smart clients) that need
    /// to support this functionality. It will default to creating a header element
    /// with the QName "i18n:international" in the outgoing request, however, this can
    /// be customized with the <see cref="HeaderName"/> property. In general this is more
    /// beneficial to for use with WSDL contracts that do not explicitly declare support
    /// for ws-i18n in message contracts but where the service implementation is known to
    /// support it.
    /// </summary>
    public class GlobalizationClientMessageInspector : IClientMessageInspector
    {
        #region Fields

        public static readonly XName Default = XName.Get("international", Constants.Namespace);
        private XName headerName = Default;

        #endregion
        
        #region Properties

        /// <summary>
        /// Gets or sets the <see cref="CultureInfo"/> that should be leveraged to automatically 
        /// create an <see cref="International"/> header element for. This value will be used to
        /// create the <see cref="International.Locale"/> value.
        /// </summary>
        /// <value>
        /// The <see cref="CultureInfo"/> that should be leveraged to automatically create an
        /// <see cref="International"/> header element for. This value may be null.
        /// </value>
        public virtual CultureInfo Locale
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the value that should be leveraged to automatically create the
        /// <see cref="International.Timezone"/> header element value for.
        /// </summary>
        /// <value>
        /// The value that should be leveraged to automatically create the <see cref="International.Timezone"/> 
        /// header element value for. This value may be null.
        /// </value>
        public virtual String Timezone
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the QName that the added header should use. The value will default to 
        /// "i18n:international" and can not be null.
        /// </summary>
        /// <value>The QName that the added header should use.</value>
        public virtual XName HeaderName
        {
            get
            {
                return this.headerName;
            }
            set
            {
                value = value ?? headerName;
                this.headerName = value;
            }
        }

        #endregion

        #region IClientMessageInspector Members

        /// <summary>
        /// Enables inspection or modification of a message before a request message is sent to a service.
        /// </summary>
        /// <param name="request">The message to be sent to the service.</param>
        /// <param name="channel">The  client object channel.</param>
        /// <returns>
        /// Always returns null as no correlation is required.
        /// </returns>
        public Object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            var international = new International(this.Locale);

            if (!String.IsNullOrWhiteSpace(this.Timezone)) international.Timezone = this.Timezone;

            var header = MessageHeader.CreateHeader("international", Constants.Namespace, international);
            request.Headers.Add(header);

            return null;
        }

        /// <summary>
        /// Enables inspection or modification of a message after a reply message is received but prior to passing it back to the client application.
        /// </summary>
        /// <param name="reply">The message to be transformed into types and handed back to the client application.</param><param name="correlationState">Correlation state data.</param>
        public void AfterReceiveReply(ref Message reply, Object correlationState)
        {
        }

        #endregion
    }
}
