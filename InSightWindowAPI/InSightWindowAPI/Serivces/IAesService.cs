using System.Security.Cryptography;

namespace InSightWindowAPI.Serivces
{
    public interface IAesService
    {
        Aes Aes { get; set; }

        string DecryptStringFromBytes_Aes(byte[] cipherText);
        byte[] EncryptStringToBytes_Aes(string plainText);
    }
}