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
    /// <para>
    /// Some interesting types and methods:
    /// <see cref="SimpleType.ProtectedMethod"/>,<br/>
    /// <see cref="SimpleType.OverloadedMethod(string, int)"/>,<br/>
    /// <see cref="Library.With.Dot.DemoDelegate"/>,<br/>
    /// <see cref="Library.With.Dot.OuterClass"/>,<br/>
    /// <see cref="Library.With.Dot.OuterClass.MethodA(int, double)"/>,<br/>
    /// <see cref="Library.With.Dot.OuterClass.InnerClass"/>,<br/>
    /// <see cref="Library.With.Dot.OuterClass.InnerClass.IWillRise"/>,<br/>
    /// <see cref="Library.With.Dot.GenericOuterClass{TC1, TC2}"/>,<br/>
    /// <see cref="Library.With.Dot.GenericOuterClass{TC1, TC2}.Method1{TM3}(TC1, TM3)"/>,<br/>
    /// <see cref="Library.With.Dot.GenericOuterClass{TC1, TC2}.GenericInnerClass{TC3}.Method(TC1, TC3)"/>.
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
        /// This is a private Method an it <em>should not</em> be documented.
        /// </summary>
        /// <param name="a">The left operand.</param>
        /// <param name="b">The right operand.</param>
        /// <returns>An <see cref="int"/> with the value of
        /// <paramref name="a"/> <c>+</c> <paramref name="b"/>.</returns>
        private int PrivateMethod(int a, int b) { return a + b; }

        /// <summary>
        /// This is an internal Method an it <em>should not</em> be documented.
        /// </summary>
        /// <param name="a">The left operand.</param>
        /// <param name="b">The right operand.</param>
        /// <returns>An <see cref="int"/> with the value of
        /// <paramref name="a"/> <c>+</c> <paramref name="b"/>.</returns>
        internal int InternalMethod(int a, int b) { return a + b; }

        /// <summary>
        /// This is a protected Method and it <em>should</em> be documented.
        /// </summary>
        /// <returns>An <see cref="int"/> with the value of <c>0</c>.</returns>
        protected int ProtectedMethod() { return 0; }

        /// <summary>
        /// An overloaded method.
        /// </summary>
        /// <param name="x">A simple parameter.</param>
        /// <returns>The value of the given parameter.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If the given value is negative.</exception>
        /// <exception cref="InvalidOperationException">If the simple type is not in the mood.</exception>
        public int OverloadedMethod(int x) { return x; }

        /// <summary>
        /// The first variation of the overloaded method.
        /// </summary>
        /// <param name="x">The simple parameter.</param>
        /// <param name="y">Another simple parameter.</param>
        /// <returns>The sum of both parameters.</returns>
        public int OverloadedMethod(int x, int y) { return x + y; }

        /// <summary>
        /// This variation of the overloaded method can only distinguished
        /// by parameter type from <see cref="OverloadedMethod(int, int)"/>.
        /// </summary>
        /// <param name="x">A simple parameter with a different type.</param>
        /// <param name="y">The second simple parameter.</param>
        /// <returns>The value of <paramref name="y"/>.</returns>
        public int OverloadedMethod(string x, int y) { return y; }

        /// <summary>
        /// This overloaded method uses varargs.
        /// </summary>
        /// <param name="x">The simple parameter.</param>
        /// <param name="y">Another simple parameter.</param>
        /// <param name="zs">Additional parameter.</param>
        /// <returns>The sum of <paramref name="x"/> and <paramref name="y"/>.</returns>
        public int OverloadedMethod(int x, int y, params int[] zs)
        {
            return x + y;
        }

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
        /// This operator adds an integer value to a <see cref="SimpleType"/> and returns a new
        /// <see cref="SimpleType"/>.
        /// </summary>
        /// <param name="v">The simple type object.</param>
        /// <param name="a">The numeric value to add.</param>
        /// <returns>A new <see cref="SimpleType"/>.</returns>
        public static SimpleType operator +(SimpleType v, int a)
        {
            return new SimpleType();
        }

        /// <summary>
        /// Converts this <see cref="SimpleType"/> per explicit
        /// cast into an <see cref="Int32"/>.
        /// </summary>
        /// <param name="v">The simple type instance.</param>
        /// <returns><c>0</c></returns>.
        public static explicit operator int(SimpleType v)
        {
            return 0;
        }

        /// <summary>
        /// Converts this <see cref="SimpleType"/> per implicit
        /// cast into a <see cref="String"/>.
        /// </summary>
        /// <param name="v">The simple type sinstance.</param>
        /// <returns>The empty string.</returns>
        public static implicit operator string(SimpleType v)
        {
            return string.Empty;
        }

        /// <summary>
        /// Destructor of the class.
        /// </summary>
        ~SimpleType() { }
    }
}
