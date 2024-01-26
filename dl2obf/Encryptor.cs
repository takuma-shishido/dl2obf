using System;
using System.Security.Cryptography;
using System.Text;

namespace dl2obf
{
    public class StringObfuscator
    {
        static string EncryptHash = "";
        public static string ObfuscateString(string inputStr)
        {
            if (StringObfuscator.EncryptHash == "") StringObfuscator.EncryptHash = Guid.NewGuid().ToString("N").Substring(0, 10);

            using (SHA1 sha1 = SHA1.Create())
            {
                byte[] array = Encoding.Unicode.GetBytes(StringObfuscator.EncryptHash + inputStr);
                byte[] hashVal = sha1.ComputeHash(array);
                return Sha1ToBeeStr(hashVal);
            }
        }

        private static string Sha1ToBeeStr(byte[] hashVal)
        {
            StringBuilder beeStr = new StringBuilder();
            int num2 = 4;
            int num3 = 0;
            int i = 0;

            while (i < 6)
            {
                int num4 = hashVal[i];
                int j = 8;

                while (j > 0)
                {
                    if (num2 == 0)
                    {
                        beeStr.Append(GetChar(65 + num3));
                        num3 = 0;
                        num2 = 4;
                    }

                    int num5 = Math.Min(j, num2);
                    int num6 = 1 << num5;
                    num3 += num4 % num6;
                    num2 -= num5;
                    num3 <<= num2;
                    j -= num5;
                    num4 >>= num5;
                }

                i++;
            }
            return beeStr.ToString().Substring(0, 11);
        }

        private static char GetChar(int value)
        {
            int num = 0;
            foreach (int num2 in new int[] { 33, 35, 36, 37, 38, 47, 92, 95, 46, 0 })
            {
                if (value + num >= num2 && num2 >= 65)
                {
                    num++;
                }
            }

            return (char)(value + num);
        }
    }
}
