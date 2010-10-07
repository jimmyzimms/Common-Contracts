using System;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Xml;
using System.Xml.Linq;
using NUnit.Framework;

namespace CommonContracts.WsBaseFaults.Tests
{
    [TestFixture()]
    public class UnknownBaseFaultTests
    {
        [Test()]
        [Description("Confirms that the parameter is checked for null")]
        public void ConstructorRequiresReader()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => new UnknownBaseFault(null));

            Assert.That(exception.Message, Is.EqualTo("Precondition failed: reader != null  reader\r\nParameter name: reader"));
            Assert.That(exception.ParamName, Is.EqualTo("reader"));
        }

        [Test()]
        [Description("Confirms that the supplied XmlReader must be interactive")]
        public void ConstructorRequiresReaderInInteractiveState()
        {
            // Note: This xml is not valid according to the schema but it is not an issue as we're testing the reader here.
            const String xml = "<wsbf:BaseFault xmlns:wsbf='http://docs.oasis-open.org/wsrf/bf-2'/>";
            var reader = new XmlTextReader(new StringReader(xml));

            Assert.That(reader.ReadState, Is.EqualTo(ReadState.Initial)); // Sanity check
            var exception = Assert.Throws<ArgumentException>(() => new UnknownBaseFault(reader));

            Assert.That(exception.Message, Is.EqualTo("Precondition failed: reader.ReadState == ReadState.Interactive  reader\r\nParameter name: reader"));
            Assert.That(exception.ParamName, Is.EqualTo("reader"));
        }

        [Test()]
        [Description("Confirms that the XmlReader can be used to initialize the current fault")]
        public void ConstructorCanProcessReader()
        {
            // Here's just the regular old XML
            var xml = "<wsbf:BaseFault xmlns:wsbf='http://docs.oasis-open.org/wsrf/bf-2'><wsbf:Timestamp>2001-01-02T03:04:05Z</wsbf:Timestamp><wsbf:Originator xmlns:wsa='http://www.w3.org/2005/08/addressing' xsi:type='http://www.w3.org/2005/08/addressing:EndpointReference' xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'><wsa:Address>http://someuri/</wsa:Address></wsbf:Originator><wsbf:ErrorCode dialect='http://foo/'/><wsbf:Description>some desc</wsbf:Description></wsbf:BaseFault>";
            var reader = new XmlTextReader(new StringReader(xml));
            reader.Read();

            // Sanity checks for base behaviors
            var target = new UnknownBaseFault(reader);
            Assert.That(target, Is.Not.Null);
            Assert.That(target.Descriptions.Count, Is.EqualTo(1));
            Assert.That(target.FaultCause, Is.Null);
            Assert.That(target.Originator.ToEndpointAddress(), Is.EqualTo(new EndpointAddress("http://someuri/")));
            Assert.That(target.Timestamp, Is.EqualTo(new DateTime(2001, 1, 2, 3, 4, 5, DateTimeKind.Utc)));

            // test the new properties
            Assert.That(target.NamespaceUri, Is.EqualTo("http://docs.oasis-open.org/wsrf/bf-2"));
            Assert.That(target.LocalName, Is.EqualTo("BaseFault"));
            Assert.That(target.XmlType, Is.Empty);
            Assert.That(target.AdditionalContent, Is.Empty);
            Assert.That(target.AdditionalAttributes, Is.Empty);

            // Here's for some new element of wsbf:BaseFaultType
            xml = "<MyNewFault xmlns:wsbf='http://docs.oasis-open.org/wsrf/bf-2'><wsbf:Timestamp>2001-01-02T03:04:05Z</wsbf:Timestamp><wsbf:Originator xmlns:wsa='http://www.w3.org/2005/08/addressing' xsi:type='http://www.w3.org/2005/08/addressing:EndpointReference' xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'><wsa:Address>http://someuri/</wsa:Address></wsbf:Originator><wsbf:ErrorCode dialect='http://foo/'/><wsbf:Description>some desc</wsbf:Description></MyNewFault>";
            reader = new XmlTextReader(new StringReader(xml));
            reader.Read();
            target = new UnknownBaseFault(reader);

            // test the new properties
            Assert.That(target.NamespaceUri, Is.Empty);
            Assert.That(target.LocalName, Is.EqualTo("MyNewFault"));
            Assert.That(target.XmlType, Is.Empty);
            Assert.That(target.AdditionalContent, Is.Empty);
            Assert.That(target.AdditionalAttributes, Is.Empty);

            // Here's for some new element of wsbf:BaseFaultType with additional XML content
            xml = "<MyNewFault xmlns:wsbf='http://docs.oasis-open.org/wsrf/bf-2'><wsbf:Timestamp>2001-01-02T03:04:05Z</wsbf:Timestamp><wsbf:Originator xmlns:wsa='http://www.w3.org/2005/08/addressing' xsi:type='http://www.w3.org/2005/08/addressing:EndpointReference' xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'><wsa:Address>http://someuri/</wsa:Address></wsbf:Originator><wsbf:ErrorCode dialect='http://foo/'/><wsbf:Description>some desc</wsbf:Description><AdditionalType>With Value</AdditionalType></MyNewFault>";
            reader = new XmlTextReader(new StringReader(xml));
            reader.Read();
            target = new UnknownBaseFault(reader);

            // test the new properties
            Assert.That(target.NamespaceUri, Is.Empty);
            Assert.That(target.LocalName, Is.EqualTo("MyNewFault"));
            Assert.That(target.XmlType, Is.Empty);
            Assert.That(target.AdditionalContent.Count(), Is.EqualTo(1));
            Assert.That(target.AdditionalAttributes, Is.Empty);
            Assert.IsTrue(XNode.DeepEquals(target.AdditionalContent.First(), XElement.Parse("<AdditionalType>With Value</AdditionalType>")));

            // Here's for some new extension of wsbf:BaseFault element with additional XML content including attribute
            xml = "<wsbf:BaseFault test='a' xsi:type='urn:foo' xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:wsbf='http://docs.oasis-open.org/wsrf/bf-2'><wsbf:Timestamp>2001-01-02T03:04:05Z</wsbf:Timestamp><wsbf:Originator xmlns:wsa='http://www.w3.org/2005/08/addressing' xsi:type='http://www.w3.org/2005/08/addressing:EndpointReference' xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'><wsa:Address>http://someuri/</wsa:Address></wsbf:Originator><wsbf:ErrorCode dialect='http://foo/'/><wsbf:Description>some desc</wsbf:Description><AdditionalType>With Value</AdditionalType></wsbf:BaseFault>";
            reader = new XmlTextReader(new StringReader(xml));
            reader.Read();
            target = new UnknownBaseFault(reader);

            // test the new properties
            Assert.That(target.NamespaceUri, Is.EqualTo("http://docs.oasis-open.org/wsrf/bf-2"));
            Assert.That(target.LocalName, Is.EqualTo("BaseFault"));
            Assert.That(target.XmlType, Is.EqualTo("urn:foo"));
            Assert.That(target.AdditionalContent.Count(), Is.EqualTo(1));
            Assert.That(target.AdditionalAttributes.Count(), Is.EqualTo(1));
            Assert.That(target.AdditionalAttributes.First().ToString(), Is.EqualTo(new XAttribute(XName.Get("test"), "a").ToString()));
            Assert.IsTrue(XNode.DeepEquals(target.AdditionalContent.First(), XElement.Parse("<AdditionalType>With Value</AdditionalType>")));
        }
    }
}
