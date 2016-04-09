/*
    efisSQL - data base management tool
    Copyright (C) 2011  Lucian Voinescu

    This file is part of efisSQL

    efisSQL is free software: you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    efisSQL is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public License
    along with efisSQL. If not, see <http://www.gnu.org/licenses/>.
*/


using System;
using System.Text;
using System.Security.Cryptography;

namespace DBMS.core
{
    public class Krypto
    {
        private const int KeySize = 24;

        private static byte[] GetKeyBytes(int keySize)
        {
            // Just generate something unique for this user on this machine.
            // No, this is not incredibly secure but it is better than storing the password in plain text
            StringBuilder builder = new StringBuilder();
            while (builder.Length < keySize)
            {
                builder.AppendFormat("Computer:{0} User:{1}", Environment.MachineName, Environment.UserName);
            }
            byte[] bytes = UTF8Encoding.UTF8.GetBytes(builder.ToString());
            if (keySize != bytes.Length)
            {
                // Copy only the number of bytes desired for key
                byte[] rightSizedKey = new byte[keySize];
                for (int idx = 0; idx < keySize; ++idx)
                {
                    rightSizedKey[idx] = bytes[idx];
                }
                bytes = rightSizedKey;
            }
            return bytes;
        }

        public static string EncryptPassword(string password)
        {
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(password);
            byte[] keyArray = GetKeyBytes(KeySize);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray,
            0, toEncryptArray.Length);

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        public static string DecryptPassword(string encryptedPassword)
        {
            try
            {
                byte[] toEncryptArray = Convert.FromBase64String(encryptedPassword);
                TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
                tdes.Key = GetKeyBytes(KeySize);
                tdes.Mode = CipherMode.ECB;
                tdes.Padding = PaddingMode.PKCS7;

                ICryptoTransform cTransform = tdes.CreateDecryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray,
                0, toEncryptArray.Length);

                return UTF8Encoding.UTF8.GetString(resultArray);
            }
            catch (Exception ex)
            {
                return null;
            }
        }


    }
}
