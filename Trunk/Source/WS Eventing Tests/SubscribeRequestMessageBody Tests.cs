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

using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Dispatcher;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using Moq;
using NUnit.Framework;

namespace CommonContracts.WsEventing.Tests
{
    [TestFixture()]
    public class SubscribeRequestMessageBodyTests
    {
        [Test()]
        public void Serialize()
        {
            var serializer = new XmlSerializer(typeof(SubscribeRequestMessageBody));

            var message = new SubscribeRequestMessageBody();
            // Supply mock Delivery
            var delivery = new Mock<Delivery>(MockBehavior.Strict);
            delivery.As<IXmlSerializable>().Setup(item => item.WriteXml(It.IsAny<XmlWriter>()));
            message.Delivery = delivery.Object;
            message.EndTo = new EndpointAddress("http://tempuri.org/endTo");
            // Supply mock Expires
            var expires = new Mock<Expires>(MockBehavior.Strict);
            expires.As<IXmlSerializable>().Setup(item => item.WriteXml(It.IsAny<XmlWriter>()));
            message.Expires = expires.Object;
            message.Filter = new XPathMessageFilter(@"/foo");

            XElement xml;
            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, message);
                stream.Position = 0;
                xml = XElement.Load(stream);
            }
            var areEqual = XNode.DeepEquals(XElement.Parse("<wse:Subscribe xmlns:wse='http://schemas.xmlsoap.org/ws/2004/08/eventing'><wse:EndTo><Address xmlns='http://www.w3.org/2005/08/addressing'>http://tempuri.org/endTo</Address></wse:EndTo><wse:Filter>/foo</wse:Filter></wse:Subscribe>"), xml.FirstNode);
            Assert.IsTrue(areEqual);
        }

        [Test()]
        public void Deserialize()
        {
            var serializer = new XmlSerializer(typeof(SubscribeRequestMessageBody));

            var xml = XElement.Parse("<wse:Subscribe xmlns:wse='http://schemas.xmlsoap.org/ws/2004/08/eventing'><wse:EndTo><Address xmlns='http://www.w3.org/2005/08/addressing'>http://tempuri.org/endTo</Address></wse:EndTo><wse:Delivery><wse:NotifyTo><Address xmlns='http://www.w3.org/2005/08/addressing'>http://tempuri.org/</Address></wse:NotifyTo></wse:Delivery><wse:Expires>2010-08-23T00:00:00Z</wse:Expires><wse:Filter>/foo</wse:Filter></wse:Subscribe>");
            SubscribeRequestMessageBody body = (SubscribeRequestMessageBody)serializer.Deserialize(xml.CreateReader());
            Assert.That(body.Delivery, Is.Not.Null);
            Assert.That(body.Expires, Is.Not.Null);
            Assert.That(body.Filter.XPath, Is.EqualTo(@"/foo"));
            Assert.That(body.FilterDialect, Is.EqualTo(Constants.WsEventing.Dialects.XPath));
        }

        [Test()]
        public void UnsupportedFilterDialectShouldSetFilterToNull()
        {
            var serializer = new XmlSerializer(typeof(SubscribeRequestMessageBody));

            var xml = XElement.Parse("<wse:Subscribe xmlns:wse='http://schemas.xmlsoap.org/ws/2004/08/eventing'><wse:EndTo><Address xmlns='http://www.w3.org/2005/08/addressing'>http://tempuri.org/endTo</Address></wse:EndTo><wse:Delivery><wse:NotifyTo><Address xmlns='http://www.w3.org/2005/08/addressing'>http://tempuri.org/</Address></wse:NotifyTo></wse:Delivery><wse:Expires>2010-08-23T00:00:00Z</wse:Expires><wse:Filter Dialect='urn:fakeDialect'>/foo</wse:Filter></wse:Subscribe>");
            var body = (SubscribeRequestMessageBody)serializer.Deserialize(xml.CreateReader());
            Assert.That(body.Filter, Is.Null);
            Assert.That(body.FilterDialect, Is.EqualTo("urn:fakeDialect"));
        }

        [Test()]
        public void AcquireSchemaShouldLoadSchemas()
        {
            var schemas = new XmlSchemaSet();
            var qName = SubscribeRequestMessageBody.AcquireSchema(schemas);

            Assert.That(qName.Name, Is.EqualTo("SubscribeType"));
            Assert.That(qName.Namespace, Is.EqualTo(Constants.WsEventing.Namespace));

            Assert.That(schemas.Count, Is.EqualTo(3));
            Assert.That(schemas.Schemas().Cast<XmlSchema>().Select(schema => schema.TargetNamespace), Is.EquivalentTo(new[] { "http://www.w3.org/XML/1998/namespace", Constants.WsAddressing.Namespace, Constants.WsEventing.Namespace }));
        }
    }
}
