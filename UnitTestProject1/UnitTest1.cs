using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClassLibrary1;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod2()
        {
            Assert.IsTrue(new Password("qweqwe1+").CheckPassword() == "");
            Assert.IsTrue(new Password("qweasd2}").CheckPassword() == "");
            Assert.IsTrue(new Password("qweasd2}").CheckPassword() == "");
            Assert.IsTrue(new Password("qw2}rrrrwww3ww").CheckPassword() == "");
            Assert.IsTrue(new Password("qweasd5}").CheckPassword() == "");



            Assert.IsFalse(new Password("1").CheckPassword() == "");
            Assert.IsFalse(new Password("111111").CheckPassword() == "");
            Assert.IsFalse(new Password("qqqqqq").CheckPassword() == "");
            Assert.IsFalse(new Password("}}qw2").CheckPassword() == "");
            Assert.IsFalse(new Password("qazxsw").CheckPassword() == "");
        }
    }
}
