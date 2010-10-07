using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Xml.Serialization;

namespace CommonContracts.WsBaseFaults.Tests
{
    /// <content>Partials class file.</content>
    public partial class BaseFaultFullTests
    {
        /// <summary>
        /// Required for use on the <see cref="ConstructorShouldAllowNulls"/> as the constructors are ambiguous with a mock
        /// Required for the <see cref="Deserialize"/> test.
        /// Required for the <see cref="NestedFaultsShouldSerialize"/> test.
        /// </summary>
        [XmlRoot("BaseFault", Namespace = Constants.WsBaseFaultsNamespace, DataType = Constants.WsBaseFaultsNamespace + ":BaseFaultType")]
        public sealed class TestFault : BaseFaultFull
        {
            public TestFault() { }

            public TestFault(EndpointAddress originator)
                : base(originator)
            {
            }

            public TestFault(IEnumerable<Description> descriptions)
                : base(descriptions)
            {
            }

            public TestFault(ErrorCode errorCode)
                : base(errorCode)
            {
            }

            public TestFault(DateTime utc)
                : base(utc)
            {
            }

            public TestFault(DateTime utc, EndpointAddress originator, ErrorCode errorCode, IEnumerable<Description> descriptions)
                : base(utc, originator, errorCode, descriptions)
            {
            }
        }
    }
}
