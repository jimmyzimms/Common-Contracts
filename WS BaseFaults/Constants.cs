using System;

namespace CommonContracts.WsBaseFaults
{
    /// <summary>
    /// Provides compile time constants for the WS-BaseFaults specification.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Holds the soap action for the base SOAP Fault message action.
        /// </summary>
        public const String WsAddressingBaseFaultAction = "http://schemas.xmlsoap.org/ws/2004/08/addressing/fault/";

        /// <summary>
        /// Holds the namespace for the WS-BaseFaults types.
        /// </summary>
        public const String WsBaseFaultsNamespace = "http://docs.oasis-open.org/wsrf/2004/06/wsrf-WS-BaseFaults-1.2-draft-02.xsd";

    }
}