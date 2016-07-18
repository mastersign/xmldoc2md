using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nested.Namespace
{
    /// <summary>
    /// This is a simple public class ‒ a type with no special attributes.
    /// </summary>
    public class SimpleType
    {
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
        /// Destructor of the class.
        /// </summary>
        ~SimpleType() { }
    }
}
