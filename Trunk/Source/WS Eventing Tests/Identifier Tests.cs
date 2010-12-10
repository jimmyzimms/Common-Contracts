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
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using NUnit.Framework;

namespace CommonContracts.WsEventing.Tests
{
    [TestFixture()]
    public class IdentifierTests
    {
        [Test()]
        public void ConstructorShouldSetExpectedValues()
        {
            const String id = "13bb3ec2-a118-4dfe-83de-8d28c7ec8559";

            var tests = new Func<Identifier> []
                            {
                                () => new Identifier(new EndpointAddress(new Uri("http://tempuri.org/"), AddressHeader.CreateAddressHeader("Identifier", Constants.WsEventing.Namespace, "uuid:" + id))),
                                () => new Identifier(new Guid(id)),
                                () => new Identifier("uuid:" + id),
                                () => new Identifier(new UniqueId("uuid:" + id))
                            };

            foreach (var test in tests)
            {
                var identity = test();

                Assert.That(identity.Value.ToString(), Is.EqualTo("uuid:" + id));
            }
        }

        [Test()]
        public void ConstructorShouldRequireParameters()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => new Identifier((EndpointAddress)null));
            Assert.That(exception.Message, Is.EqualTo("Precondition failed: epa != null  epa\r\nParameter name: epa"));
            Assert.That(exception.ParamName, Is.EqualTo("epa"));

            exception = Assert.Throws<ArgumentNullException>(() => new Identifier((Identifier)null));
            Assert.That(exception.Message, Is.EqualTo("Precondition failed: id != null  id\r\nParameter name: id"));
            Assert.That(exception.ParamName, Is.EqualTo("id"));

            exception = Assert.Throws<ArgumentNullException>(() => new Identifier((UniqueId)null));
            Assert.That(exception.Message, Is.EqualTo("Precondition failed: id != null  id\r\nParameter name: id"));
            Assert.That(exception.ParamName, Is.EqualTo("id"));
        }

        [Test()]
        public void Serialize()
        {
            var identifier = new Identifier(new Guid("13bb3ec2-a118-4dfe-83de-8d28c7ec8559"));

            var serializer = new XmlSerializer(typeof (TestXmlWrapper<Identifier>));

            XElement xml;

            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, new TestXmlWrapper<Identifier> {Item = identifier});

                stream.Position = 0;

                xml = XElement.Load(stream);
            }

            var areEqual = XNode.DeepEquals(XElement.Parse("<wse:Identifier xmlns:wse='http://schemas.xmlsoap.org/ws/2004/08/eventing'>uuid:13bb3ec2-a118-4dfe-83de-8d28c7ec8559</wse:Identifier>"), xml.FirstNode);
            Assert.IsTrue(areEqual);
        }

        [Test()]
        public void Deserialize()
        {
            var xml = XElement.Parse("<wse:Identifier xmlns:wse='http://schemas.xmlsoap.org/ws/2004/08/eventing'>uuid:13bb3ec2-a118-4dfe-83de-8d28c7ec8559</wse:Identifier>");
            var serializer = new XmlSerializer(typeof(Identifier));

            Identifier identifier = (Identifier)serializer.Deserialize(xml.CreateReader());
            Assert.That(identifier.Value.ToString(), Is.EqualTo("uuid:13bb3ec2-a118-4dfe-83de-8d28c7ec8559"));
        }

        [Test()]
        public void AcquireSchemaShouldLoadSchemas()
        {
            var schemas = new XmlSchemaSet();
            var qName = Identifier.AcquireSchema(schemas);

            Assert.That(qName.Name, Is.EqualTo("anyURI"));
            Assert.That(qName.Namespace, Is.EqualTo("http://www.w3.org/2001/XMLSchema"));

            Assert.That(schemas.Schemas(), Is.Empty);
        }
    }
}
