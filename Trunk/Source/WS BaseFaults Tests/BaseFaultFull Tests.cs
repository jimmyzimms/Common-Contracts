﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Moq;
using NUnit.Framework;

namespace CommonContracts.WsBaseFaults.Tests
{
    [TestFixture()]
    public partial class BaseFaultFullTests
    {
        [Test()]
        [Description("Confirms that the properties will be set by their constructor parameter counterparts")]
        public void ConstructorShouldSetProperties()
        {
            var endpoint = new EndpointAddress("http://someUri");
            var description = new Description("some desc");
            var errorCode = new ErrorCode(new Uri("http://foo"));
            var utc = DateTime.UtcNow;

            var mock = new Mock<BaseFaultFull>(endpoint);
            mock.CallBase = true;
            var baseFault = mock.Object;

            Assert.That(baseFault.Descriptions, Is.Empty);
            Assert.That(baseFault.ErrorCode, Is.Null);
            Assert.That(baseFault.FaultCause, Is.Null);
            Assert.That(baseFault.Originator.ToEndpointAddress(), Is.EqualTo(endpoint));

            mock = new Mock<BaseFaultFull>((IEnumerable<Description>)new[] { description });
            mock.CallBase = true;
            baseFault = mock.Object;

            Assert.That(baseFault.Descriptions, Is.EquivalentTo(new[] { description }));
            Assert.That(baseFault.ErrorCode, Is.Null);
            Assert.That(baseFault.FaultCause, Is.Null);
            Assert.That(baseFault.Originator, Is.Null);

            mock = new Mock<BaseFaultFull>(errorCode);
            mock.CallBase = true;
            baseFault = mock.Object;

            Assert.That(baseFault.Descriptions, Is.Empty);
            Assert.That(baseFault.ErrorCode, Is.SameAs(errorCode));
            Assert.That(baseFault.FaultCause, Is.Null);
            Assert.That(baseFault.Originator, Is.Null);

            mock = new Mock<BaseFaultFull>(utc);
            mock.CallBase = true;
            baseFault = mock.Object;

            Assert.That(baseFault.Descriptions, Is.Empty);
            Assert.That(baseFault.ErrorCode, Is.Null);
            Assert.That(baseFault.FaultCause, Is.Null);
            Assert.That(baseFault.Originator, Is.Null);
            Assert.That(baseFault.Timestamp, Is.EqualTo(utc));

            mock = new Mock<BaseFaultFull>(utc, endpoint, errorCode, new[] { description });
            mock.CallBase = true;
            baseFault = mock.Object;

            Assert.That(baseFault.Descriptions, Is.EquivalentTo(new[] { description }));
            Assert.That(baseFault.ErrorCode, Is.SameAs(errorCode));
            Assert.That(baseFault.FaultCause, Is.Null);
            Assert.That(baseFault.Originator.ToEndpointAddress(), Is.SameAs(endpoint));
            Assert.That(baseFault.Timestamp, Is.EqualTo(utc));
        }

        [Test()]
        [Description("Confirms that nulls are allowed by the constructors")]
        public void ConstructorShouldAllowNulls()
        {
            var baseFault = new TestFault((EndpointAddress)null);
            Assert.That(baseFault.Descriptions, Is.Empty);
            Assert.That(baseFault.ErrorCode, Is.Null);
            Assert.That(baseFault.FaultCause, Is.Null);
            Assert.That(baseFault.Originator, Is.Null);

            baseFault = new TestFault((IEnumerable<Description>)null);
            Assert.That(baseFault.Descriptions, Is.Empty);
            Assert.That(baseFault.ErrorCode, Is.Null);
            Assert.That(baseFault.FaultCause, Is.Null);
            Assert.That(baseFault.Originator, Is.Null);

            baseFault = new TestFault((ErrorCode)null);
            Assert.That(baseFault.Descriptions, Is.Empty);
            Assert.That(baseFault.ErrorCode, Is.Null);
            Assert.That(baseFault.FaultCause, Is.Null);
            Assert.That(baseFault.Originator, Is.Null);

            baseFault = new TestFault(DateTime.UtcNow, (EndpointAddress)null, (ErrorCode)null, (IEnumerable<Description>)null);
            Assert.That(baseFault.Descriptions, Is.Empty);
            Assert.That(baseFault.ErrorCode, Is.Null);
            Assert.That(baseFault.FaultCause, Is.Null);
            Assert.That(baseFault.Originator, Is.Null);
        }

        [Test()]
        [Description("Confirms that the parameterized constructor will coerce a non UTC DateTime value")]
        public void ConstructorShouldSetDateToUtc()
        {
            var now = DateTime.Now;
            Assert.AreEqual(DateTimeKind.Local, now.Kind); // Sanity check

            var mock = new Mock<BaseFaultFull>(now);
            mock.CallBase = true;

            BaseFaultFull target = mock.Object;
            Assert.AreEqual(now.ToUniversalTime(), target.Timestamp);

            mock = new Mock<BaseFaultFull>(now, null, null, null);
            mock.CallBase = true;

            var parentTarget = mock.Object;
            Assert.AreEqual(now.ToUniversalTime(), parentTarget.Timestamp);
        }

        [Test()]
        [Description("Confirms that the properties can be set/get")]
        public void CanSetProperties()
        {
            var endpoint = EndpointAddress10.FromEndpointAddress(new EndpointAddress("http://someUri"));
            var errorCode = new ErrorCode(new Uri("http://foo"));
            var now = DateTime.UtcNow;
            var faultCause = (new Mock<BaseFaultFull>()).Object;

            var mock = new Mock<BaseFaultFull>(now);
            mock.CallBase = true;

            BaseFaultFull target = mock.Object;
            Assert.IsNull(target.ErrorCode); // Sanity check
            Assert.IsNull(target.FaultCause); // Sanity check
            Assert.IsNull(target.Originator); // Sanity check

            target.ErrorCode = errorCode;
            Assert.That(target.ErrorCode, Is.SameAs(errorCode));
            target.ErrorCode = null;
            Assert.That(target.ErrorCode, Is.Null);

            target.FaultCause = faultCause;
            Assert.That(target.FaultCause, Is.SameAs(faultCause));
            target.FaultCause = null;
            Assert.That(target.FaultCause, Is.Null);

            target.Originator = endpoint;
            Assert.That(target.Originator, Is.SameAs(endpoint));
            target.Originator = null;
            Assert.That(target.Originator, Is.Null);
        }

        [Test()]
        public void CanAddRemoveDescriptionItems()
        {
            var description = new Description("some desc");

            var mock = new Mock<BaseFaultFull>();
            mock.CallBase = true;
            var target = mock.Object;

            target.Descriptions.Add(description);
            Assert.That(target.Descriptions.Count, Is.EqualTo(1));
            Assert.That(target.Descriptions.First(), Is.SameAs(description));

            target.Descriptions.Add(description);
            Assert.That(target.Descriptions.Count, Is.EqualTo(2));
            Assert.That(target.Descriptions.First(), Is.SameAs(target.Descriptions.Last()));

            target.Descriptions.Remove(description);
            Assert.That(target.Descriptions.Count, Is.EqualTo(1));
        }

        [Test()]
        public void CantAddNullDescription()
        {
            var mock = new Mock<BaseFaultFull>();
            mock.CallBase = true;
            var target = mock.Object;

            var exception = Assert.Throws<InvalidOperationException>(() => target.Descriptions.Add(null));
            Assert.That(exception.Message, Is.EqualTo("A null Description cannot be added"));
        }

        [Test()]
        [Description("Confirms that serialization logic works as expected")]
        [Category("Functional Tests")]
        public void Serializable()
        {
            var endpoint = new EndpointAddress("http://someUri");
            var description = new Description("some desc");
            var errorCode = new ErrorCode(new Uri("http://foo"));
            var now = new DateTime(2001, 1, 2, 3, 4, 5, DateTimeKind.Utc);
            //var faultCause = (new Mock<BaseFaultFull>()).Object;

            var mock = new Mock<BaseFaultFull>(now, endpoint, errorCode, new[] { description });
            mock.CallBase = true;

            var target = mock.Object;

            var serializer = new XmlSerializer(typeof(BaseFaultFull));

            XElement xml;

            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, target);
                stream.Position = 0;

                xml = XElement.Load(stream);
            }

            var areEqual = XNode.DeepEquals(XElement.Parse("<wsbf:BaseFault xmlns:wsbf='http://docs.oasis-open.org/wsrf/bf-2'><wsbf:Timestamp>2001-01-02T03:04:05Z</wsbf:Timestamp><wsbf:Originator xmlns:wsa='http://www.w3.org/2005/08/addressing' xsi:type='http://www.w3.org/2005/08/addressing:EndpointReference' xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'><wsa:Address>http://someuri/</wsa:Address></wsbf:Originator><wsbf:ErrorCode dialect='http://foo/' /><wsbf:Description>some desc</wsbf:Description></wsbf:BaseFault>"), xml.FirstNode);
            Assert.IsTrue(areEqual);
        }

        [Test()]
        [Description("Specifically showing the use case for nested faults")]
        [Category("Functional Tests")]
        public void NestedFaultsShouldSerialize()
        {
            var now = new DateTime(2001, 1, 2, 3, 4, 5, DateTimeKind.Utc);

            var mock = new Mock<BaseFaultFull>();
            mock.CallBase = true;
            mock.SetupGet(item => item.Timestamp).Returns(now);
            var innerFault = mock.Object;

            var target = new TestFault(now) {FaultCause = innerFault};
            var serializer = new XmlSerializer(typeof(BaseFaultFull));

            XElement xml;

            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, target);
                stream.Position = 0;

                xml = XElement.Load(stream);
            }
            
            var areEqual = XNode.DeepEquals(XElement.Parse("<wsbf:BaseFault xmlns:wsbf='http://docs.oasis-open.org/wsrf/bf-2'><wsbf:Timestamp>2001-01-02T03:04:05Z</wsbf:Timestamp><wsbf:FaultCause><wsbf:BaseFault><wsbf:Timestamp>2001-01-02T03:04:05Z</wsbf:Timestamp></wsbf:BaseFault></wsbf:FaultCause></wsbf:BaseFault>"), xml.FirstNode);
            Assert.IsTrue(areEqual);
        }

        [Test()]
        [Description("Confirms that deserialization logic works as expected")]
        [Category("Functional Tests")]
        public void Deserialize()
        {
            const String xml = "<wsbf:BaseFault xmlns:wsbf='http://docs.oasis-open.org/wsrf/bf-2'><wsbf:Timestamp>2001-01-02T03:04:05Z</wsbf:Timestamp><wsbf:Originator xmlns:wsa='http://www.w3.org/2005/08/addressing' xsi:type='http://www.w3.org/2005/08/addressing:EndpointReference' xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'><wsa:Address>http://someuri/</wsa:Address></wsbf:Originator><wsbf:ErrorCode dialect='http://foo/'/><wsbf:Description>some desc</wsbf:Description></wsbf:BaseFault>";

            var reader = new XmlTextReader(new StringReader(xml));

            var serializer = new XmlSerializer(typeof(TestFault));
            var target = (BaseFaultFull)serializer.Deserialize(reader);

            Assert.That(target, Is.Not.Null);
            Assert.That(target.Descriptions.Count, Is.EqualTo(1));
            Assert.That(target.FaultCause, Is.Null);
            Assert.That(target.Originator.ToEndpointAddress(), Is.EqualTo(new EndpointAddress("http://someuri/")));
            Assert.That(target.Timestamp, Is.EqualTo(new DateTime(2001, 1, 2, 3, 4, 5, DateTimeKind.Utc)));
        }

        [Test()]
        [Description("Specifically showing the use case for nested faults")]
        [Category("Functional Tests")]
        public void NestedFaultsShouldDeserialize()
        {
            var now = new DateTime(2001, 1, 2, 3, 4, 5, DateTimeKind.Utc);

            const String xml = "<wsbf:BaseFault xmlns:wsbf='http://docs.oasis-open.org/wsrf/bf-2'><wsbf:Timestamp>2001-01-02T03:04:05Z</wsbf:Timestamp><wsbf:FaultCause><wsbf:BaseFault><wsbf:Timestamp>2001-01-02T03:04:05Z</wsbf:Timestamp></wsbf:BaseFault></wsbf:FaultCause></wsbf:BaseFault>";
            var reader = new XmlTextReader(new StringReader(xml));
            var serializer = new XmlSerializer(typeof(TestFault));
            var target = (BaseFaultFull)serializer.Deserialize(reader);

            Assert.That(target, Is.Not.Null);
            Assert.That(target.FaultCause, Is.Not.Null);
            Assert.That(target.FaultCause.Timestamp, Is.EqualTo(now));
            Assert.That(target.FaultCause, Is.TypeOf<UnknownBaseFault>());
        }

        [Test()]
        [Description("Confirms that we use the UnknownBaseFault type by default when deserializing FaultCause elements")]
        [Category("Functional Tests")]
        public void DeserializedNestedFaultsShouldDeserializeAsUnknownBaseFault()
        {
            const String xml = "<wsbf:BaseFault xmlns:wsbf='http://docs.oasis-open.org/wsrf/bf-2'><wsbf:Timestamp>2001-01-02T03:04:05Z</wsbf:Timestamp><wsbf:FaultCause><wsbf:BaseFault><wsbf:Timestamp>2001-01-02T03:04:05Z</wsbf:Timestamp></wsbf:BaseFault></wsbf:FaultCause></wsbf:BaseFault>";
            var reader = new XmlTextReader(new StringReader(xml));
            var serializer = new XmlSerializer(typeof(TestFault));
            var target = (BaseFaultFull)serializer.Deserialize(reader);

            Assert.That(target.FaultCause, Is.TypeOf<UnknownBaseFault>());
        }
    }
}
