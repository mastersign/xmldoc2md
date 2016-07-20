using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nested.Namespace
{
    /// <summary>
    /// <para>
    /// This is a simple public class ‒ a type with no special attributes.
    /// </para>
    /// <strong>A simple list:</strong>
    /// <list type="bullet">
    ///     <item>Item A is without term and description.</item>
    ///     <item>
    ///         <term>Item B</term>
    ///         <description>is split into term and description.</description>
    ///     </item>
    /// </list>
    /// <strong>A numbered list:</strong>
    /// <list type="number">
    ///     <item>This is the first item.</item>
    ///     <item>This is the second item.</item>
    /// </list>
    /// <strong>And now a table list.</strong>
    /// <list type="table">
    ///     <listheader>
    ///         <term>Keyword</term>
    ///         <description>Description</description>
    ///     </listheader>
    ///     <item>
    ///         <term>London</term>
    ///         <description>Great Britain</description>
    ///     </item>
    ///     <item>
    ///         <term>Berlin</term>
    ///         <description>Germany</description>
    ///     </item>
    /// </list>
    /// <para>
    /// This <strong>class description</strong> is used to test <em>some markup</em>.
    /// </para>
    /// </summary>
    /// <example>
    /// This example shows the instantiation of <see cref="SimpleType"/>.
    /// <code>
    /// // Instantiation
    /// SimpleType st = new SimpleType(10, "ten");
    /// // Usage
    /// Console.WriteLine(st.Prop1);
    /// Console.WriteLine(SimpleType.CONSTANT);
    /// </code>
    /// </example>
    /// <remarks>
    /// This type is only designed to demonstrate the XML DOC strings.
    /// </remarks>
    /// <seealso cref="Library.With.Dot.OuterClass"/>
    /// <seealso cref="Library.With.Dot.GenericOuterClass{T1, T2}"/>
    public class SimpleType
    {
        /// <summary>
        /// A static constructor.
        /// </summary>
        static SimpleType() { }

        /// <summary>
        /// A static field.
        /// </summary>
        public static int staticField;

        /// <summary>
        /// A static read-only field.
        /// </summary>
        public static readonly int staticReadonlyField;

        /// <summary>
        /// A constant.
        /// </summary>
        public const int CONSTANT = 0;

        /// <summary>
        /// The default constructor.
        /// </summary>
        public SimpleType() { }

        /// <summary>
        /// A parameterized constructor.
        /// </summary>
        /// <param name="a">Parameter <paramref name="a"/> with type <see cref="int"/>.</param>
        /// <param name="b">Parameter <paramref name="b"/> with type <see cref="string"/>.</param>
        public SimpleType(int a, string b) { }

        /// <summary>
        /// This is a simple read-only property.
        /// </summary>
        public string Prop1 { get; private set; }

        /// <summary>
        /// This is a property with a more complex type.
        /// </summary>
        /// <value>This property holds a <see cref="Dictionary{DateTie, SimpleType}" />.</value>
        public Dictionary<DateTime, SimpleType> Prop2 { get; set; }

        /// <summary>
        /// An item accessor.
        /// </summary>
        /// <param name="a">A number between 1 and 10.</param>
        /// <returns>The given number.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Is thrown if the number is smaller than 1 or larger than 10.
        /// </exception>
        public int this[int a] { get { return a; } }

        /// <summary>
        /// Destructor of the class.
        /// </summary>
        ~SimpleType() { }
    }
}
