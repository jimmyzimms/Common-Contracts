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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.ServiceModel.Channels;

namespace CommonContracts.WsEventing
{
    /// <summary>
    /// Provides a collection type for contract types that have XML elements that can have expando values added to them.
    /// </summary>
    /// <remarks>
    /// Since the presence of additional XML information is supported by some elements, the
    /// use of <see cref="AddressHeader"/> elements, each repesenting a single additional child
    /// element simplies the consumption of this information despite the fact that technically
    /// the additional information is not a header.
    /// </remarks>
    public class HeaderCollection : Collection<AddressHeader>
    {
        #region Overrides
        
        /// <summary>
        /// Inserts an element into the <see cref="HeaderCollection"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
        /// <param name="item">The <see cref="AddressHeader"/> to insert.</param>
        /// <exception cref="ArgumentNullException"><paramref name="item"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <para><paramref name="index"/> is less than zero</para>
        /// <para>-or-</para>
        /// <para><paramref name="index"/> is greater than <see cref="Collection{T}.Count"/>.</para>
        /// </exception>
        protected override void InsertItem(Int32 index, AddressHeader item)
        {
            if (item == null) throw new ArgumentNullException("item");

            base.InsertItem(index, item);
        }

        /// <summary>
        /// Replaces the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to replace.</param>
        /// <param name="item">The new value for the element at the specified index.</param>
        /// <exception cref="ArgumentNullException"><paramref name="item"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <para><paramref name="index"/> is less than zero</para>
        /// <para>-or-</para>
        /// <para><paramref name="index"/> is greater than <see cref="Collection{T}.Count"/>.</para>
        /// </exception>
        protected override void SetItem(Int32 index, AddressHeader item)
        {
            if (item == null) throw new ArgumentNullException("item");

            base.SetItem(index, item);
        }

        #endregion

        #region Methods
        
        /// <summary>
        /// Creates a new query expression based on the supplied QName component values to find an <see cref="AddressHeader"/> in the collection.
        /// </summary>
        /// <param name="name">The local name of the <see cref="AddressHeader"/> to find.</param>
        /// <param name="ns">The namespace of the <seealso cref="AddressHeader"/> to find.</param>
        /// <returns>A query expression that will match the supplied QName component values.</returns>
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

        /// <summary>
        /// Returns all <see cref="AddressHeader"/> instances found that match the supplied <paramref name="name"/> and <paramref name="ns"/>.
        /// </summary>
        /// <param name="name">The local name of the <see cref="AddressHeader"/> to find.</param>
        /// <param name="ns">The namespace of the <seealso cref="AddressHeader"/> to find.</param>
        /// <returns>A sequence containing all <see cref="AddressHeader"/> instances found matching the supplied QName component values.</returns>
        public virtual IEnumerable<AddressHeader> FindAll(String name, String ns)
        {
            Contract.Requires<ArgumentNullException>(!String.IsNullOrWhiteSpace(name), "name");
            Contract.Requires<ArgumentNullException>(!String.IsNullOrWhiteSpace(ns), "ns");

            var result = this.CreateQuery(name, ns);
            return result;
        }

        /// <summary>
        /// Returns the first <see cref="AddressHeader"/> instance found that matches the supplied <paramref name="name"/> and <paramref name="ns"/>.
        /// </summary>
        /// <param name="name">The local name of the <see cref="AddressHeader"/> to find.</param>
        /// <param name="ns">The namespace of the <seealso cref="AddressHeader"/> to find.</param>
        /// <returns>If found, the first instance of the <see cref="AddressHeader"/> that matches the supplied QName component values; otherwise null.</returns>
        public virtual AddressHeader FindFirst(String name, String ns)
        {
            Contract.Requires<ArgumentNullException>(!String.IsNullOrWhiteSpace(name), "name");
            Contract.Requires<ArgumentNullException>(!String.IsNullOrWhiteSpace(ns), "ns");

            var result = this.FindAll(name, ns).FirstOrDefault();
            return result;
        }

        /// <summary>
        /// Returns the value cast as <typeparamref name="T"/> for the first <see cref="AddressHeader"/> instance found that matches the supplied <paramref name="name"/> and <paramref name="ns"/>.
        /// </summary>
        /// <typeparam name="T">The type to cast the value of the <see cref="AddressHeader"/> to.</typeparam>
        /// <param name="name">The local name of the <see cref="AddressHeader"/> to find.</param>
        /// <param name="ns">The namespace of the <seealso cref="AddressHeader"/> to find.</param>
        /// <returns>If found, the value, cast as <typeparamref name="T"/>, of the first instance of the <see cref="AddressHeader"/> that matches the supplied QName component values; otherwise the default value of <typeparamref name="T"/>.</returns>
        public virtual T GetValue<T>(String name, String ns)
        {
            Contract.Requires<ArgumentNullException>(!String.IsNullOrWhiteSpace(name), "name");
            Contract.Requires<ArgumentNullException>(!String.IsNullOrWhiteSpace(ns), "ns");

            AddressHeader ah = this.FindFirst(name, ns);
            return ah == null ? default(T) : ah.GetValue<T>();
        }

        #endregion
    }
}
