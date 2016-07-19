using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Library.With.Dot
{
    /// <summary>
    /// A simple class with nested classes.
    /// </summary>
    public class OuterClass
    {
        /// <summary>
        /// A method in the <see cref="OuterClass"/>.
        /// </summary>
        /// <param name="a">An integer.</param>
        /// <param name="b"></param>
        /// <returns>The concatenation of the string representation for
        /// <paramref name="a"/> and <paramref name="b"/>.</returns>
        public string MethodA(int a, double b)
        {
            return a.ToString() + b.ToString();
        }

        /// <summary>
        /// A one time nested class.
        /// </summary>
        public class InnerClass
        {
            /// <summary>
            /// This event will rise some day.
            /// </summary>
            public event EventHandler<UnhandledExceptionEventArgs> IWillRise;

            /// <summary>
            /// Call this to start the rise.
            /// </summary>
            protected void OnTheRise()
            {
                var handler = IWillRise;
                if (handler != null) handler(this, new UnhandledExceptionEventArgs(null, false));
            }

            /// <summary>
            /// A two times nested class.
            /// </summary>
            public class NestedClass
            {
                /// <summary>
                /// This point in time is the reference.
                /// </summary>
                public static DateTime T0 = DateTime.Now;

                /// <summary>
                /// Access to the secrets of strings.
                /// </summary>
                /// <param name="name">The string in question.</param>
                /// <returns>The magnitude of the string.</returns>
                public int this[string name]
                {
                    get { return name.Length; }
                }
            }
        }
    }
}
