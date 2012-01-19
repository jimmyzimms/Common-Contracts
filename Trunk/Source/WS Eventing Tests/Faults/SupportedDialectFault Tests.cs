using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using CommonContracts.WsEventing.Faults;
using NUnit.Framework;

namespace CommonContracts.WsEventing.Tests.Faults
{
    [TestFixture()]
    public class SupportedDialectFaultTests
    {
        [Test()]
        public void CanCreateCorrectFaultReason()
        {
            var reason = SupportedDialectFault.CreateFaultReason();
            Assert.That(reason.ToString(), Is.EqualTo("The requested filter dialect is not supported."));
        }

        [Test()]
        public void CanCreateCorrectFaultCode()
        {
            var code = SupportedDialectFault.CreateFaultCode();
            Assert.That(code.IsPredefinedFault, Is.True);
            Assert.That(code.IsReceiverFault, Is.False);
            Assert.That(code.IsSenderFault, Is.True);

            code = code.SubCode;
            Assert.That(code.Name, Is.EqualTo("FilteringRequestedUnavailable"));
            Assert.That(code.Namespace, Is.EqualTo(Constants.WsEventing.Namespace));
        }

        [Test()]
        public void Serialize()
        {
            var serializer = new XmlSerializer(typeof(TestXmlWrapper<SupportedDialectFault>));

            var fault = new SupportedDialectFault("1", "2");
            SupportedDeliveryModeFault.AcquireSchema(new XmlSchemaSet());
            XElement result;
            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, new TestXmlWrapper<SupportedDialectFault> { Item = fault }, new XmlSerializerNamespaces());
                stream.Position = 0;
                result = XElement.Load(stream);
            }

            var xml = result.Elements().First();
            var equal = XNode.DeepEquals(xml, XElement.Parse("<wse:SupportedDialect xmlns:wse='http://schemas.xmlsoap.org/ws/2004/08/eventing'>1</wse:SupportedDialect>"));
            Assert.IsTrue(equal);

            xml = result.Elements().Last();
            equal = XNode.DeepEquals(xml, XElement.Parse("<wse:SupportedDialect xmlns:wse='http://schemas.xmlsoap.org/ws/2004/08/eventing'>2</wse:SupportedDialect>"));
            Assert.IsTrue(equal);
        }

        [Test()]
        public void Deserialize()
        {
            var serializer = new XmlSerializer(typeof(SupportedDialectFault));
            var xml = XElement.Parse("<wse:SupportedDialect xmlns:wse='http://schemas.xmlsoap.org/ws/2004/08/eventing'/>");
            var fault = (SupportedDialectFault)serializer.Deserialize(xml.CreateReader());
            Assert.That(fault.Dialects, Is.Empty);

            xml = XElement.Parse("<wse:SupportedDialect xmlns:wse='http://schemas.xmlsoap.org/ws/2004/08/eventing'>1</wse:SupportedDialect>");
            fault = (SupportedDialectFault)serializer.Deserialize(xml.CreateReader());
            Assert.That(fault.Dialects.Count, Is.EqualTo(1));
            Assert.That(fault.Dialects.First(), Is.EqualTo("1"));

            var reader = new XmlTextReader("<wse:SupportedDialect xmlns:wse='http://schemas.xmlsoap.org/ws/2004/08/eventing'>1</wse:SupportedDialect><wse:SupportedDialect xmlns:wse='http://schemas.xmlsoap.org/ws/2004/08/eventing'>2</wse:SupportedDialect>", XmlNodeType.Element, null);
            fault = (SupportedDialectFault)serializer.Deserialize(reader);
            Assert.That(fault.Dialects.Count, Is.EqualTo(2));
            Assert.That(fault.Dialects.First(), Is.EqualTo("1"));
            Assert.That(fault.Dialects.Last(), Is.EqualTo("2"));
        }
    }
}
