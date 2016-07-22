using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Mastersign.XmlDoc
{
    [TestFixture]
    public class CRefFormattingTest
    {

        [Test]
        public void LabelMethodTest()
        {
            var f = new CRefFormatting();

            Assert.AreEqual("x()", f.Label("M:A.B.x"));
            Assert.AreEqual("x<?, ?>(int, B)", f.Label("M:A.B.x`2(System.Int32,A.B)"));
        }

        [Test]
        public void FullLabelMethodTest()
        {
            var f = new CRefFormatting();

            Assert.AreEqual("A.B.x()", f.FullLabel("M:A.B.x"));
            Assert.AreEqual("A.B.x<?, ?>(System.Int32, A.B)", f.FullLabel("M:A.B.x`2(System.Int32,A.B)"));
        }

        [Test]
        public void UrlTest()
        {
            var f = new CRefFormatting();
            f.UrlBase = "BASE/";
            f.UrlFileNameExtension = ".ext";

            Assert.AreEqual("BASE/ns_Abc.Def.ext", f.Url("N:Abc.Def"));
            Assert.AreEqual("BASE/Ab.Cd.ext#Method1", f.Url("M:Ab.Cd`1.Method1(`0)"));
        }
    }
}
