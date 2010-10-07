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
