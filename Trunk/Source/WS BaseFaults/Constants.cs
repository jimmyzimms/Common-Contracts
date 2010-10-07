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
        public const String WsBaseFaultsNamespace = "http://docs.oasis-open.org/wsrf/bf-2";

        /// <summary>
        /// Holds the namespace for the WS-Addressing types.
        /// </summary>
        internal const String WsAddressingNamespace = "http://www.w3.org/2005/08/addressing";

        /// <summary>
        /// Holds the namespace for the XSI types.
        /// </summary>
        internal const String XmlSchemaTypeNamespace = "http://www.w3.org/2001/XMLSchema-instance";
    }
}