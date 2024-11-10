﻿using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Cryptography;
using System.Text;

namespace InSightWindowAPI.Serivces
{
    public class AesService : IAesService
    {
        public Aes Aes { get; set; }

        public byte[] IV { get; set; }

        public AesService(string key, string iv)
        {
            Aes = Aes.Create();
            IV = _getProperByteData(iv);
            Aes.Key = _getProperByteData(iv);

            Aes.Mode = CipherMode.CBC;
            Aes.BlockSize = 128;
            Aes.Padding = PaddingMode.Zeros;
            
        }
        public byte[] EncryptStringToBytes_Aes(string plainText)
        {

            // Create an encryptor to perform the stream transform.
            ICryptoTransform encryptor = Aes.CreateEncryptor(Aes.Key, Aes.IV);

            byte[] encrypted;
            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        //Write all data to the stream.

                        swEncrypt.Write(plainText);
                    }
                }

                encrypted = msEncrypt.ToArray();
            }


            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }

        public string DecryptStringFromBytes_Aes(byte[] cipherText)
        {
            // Check arguments.

            string plaintext = null;

            // Create an Aes object
            // with the specified key and IV.

            // Create a decryptor to perform the stream transform.
            Aes.IV =IV;
            ICryptoTransform decryptor = Aes.CreateDecryptor(Aes.Key, Aes.IV);

            // Create the streams used for decryption.
            using (MemoryStream msDecrypt = new MemoryStream(cipherText))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {

                        // Read the decrypted bytes     from the decrypting stream
                        // and place them in a string.
                        plaintext = srDecrypt.ReadToEnd();
                    }
                }
            }

            return plaintext;
        }
        private byte[] _getProperByteData(string text)
        {
            byte[] oldArray = Encoding.UTF8.GetBytes(text);
            if (oldArray.Length % 16 == 0)
                return oldArray;
            var newArray = new byte[oldArray.Length + (16 - oldArray.Length % 16)];
            Array.Copy(oldArray, newArray, oldArray.Length);
            return newArray;
        }


    }
}