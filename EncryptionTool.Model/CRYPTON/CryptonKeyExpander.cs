using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncryptionTool.Model.CRYPTON
{
    public class CryptonKeyExpander : IKeyExpander
    {
        public byte[] Expand(byte[] sourceKey)
        {
            // 12 128-bit keys for 12 rounds and 1 for prepare operation
            var result = new byte[13 * 128 / 8];

            // add zeros to source key for expanding to 256 bit length
            var preparedKey = new byte[32];
            sourceKey.CopyTo(preparedKey, 0);

            var U = new byte[16]; // for evens
            var V = new byte[16]; // for odds

            for (int i = 0; i < 16; ++i)
            {
                U[i] = preparedKey[i * 2];
                V[i] = preparedKey[1 + i * 2];
            }

            var zeroKey = new byte[16];
            // Do even round with U vector
            U = CryptonMethod.DoRound(RoundTypes.OddRound, U, zeroKey);
            // Do odd round with V vector
            U = CryptonMethod.DoRound(RoundTypes.EvenRound, V, zeroKey);

            var T0 = new byte[4];
            var T1 = new byte[4];

            for (int i = 0; i < 4; ++i)
            {
                T0[i] = (byte)(U[4 * i] ^ U[4 * i + 1] ^ U[4 * i + 2] ^ U[4 * i + 3]);
                T1[i] = (byte)(V[4 * i] ^ V[4 * i + 1] ^ V[4 * i + 2] ^ V[4 * i + 3]);
            }

            for (int i = 0; i < result.Length; ++i)
            {
                result[i] = (byte)(T0[i % 4] ^ T1[i % 4] ^ U[i % 16] ^ V[i % 16]);
            }

            return result;
        }
    }
}
