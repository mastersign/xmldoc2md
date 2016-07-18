using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Library.With.Dot
{
    /// <summary>
    /// A generic class with two type parameters.
    /// </summary>
    /// <typeparam name="T1">Type parameter 1.</typeparam>
    /// <typeparam name="T2">Type parameter 2.</typeparam>
    public class GenericOuterClass<T1, T2>
    {
        /// <summary>
        /// A generic method in a generic class referencing type parameters from the class and the method.
        /// </summary>
        /// <typeparam name="T3">Method type parameter.</typeparam>
        /// <param name="t1">Type from class type parameter.</param>
        /// <param name="t3">Type from method type parameter.</param>
        public void Method1<T3>(T1 t1, T3 t3) { }

        /// <summary>
        /// An non generic inner class of a generic class.
        /// </summary>
        public class GenericInnerClass1
        {
            /// <summary>
            /// A non generic method of a non generic inner class in a generic class.
            /// </summary>
            public void Method1() { }

            /// <summary>
            /// A non generic method of a non generic inner class in generic class referencing a type parameter.
            /// </summary>
            /// <param name="t1"></param>
            public void Method2(T1 t1) { }
        }

        /// <summary>
        /// A generic inner class in a generic class.
        /// </summary>
        /// <typeparam name="T3">Type parameter 3.</typeparam>
        public class GenericInnerClass2<T3>
        {
            /// <summary>
            /// A non generic method referencing type parameters from its class and an outer class.
            /// </summary>
            /// <param name="t1"></param>
            /// <param name="t3"></param>
            public void Method(T1 t1, T3 t3) { }
        }
    }
}
