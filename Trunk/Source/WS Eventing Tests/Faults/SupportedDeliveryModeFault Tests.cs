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
    public class SupportedDeliveryModeFaultTests
    {
        [Test()]
        public void CanCreateCorrectFaultReason()
        {
            var reason = SupportedDeliveryModeFault.CreateFaultReason();
            Assert.That(reason.ToString(), Is.EqualTo("The requested delivery mode is not supported."));
        }

        [Test()]
        public void CanCreateCorrectFaultCode()
        {
            var code = SupportedDeliveryModeFault.CreateFaultCode();
            Assert.That(code.IsPredefinedFault, Is.True);
            Assert.That(code.IsReceiverFault, Is.False);
            Assert.That(code.IsSenderFault, Is.True);

            code = code.SubCode;
            Assert.That(code.Name, Is.EqualTo("DeliveryModeRequestedUnvailable"));
            Assert.That(code.Namespace, Is.EqualTo(Constants.WsEventing.Namespace));
        }

        [Test()]
        public void Serialize()
        {
            var serializer = new XmlSerializer(typeof(TestXmlWrapper<SupportedDeliveryModeFault>));

            var fault = new SupportedDeliveryModeFault("1", "2");
            SupportedDeliveryModeFault.AcquireSchema(new XmlSchemaSet());
            XElement result;
            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, new TestXmlWrapper<SupportedDeliveryModeFault>{ Item = fault }, new XmlSerializerNamespaces());
                stream.Position = 0;
                result = XElement.Load(stream);
            }

            var xml = result.Elements().First();
            var equal = XNode.DeepEquals(xml, XElement.Parse("<wse:SupportedDeliveryMode xmlns:wse='http://schemas.xmlsoap.org/ws/2004/08/eventing'>1</wse:SupportedDeliveryMode>"));
            Assert.IsTrue(equal);

            xml = result.Elements().Last();
            equal = XNode.DeepEquals(xml, XElement.Parse("<wse:SupportedDeliveryMode xmlns:wse='http://schemas.xmlsoap.org/ws/2004/08/eventing'>2</wse:SupportedDeliveryMode>"));
            Assert.IsTrue(equal);
        }

        [Test()]
        public void Deserialize()
        {
            var serializer = new XmlSerializer(typeof(SupportedDeliveryModeFault));
            var xml = XElement.Parse("<wse:SupportedDeliveryMode xmlns:wse='http://schemas.xmlsoap.org/ws/2004/08/eventing'/>");
            var fault = (SupportedDeliveryModeFault)serializer.Deserialize(xml.CreateReader());
            Assert.That(fault.Modes, Is.Empty);

            xml = XElement.Parse("<wse:SupportedDeliveryMode xmlns:wse='http://schemas.xmlsoap.org/ws/2004/08/eventing'>1</wse:SupportedDeliveryMode>");
            fault = (SupportedDeliveryModeFault)serializer.Deserialize(xml.CreateReader());
            Assert.That(fault.Modes.Count, Is.EqualTo(1));
            Assert.That(fault.Modes.First(), Is.EqualTo("1"));

            var reader = new XmlTextReader("<wse:SupportedDeliveryMode xmlns:wse='http://schemas.xmlsoap.org/ws/2004/08/eventing'>1</wse:SupportedDeliveryMode><wse:SupportedDeliveryMode xmlns:wse='http://schemas.xmlsoap.org/ws/2004/08/eventing'>2</wse:SupportedDeliveryMode>", XmlNodeType.Element, null);
            fault = (SupportedDeliveryModeFault) serializer.Deserialize(reader);
            Assert.That(fault.Modes.Count, Is.EqualTo(2));
            Assert.That(fault.Modes.First(), Is.EqualTo("1"));
            Assert.That(fault.Modes.Last(), Is.EqualTo("2"));
        }
    }
}
