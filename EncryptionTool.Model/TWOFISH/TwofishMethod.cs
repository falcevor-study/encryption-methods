using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncryptionTool.Model.TWOFISH
{
    public class TwofishMethod
    {
        private readonly byte[,] MDS = 
        {
            { 0x01, 0xEF, 0x5B, 0x5B },
            { 0x5B, 0xEF, 0xEF, 0x01 },
            { 0xEF, 0x5B, 0x01, 0xEF },
            { 0xEF, 0x01, 0xEF, 0x5B }
        };

        private readonly byte[,] RS =
        {
            { 0x01, 0xA4, 0x55, 0x87, 0x5A, 0x58, 0xDB, 0x9E },
            { 0xA4, 0x56, 0x82, 0xF3, 0x1E, 0xC6, 0x68, 0xE5 },
            { 0x02, 0xA1, 0xFC, 0xC1, 0x47, 0xAE, 0x3D, 0x19 },
            { 0xA4, 0x55, 0x87, 0x5A, 0x58, 0xDB, 0x9E, 0x03 }
        };


        private readonly int polynomial = 0b101101001; // x^8 + x^6 -+ x^5 + x^3 + 1

        private int _keySize;
        private int[] _key;

        public TwofishMethod(int keySize, int[] key)
        {
            this._keySize = keySize;
            this._key = key;
        }

        public byte[] Decode(byte[] text)
        {
            return Encode(text);
        }

        public byte[] Encode(byte[] bytes)
        {
            var text = new List<int>();

            int count = 0;
            byte[] word = new byte[4];

            foreach (var x in bytes)
            {
                word[count++] = x;
                if (count == 4)
                {
                    text.Add(BitConverter.ToInt32(word, 0));
                    count = 0;
                }
            }

            if (count != 0)
            {
                for (; count < 4; count++)
                {
                    word[count] = 0;
                }
                text.Add(BitConverter.ToInt32(word, 0));
            }

            var result = new List<int>();

            int cur = 0;
            int[] block = new int[4];

            foreach(var x in text)
            {
                block[cur++] = x;
                if (cur == 4)
                {
                    result.AddRange(EncodeBlock(block));
                    block = new int[4];
                    cur = 0;
                }
            }

            if (cur != 0)
            {
                for (; cur < 4; ++cur)
                {
                    block[cur] = 0;
                }
                result.AddRange(EncodeBlock(block));
            }

            return result.SelectMany(x => BitConverter.GetBytes(x)).ToArray();
        }

        private int[] EncodeBlock(int[] block)
        {
            if (block.Length != 4)
                throw new ArgumentException("Block size must be 128 bits");

            int[] K;
            int[] V;

            GenerateKeys(_key, out K, out V);

            Whitering(block, K);

            for (int i = 0; i < 16; ++i)
            {
                DoRound(i, block, K, V);
            }

            Whitering(block, K);

            return block;
        }

        private void DoRound(int r, int[] block, int[] K, int[] V)
        {
            block[1] = LeftShift(block[1], 8);
            block[0] = h(block[0], V);
            block[1] = h(block[1], V);
            block[0] = SumByMod(block[0], block[1]);
            block[1] = SumByMod(block[0], block[1]);
            block[0] = SumByMod(block[0], K[2 * r + 8]);
            block[1] = SumByMod(block[1], K[2 * r + 9]);
            block[2] = block[2] ^ block[0];
            block[3] = LeftShift(block[3], 1);
            block[3] = block[3] ^ block[1];
            block[2] = RightShift(block[2], 1);
        }

        #region h function
        private int h(int word, int[] key)
        {
            var result = word;
            var frags = BitConverter.GetBytes(result);

            if (_keySize == 256)
            {
                frags[0] = q1(frags[0]);
                frags[1] = q0(frags[1]);
                frags[2] = q0(frags[2]);
                frags[3] = q1(frags[3]);
                result = BitConverter.ToInt32(frags, 0) + key[3];
                frags = BitConverter.GetBytes(result);
            }

            if (_keySize >= 192)
            {
                frags[0] = q1(frags[0]);
                frags[1] = q1(frags[1]);
                frags[2] = q0(frags[2]);
                frags[3] = q0(frags[3]);
                result = BitConverter.ToInt32(frags, 0) + key[2];
                frags = BitConverter.GetBytes(result);
            }

            frags[0] = q0(frags[0]);
            frags[1] = q1(frags[1]);
            frags[2] = q0(frags[2]);
            frags[3] = q1(frags[3]);
            result = BitConverter.ToInt32(frags, 0) + key[1];
            frags = BitConverter.GetBytes(result);

            frags[0] = q0(frags[0]);
            frags[1] = q0(frags[1]);
            frags[2] = q1(frags[2]);
            frags[3] = q1(frags[3]);
            result = BitConverter.ToInt32(frags, 0) + key[0];
            frags = BitConverter.GetBytes(result);

            frags[0] = q1(frags[0]);
            frags[1] = q0(frags[1]);
            frags[2] = q1(frags[2]);
            frags[3] = q0(frags[3]);
            result = BitConverter.ToInt32(MultByMDS(frags), 0);

            return result;
        }

        private byte q0(byte x)
        {
            var table = new byte[,]
            {
                { 0x8, 0x1, 0x7, 0xD, 0x6, 0xF, 0x3, 0x2, 0x0, 0xB, 0x5, 0x9, 0xE, 0xC, 0xA, 0x4 },
                { 0xE, 0xC, 0xB, 0x8, 0x1, 0x2, 0x3, 0x5, 0xF, 0x4, 0xA, 0x6, 0x7, 0x0, 0x9, 0xD },
                { 0xB, 0xA, 0x5, 0xE, 0x6, 0xD, 0x9, 0x0, 0xC, 0x8, 0xF, 0x3, 0x2, 0x4, 0x7, 0x1 },
                { 0xD, 0x7, 0xF, 0x4, 0x1, 0x2, 0x6, 0xE, 0x9, 0xB, 0x3, 0x0, 0x8, 0x5, 0xC, 0xA }
            };

            return q(x, table);
        }

        private byte q1(byte x)
        {
            var table = new byte[,]
            {
                { 0x2, 0x8, 0xB, 0xD, 0xF, 0x7, 0x6, 0xE, 0x3, 0x1, 0x9, 0x4, 0x0, 0xA, 0xC, 0x5 },
                { 0x1, 0xE, 0x2, 0xB, 0x4, 0xC, 0x3, 0x7, 0x6, 0xD, 0xA, 0x5, 0xF, 0x9, 0x0, 0x8 },
                { 0x4, 0xC, 0x7, 0x5, 0x1, 0x6, 0x9, 0xA, 0x0, 0xE, 0xD, 0x8, 0x2, 0xB, 0x3, 0xF },
                { 0xB, 0x9, 0x5, 0x1, 0xC, 0x3, 0xD, 0xE, 0x6, 0x4, 0x7, 0xF, 0x2, 0x0, 0x8, 0xA }
            };

            return q(x, table);
        }

        private byte q(byte x, byte[,] table)
        {
            byte a0 = (byte)(x / 16);
            byte b0 = (byte)(x % 16);
            byte a1 = (byte)(a0 ^ b0);
            byte b1 = (byte)((a0 ^ RightShift4(b0, 1) ^ (8 * a0)) % 16);
            byte a2 = (byte)(table[0, a1]);
            byte b2 = (byte)(table[1, b1]);
            byte a3 = (byte)(a2 ^ b2);
            byte b3 = (byte)((a2 ^ RightShift4(b2, 1) ^ (8 * a2)) % 16); ;
            byte a4 = (byte)(table[2, a3]);
            byte b4 = (byte)(table[3, b3]);

            return (byte)(16 * b4 + a4);
        }
        #endregion

        private void GenerateKeys(int[] key, out int[] K, out int[] V)
        {
            int p = (int)(Math.Pow(2, 24) + Math.Pow(2, 16) + Math.Pow(2, 8) + 1);
            K = new int[40];

            var MeList = new List<int>();
            var M0List = new List<int>();
            var sourceKey = key.SelectMany(x => BitConverter.GetBytes(x)).ToArray();

            byte[] word = new byte[4];

            int count = 0;

            for (int i = 0; i < sourceKey.Length; ++i)
            {
                if (i % 4 == 0 && i != 0)
                {
                    (count % 2 == 1 ? M0List : MeList).Add(BitConverter.ToInt32(word, 0));
                    word = new byte[4];
                    count++;
                }
                word[3 - (i % 4)] = sourceKey[i];
            }

            var Me = MeList.ToArray();
            var M0 = M0List.ToArray();

            for (int i = 0; i < 20; ++i)
            {
                var a = h(2 * i * p, Me);
                var b = LeftShift(h((2 * i + 1) * p, M0), 8);
                K[2 * i] = SumByMod(a, b);
                K[2 * i + 1] = LeftShift(SumByMod(a, 2 * b), 9);
            }

            V = new int[_keySize / 64];

            for (int i = 0; i < V.Length; ++i)
            {
                var vector = new byte[8];
                for (int j = 0; j < 8; ++j)
                    vector[j] = sourceKey[8 * i + j];

                vector = MultByRS(vector);

                V[i] = BitConverter.ToInt32(vector, 0);
            }

        }

        private void Whitering(int[] text, int[] key)
        {
            for (int i = 0; i < text.Length; ++i)
                text[i] = text[i] ^ key[i % key.Length];
        }

        // Mult by MDS mutrix with mod by polynomial of Galois field.
        private byte[] MultByMDS(byte[] word)
        {
            if (word.Length != 4)
                throw new ArgumentException("Word must be 4-bytes");

            var result = new int[4];

            for (int i = 0; i < 4; ++i)
            {
                for (int j = 0; j < 4; ++j)
                {
                    result[i] += word[j] * MDS[i, j];
                }
            }

            return result.Select(x => Mod(x, polynomial)).ToArray();
        }

        private byte[] MultByRS(byte[] word)
        {
            if (word.Length != 8)
                throw new ArgumentException("Word must be 8-bytes");

            var result = new int[4];

            for (int i = 0; i < 4; ++i)
            {
                for (int j = 0; j < 8; ++j)
                {
                    result[i] += word[j] * RS[i, j];
                }
            }

            return result.Select(x => Mod(x, polynomial)).ToArray();
        }

        #region utils
        public static int SumByMod(int x, int y)
        {
            return (int)(((long)x + (long)y) % (long)Math.Pow(2, 32));
        }

        public static int RightShift(int x, int n)
        {
            return (x >> n) | (x << (32 - n));
        }

        public static int LeftShift(int x, int n)
        {
            return (x << n) | (x >> (32 - n));
        }

        public static byte RightShift4(byte x, int n)
        {
            var source = new BitArray(new byte[] { x });
            var result = new BitArray(new byte[] { x });

            for (int k = 0; k < 4; ++k)
            {
                int index = k + n % 4 < 4 ? k + n % 4 : k + n % 4 - 4;
                result[k] = source[index];
            }

            var byteResult = new byte[1];
            result.CopyTo(byteResult, 0);
            return byteResult[0];
        }

        public static byte LeftShift4(byte x, int n)
        {
            var source = new BitArray(new byte[] { x });
            var result = new BitArray(new byte[] { x });

            for (int k = 0; k < 4; ++k)
            {
                int index = k - n % 4 >= 0 ? k - n % 4 : 4 + k - n % 4;
                result[k] = source[index];
            }

            var byteResult = new byte[1];
            result.CopyTo(byteResult, 0);
            return byteResult[0];
        }

        // Mod in Galois field by polynomial b
        public static byte Mod(int a, int b)
        {
            int[] bitA = Convert.ToString(a, 2)
                         .Select(c => int.Parse(c.ToString()))
                         .ToArray();

            int[] bitB = Convert.ToString(b, 2)
                         .Select(c => int.Parse(c.ToString()))
                         .ToArray();

            for (int k = 0; k < bitA.Length; ++k)
            {
                if (bitA.Length - k < bitB.Length)
                    break;

                if (bitA[k] == 1)
                {
                    for (int i = k; i < k + bitB.Length; ++i)
                    {
                        bitA[i] ^= bitB[i - k];
                    }
                }
            }

            return (byte)Convert.ToInt16(string.Join("", bitA), 2);
        }
        #endregion
    }
}
