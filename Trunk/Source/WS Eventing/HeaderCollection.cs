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
using System.Diagnostics.Contracts;
using System.Linq;
using System.ServiceModel.Channels;

namespace CommonContracts.WsEventing
{
    /// <summary>
    /// Provides a base type for XML elements that can have expando values added to them.
    /// </summary>
    public abstract class HeaderCollection : List<AddressHeader>
    {
        protected virtual IQueryable<AddressHeader> CreateQuery(String name, String ns)
        {
            Contract.Requires<ArgumentNullException>(!String.IsNullOrWhiteSpace(name), "name");
            Contract.Requires<ArgumentNullException>(!String.IsNullOrWhiteSpace(ns), "ns");
            Contract.Ensures(Contract.Result<IQueryable<AddressHeader>>() != null, "The result of the CreateQuery method cannot be null");

            var query = (from header in this
                         where header.Name == name && header.Namespace == ns
                         select header).AsQueryable();
            return query;
        }

        public IEnumerable<AddressHeader> FindAll(String name, String ns)
        {
            Contract.Requires<ArgumentNullException>(!String.IsNullOrWhiteSpace(name), "name");
            Contract.Requires<ArgumentNullException>(!String.IsNullOrWhiteSpace(ns), "ns");

            var result = this.CreateQuery(name, ns);
            return result;
        }

        public AddressHeader FindFirst(String name, String ns)
        {
            Contract.Requires<ArgumentNullException>(!String.IsNullOrWhiteSpace(name), "name");
            Contract.Requires<ArgumentNullException>(!String.IsNullOrWhiteSpace(ns), "ns");

            var result = this.FindAll(name, ns).FirstOrDefault();
            return result;
        }

        public T GetValue<T>(String name, String ns)
        {
            Contract.Requires<ArgumentNullException>(!String.IsNullOrWhiteSpace(name), "name");
            Contract.Requires<ArgumentNullException>(!String.IsNullOrWhiteSpace(ns), "ns");

            AddressHeader ah = this.FindFirst(name, ns);
            return ah == null ? default(T) : ah.GetValue<T>();
        }
    }
}
