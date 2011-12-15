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
using System.Linq;
using System.ServiceModel;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using NUnit.Framework;

namespace CommonContracts.WsEventing.Tests
{
    [TestFixture()]
    public class DeliveryTests
    {
        [Test()]
        public void ConstructorShouldSetExpectedValues()
        {
            var delivery = new Delivery();
            delivery.DeliveryMode = new Uri(Constants.WsEventing.DeliverModes.Push);
            Assert.That(delivery.Extensions, Is.Not.Null);
            Assert.That(delivery.Extensions, Is.Empty);
            Assert.That(delivery.NotifyTo.ToEndpointAddress(), Is.EqualTo(new EndpointAddress(EndpointAddress.NoneUri)));

            delivery = new Delivery(new Uri(Constants.WsEventing.DeliverModes.Wrapped), new EndpointAddress("http://someaddress"));
            delivery.DeliveryMode = new Uri(Constants.WsEventing.DeliverModes.Wrapped);
            Assert.That(delivery.Extensions, Is.Not.Null);
            Assert.That(delivery.Extensions, Is.Empty);
            Assert.That(delivery.NotifyTo.ToEndpointAddress(), Is.EqualTo(new EndpointAddress("http://someaddress")));
        }

        [Test()]
        public void CanAddRemoveExtensionElements()
        {
            var delivery = new Delivery(new Uri(Constants.WsEventing.DeliverModes.Wrapped), new EndpointAddress("http://someaddress"));
            delivery.Extensions.Add(XElement.Parse("<name xmlns='namespace'>value</name>"));
            Assert.That(delivery.Extensions.Count, Is.EqualTo(1));
            Assert.That(delivery.Extensions.First().Name.LocalName, Is.EqualTo("name"));
            Assert.That(delivery.Extensions.First().Name.NamespaceName, Is.EqualTo("namespace"));
            Assert.That(delivery.Extensions.First().Value, Is.EqualTo("value"));

            delivery.Extensions.RemoveAt(0);
            Assert.That(delivery.Extensions, Is.Empty);
        }

        [Test()]
        public void ConstructorShouldRequireMode()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => new Delivery(null, new EndpointAddress("http://someaddress")));

            Assert.That(exception.Message, Is.EqualTo("Precondition failed: mode != null  mode\r\nParameter name: mode"));
            Assert.That(exception.ParamName, Is.EqualTo("mode"));
        }

        [Test()]
        public void ConstructorShouldRequireEndpoint()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => new Delivery(new Uri(Constants.WsEventing.DeliverModes.Push), null));

            Assert.That(exception.Message, Is.EqualTo("Precondition failed: notifyTo != null  notifyTo\r\nParameter name: notifyTo"));
            Assert.That(exception.ParamName, Is.EqualTo("notifyTo"));
        }

        [Test()]
        public void CanSetMode()
        {
            var newValue = new Uri("http://someuri");
            
            var delivery = new Delivery();
            delivery.DeliveryMode = newValue;
            Assert.That(delivery.DeliveryMode, Is.SameAs(newValue));
        }

        [Test()]
        public void ModeShouldDefaultToPush()
        {
            var expected = new Uri(Constants.WsEventing.DeliverModes.Push);

            var delivery = new Delivery();
            delivery.DeliveryMode = new Uri(Constants.WsEventing.DeliverModes.Wrapped);
            delivery.DeliveryMode = null;
            Assert.That(delivery.DeliveryMode, Is.EqualTo(expected));
        }

        [Test()]
        public void NotifyToShouldBeRequired()
        {
            var delivery = new Delivery();
            var exception = Assert.Throws<ArgumentNullException>(() => delivery.NotifyTo = null);

            Assert.That(exception.Message, Is.EqualTo("Precondition failed: value != null  NotifyTo\r\nParameter name: NotifyTo"));
            Assert.That(exception.ParamName, Is.EqualTo("NotifyTo"));
        }

        [Test()]
        public void Serialize()
        {
            var serializer = new XmlSerializer(typeof(TestXmlWrapper<Delivery>));

            // First test the basic serialization
            var delivery = new Delivery(new Uri(Constants.WsEventing.DeliverModes.Push), new EndpointAddress("http://tempuri.org"));
            XElement withoutHeaders;
            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, new TestXmlWrapper<Delivery> { Item = delivery }, new XmlSerializerNamespaces());
                stream.Position = 0;
                withoutHeaders = XElement.Load(stream);
            }

            var areEqual = XNode.DeepEquals(XElement.Parse("<wse:Delivery xmlns:wse='http://schemas.xmlsoap.org/ws/2004/08/eventing'><wse:NotifyTo><Address xmlns='http://schemas.xmlsoap.org/ws/2004/08/addressing'>http://tempuri.org/</Address></wse:NotifyTo></wse:Delivery>"), withoutHeaders.FirstNode);
            Assert.IsTrue(areEqual);

            // Now we'll confirm that the custom headers are created
            delivery.Extensions.Add(XElement.Parse("<testElement xmlns='urn:unittests'>value</testElement>"));
            XElement withHeaders;
            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, new TestXmlWrapper<Delivery> { Item = delivery });
                stream.Position = 0;
                withHeaders = XElement.Load(stream);
            }
            areEqual = XNode.DeepEquals(XElement.Parse("<wse:Delivery xmlns:wse='http://schemas.xmlsoap.org/ws/2004/08/eventing'><wse:NotifyTo><Address xmlns='http://schemas.xmlsoap.org/ws/2004/08/addressing'>http://tempuri.org/</Address></wse:NotifyTo><testElement xmlns='urn:unittests'>value</testElement></wse:Delivery>"), withHeaders.FirstNode);
            Assert.IsTrue(areEqual);

            // Last the use of custom Push types are correctly serialized
            delivery.DeliveryMode = new Uri(Constants.WsEventing.DeliverModes.Wrapped);
            XElement withCustomDeliveryMode;
            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, new TestXmlWrapper<Delivery> { Item = delivery });
                stream.Position = 0;
                withCustomDeliveryMode = XElement.Load(stream);
            }
            areEqual = XNode.DeepEquals(XElement.Parse("<wse:Delivery wse:Mode='http://schemas.xmlsoap.org/ws/2004/08/eventing/DeliveryModes/Wrap' xmlns:wse='http://schemas.xmlsoap.org/ws/2004/08/eventing'><wse:NotifyTo><Address xmlns='http://schemas.xmlsoap.org/ws/2004/08/addressing'>http://tempuri.org/</Address></wse:NotifyTo><testElement xmlns='urn:unittests'>value</testElement></wse:Delivery>"), withCustomDeliveryMode.FirstNode);
            Assert.IsTrue(areEqual);
        }

        [Test()]
        public void Deserialize()
        {
            var serializer = new XmlSerializer(typeof(Delivery));

            var xml = XElement.Parse("<wse:Delivery xmlns:wse='http://schemas.xmlsoap.org/ws/2004/08/eventing'><wse:NotifyTo><Address xmlns='http://schemas.xmlsoap.org/ws/2004/08/addressing'>http://tempuri.org/</Address></wse:NotifyTo></wse:Delivery>");
            Delivery delivery = (Delivery)serializer.Deserialize(xml.CreateReader());
            Assert.That(delivery.DeliveryMode, Is.EqualTo(new Uri(Constants.WsEventing.DeliverModes.Push)));
            Assert.That(delivery.NotifyTo.ToEndpointAddress(), Is.EqualTo(new EndpointAddress("http://tempuri.org")));
            Assert.That(delivery.Extensions, Is.Empty);

            xml = XElement.Parse("<wse:Delivery xmlns:wse='http://schemas.xmlsoap.org/ws/2004/08/eventing'><wse:NotifyTo><Address xmlns='http://schemas.xmlsoap.org/ws/2004/08/addressing'>http://tempuri.org/</Address></wse:NotifyTo><testElement xmlns='urn:unittests'>value</testElement></wse:Delivery>");
            delivery = (Delivery)serializer.Deserialize(xml.CreateReader());
            Assert.That(delivery.Extensions.Select(header => header.Name.NamespaceName + ":" + header.Name.LocalName).ToList(), Is.EquivalentTo(new[] { "urn:unittests:testElement" }));

            xml = XElement.Parse("<wse:Delivery wse:Mode='http://schemas.xmlsoap.org/ws/2004/08/eventing/DeliveryModes/Wrap' xmlns:wse='http://schemas.xmlsoap.org/ws/2004/08/eventing'><wse:NotifyTo><Address xmlns='http://schemas.xmlsoap.org/ws/2004/08/addressing'>http://tempuri.org/</Address></wse:NotifyTo><testElement xmlns='urn:unittests'>value</testElement></wse:Delivery>");
            delivery = (Delivery)serializer.Deserialize(xml.CreateReader());
            Assert.That(delivery.DeliveryMode, Is.EqualTo(new Uri(Constants.WsEventing.DeliverModes.Wrapped)));
        }

        [Test()]
        public void AcquireSchemaShouldLoadSchemas()
        {
            var schemas = new XmlSchemaSet();
            var qName = Delivery.AcquireSchema(schemas);

            Assert.That(qName.Name, Is.EqualTo("DeliveryType"));
            Assert.That(qName.Namespace, Is.EqualTo(Constants.WsEventing.Namespace));

            Assert.That(schemas.Count, Is.EqualTo(3));
            Assert.That(schemas.Schemas().Cast<XmlSchema>().Select(schema => schema.TargetNamespace).ToList(), Is.EquivalentTo(new[] { "http://www.w3.org/XML/1998/namespace", "http://schemas.xmlsoap.org/ws/2004/08/eventing", "http://schemas.xmlsoap.org/ws/2004/08/addressing" }));
        }
    }
}
