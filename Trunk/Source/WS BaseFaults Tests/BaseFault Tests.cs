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
using System.Runtime.Serialization;
using System.Xml.Linq;
using Microsoft.QualityTools.Testing.Fakes;
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
        [DataContract()]
        private sealed class TestFault : BaseFault
        {
        }

        [TestMethod()]
        [Description("Confirms that the constructor uses the DateTime.UtcNow value")]
        public void ConstructorShouldSetToUtcNow()
        {
            using (ShimsContext.Create())
            {
                System.Fakes.ShimDateTime.UtcNowGet = () => new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

                BaseFault target = new TestFault();
                Assert.AreEqual(new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), target.Timestamp);
            }
        }

        [TestMethod()]
        [Description("Confirms that the parameterized constructor uses the supplied DateTime value")]
        public void ConstructorShouldSetSuppliedParametersoProperties()
        {
            var now = DateTime.UtcNow;
            var mock = new Mock<BaseFault>(now);
            mock.CallBase = true;

            BaseFault target = mock.Object;
            Assert.AreEqual(now, target.Timestamp);
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
        }

        [TestMethod()]
        [Description("Confirms that the class can be serialized")]
        public void CanSerialize()
        {
            using (ShimsContext.Create())
            {
                System.Fakes.ShimDateTime.UtcNowGet = () => new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

                var serializer = new DataContractSerializer(typeof (TestFault));
                var stream = new MemoryStream();

                serializer.WriteObject(stream, new TestFault());

                stream.Position = 0;

                var xml = XElement.Load(stream);
                var areEqual = XNode.DeepEquals(xml,
                    XElement.Parse("<BaseFaultTests.TestFault xmlns='http://schemas.datacontract.org/2004/07/CommonContracts.WsBaseFaults.Tests' xmlns:i='http://www.w3.org/2001/XMLSchema-instance'><Timestamp xmlns='http://docs.oasis-open.org/wsrf/bf-2'>2000-01-01T00:00:00Z</Timestamp></BaseFaultTests.TestFault>"));
                Assert.IsTrue(areEqual);
            }
        }
    }
}
