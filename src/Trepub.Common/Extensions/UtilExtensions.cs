using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Logging;
using Trepub.Common.Facade;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace Trepub.Common.Extensions
{
    public static class UtilExtensions
    {
        public static String ToJalaliDateCompactForm(this DateTime dateTime)
        {
            PersianCalendar pc = new PersianCalendar();
            string result = String.Format("{0}{1}{2}",
                pc.GetYear(dateTime),
                pc.GetMonth(dateTime).ToString().PadLeft(2, '0'),
                pc.GetDayOfMonth(dateTime).ToString().PadLeft(2, '0'));
            return result;
        }
        public static String ToJalaliDateTimeCompactForm(this DateTime dateTime)
        {
            PersianCalendar pc = new PersianCalendar();
            string result = String.Format("{0}{1}{2}{3}{4}{5}{6}",
                pc.GetYear(dateTime),
                pc.GetMonth(dateTime).ToString().PadLeft(2, '0'),
                pc.GetDayOfMonth(dateTime).ToString().PadLeft(2, '0'),
                pc.GetHour(dateTime).ToString().PadLeft(2, '0'),
                pc.GetMinute(dateTime).ToString().PadLeft(2, '0'),
                pc.GetSecond(dateTime).ToString().PadLeft(2, '0'),
                pc.GetMilliseconds(dateTime).ToString().PadLeft(3, '0')
                );
            return result;
        }

        //for export data (dd/mm/yyyy)
        public static String ToJalaliDate(this DateTime dateTime)
        {
            PersianCalendar pc = new PersianCalendar();
            string result = String.Format("{2}/{1}/{0}",
                pc.GetYear(dateTime),
                pc.GetMonth(dateTime).ToString().PadLeft(2, '0'),
                pc.GetDayOfMonth(dateTime).ToString().PadLeft(2, '0'));
            return result;
        }

        public static String ToJalaliDateTime(this DateTime dateTime)
        {
            PersianCalendar pc = new PersianCalendar();
            string result = String.Format("{2}/{1}/{0} {3}:{4}:{5}.{6}",
                pc.GetYear(dateTime),
                pc.GetMonth(dateTime).ToString().PadLeft(2, '0'),
                pc.GetDayOfMonth(dateTime).ToString().PadLeft(2, '0'),
                pc.GetHour(dateTime).ToString().PadLeft(2, '0'),
                pc.GetMinute(dateTime).ToString().PadLeft(2, '0'),
                pc.GetSecond(dateTime).ToString().PadLeft(2, '0'),
                pc.GetMilliseconds(dateTime).ToString().PadLeft(3, '0')
                );
            return result;
        }

        public static DateTime ToDateTimeFromJalaliCompactForm(this string jalaliDate)
        {
            return new PersianCalendar().ToDateTime(
                int.Parse(jalaliDate.Substring(0, 4)),
                int.Parse(jalaliDate.Substring(4, 2)),
                int.Parse(jalaliDate.Substring(6, 2)), 0, 0, 0, 0);
        }

        public static string HashPassword(this string password)
        {
            // generate a 128-bit salt using a secure PRNG
            byte[] salt = Encoding.ASCII.GetBytes("12hi^UfgHDR8iY2H");
            Console.WriteLine($"Salt: {Convert.ToBase64String(salt)}");
            // derive a 256-bit subkey (use HMACSHA1 with 10,000 iterations)
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));
            return hashed;
        }

        public static string Encrypt_Aes(this string plainText, string Key, string IV)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;
            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.BlockSize = 128;
                aesAlg.KeySize = 256;
                aesAlg.Key = Encoding.Unicode.GetBytes(Key);
                aesAlg.IV = Encoding.Unicode.GetBytes(IV);

                // Create a decrytor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt
                        , encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
            // Return the encrypted bytes from the memory stream.
            var result = Convert.ToBase64String(encrypted);
            return result;
        }

        public static string Decrypt_Aes(this string encryphtedText, string Key, string IV)
        {
            // Check arguments.
            if (encryphtedText == null || encryphtedText.Length <= 0)
                throw new ArgumentNullException("encryphtedText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.BlockSize = 128;
                aesAlg.KeySize = 256;
                aesAlg.Key = Encoding.Unicode.GetBytes(Key);
                aesAlg.IV = Encoding.Unicode.GetBytes(IV);

                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(encryphtedText)))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            return plaintext;
        }


        public static void CopyPropertiesTo<T, TU>(this T source, TU dest)
        {
            if (source != null)
            {
                var sourceProps = typeof(T).GetProperties().Where(x => x.CanRead).ToList();
                var destProps = typeof(TU).GetProperties()
                        .Where(x => x.CanWrite)
                        .ToList();

                foreach (var sourceProp in sourceProps)
                {
                    if (destProps.Any(x => x.Name == sourceProp.Name))
                    {
                        var p = destProps.First(x => x.Name == sourceProp.Name);
                        if (p.CanWrite)
                        { // check if the property can be set or no.
                            p.SetValue(dest, sourceProp.GetValue(source, null), null);
                        }
                    }

                }
            }
        }

        public static T ConvertObj<T, S>(this S source) where T: class, new() 
        {
            if(source != null)
            {
                var t = new T();
                source.CopyPropertiesTo(t);
                return t;
            }
            return null;
        }

        public static List<T> ConvertObj<T, S>(this List<S> source) where T: class, new()
        {
            var result = new List<T>();
            foreach (var s in source)
            {
                T t = new T();
                s.CopyPropertiesTo(t);
                result.Add(t);
            }
            return result;
        }


    }
}
