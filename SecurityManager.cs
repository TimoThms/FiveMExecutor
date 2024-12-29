using System;
using System.Security.Cryptography;
using System.Text;

namespace FiveMExecutor
{
    public class SecurityManager
    {
        private readonly byte[] encryptionKey;
        private readonly byte[] iv;

        public SecurityManager()
        {
            using (var aes = Aes.Create())
            {
                encryptionKey = aes.Key;
                iv = aes.IV;
            }
        }

        public byte[] EncryptScript(string script)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = encryptionKey;
                aes.IV = iv;

                using (var encryptor = aes.CreateEncryptor())
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    using (var swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(script);
                    }

                    return msEncrypt.ToArray();
                }
            }
        }

        public string DecryptScript(byte[] encryptedScript)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = encryptionKey;
                aes.IV = iv;

                using (var decryptor = aes.CreateDecryptor())
                using (var msDecrypt = new MemoryStream(encryptedScript))
                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                using (var srDecrypt = new StreamReader(csDecrypt))
                {
                    return srDecrypt.ReadToEnd();
                }
            }
        }
    }
}
