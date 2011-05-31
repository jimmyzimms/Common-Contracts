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
using System.IO;
using System.Moles;
using System.Runtime.Serialization;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CommonContracts.WsBaseFaults.Tests
{
    [TestClass()]
    public class BaseFaultGenericTests
    {
        /// <summary>
        /// Moles and Moq dont always work nicely together so this one type is required for the <see cref="ConstructorShouldSetToUtcNow"/> test.
        /// </summary>
        [DataContractAttribute()]
        private sealed class TestFault : BaseFault<TestFault>
        {
            public TestFault() { }

            public TestFault(TestFault faultCause) : base(faultCause) { }
        }

        [TestMethod()]
        [HostType("Moles")]
        [Description("Confirms that the constructor uses the DateTime.UtcNow value")]
        public void ConstructorShouldSetToUtcNow()
        {
            MDateTime.UtcNowGet = () => new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

            TestFault target = new TestFault();
            Assert.AreEqual(new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), target.Timestamp);

            target = new TestFault(target);
            Assert.AreEqual(new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), target.Timestamp);
        }

        [TestMethod()]
        [Description("Confirms that the parameterized constructor uses the supplied DateTime / BaseFault values")]
        public void ConstructorShouldSetSuppliedParametersoProperties()
        {
            var now = DateTime.UtcNow;
            var mock = new Mock<BaseFault<Object>>(now);
            mock.CallBase = true;

            BaseFault<Object> target = mock.Object;
            Assert.AreEqual(now, target.Timestamp);
            Assert.IsNull(target.FaultCause);

            mock = new Mock<BaseFault<Object>>(now, target);
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

            var mock = new Mock<BaseFault<Object>>(now);
            mock.CallBase = true;

            BaseFault target = mock.Object;
            Assert.AreEqual(now.ToUniversalTime(), target.Timestamp);

            mock = new Mock<BaseFault<Object>>(now, target);
            mock.CallBase = true;

            var parentTarget = mock.Object;
            Assert.AreEqual(now.ToUniversalTime(), parentTarget.Timestamp);
        }

        [TestMethod()]
        [Description("Confirms that the FaultSource property can be set/get")]
        public void CanSetFaultCauseProperty()
        {
            var now = DateTime.Now;
            var mock = new Mock<BaseFault<Object>>(now);
            mock.CallBase = true;

            BaseFault<Object> target = mock.Object;
            Assert.IsNull(target.FaultCause); // Sanity check

            mock = new Mock<BaseFault<Object>>(now);
            mock.CallBase = true;
            BaseFault<Object> otherFault = mock.Object;

            target.FaultCause = otherFault;
            Assert.AreSame(target.FaultCause, otherFault);
        }

        [TestMethod()]
        [Description("Confirms that the FaultSource property allows null")]
        public void CanSetFaultCausePropertyToNull()
        {
            var now = DateTime.Now;
            var mock = new Mock<BaseFault<Object>>(now);
            mock.CallBase = true;

            BaseFault<Object> target = mock.Object;
            Assert.IsNull(target.FaultCause); // Sanity check
            target.FaultCause = null;
            Assert.IsNull(target.FaultCause);

            mock = new Mock<BaseFault<Object>>(now);
            mock.CallBase = true;
            BaseFault<Object> otherFault = mock.Object;

            target.FaultCause = otherFault;
            Assert.IsNotNull(target.FaultCause); // Sanity check
            target.FaultCause = null;
            Assert.IsNull(target.FaultCause);
        }

        [TestMethod()]
        [Description("The FaultCause property must check that the value supplied does is not a reference to itself to help avoid circular references")]
        public void CannotSetFaultCauseToSameReference()
        {
            var mock = new Mock<BaseFault<Object>>();
            mock.CallBase = true;

            BaseFault<Object> target = mock.Object;

            Assert.IsFalse(ReferenceEquals(target, target.FaultCause)); // Sanity check
            try
            {
                target.FaultCause = target;
                Assert.Fail("Exception should have thrown");
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual("You cannot nest a BaseFault with the same reference as itself as this would cause a cirular reference in the FaultCause chain.\r\nParameter name: value", ex.Message);
                Assert.AreEqual("value", ex.ParamName);
            }
        }

        [HostType("Moles")]
        [TestMethod()]
        [Description("Confirms that the class can be serialized")]
        public void CanSerialize()
        {
            MDateTime.UtcNowGet = () => new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

            var serializer = new DataContractSerializer(typeof (TestFault));
            var stream = new MemoryStream();

            serializer.WriteObject(stream, new TestFault());

            stream.Position = 0;

            var xml = XElement.Load(stream);
            var areEqual = XNode.DeepEquals(xml, XElement.Parse("<BaseFaultGenericTests.TestFault xmlns='http://schemas.datacontract.org/2004/07/CommonContracts.WsBaseFaults.Tests' xmlns:i='http://www.w3.org/2001/XMLSchema-instance'><Timestamp xmlns='http://docs.oasis-open.org/wsrf/bf-2'>2000-01-01T00:00:00Z</Timestamp></BaseFaultGenericTests.TestFault>"));
            Assert.IsTrue(areEqual);
        }
    }
}
