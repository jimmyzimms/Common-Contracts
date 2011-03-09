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
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using CommonContracts.Globalization;
using NUnit.Framework;

namespace Globalization.Tests
{
    [TestFixture()]
    public class PreferencesTests
    {
        [Test()]
        public void ConstructorShouldCreateExpectedDefaults()
        {
            Preferences.AcquireSchema(new XmlSchemaSet());
            var preferences = new Preferences();
            Assert.That(preferences.Content, Is.Empty);
        }

        [Test()]
        public void CanSerialize()
        {
            var serializer = new XmlSerializer(typeof(Preferences));

            XElement xml;
            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, new Preferences());
                stream.Position = 0;
                xml = XElement.Load(stream);
            }

            var areEqual = XNode.DeepEquals(xml, XElement.Parse("<preferences xmlns='http://www.w3.org/2005/09/ws-i18n' />"));
            Assert.IsTrue(areEqual);

            using (var stream = new MemoryStream())
            {
                var preferences = new Preferences();
                preferences.Content.Add(XElement.Parse("<test/>"));
                serializer.Serialize(stream, preferences);
                stream.Position = 0;
                xml = XElement.Load(stream);
            }

            areEqual = XNode.DeepEquals(xml, XElement.Parse("<preferences xmlns='http://www.w3.org/2005/09/ws-i18n'><test xmlns=''/></preferences>"));
            Assert.IsTrue(areEqual);

            using (var stream = new MemoryStream())
            {
                var preferences = new Preferences();
                preferences.Content.Add(XElement.Parse("<test/>"));
                preferences.Content.Add(XElement.Parse("<other xmlns='urn:foo'/>"));
                serializer.Serialize(stream, preferences);
                stream.Position = 0;
                xml = XElement.Load(stream);
            }

            areEqual = XNode.DeepEquals(xml, XElement.Parse("<preferences xmlns='http://www.w3.org/2005/09/ws-i18n'><test xmlns=''/><other xmlns='urn:foo'/></preferences>"));
            Assert.IsTrue(areEqual);
        }

        [Test()]
        public void CanDeserialize()
        {
            var serializer = new XmlSerializer(typeof (Preferences));

            var xml = XElement.Parse("<preferences xmlns='http://www.w3.org/2005/09/ws-i18n' />");
            var preferences = serializer.Deserialize(xml.CreateReader()) as Preferences;
            Assert.That(preferences.Content, Is.Empty);

            xml = XElement.Parse("<preferences xmlns='http://www.w3.org/2005/09/ws-i18n'><test xmlns=''/></preferences>");
            preferences = serializer.Deserialize(xml.CreateReader()) as Preferences;
            Assert.That(preferences.Content.Count, Is.EqualTo(1));
            var areEqual = XNode.DeepEquals(preferences.Content.First(), XElement.Parse("<test xmlns=''/>"));
            Assert.IsTrue(areEqual);

            xml = XElement.Parse("<preferences xmlns='http://www.w3.org/2005/09/ws-i18n'><test xmlns=''/><other xmlns='urn:foo'/></preferences>");
            preferences = serializer.Deserialize(xml.CreateReader()) as Preferences;
            Assert.That(preferences.Content.Count, Is.EqualTo(2));
            areEqual = XNode.DeepEquals(preferences.Content.First(), XElement.Parse("<test xmlns=''/>"));
            Assert.IsTrue(areEqual);
            areEqual = XNode.DeepEquals(preferences.Content.Last(), XElement.Parse("<other xmlns='urn:foo'/>"));
            Assert.IsTrue(areEqual);
        }
    }
}