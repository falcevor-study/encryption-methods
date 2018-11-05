using System;
using System.Collections.Generic;
namespace EncryptionTool.Model
{
    public class PikeMethod : IEncryptionMethod
    {
        private IGenerator _gen1;
        private IGenerator _gen2;
        private IGenerator _gen3;
        private ISyncScheme _sync;

        private byte[] _reg1;
        private byte[] _reg2;
        private byte[] _reg3;

        public PikeMethod(byte[] reg1, byte[] reg2, byte[] reg3)
        {
            _reg1 = reg1;
            _reg2 = reg2;
            _reg3 = reg3;


            if (reg1.Length < 56 || reg2.Length < 58 || reg3.Length < 59)
                throw new Exception("Keys is too short. Minimal sezes is 56, 58 and 59");
        }

        private void Reset()
        {
            _gen1 = new AdditiveGenerator(_reg1, 55, 24, (int)Math.Pow(2, 32));
            _gen2 = new AdditiveGenerator(_reg1, 57, 7, (int)Math.Pow(2, 32));
            _gen3 = new AdditiveGenerator(_reg1, 58, 19, (int)Math.Pow(2, 32));
            _sync = new PikeSyncScheme(_gen1, _gen2, _gen3);
        }

        public byte[] Decode(byte[] text)
        {
            return Encode(text);
        }

        public byte[] Encode(byte[] text)
        {
            Reset();

            var result = new List<byte>();

            foreach (var x in text)
            {
                var gamma = GetNextGammaItem();
                result.Add((byte)(x ^ gamma));
            }

            return result.ToArray();
        }

        private byte GetNextGammaItem()
        {
            var result = (byte)(_gen1.GetValue() ^ _gen2.GetValue() ^ _gen3.GetValue());
            _sync.ShiftGenerators();
            return result;
        }
    }
}
