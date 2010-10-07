using System;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using NUnit.Framework;

namespace CommonContracts.WsBaseFaults.Tests
{
    [TestFixture()]
    public class DescriptionTests
    {
        [Test()]
        public void ConstructorsRequireValueParameter()
        {
            var constructors = new TestDelegate[]
                                   {
                                       () => new Description((String) null),
                                       () => new Description(null, CultureInfo.CurrentCulture)
                                   };

            foreach (var constructor in constructors)
            {
                var exception = Assert.Throws<ArgumentNullException>(constructor);
                Assert.That(exception.Message, Is.EqualTo("Precondition failed: !String.IsNullOrWhiteSpace(value)  value\r\nParameter name: value"));
                Assert.That(exception.ParamName, Is.EqualTo("value"));
            }
        }

        [Test()]
        public void ConstructorRequiresXmlReader()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => new Description((XmlReader)null));
            Assert.That(exception.Message, Is.EqualTo("Precondition failed: reader != null  reader\r\nParameter name: reader"));
            Assert.That(exception.ParamName, Is.EqualTo("reader"));
        }

        [Test()]
        public void ConstructorSetsProperties()
        {
            const String Value = "A Value";

            var description = new Description(Value);
            Assert.That(description.Value, Is.EqualTo(Value));
            Assert.That(description.Language, Is.Null);

            description = new Description(Value, CultureInfo.CurrentCulture);
            Assert.That(description.Value, Is.EqualTo(Value));
            Assert.That(description.Language, Is.EqualTo(CultureInfo.CurrentCulture));
        }

        [Test()]
        [Description("Confirms that serialization logic works as expected")]
        public void Serializable()
        {
            const String Value = "A value";
            var target = new Description(Value, CultureInfo.CurrentCulture);

            XElement xml;

            using (var stream = new MemoryStream())
            {
                XmlWriter writer = XmlWriter.Create(stream);
                ((IXmlSerializable)target).WriteXml(writer);
                writer.Flush();
                stream.Position = 0;

                xml = XElement.Load(stream);
            }

            var areEqual = XNode.DeepEquals(XElement.Parse("<wsbf:Description xml:lang='en-US' xmlns:wsbf='http://docs.oasis-open.org/wsrf/bf-2'>A value</wsbf:Description>"), xml);
            Assert.IsTrue(areEqual);
        }

        [Test()]
        [Description("Confirms that deserialization logic works as expected")]
        public void Deserialize()
        {
            String xml = "<wsbf:Description xmlns:wsbf='http://docs.oasis-open.org/wsrf/bf-2' xml:lang='en-US'>some desc</wsbf:Description>";
            var reader = new XmlTextReader(new StringReader(xml));
            reader.Read();
            var target = new Description(reader);

            Assert.That(target.Language, Is.EqualTo(CultureInfo.GetCultureInfo("en-US")));
            Assert.That(target.Value, Is.EqualTo("some desc"));

            xml = "<wsbf:Description xmlns:wsbf='http://docs.oasis-open.org/wsrf/bf-2'>some desc</wsbf:Description>";
            reader = new XmlTextReader(new StringReader(xml));
            reader.Read();
            target = new Description(reader);

            Assert.That(target.Language, Is.Null);
            Assert.That(target.Value, Is.EqualTo("some desc"));

            xml = "<wsbf:Description xmlns:wsbf='http://docs.oasis-open.org/wsrf/bf-2'></wsbf:Description>";
            reader = new XmlTextReader(new StringReader(xml));
            reader.Read();
            target = new Description(reader);

            Assert.That(target.Language, Is.Null);
            Assert.That(target.Value, Is.Empty);

            xml = "<wsbf:Description xmlns:wsbf='http://docs.oasis-open.org/wsrf/bf-2'/>";
            reader = new XmlTextReader(new StringReader(xml));
            reader.Read();
            target = new Description(reader);

            Assert.That(target.Language, Is.Null);
            Assert.That(target.Value, Is.Empty);
        }

        [Test()]
        [Description("Confirms that the reader must be interactive")]
        public void ConstructorRequiresXmlReaderInInteractivestate()
        {
            const String xml = "<wsbf:Description xmlns:wsbf='http://docs.oasis-open.org/wsrf/bf-2' xml:lang='en-US'>some desc</wsbf:Description>";
            var reader = new XmlTextReader(new StringReader(xml));

            Assert.That(reader.ReadState, Is.EqualTo(ReadState.Initial)); // Sanity check
            var exception = Assert.Throws<ArgumentException>(() => new Description(reader));

            Assert.That(exception.Message, Is.EqualTo("Precondition failed: reader.ReadState == ReadState.Interactive  reader\r\nParameter name: reader"));
            Assert.That(exception.ParamName, Is.EqualTo("reader"));
        }
    }
}
