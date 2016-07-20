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
    }
}
