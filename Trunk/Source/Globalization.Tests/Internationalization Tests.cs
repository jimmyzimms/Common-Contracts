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

using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Xml.Linq;
using CommonContracts.Globalization;
using NUnit.Framework;

namespace Globalization.Tests
{
    [TestFixture()]
    public class InternationalizationTests
    {
        [Test()]
        public void ConstructorShouldInitializeWithExpectedValues()
        {
            var target = new International();
            Assert.That(target.Locale, Is.EqualTo("$default"));
            Assert.That(target.Preferences, Is.Not.Null);
            Assert.That(target.TimeZone, Is.Null);
        }

        [Test()]
        public void ConstructorShouldDefaultToCurrentUiCulture()
        {
            var culture = CultureInfo.CurrentUICulture;
            var target = new International((CultureInfo)null);
            Assert.That(target.Locale, Is.EqualTo(culture.IetfLanguageTag));
        }

        [Test()]
        public void CanSerialize()
        {
            var serializer = new DataContractSerializer(typeof (International));

            XElement xml;
            using (var stream = new MemoryStream())
            {
                serializer.WriteObject(stream, new International());
                stream.Position = 0;
                xml = XElement.Load(stream);
            }

            var areEqual = XNode.DeepEquals(xml, XElement.Parse("<international xmlns='http://www.w3.org/2005/09/ws-i18n' xmlns:i='http://www.w3.org/2001/XMLSchema-instance'><locale>$default</locale><tz i:nil='true' /><preferences /></international>"));
            Assert.IsTrue(areEqual);

            using (var stream = new MemoryStream())
            {
                serializer.WriteObject(stream, new International(CultureInfo.CurrentUICulture));
                stream.Position = 0;
                xml = XElement.Load(stream);
            }

            areEqual = XNode.DeepEquals(xml, XElement.Parse("<international xmlns='http://www.w3.org/2005/09/ws-i18n' xmlns:i='http://www.w3.org/2001/XMLSchema-instance'><locale>en-US</locale><tz i:nil='true' /><preferences /></international>"));
            Assert.IsTrue(areEqual);

            using (var stream = new MemoryStream())
            {
                var target = new International();
                target.Preferences.Content.Add(XElement.Parse("<test/>"));
                serializer.WriteObject(stream, target);
                stream.Position = 0;
                xml = XElement.Load(stream);
            }

            areEqual = XNode.DeepEquals(xml, XElement.Parse("<international xmlns='http://www.w3.org/2005/09/ws-i18n' xmlns:i='http://www.w3.org/2001/XMLSchema-instance'><locale>$default</locale><tz i:nil='true' /><preferences><test xmlns='' /></preferences></international>"));
            Assert.IsTrue(areEqual);

            using (var stream = new MemoryStream())
            {
                var target = new International("Europe/Andorra");
                serializer.WriteObject(stream, target);
                stream.Position = 0;
                xml = XElement.Load(stream);
            }

            areEqual = XNode.DeepEquals(xml, XElement.Parse("<international xmlns='http://www.w3.org/2005/09/ws-i18n' xmlns:i='http://www.w3.org/2001/XMLSchema-instance'><locale>$default</locale><tz>Europe/Andorra</tz><preferences /></international>"));
            Assert.IsTrue(areEqual);            
        }

        [Test()]
        public void CanDeserialize()
        {
            var serializer = new DataContractSerializer(typeof(International));

            XElement xml = XElement.Parse("<international xmlns='http://www.w3.org/2005/09/ws-i18n' xmlns:i='http://www.w3.org/2001/XMLSchema-instance'><locale>$default</locale><tz i:nil='true' /><preferences /></international>");
            var target = serializer.ReadObject(xml.CreateReader()) as International;
            Assert.That(target.Locale, Is.EqualTo("$default"));
            Assert.That(target.Preferences, Is.Not.Null);
            Assert.That(target.TimeZone, Is.Empty);

            xml = XElement.Parse("<international xmlns='http://www.w3.org/2005/09/ws-i18n' xmlns:i='http://www.w3.org/2001/XMLSchema-instance'><locale>en-US</locale><tz i:nil='true' /><preferences /></international>");
            target = serializer.ReadObject(xml.CreateReader()) as International;
            Assert.That(target.Locale, Is.EqualTo("en-US"));
            Assert.That(target.Preferences, Is.Not.Null);
            Assert.That(target.TimeZone, Is.Empty);

            xml = XElement.Parse("<international xmlns='http://www.w3.org/2005/09/ws-i18n' xmlns:i='http://www.w3.org/2001/XMLSchema-instance'><locale>$default</locale><tz i:nil='true' /><preferences><test xmlns='' /></preferences></international>");
            target = serializer.ReadObject(xml.CreateReader()) as International;
            Assert.That(target.Locale, Is.EqualTo("$default"));
            Assert.That(target.Preferences, Is.Not.Null);
            Assert.That(target.TimeZone, Is.Empty);

            xml = XElement.Parse("<international xmlns='http://www.w3.org/2005/09/ws-i18n' xmlns:i='http://www.w3.org/2001/XMLSchema-instance'><locale>$default</locale><tz>Europe/Andorra</tz><preferences /></international>");
            target = serializer.ReadObject(xml.CreateReader()) as International;
            Assert.That(target.Locale, Is.EqualTo("$default"));
            Assert.That(target.Preferences, Is.Not.Null);
            Assert.That(target.TimeZone, Is.EqualTo("Europe/Andorra"));
        }
    }
}
