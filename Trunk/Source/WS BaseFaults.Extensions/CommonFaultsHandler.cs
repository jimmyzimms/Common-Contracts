using System;
using System.ComponentModel.DataAnnotations;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Xml.Schema;

namespace CommonContracts.WsBaseFaults.Extensions
{
    /// <summary>
    /// An <see cref="IErrorHandler"/> implementation that performs the standard <see cref="Exception"/>
    /// to <see cref="FaultException">Fault</see> mapping for the fault types declared in the assembly.
    /// </summary>
    /// <remarks>
    /// This handler will automatically map and handle all instances of the following exceptions:
    /// <list type="table">
    /// <listheader><term>Exception Type</term><description>Fault Contract</description></listheader>
    /// <item><term><see cref="ValidationException"/></term><description><see cref="MessageValidationFault"/></description></item>
    /// <item><term><see cref="XmlSchemaValidationException"/></term><description><see cref="MessageValidationFault"/></description></item>
    /// <item><term><see cref="TimeoutException"/></term><description><see cref="SlaViolationFault"/></description></item>
    /// <item><term><see cref="FaultException"/></term><description>Unchanged. Will be returned to the client as is.</description></item>
    /// <item><term>Any other <see cref="Exception"/></term><description><see cref="ServiceUnavailableFault"/></description></item>
    /// </list>
    /// </remarks>
    public class CommonFaultsHandler : IErrorHandler
    {
        /// <summary>
        /// Enables the creation of a custom <see cref="FaultException{T}"/> that is returned from an exception in the course of a service method.
        /// </summary>
        /// <param name="error">The <see cref="Exception"/> object thrown in the course of the service operation.</param>
        /// <param name="version">The SOAP version of the message.</param>
        /// <param name="fault">The <see cref="Message"/> object that is returned to the client, or service, in the duplex case.</param>
        public virtual void ProvideFault(Exception error, MessageVersion version, ref Message fault)
        {
            if (error is ValidationException)
            {
                var validationException = error as ValidationException;
                var validationFault = new MessageValidationFault(validationException.ValidationResult.ErrorMessage);
                fault = Message.CreateMessage(version, MessageValidationFault.CreateStandardFault(validationFault), Constants.Faults.MessageValidationFault.Action);
            }
            else if (error is XmlSchemaValidationException)
            {
                var validationException = error as XmlSchemaValidationException;
                var validationFault = new MessageValidationFault(validationException.Message);
                fault = Message.CreateMessage(version, MessageValidationFault.CreateStandardFault(validationFault), Constants.Faults.MessageValidationFault.Action);
            }
            else if (error is TimeoutException)
            {
                var slaFault = new SlaViolationFault();
                slaFault.Descriptions.Add(new Description(error.Message));
                fault = Message.CreateMessage(version, SlaViolationFault.CreateStandardFault(slaFault), Constants.Faults.SlaViolationFault.Action);
            }
            else if (error is FaultException)
            {
                // Allow through
            }
            else
            {
                var serviceUnavailableFault = new ServiceUnavailableFault();
                fault = Message.CreateMessage(version, ServiceUnavailableFault.CreateStandardFault(serviceUnavailableFault), Constants.Faults.ServiceUnavailableFault.Action);
            }
        }

        /// <summary>
        /// Enables error-related processing and returns a value that indicates whether the dispatcher aborts the session and the instance context in certain cases. 
        /// </summary>
        /// <returns>
        /// True if  should not abort the session (if there is one) and instance context if the instance context is not <see cref="InstanceContextMode.Single"/>; otherwise, false. The default is false.
        /// </returns>
        /// <param name="error">The exception thrown during processing.</param>
        public virtual Boolean HandleError(Exception error)
        {
            if (error is ValidationException || error is TimeoutException)
            {
                return true;
            }

            return false;
        }
    }
}
