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

using System.ServiceModel;

namespace CommonContracts.WsEventing
{
    /// <summary>
    /// Represents the WSDL portType contract used in the WS-Eventing specification.
    /// </summary>
    /// <remarks>
    /// This version of the contract is used to model the basic subscription API. It lacks any
    /// concept of callback clients and channels in the produced runtime WSDL. It is considered
    /// to be the "default" interface for use in the protocol. It will not define any callback
    /// interface that should be leveraged for event sinks (it is considered that this would be
    /// documented and constructed out of band for event sources/sinks). This interface is most
    /// useful as the management subscription API for persistent subscriptions for long running
    /// services (or services that maintain external state).
    /// </remarks>
    [ServiceContract(Name = "EventSource", Namespace = Constants.WsEventing.Namespace)]
    [XmlSerializerFormat(Style = OperationFormatStyle.Document)]
    public interface IEventSource
    {
        /// <summary>
        /// The base subscription operation for the WS-Eventing protocol. This operation will create the
        /// subscription (or fault if not valid or accepted) and return the details to the event sink.
        /// </summary>
        /// <param name="request">The <see cref="SubscribeRequestMessage">request message</see> containing the subscription request details.</param>
        /// <returns>The <see cref="SubscribeResponseMessage">SubscribeResponseMessage</see> containing the subscription details.</returns>
        [OperationContract(Action = Constants.WsEventing.Actions.Subscribe, ReplyAction = Constants.WsEventing.Actions.SubscribeReply)]
        [FaultContract(typeof(SupportedDeliveryModeFault), Name = "SupportedDeliveryModeFault")]
        [FaultContract(typeof(SupportedDialectFault), Name = "SupportedDialectFault")]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        SubscribeResponseMessage Subscribe(SubscribeRequestMessage request);
    }
}
