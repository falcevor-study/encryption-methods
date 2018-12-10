using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EncryptionTool.Model.TWOFISH;
using System.Linq;

namespace SimpleTests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class Twofish
    {
        public Twofish()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void CheckGaluaFieldMod()
        { 
            Assert.AreEqual(TwofishMethod.Mod(103, 15), 1);
            Assert.AreEqual(TwofishMethod.Mod(79, 13), 4);
        }

        [TestMethod]
        public void CheckShift()
        {
            int x = 42345;
            int y = TwofishMethod.LeftShift(x, 100);
            Assert.AreEqual(TwofishMethod.RightShift(y, 100), x);

            Assert.AreEqual(TwofishMethod.RightShift4(8, 3), 1);
            Assert.AreEqual(TwofishMethod.RightShift4(10, 3), 5);
            Assert.AreEqual(TwofishMethod.RightShift4(11, 4), 11);
            Assert.AreEqual(TwofishMethod.RightShift4(11, 2), 14);

            Assert.AreEqual(TwofishMethod.LeftShift4(1, 3), 8);
            Assert.AreEqual(TwofishMethod.LeftShift4(5, 3), 10);
            Assert.AreEqual(TwofishMethod.LeftShift4(11, 4), 11);
            Assert.AreEqual(TwofishMethod.LeftShift4(14, 2), 11);
        }

        [TestMethod]
        public void CheckTwofish()
        {
            var key = new int[32];
            var random = new Random();
            for (int i = 0; i < key.Length; ++i)
                key[i] = random.Next();

            var method = new TwofishMethod(256, key);

            var sourceBytes = Encoding.UTF8.GetBytes("Hello world");
            var encoded = method.Encode(sourceBytes);
            var decoded = method.Decode(encoded);
            var decodedText = Encoding.UTF8.GetString(decoded);
        }
    }
}
