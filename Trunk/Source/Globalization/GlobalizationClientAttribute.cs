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
using System.Linq;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Xml.Linq;

namespace CommonContracts.Globalization
{
    /// <summary>
    /// An <see cref="IOperationBehavior"/> that can be placed on any operation contract method to
    /// introduce a <see cref="GlobalizationClientMessageInspector"/> on the channel. Simply add
    /// to the operation to automatically add support for ws-i18n outgoing headers.
    /// </summary>
    [Serializable()]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class GlobalizationClientAttribute : Attribute, IOperationBehavior
    {
        #region Fields
        
        private readonly Boolean useCurrentUICulture;
        private String cultureInfo;
        private String name;

        #endregion

        #region Constructor
        
        /// <summary>
        /// Initializes a new instance of the <see cref="GlobalizationClientAttribute"/> class.
        /// </summary>
        /// <param name="useCurrentUICulture">Indicates if the current <see cref="System.Globalization.CultureInfo.CurrentUICulture"/> at the time of the service call should be automatically used.</param>
        public GlobalizationClientAttribute(Boolean useCurrentUICulture)
        {
            this.useCurrentUICulture = useCurrentUICulture;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the local name portion of the QName that should be used with the header element. This value may be null.
        /// </summary>
        /// <value>The local name portion of the QName that should be used with the header element. This value may be null.</value>
        [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is a parameter name used in Code Contracts")]
        public String Name
        {
            get
            {
                return this.name;
            }
            set
            {
                Contract.Ensures(String.IsNullOrEmpty(this.name) ? String.IsNullOrEmpty(this.Namespace) : true, "If Name is null the Namespace value must be as well");

                if (String.IsNullOrWhiteSpace(value))
                {
                    this.name = String.Empty;
                    this.Namespace = String.Empty;
                }
                else
                {
                    this.name = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the namespace portion of the QName that should be used for the header element. This value may be null but will always be null if <see cref="Name"/> is.
        /// </summary>
        /// <value>The namespace portion of the QName that should be used for the header element. This value may be null but will always be null if <see cref="Name"/> is.</value>
        public String Namespace { get; set; }

        /// <summary>
        /// Gets or sets the time zone information that should be used in the "i18n:tz" element.
        /// </summary>
        /// <value>The time zone information that should be used in the "i18n:tz" element.</value>
        public String TimeZone
        {
            get; set;
        }

        private XName QName
        {
            get
            {
                if (String.IsNullOrEmpty(this.Name))
                {
                    return null;
                }
                return XName.Get(this.Name, this.Namespace ?? String.Empty);
            }
        }

        /// <summary>
        /// Gets or sets the value that should be used for a current culture. This value cannot be set if <see cref="UseCurrentUICulture"/> is true.
        /// </summary>
        /// <value>The value that should be used for a current culture.</value>
        [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is a parameter name used in Code Contracts")]
        public String CultureInfo 
        {
            get
            {
                if (this.UseCurrentUICulture)
                {
                    return System.Globalization.CultureInfo.CurrentUICulture.IetfLanguageTag;
                }
                return this.cultureInfo;
            }
            set
            {
                Contract.Requires<InvalidOperationException>(!this.UseCurrentUICulture, "You cannot support both automatic UI culture use and explicitly setting this value");

                this.cultureInfo = value;
            }
        }

        /// <summary>
        /// Indicates whether the current UI culture should be automatically used when making a service request.
        /// </summary>
        /// <value>True if the current UI culture should be automatically used when making a service request; otherwise false.</value>
        public Boolean UseCurrentUICulture
        {
            get { return this.useCurrentUICulture; }
        }

        #endregion

        #region IOperationBehavior Members

        /// <summary>
        /// Implement to confirm that the operation meets some intended criteria.
        /// </summary>
        /// <param name="operationDescription">The operation being examined. Use for examination only. If the operation description is modified, the results are undefined.</param>
        public void Validate(OperationDescription operationDescription)
        {
        }

        /// <summary>
        /// Implements a modification or extension of the service across an operation.
        /// </summary>
        /// <param name="operationDescription">The operation being examined. Use for examination only. If the operation description is modified, the results are undefined.</param><param name="dispatchOperation">The run-time object that exposes customization properties for the operation described by <paramref name="operationDescription"/>.</param>
        public void ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
        {
        }

        /// <summary>
        /// Implements a modification or extension of the client across an operation.
        /// </summary>
        /// <param name="operationDescription">The operation being examined. Use for examination only. If the operation description is modified, the results are undefined.</param><param name="clientOperation">The run-time object that exposes customization properties for the operation described by <paramref name="operationDescription"/>.</param>
        public void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
        {
            if (clientOperation == null) throw new ArgumentNullException("clientOperation");

            if (clientOperation.Parent.MessageInspectors.OfType<GlobalizationClientMessageInspector>().Any()) return;

            var inspector = new GlobalizationClientMessageInspector();
            if (!String.IsNullOrEmpty(this.CultureInfo))
            {
                inspector.Locale = System.Globalization.CultureInfo.GetCultureInfoByIetfLanguageTag(this.CultureInfo);
            }

            if (this.QName != null)
            {
                inspector.HeaderName = this.QName;
            }

            if (!String.IsNullOrWhiteSpace(this.TimeZone))
            {
                inspector.TimeZone = this.TimeZone;
            }

            clientOperation.Parent.MessageInspectors.Add(inspector);
        }

        /// <summary>
        /// Implement to pass data at runtime to bindings to support custom behavior.
        /// </summary>
        /// <param name="operationDescription">The operation being examined. Use for examination only. If the operation description is modified, the results are undefined.</param><param name="bindingParameters">The collection of objects that binding elements require to support the behavior.</param>
        public void AddBindingParameters(OperationDescription operationDescription, BindingParameterCollection bindingParameters)
        {
        }

        #endregion
    }
}
