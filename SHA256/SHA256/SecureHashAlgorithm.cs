using System;
using System.Collections.Generic;
using System.Text;

namespace FastSHA256
{
    class SecureHashAlgorithm
    {

        //This block of K is defined the FIPS document. Always the same block.
        public static readonly UInt32[] K = new UInt32[] 
        {   0x428A2F98, 0x71374491, 0xB5C0FBCF, 0xE9B5DBA5,
            0x3956C25B, 0x59F111F1, 0x923F82A4, 0xAB1C5ED5,
            0xD807AA98, 0x12835B01, 0x243185BE, 0x550C7DC3,
            0x72BE5D74, 0x80DEB1FE, 0x9BDC06A7, 0xC19BF174,
            0xE49B69C1, 0xEFBE4786, 0x0FC19DC6, 0x240CA1CC,
            0x2DE92C6F, 0x4A7484AA, 0x5CB0A9DC, 0x76F988DA,
            0x983E5152, 0xA831C66D, 0xB00327C8, 0xBF597FC7,
            0xC6E00BF3, 0xD5A79147, 0x06CA6351, 0x14292967,
            0x27B70A85, 0x2E1B2138, 0x4D2C6DFC, 0x53380D13,
            0x650A7354, 0x766A0ABB, 0x81C2C92E, 0x92722C85,
            0xA2BFE8A1, 0xA81A664B, 0xC24B8B70, 0xC76C51A3,
            0xD192E819, 0xD6990624, 0xF40E3585, 0x106AA070,
            0x19A4C116, 0x1E376C08, 0x2748774C, 0x34B0BCB5,
            0x391C0CB3, 0x4ED8AA4A, 0x5B9CCA4F, 0x682E6FF3,
            0x748F82EE, 0x78A5636F, 0x84C87814, 0x8CC70208,
            0x90BEFFFA, 0xA4506CEB, 0xBEF9A3F7, 0xC67178F2};

        //Always the same Starting H values
        public static readonly UInt32[] InitialH = new UInt32[] { 0x6A09E667, 0xBB67AE85, 0x3C6EF372, 0xA54FF53A, 0x510E527F, 0x9B05688C, 0x1F83D9AB, 0x5BE0CD19 };

        private UInt32[] Hash = new UInt32[8];
        private UInt32[] AtoH = new UInt32[8];
        public static readonly UInt32 A = 0, B = 1, C = 2, D = 3, E = 4, F = 5, G = 6, H = 7;
        private UInt32 T1, T2;
        private UInt32[] W = new UInt32[64];

        private List<byte[]> MBlocks = new List<byte[]>();
        public List<UInt32[]> InternalStates = new List<uint[]>();
        private UInt64 BitCount;
        private UInt32 ByteCount;


        public SecureHashAlgorithm() { InitH(); }

        public void InitH()
        {
            MBlocks.Clear();
            for (int i = 0; i < 8; i++) { Hash[i] = InitialH[i]; }
            InternalStates.Add((UInt32[])Hash.Clone());
        }

        public byte[] GetHash(byte[] msg, bool useRecursion = false)
        {
            //First it finds out how many 64 byte chunks there are
            //If it is right on a boundary, there has to be an extra block
            ByteCount = (UInt32)msg.Length;
            BitCount = ByteCount * 8;
            uint blocks = ByteCount / 64;
            if (blocks << 6 <= ByteCount) ++blocks;
            int b = 0;
            byte[] m;
            for (; b<blocks-1; ++b)
            {
                m = new byte[64];
                Array.Copy(msg, b << 6, m, 0, 64);
                MBlocks.Add(m);
            }

            m = new byte[64];
            int remaining = (int)(ByteCount - (b << 6));

            Array.Copy(msg, b << 6, m, 0, remaining);
            m[remaining] = 0x80;
            //If there are too many bytes remaining
            //We need to add this to the block list and 
            //add an extra one for the bit count padding
            if(remaining>=56)
            {
                MBlocks.Add(m);
                m = new byte[64];
            }
            int j = 56;
            for (int i = 56; i < 64; i++)
            {
                m[i] = (byte)((BitCount >> j) & 0xFF);
                j -= 8;
            }
            MBlocks.Add(m);

            //Now we process each 64byte block
            foreach(byte[] M in MBlocks)
            {
                PerformHash(M, useRecursion);
            }            

            byte[] hash = new byte[32];
            //Convert UInts to Bytes
            for(int h=0;h<hash.Length;h+=4)
            {
                hash[h] = (byte)(Hash[h >> 2]>>24 & 0xFF );
                hash[h+1] = (byte)(Hash[h >> 2]>>16 & 0xFF);
                hash[h+2] = (byte)(Hash[h >> 2]>>8 & 0xFF);
                hash[h+3] = (byte)(Hash[h >> 2] & 0xFF);
            }

            return hash;

        }

        private UInt32 GetUIntChunk(byte[] m, int offset)
        {
            return ((UInt32)m[offset] << 24 | (UInt32)m[offset+1] << 16 | (UInt32)m[offset+2] << 8 | (UInt32)m[offset+3]);
        }

        private void PerformHash(byte[] M, bool useRecursion = false)
        {
            for(int i =0; i< Hash.Length;++i)
            {
                AtoH[i] = Hash[i];
            }
            int t;
            for (t=0; t<16; ++t)
            {
                W[t] = GetUIntChunk(M,t<<2);
            }
            for (; t < 64; ++t)
            {
                W[t] = ((Gamma1256(W[t - 2]) + W[t - 7] + Gamma0256(W[t - 15]) + W[t - 16]));
            }
            //This is a slight deviation from the FIPS, I just add all the ks to the ws first
            //since W and K are both constant during the loops
            for (t = 0; t < 64; ++t)
            {
                W[t] += K[t];
            }
            if (useRecursion)
            {
                AtoH = RecurseHash(AtoH[A], AtoH[B], AtoH[C], AtoH[D], AtoH[E], AtoH[F], AtoH[G], AtoH[H], 0);
            }
            else
            {
                for (int i = 0; i < 64; i++)
                {
                    T1 = AtoH[H] + Sigma1256(AtoH[E]) + Ch(AtoH[E], AtoH[F], AtoH[G]) + W[i];
                    T2 = Sigma0256(AtoH[A]) + Maj(AtoH[A], AtoH[B], AtoH[C]);
                    AtoH[H] = AtoH[G];
                    AtoH[G] = AtoH[F];
                    AtoH[F] = AtoH[E];
                    AtoH[E] = AtoH[D] + T1;
                    AtoH[D] = AtoH[C];
                    AtoH[C] = AtoH[B];
                    AtoH[B] = AtoH[A];
                    AtoH[A] = T1 + T2;
                    InternalStates.Add((UInt32[])AtoH.Clone()); //Just for monitoring how the bits flow and change
                }
            }
            for (int i=0;i<Hash.Length;i++)
            {
                Hash[i] += AtoH[i];
            }
            InternalStates.Add((UInt32[])Hash.Clone());
        }

        private UInt32[] RecurseHash(UInt32 A, UInt32 B, UInt32 C, UInt32 D, UInt32 E, UInt32 F, UInt32 G, UInt32 H, int depth)
        {
            if (depth == 64) return new UInt32[] { A, B, C, D, E, F, G, H };
            InternalStates.Add(new UInt32[] { A, B, C, D, E, F, G, H });
            T1 = H + Sigma1256(E) + Ch(E, F, G) + W[depth];
            T2 = Sigma0256(A) + Maj(A, B, C);
            return RecurseHash(T1 + T2, A, B, C, D + T1, E, F, G, depth+1);
        }





        public static UInt32 RotateLeft(UInt32 value, int bits)
        {
            return (value << bits) | (value >> (32 - bits));
        }
      
        public static UInt32 RotateRight(UInt32 value, int bits)
        {
            return (value >> bits) | (value << (32 - bits));
        }

        public static UInt32 ShiftRight(UInt32 value, int bits)
        {
            return (value >> bits);
        }

        public static UInt32 Ch(UInt32 x, UInt32 y, UInt32 z)
        {
            return ((x & y) ^ ((~x) & z));
        }

        public static UInt32 Maj(UInt32 x, UInt32 y, UInt32 z)
        {
            return ((x & y) ^ (x & z) ^ (y & z));
        }

        public static UInt32 Sigma0256(UInt32 x)
        {
            return (RotateRight(x, 2) ^ RotateRight(x, 13) ^ RotateRight(x, 22));
        }

        public static UInt32 Sigma1256(UInt32 x)
        {
            return (RotateRight(x, 6) ^ RotateRight(x, 11) ^ RotateRight(x, 25));
        }

        public static UInt32 Gamma0256(UInt32 x)
        {
            return (RotateRight(x, 7) ^ RotateRight(x, 18) ^ ShiftRight(x, 3));
        }

        public static UInt32 Gamma1256(UInt32 x)
        {
            return (RotateRight(x, 17) ^ RotateRight(x, 19) ^ ShiftRight(x, 10));
        }
    }
}
