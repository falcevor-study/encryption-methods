using System;
using System.Text;
using EncryptionTool.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SimpleTests
{
    [TestClass]
    public class Pike_DecodedEqualsSource
    {
        private byte[] _reg1;
        private byte[] _reg2;
        private byte[] _reg3;

        private IEncryptionMethod _method;

        public Pike_DecodedEqualsSource()
        {
            _reg1 = Encoding.ASCII.GetBytes("asdfasdgasfgsdfgsdfgsdfgsdfgsdfgggggggggggggsdfgjhfgjhfsdfgjfgsdfg");
            _reg2 = Encoding.ASCII.GetBytes("gg5wgdg5w4tg6yhe245q34r3w4tgw54gw45hw45w45hwtgw5thffh3hgjfgfgjhfgewrgdfg");
            _reg3 = Encoding.ASCII.GetBytes("dgw5eg5wgw4gwdgsfdgsdfg5wg5wg5gwdgwergrwegwerg5wjghfgfgjhgfjh4gwfsdgsdfg");

            _method = new PikeMethod(_reg1, _reg2, _reg3);
        }

        [TestMethod]
        public void CheckEquality()
        {
            var sourceText = "Проверка. Check. 1234567890-=]\'/.';[,♀☻♥♦☺";
            var sourceBytes = Encoding.UTF8.GetBytes(sourceText);

            var encodedBytes = _method.Encode(sourceBytes);
            var decodedBytes = _method.Decode(encodedBytes);

            var decodedText = Encoding.UTF8.GetString(decodedBytes);

            Assert.AreEqual(sourceText, decodedText);
        }
    }
}
