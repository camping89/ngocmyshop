﻿using Nop.Web.Areas.Admin.Models.Customers;
using System;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Nop.Web.Extensions
{
    public class StringExtensions
    {
        public static CustomerFirstLastName GetFirstLastNameFromFullName(string fullName)
        {
            var index = fullName.LastIndexOf(' ');
            if (index != -1)
            {
                return new CustomerFirstLastName
                {
                    LastName = fullName.Substring(index + 1),
                    FirstName = fullName.Substring(0, index)
                };
            }
            else
            {
                return new CustomerFirstLastName
                {
                    LastName = string.Empty,
                    FirstName = fullName
                };
            }
        }
        // Generate a random string with a given size  
        public static string RandomString(int size, bool lowerCase)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            if (lowerCase)
                return builder.ToString().ToLower();
            return builder.ToString();
        }
        public static string GetPackageCode(int lastPackageId)
        {
            lastPackageId++;
            return $"KH.{DateTime.Now.Year}.{DateTime.Now.Month}.{DateTime.Now.Day}_{lastPackageId}";
        }
        public static string RemoveWhitespace(string str)
        {
            return string.Join("", str.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
        }
        public static string ConvertToUnSign3(string s)
        {
            s = RemoveWhitespace(s);
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = s.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
        }

        public static string GenerateEmailAddress(string firstName, string lastName)
        {
            var random = new Random();
            firstName = ConvertToUnSign3(firstName.Trim());
            lastName = ConvertToUnSign3(lastName);
            return $"{lastName.ToLower()}{firstName.ToLower()}{random.Next(100, 999)}@gmail.com";
        }

        public static string Encrypt(string toEncrypt, bool useHashing, string keyEncryptUserName)
        {
            byte[] keyArray;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

            System.Configuration.AppSettingsReader settingsReader =
                new AppSettingsReader();

            //If hashing use get hashcode regards to your key
            if (useHashing)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(keyEncryptUserName));
                //Always release the resources and flush data
                // of the Cryptographic service provide. Best Practice

                hashmd5.Clear();
            }
            else
                keyArray = UTF8Encoding.UTF8.GetBytes(keyEncryptUserName);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            //set the secret key for the tripleDES algorithm
            tdes.Key = keyArray;
            //mode of operation. there are other 4 modes.
            //We choose ECB(Electronic code Book)
            tdes.Mode = CipherMode.ECB;
            //padding mode(if any extra byte added)

            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateEncryptor();
            //transform the specified region of bytes array to resultArray
            byte[] resultArray =
                cTransform.TransformFinalBlock(toEncryptArray, 0,
                    toEncryptArray.Length);
            //Release resources held by TripleDes Encryptor
            tdes.Clear();
            //Return the encrypted data into unreadable string format
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        public static string Decrypt(string cipherString, bool useHashing, string keyEncryptUserName)
        {
            byte[] keyArray;
            //get the byte code of the string

            byte[] toEncryptArray = Convert.FromBase64String(cipherString);

            System.Configuration.AppSettingsReader settingsReader =
                new AppSettingsReader();
            //Get your key from config file to open the lock!
            string key = (string)settingsReader.GetValue("SecurityKey",
                typeof(String));

            if (useHashing)
            {
                //if hashing was used get the hash code with regards to your key
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                //release any resource held by the MD5CryptoServiceProvider

                hashmd5.Clear();
            }
            else
            {
                //if hashing was not implemented get the byte code of the key
                keyArray = UTF8Encoding.UTF8.GetBytes(key);
            }

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            //set the secret key for the tripleDES algorithm
            tdes.Key = keyArray;
            //mode of operation. there are other 4 modes. 
            //We choose ECB(Electronic code Book)

            tdes.Mode = CipherMode.ECB;
            //padding mode(if any extra byte added)
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(
                toEncryptArray, 0, toEncryptArray.Length);
            //Release resources held by TripleDes Encryptor                
            tdes.Clear();
            //return the Clear decrypted TEXT
            return UTF8Encoding.UTF8.GetString(resultArray);
        }

        public static DateTime? StringToDateTime(string dataString)
        {
            if (string.IsNullOrEmpty(dataString))
            {
                return null;
            }
            var registerDateString = dataString;
            var timeZoneIndex = registerDateString.IndexOf("(", System.StringComparison.Ordinal);
            var timeZoneId = registerDateString.Substring(timeZoneIndex);
            var dateTimeString = registerDateString.Substring(0, timeZoneIndex - 1);
            var formattedDate = DateTime.ParseExact(dateTimeString, "ddd MMM dd yyyy HH:mm:ss 'GMT'zzz", System.Globalization.CultureInfo.InvariantCulture);
            return formattedDate;
        }
    }
}
