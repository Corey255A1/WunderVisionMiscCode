using System;
using System.Security.Cryptography;
using System.Text;
using System.Diagnostics;
namespace FastSHA256
{
    class Program
    {

        static void PrintBytes(byte[] b)
        {
            foreach(byte i in b)
            {
                Console.Write(i.ToString("X2"));
            }
            Console.WriteLine();
        }

        static void Main(string[] args)
        { 
            Console.WriteLine("Hello World!");

            string TestString = "Hello This is a Test of the Byte Block Message. This is 64 Bytes";
            byte[] TestBytes = Encoding.UTF8.GetBytes(TestString);
            byte[] calcBytes;
            byte[] baseBytes;
            //My Version of SHA
            SecureHashAlgorithm sha = new SecureHashAlgorithm();

            //Built in version of SHA
            SHA256 mySHA256 = SHA256.Create();

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            baseBytes =  mySHA256.ComputeHash(TestBytes);
            stopWatch.Stop();
            PrintBytes(baseBytes);
            Console.WriteLine(stopWatch.ElapsedTicks);

            stopWatch.Restart();
            calcBytes = sha.GetHash(TestBytes);
            stopWatch.Stop();
            PrintBytes(calcBytes);
            Console.WriteLine(stopWatch.ElapsedTicks);

            sha.InitH();
            stopWatch.Restart();
            calcBytes = sha.GetHash(TestBytes,true);
            stopWatch.Stop();
            PrintBytes(calcBytes);
            Console.WriteLine(stopWatch.ElapsedTicks);

            Console.ReadKey();

        }




    }
}
