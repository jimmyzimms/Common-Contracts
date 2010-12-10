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
using System.ServiceModel.Channels;
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

            var areEqual = XNode.DeepEquals(XElement.Parse("<wse:Delivery xmlns:wse='http://schemas.xmlsoap.org/ws/2004/08/eventing'><wse:NotifyTo><Address xmlns='http://www.w3.org/2005/08/addressing'>http://tempuri.org/</Address></wse:NotifyTo></wse:Delivery>"), withoutHeaders.FirstNode);
            Assert.IsTrue(areEqual);

            // Now we'll confirm that the custom headers are created
            delivery.Add(AddressHeader.CreateAddressHeader("testElement", "urn:unittests", "value"));
            XElement withHeaders;
            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, new TestXmlWrapper<Delivery> { Item = delivery });
                stream.Position = 0;
                withHeaders = XElement.Load(stream);
            }
            areEqual = XNode.DeepEquals(XElement.Parse("<wse:Delivery xmlns:wse='http://schemas.xmlsoap.org/ws/2004/08/eventing'><wse:NotifyTo><Address xmlns='http://www.w3.org/2005/08/addressing'>http://tempuri.org/</Address></wse:NotifyTo><testElement xmlns='urn:unittests'>value</testElement></wse:Delivery>"), withHeaders.FirstNode);
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
            areEqual = XNode.DeepEquals(XElement.Parse("<wse:Delivery wse:Mode='http://schemas.xmlsoap.org/ws/2004/08/eventing/DeliveryModes/Wrap' xmlns:wse='http://schemas.xmlsoap.org/ws/2004/08/eventing'><wse:NotifyTo><Address xmlns='http://www.w3.org/2005/08/addressing'>http://tempuri.org/</Address></wse:NotifyTo><testElement xmlns='urn:unittests'>value</testElement></wse:Delivery>"), withCustomDeliveryMode.FirstNode);
            Assert.IsTrue(areEqual);
        }

        [Test()]
        public void Deserialize()
        {
            var serializer = new XmlSerializer(typeof(Delivery));

            var xml = XElement.Parse("<wse:Delivery xmlns:wse='http://schemas.xmlsoap.org/ws/2004/08/eventing'><wse:NotifyTo><Address xmlns='http://www.w3.org/2005/08/addressing'>http://tempuri.org/</Address></wse:NotifyTo></wse:Delivery>");
            Delivery delivery = (Delivery)serializer.Deserialize(xml.CreateReader());
            Assert.That(delivery.DeliveryMode, Is.EqualTo(new Uri(Constants.WsEventing.DeliverModes.Push)));
            Assert.IsTrue(delivery.NotifyTo.Equals(new EndpointAddress("http://tempuri.org")));
            Assert.That(delivery, Is.Empty);

            xml = XElement.Parse("<wse:Delivery xmlns:wse='http://schemas.xmlsoap.org/ws/2004/08/eventing'><wse:NotifyTo><Address xmlns='http://www.w3.org/2005/08/addressing'>http://tempuri.org/</Address></wse:NotifyTo><testElement xmlns='urn:unittests'>value</testElement></wse:Delivery>");
            delivery = (Delivery)serializer.Deserialize(xml.CreateReader());
            Assert.That(delivery.Select(header => header.Namespace + ":" + header.Name).ToList(), Is.EquivalentTo(new[] { "urn:unittests:testElement" }));

            xml = XElement.Parse("<wse:Delivery wse:Mode='http://schemas.xmlsoap.org/ws/2004/08/eventing/DeliveryModes/Wrap' xmlns:wse='http://schemas.xmlsoap.org/ws/2004/08/eventing'><wse:NotifyTo><Address xmlns='http://www.w3.org/2005/08/addressing'>http://tempuri.org/</Address></wse:NotifyTo><testElement xmlns='urn:unittests'>value</testElement></wse:Delivery>");
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

            Assert.That(schemas.Count, Is.EqualTo(2));
            Assert.That(schemas.Schemas().Cast<XmlSchema>().Select(schema => schema.TargetNamespace).ToList(), Is.EquivalentTo(new[] { "http://www.w3.org/XML/1998/namespace", "http://schemas.xmlsoap.org/ws/2004/08/eventing" }));
        }
    }
}
