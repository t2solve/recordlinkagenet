using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;

namespace RecordLinkageNet.Util
{
    public class HashValueFactory
    {
        private static readonly HashAlgorithm algoHash = new SHA512Managed(); //TODO check use 1 ? 

        public static bool CheckFileHasSha512Value(string shaToCheck, string filename)
        {
            bool success = false;
            string shaWeDoHave = GetSha512Value(filename);
            if (string.Compare(shaWeDoHave, shaToCheck) == 0)
                success = true;
            return success;
        }
        public static string GetSha512Value(string filename)
        {
            string retValue = null;
            byte[] values = GetHashSha512FromFile(filename);
            if (values == null)
                return retValue;

            retValue = BytesToString(values);
            return retValue;
        }
        private static byte[] GetHashSha512FromFile(string filename)
        {
            //HashAlgorithm algoHash = new SHA512Managed(); //TODO check use 1 ? 
            byte[] foo = null;
            try
            {
                using (FileStream stream = File.OpenRead(filename))
                {
                    foo = new byte[stream.Length];
                    foo = algoHash.ComputeHash(stream);
                    stream.Close();
                    stream.Dispose();

                    return foo;
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.ToString());
                return foo;
            }
        }

        // Return a byte array as a sequence of hex values.
        private static string BytesToString(byte[] bytes)
        {
            string result = "";
            foreach (byte b in bytes) result += b.ToString("x2");
            return result;
        }


        private static byte[] StringToByteArray(string str)
        {
            System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
            return enc.GetBytes(str);
        }

        private static byte[] GenerateSaltedHash(byte[] plainText, byte[] salt)
        {
            byte[] plainTextWithSaltBytes = new byte[plainText.Length + salt.Length];

            for (int i = 0; i < plainText.Length; i++)
            {
                plainTextWithSaltBytes[i] = plainText[i];
            }
            for (int i = 0; i < salt.Length; i++)
            {
                plainTextWithSaltBytes[plainText.Length + i] = salt[i];
            }

            return algoHash.ComputeHash(plainTextWithSaltBytes);
        }

        public static string CalcStringTest(string input, string saltIn)
        {
            string s2 = "";

            //we prepate sec
            byte[] salt = StringToByteArray(saltIn); // GetHashSha512FromFile(f); TODO save salt in other place
            byte[] pw = StringToByteArray(input);
            byte[] s = null;
            s = GenerateSaltedHash(pw, salt);
            s2 = BytesToString(s);

            return s2;
        }
    }
}
