using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Mastersign.XmlDoc
{
    [TestFixture]
    public class CRefParsingTest
    {
        private CRefParsing parsing = new CRefParsing();

        [Test]
        public void MemberKindTest()
        {
            Assert.Throws<ArgumentNullException>(() => { CRefParsing.Parse(null); });
            Assert.AreEqual(CRefKind.Namespace, CRefParsing.Parse("N:A.B").Kind);
            Assert.AreEqual(CRefKind.Type, CRefParsing.Parse("T:A.B.C").Kind);
            Assert.AreEqual(CRefKind.Field, CRefParsing.Parse("F:A.B.C.x").Kind);
            Assert.AreEqual(CRefKind.Method, CRefParsing.Parse("M:A.B.C.z()").Kind);
            Assert.AreEqual(CRefKind.Property, CRefParsing.Parse("P:A.B.C.y").Kind);
            Assert.AreEqual(CRefKind.Event, CRefParsing.Parse("E:A.B.C.e").Kind);
            Assert.AreEqual(CRefKind.Unknown, CRefParsing.Parse("U:Unknown").Kind);
            Assert.AreEqual(CRefKind.Error, CRefParsing.Parse("!Error Message").Kind);
            Assert.AreEqual(CRefKind.Invalid, CRefParsing.Parse("invalid syntax").Kind);
        }

        [Test]
        public void ResultTypeTest()
        {
            Assert.IsInstanceOf<CRefNamespace>(CRefParsing.Parse("N:A"));
            Assert.IsInstanceOf<CRefType>(CRefParsing.Parse("T:A"));
            Assert.IsInstanceOf<CRefField>(CRefParsing.Parse("F:A.x"));
            Assert.IsInstanceOf<CRefMethod>(CRefParsing.Parse("M:A.x()"));
            Assert.IsInstanceOf<CRefProperty>(CRefParsing.Parse("P:A.x"));
            Assert.IsInstanceOf<CRefEvent>(CRefParsing.Parse("E:A.x"));
            Assert.IsInstanceOf<CRefParsingResult>(CRefParsing.Parse("X:A"));
            Assert.IsInstanceOf<CRefParsingResult>(CRefParsing.Parse("! Error Message"));
            Assert.IsInstanceOf<CRefParsingResult>(CRefParsing.Parse("invalid syntax"));
        }

        [Test]
        public void ErrorMessage()
        {
            Assert.Throws<ArgumentNullException>(() => { parsing.ErrorMessage(null); });
            Assert.IsNull(parsing.ErrorMessage(""));
            Assert.IsNull(parsing.ErrorMessage("T:A"));
            Assert.AreEqual("Message", parsing.ErrorMessage("!Message"));
            Assert.AreEqual("Message ", parsing.ErrorMessage("! Message "));
        }

        [Test]
        public void NamespaceExceptionTest()
        {
            Assert.Throws<ArgumentNullException>(() => { parsing.Namespace(null); });
        }

        [Test]
        public void NamespaceNullTest()
        {
            Assert.IsNull(parsing.Namespace(""));
            Assert.IsNull(parsing.Namespace("! error"));
            Assert.IsNull(parsing.Namespace("X:unknown"));
        }

        [Test]
        public void NamespaceFromNamespaceTest()
        {
            Assert.AreEqual("Aa", parsing.Namespace("N:Aa"));
            Assert.AreEqual("Aa.Bb", parsing.Namespace("N:Aa.Bb"));
            Assert.AreEqual("Aa.Bb.Cc", parsing.Namespace("N:Aa.Bb.Cc"));
        }

        [Test]
        public void NamespaceFromTypeTest()
        {
            Assert.IsNull(parsing.Namespace("T:A"));
            Assert.AreEqual("A", parsing.Namespace("T:A.B"));
            Assert.AreEqual("A.B", parsing.Namespace("T:A.B.C"));
            Assert.AreEqual("A.B.C", parsing.Namespace("T:A.B.C.D"));
            Assert.AreEqual("A.B`1", parsing.Namespace("T:A.B`1.C"));
            Assert.AreEqual("A.B`10", parsing.Namespace("T:A.B`10.C`1"));
        }

        [Test]
        public void NamespaceFromFieldTest()
        {
            Assert.IsNull(parsing.Namespace("F:A.x"));
            Assert.AreEqual("A", parsing.Namespace("F:A.B.xyz"));
            Assert.AreEqual("A.B", parsing.Namespace("F:A.B.C.x"));
            Assert.AreEqual("A.B.C", parsing.Namespace("F:A.B.C.D.x"));
            Assert.AreEqual("A.B", parsing.Namespace("F:A.B.C`1.x"));
            Assert.AreEqual("A.B`1", parsing.Namespace("F:A.B`1.C.x"));
            Assert.AreEqual("A.B`10", parsing.Namespace("F:A.B`10.C`1.x"));
        }

        [Test]
        public void NamespaceFromMethodTest()
        {
            Assert.IsNull(parsing.Namespace("M:A.x()"));
            Assert.AreEqual("A", parsing.Namespace("M:A.B.x()"));
            Assert.AreEqual("A.B", parsing.Namespace("M:A.B.C.op_implicit(A.B.C)~A.B.X"));
            Assert.AreEqual("A.B.C", parsing.Namespace("M:A.B.C.D.x(X,Y)"));
            Assert.AreEqual("A.B", parsing.Namespace("M:A.B.C`1.x()"));
            Assert.AreEqual("A.B`1", parsing.Namespace("M:A.B`1.C.x()"));
            Assert.AreEqual("A.B`10", parsing.Namespace("M:A.B`10.C`1.x()"));
            Assert.AreEqual("A.B`10", parsing.Namespace("M:A.B`10.C`1.x``2(``0,``1)"));
        }

        [Test]
        public void NamespaceFromPropertyTest()
        {
            Assert.IsNull(parsing.Namespace("P:A.x"));
            Assert.AreEqual("A", parsing.Namespace("P:A.B.xyz"));
            Assert.AreEqual("A.B", parsing.Namespace("P:A.B.C.Item(X)"));
            Assert.AreEqual("A.B.C", parsing.Namespace("P:A.B.C.D.x"));
            Assert.AreEqual("A.B", parsing.Namespace("P:A.B.C`1.x"));
            Assert.AreEqual("A.B`1", parsing.Namespace("P:A.B`1.C.x"));
            Assert.AreEqual("A.B`10", parsing.Namespace("P:A.B`10.C`1.x"));
        }

        [Test]
        public void NamespaceFromEventTest()
        {
            Assert.IsNull(parsing.Namespace("E:A.x"));
            Assert.AreEqual("A", parsing.Namespace("E:A.B.xyz"));
            Assert.AreEqual("A.B", parsing.Namespace("E:A.B.C.x"));
            Assert.AreEqual("A.B.C", parsing.Namespace("E:A.B.C.D.x"));
            Assert.AreEqual("A.B", parsing.Namespace("E:A.B.C`1.x"));
            Assert.AreEqual("A.B`1", parsing.Namespace("E:A.B`1.C.x"));
            Assert.AreEqual("A.B`10", parsing.Namespace("E:A.B`10.C`1.x"));
        }

        [Test]
        public void TypeNameExceptionTest()
        {
            Assert.Throws<ArgumentNullException>(() => { parsing.TypeName(null); });
        }

        [Test]
        public void TypeNameNullTest()
        {
            Assert.IsNull(parsing.TypeName(""));
            Assert.IsNull(parsing.TypeName("! error"));
            Assert.IsNull(parsing.TypeName("X:unknown"));
            Assert.IsNull(parsing.TypeName("N:A.B"));
        }

        [Test]
        public void TypeNameFromTypeTest()
        {
            Assert.AreEqual("A", parsing.TypeName("T:A"));
            Assert.AreEqual("Abc", parsing.TypeName("T:Abc"));
            Assert.AreEqual("B", parsing.TypeName("T:A.B"));
            Assert.AreEqual("C", parsing.TypeName("T:A.B.C"));
            Assert.AreEqual("A´1", parsing.TypeName("T:A´1"));
            Assert.AreEqual("B´2", parsing.TypeName("T:A.B´2"));
            Assert.AreEqual("C´3", parsing.TypeName("T:A.B.C´3"));
            Assert.AreEqual("C´10", parsing.TypeName("T:A.B.C´10"));
            Assert.AreEqual("C", parsing.TypeName("T:A.B´1.C"));
            Assert.AreEqual("C`2", parsing.TypeName("T:A.B´1.C`2"));
        }

        [Test]
        public void TypeNameFromFieldTest()
        {
            Assert.AreEqual("A", parsing.TypeName("F:A.xyz"));
            Assert.AreEqual("B", parsing.TypeName("F:A.B.xyz"));
            Assert.AreEqual("C", parsing.TypeName("F:A.B.C.x"));
            Assert.AreEqual("D", parsing.TypeName("F:A.B.C.D.x"));
            Assert.AreEqual("C`1", parsing.TypeName("F:A.B.C`1.x"));
            Assert.AreEqual("C", parsing.TypeName("F:A.B`1.C.x"));
            Assert.AreEqual("C`1", parsing.TypeName("F:A.B`10.C`1.x"));
        }

        [Test]
        public void TypeNameFromMethodTest()
        {
            Assert.AreEqual("B", parsing.TypeName("M:A.B.xyz()"));
            Assert.AreEqual("C", parsing.TypeName("M:A.B.C.op_implicit(A.B.C)~A.B.X"));
            Assert.AreEqual("D", parsing.TypeName("M:A.B.C.D.x(X,Y)"));
            Assert.AreEqual("C`1", parsing.TypeName("M:A.B.C`1.x()"));
            Assert.AreEqual("C", parsing.TypeName("M:A.B`1.C.x()"));
            Assert.AreEqual("C`1", parsing.TypeName("M:A.B`10.C`1.x()"));
            Assert.AreEqual("C`1", parsing.TypeName("M:A.B`10.C`1.x``2(``0,``1)"));
        }

        [Test]
        public void TypeNameFromPropertyTest()
        {
            Assert.AreEqual("B", parsing.TypeName("P:A.B.x"));
            Assert.AreEqual("C", parsing.TypeName("P:A.B.C.x"));
            Assert.AreEqual("D", parsing.TypeName("P:A.B.C.D.x"));
            Assert.AreEqual("C`1", parsing.TypeName("P:A.B.C`1.x"));
            Assert.AreEqual("C", parsing.TypeName("P:A.B`1.C.x"));
            Assert.AreEqual("C`1", parsing.TypeName("P:A.B`10.C`1.x"));
        }

        [Test]
        public void TypeNameFromEventTest()
        {
            Assert.AreEqual("A", parsing.TypeName("E:A.xyz"));
            Assert.AreEqual("B", parsing.TypeName("E:A.B.xyz"));
            Assert.AreEqual("C", parsing.TypeName("E:A.B.C.x"));
            Assert.AreEqual("D", parsing.TypeName("E:A.B.C.D.x"));
            Assert.AreEqual("C`1", parsing.TypeName("E:A.B.C`1.x"));
            Assert.AreEqual("C", parsing.TypeName("E:A.B`1.C.x"));
            Assert.AreEqual("C`1", parsing.TypeName("E:A.B`10.C`1.x"));
        }

        [Test]
        public void MemberNameNullTest()
        {
            Assert.IsNull(parsing.MemberName("N:A.B.C"));
            Assert.IsNull(parsing.MemberName("T:A.B.C"));
            Assert.IsNull(parsing.MemberName("X:A.B.C"));
        }

        [Test]
        public void MemberNameFromFieldTest()
        {
            Assert.AreEqual("x", parsing.MemberName("F:A.x"));
            Assert.AreEqual("xyz", parsing.MemberName("F:A.B.C.xyz"));
            Assert.AreEqual("_xY", parsing.MemberName("F:A.B._xY"));
            Assert.AreEqual("xyz", parsing.MemberName("F:A.B`10.C`2.xyz"));
        }

        [Test]
        public void MemberNameFromMethodTest()
        {
            Assert.AreEqual("x", parsing.MemberName("M:A.x()"));
            Assert.AreEqual("xyz", parsing.MemberName("M:A.B.C.xyz(Aa,Bb.Cc.Dd,Ee)"));
            Assert.AreEqual("_xY``1", parsing.MemberName("M:A.B._xY``1(Aa*,Bb.Cc.Dd@,``0)~A.E"));
        }

        [Test]
        public void MemberNameFromPropertyTest()
        {
            Assert.AreEqual("x", parsing.MemberName("P:A.x"));
            Assert.AreEqual("xyz", parsing.MemberName("P:A.B.C.xyz"));
            Assert.AreEqual("_xY", parsing.MemberName("P:A.B._xY(A.C)"));
            Assert.AreEqual("xyz", parsing.MemberName("P:A.B`10.C`2.xyz(A.C*,A.D@)"));
        }

        [Test]
        public void MemberNameFromEventTest()
        {
            Assert.AreEqual("x", parsing.MemberName("E:A.x"));
            Assert.AreEqual("xyz", parsing.MemberName("E:A.B.C.xyz"));
            Assert.AreEqual("_xY", parsing.MemberName("E:A.B._xY"));
            Assert.AreEqual("xyz", parsing.MemberName("E:A.B`10.C`2.xyz"));
        }

        [Test]
        public void ReturnTypeFromMethodTest()
        {
            Assert.IsNull(parsing.ReturnType("M:A.x"));
            Assert.AreEqual("A.D", parsing.ReturnType("M:A.B.x(A.C)~A.D.E").Namespace);
            Assert.AreEqual("E", parsing.ReturnType("M:A.B.x(A.C)~A.D.E").Type);
        }
    }
}
