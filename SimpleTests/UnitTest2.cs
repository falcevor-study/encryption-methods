using System;
using System.Text;
using EncryptionTool.Model.CRYPTON;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SimpleTests
{
    [TestClass]
    public class UnitTest2
    {
        [TestMethod]
        public void TestMethod1()
        {
            var sourceKey = new byte[] { 13, 4, 43, 34, 61, 14, 145, 44 };
            var expander = new CryptonKeyExpander();
            var result = expander.Expand(sourceKey);
            Assert.AreEqual(result.Length, 13 * 128 / 8);
        }

        [TestMethod]
        public void HelloWorld()
        {
            var sourceKey = new byte[] { 13, 4, 43, 34, 61, 14, 145, 44 };
            var method = new CryptonMethod(sourceKey);

            var sourceText = "Hello world";
            var sourceBytes = Encoding.UTF8.GetBytes(sourceText);

            var encoded = method.Encode(sourceBytes);
            var encodedText = Encoding.UTF8.GetString(encoded);
            Console.WriteLine(encodedText);

            var decoded = method.Decode(encoded);
            var decodedText = Encoding.UTF8.GetString(decoded);
            Console.WriteLine(decodedText);
        }


        [TestMethod]
        public void TableTransform()
        {
            var source = new byte[,]
            {
                { 1, 2, 3, 4 },
                { 5, 6, 7, 8 },
                { 9, 10, 11, 12 },
                { 13, 14, 15, 16 }
            };

            var result = CryptonMethod.TableReplace(source, RoundTypes.EvenRound, false);
            var result2 = CryptonMethod.TableReplace(result, RoundTypes.EvenRound, true);
            var result3 = CryptonMethod.TableReplace(result, RoundTypes.OddRound, false);
            var result4 = CryptonMethod.TableReplace(result, RoundTypes.OddRound, true);
        }
    }
}
