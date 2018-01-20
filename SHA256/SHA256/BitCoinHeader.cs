using System;
using System.Collections.Generic;
using System.Text;

namespace FastSHA256
{
    class BitCoinHeader
    {
        string version = "02000000"; //v2
        string hashPrevBlock = "0affed3fc96851d8c74391c2d9333168fe62165eb228bced7e00000000000000";
        string merkleRoot = "4277b65e3bd527f0ceb5298bdb06b4aacbae8a4a808c2c8aa414c20f252db801";
        string time = "130dae51";
        string difficulty = "6461011a";
        string successNonce = "3aeb9bb8"; //3aeb9bb8 -> 3097226042

        static Dictionary<char, UInt32> HexCharToByte = new Dictionary<char, UInt32>() {
            { 'F',15 },
            { 'E',14 },
            { 'D',13 },
            { 'C',12 },
            { 'B',11 },
            { 'A',10 },
            { 'f',15 },
            { 'e',14 },
            { 'd',13 },
            { 'c',12 },
            { 'b',11 },
            { 'a',10 },
            { '9',9 },
            { '8',8 },
            { '7',7 },
            { '6',6 },
            { '5',5 },
            { '4',4 },
            { '3',3 },
            { '2',2 },
            { '1',1 },
            { '0',0 }
        };

        public BitCoinHeader(UInt32 nonce)
        {
            successNonce = nonce.ToString("X8");
        }

        public byte[] GetBytes()
        {
            return HexStringToBytes(version + hashPrevBlock + merkleRoot + time + difficulty + successNonce);
        }

        public static byte[] HexStringToBytes(string s)
        {
            if (s.Length % 2 != 0) s = '0' + s;
            byte[] bytes = new byte[s.Length / 2];
            for(int i=0;i< bytes.Length; i++)
            {
                bytes[i] = (byte)((HexCharToByte[s[(i*2)]] << 4) | (HexCharToByte[s[(i*2) + 1]]));
            }
            return bytes;
        }
    }
}
