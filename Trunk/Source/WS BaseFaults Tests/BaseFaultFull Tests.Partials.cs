﻿#region Legal

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
using System.Collections.Generic;
using System.ServiceModel;
using System.Xml;
using System.Xml.Serialization;

namespace CommonContracts.WsBaseFaults.Tests
{
    /// <content>Partials class file.</content>
    public partial class BaseFaultFullTests
    {
        /// <summary>
        /// Required for use on the <see cref="ConstructorShouldAllowNulls"/> as the constructors are ambiguous with a mock
        /// Required for the <see cref="Deserialize"/> test.
        /// Required for the <see cref="NestedFaultsShouldSerialize"/> test.
        /// </summary>
        [XmlRoot("TestFault", Namespace = Constants.WsBaseFaults.Namespace, DataType = Constants.WsBaseFaults.Namespace + ":BaseFaultType")]
        public sealed class TestFault : BaseFaultFull
        {
            public TestFault() { }

            public TestFault(EndpointAddress originator)
                : base(originator)
            {
            }

            public TestFault(IEnumerable<Description> descriptions)
                : base(descriptions)
            {
            }

            public TestFault(ErrorCode errorCode)
                : base(errorCode)
            {
            }

            public TestFault(DateTime utc)
                : base(utc)
            {
            }

            public TestFault(DateTime utc, EndpointAddress originator, ErrorCode errorCode, IEnumerable<Description> descriptions)
                : base(utc, originator, errorCode, descriptions)
            {
            }

            protected override void WriteStartElement(XmlWriter writer)
            {
            }

            protected override void WriteEndElement(XmlWriter writer)
            {
            }

            protected override void ReadStartElement(XmlReader reader)
            {
                if (reader.IsStartElement("TestFault", Constants.WsBaseFaults.Namespace) == false)
                {
                    throw new XmlException("Invalid Element, it must be '" + Constants.WsBaseFaults.Namespace + ":TestFault'");
                }

                reader.ReadStartElement("TestFault", Constants.WsBaseFaults.Namespace);
            }
        }
    }
}
