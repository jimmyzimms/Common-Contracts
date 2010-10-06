using System;
using System.Moles;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CommonContracts.WsBaseFaults.Tests
{
    [TestClass()]
    public class BaseFaultTests
    {
        /// <summary>
        /// Moles and Moq dont always work nicely together so this one type is required for the <see cref="ConstructorShouldSetToUtcNow"/> test.
        /// </summary>
        private sealed class TestFault : BaseFault
        {
            public TestFault() {}

            public TestFault(BaseFault faultCause) : base(faultCause) {}
        }

        [TestMethod()]
        [HostType("Moles")]
        [Description("Confirms that the constructor uses the DateTime.UtcNow value")]
        public void ConstructorShouldSetToUtcNow()
        {
            MDateTime.UtcNowGet = () => new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

            BaseFault target = new TestFault();
            Assert.AreEqual(new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), target.Timestamp);

            target = new TestFault(target);
            Assert.AreEqual(new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), target.Timestamp);
        }

        [TestMethod()]
        [Description("Confirms that the parameterized constructor uses the supplied DateTime / BaseFault values")]
        public void ConstructorShouldSetSuppliedParametersoProperties()
        {
            var now = DateTime.UtcNow;
            var mock = new Mock<BaseFault>(now);
            mock.CallBase = true;

            BaseFault target = mock.Object;
            Assert.AreEqual(now, target.Timestamp);
            Assert.IsNull(target.FaultCause);

            mock = new Mock<BaseFault>(target, now);
            mock.CallBase = true;

            var parentTarget = mock.Object;
            Assert.AreEqual(now, parentTarget.Timestamp);
            Assert.IsNotNull(parentTarget.FaultCause);
            Assert.AreSame(parentTarget.FaultCause, target);
        }

        [TestMethod()]
        [Description("Confirms that the parameterized constructor will coerce a non UTC DateTime value")]
        public void ConstructorShouldSetDateToUtc()
        {
            var now = DateTime.Now;
            Assert.AreEqual(DateTimeKind.Local, now.Kind); // Sanity check

            var mock = new Mock<BaseFault>(now);
            mock.CallBase = true;

            BaseFault target = mock.Object;
            Assert.AreEqual(now.ToUniversalTime(), target.Timestamp);

            mock = new Mock<BaseFault>(target, now);
            mock.CallBase = true;

            var parentTarget = mock.Object;
            Assert.AreEqual(now.ToUniversalTime(), parentTarget.Timestamp);
        }

        [TestMethod()]
        [Description("Confirms that the FaultSource property can be set/get")]
        public void CanSetFaultCauseProperty()
        {
            var now = DateTime.Now;
            var mock = new Mock<BaseFault>(now);
            mock.CallBase = true;

            BaseFault target = mock.Object;
            Assert.IsNull(target.FaultCause); // Sanity check

            mock = new Mock<BaseFault>(now);
            mock.CallBase = true;
            BaseFault otherFault = mock.Object;

            target.FaultCause = otherFault;
            Assert.AreSame(target.FaultCause, otherFault);
        }

        [TestMethod()]
        [Description("Confirms that the FaultSource property allows null")]
        public void CanSetFaultCausePropertyToNull()
        {
            var now = DateTime.Now;
            var mock = new Mock<BaseFault>(now);
            mock.CallBase = true;

            BaseFault target = mock.Object;
            Assert.IsNull(target.FaultCause); // Sanity check
            target.FaultCause = null;
            Assert.IsNull(target.FaultCause);

            mock = new Mock<BaseFault>(now);
            mock.CallBase = true;
            BaseFault otherFault = mock.Object;

            target.FaultCause = otherFault;
            Assert.IsNotNull(target.FaultCause); // Sanity check
            target.FaultCause = null;
            Assert.IsNull(target.FaultCause);
        }

        [TestMethod()]
        [Description("The FaultCause property must check that the value supplied does is not a reference to itself to help avoid circular references")]
        public void CannotSetFaultCauseToSameReference()
        {
            var mock = new Mock<BaseFault>();
            mock.CallBase = true;

            BaseFault target = mock.Object;

            Assert.IsFalse(ReferenceEquals(target, target.FaultCause)); // Sanity check
            try
            {
                target.FaultCause = target;
                Assert.Fail("Exception should have thrown");
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual("You cannot nest a BaseFault with the same reference as itself as this would cause a cirular reference in the FaultCause chain.\r\nParameter name: FaultCause", ex.Message);
                Assert.AreEqual("FaultCause", ex.ParamName);
            }
        }
    }
}
