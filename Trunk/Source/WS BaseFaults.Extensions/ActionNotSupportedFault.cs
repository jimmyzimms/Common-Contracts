using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace CommonContracts.WsBaseFaults.Extensions
{
    /// <summary>
    /// The fault detail message that indicates a service implementation does not implement or support
    /// a particular SOAP action.
    /// </summary>
    /// <remarks>
    /// This fault is best used as an undeclared fault used by service implementations of well known
    /// portType contracts (such as an OASIS spec) where not all features and operations are actually
    /// implemented. It can be seen as somewhat analagous to a <see cref="NotImplementedException"/>
    /// or <see cref="NotSupportedException"/> with interface implementation.
    /// </remarks>
    [DataContract(Name = "ActionNotSupported", Namespace = Constants.Namespace)]
    public class ActionNotSupportedFault : BaseFault
    {
        #region Fields

        [DataMember(Name = "Action")]
        private readonly String action;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionNotSupportedFault"/>.
        /// </summary>
        /// <param name="action">The SOAP action that is not supported by the service.</param>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "This is checked via Code Contracts. CA Engine does not yet understand how to deal with contracts.")]
        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", Justification = "This is the parameter name of the code and globalization is not needed.")]
        public ActionNotSupportedFault(String action)
        {
            Contract.Requires<ArgumentNullException>(!String.IsNullOrWhiteSpace(action), "action");

            this.action = action;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the SOAP action that is not supported by the service.
        /// </summary>
        /// <value>The SOAP action that is not supported by the service.</value>
        public String Action
        {
            get { return this.action; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Factory used to create the expected <see cref="MessageFault"/> to be used when returning this fault type.
        /// </summary>
        /// <param name="fault">The <see cref="ActionNotSupportedFault"/> to create a <see cref="MessageFault"/> from.</param>
        /// <returns>The appropriate <see cref="MessageFault"/> for the supplied <see cref="SlaViolationFault"/>.</returns>
        public static MessageFault CreateStandardFault(ActionNotSupportedFault fault)
        {
            var faultCode = FaultCode.CreateSenderFaultCode(Constants.Faults.ActionNotSupportedFault.FaultCode, Constants.Namespace);
            var faultReason = new FaultReason("The received SOAP request to the service is not supported by this service implementation.");

            return MessageFault.CreateFault(faultCode, faultReason, fault);
        }

        #endregion
    }
}
