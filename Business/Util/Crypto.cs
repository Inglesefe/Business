using System.Security.Cryptography;
using System.Text;

namespace Business.Util
{
    /// <summary>
    /// Clase para desencriptar cadenas de texto
    /// </summary>
    public static class Crypto
    {
        #region Methods
        /// <summary>
        /// Desencripta una cadena de texto usando el algoritmo AES
        /// </summary>
        /// <param name="input">Cadena a desencriptar</param>
        /// <param name="key">Llave de cifrado</param>
        /// <param name="iv">Vector de inicialización de cifrado</param>
        /// <returns>Cadena desencriptada</returns>
        public static string Decrypt(string input, string key, string iv)
        {
            string decripted = "";
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = Encoding.UTF8.GetBytes(iv);

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                using MemoryStream msDecrypt = new(Convert.FromBase64String(input));
                using CryptoStream csDecrypt = new(msDecrypt, decryptor, CryptoStreamMode.Read);
                using StreamReader srDecrypt = new(csDecrypt);
                decripted = srDecrypt.ReadToEnd();
            }
            return decripted;
        }

        /// <summary>
        /// Encripta una cadena de texto usando el algoritmo AES
        /// </summary>
        /// <param name="input">Cadena a encriptar</param>
        /// <param name="key">Llave de cifrado</param>
        /// <param name="iv">Vector de inicialización de cifrado</param>
        /// <returns>Cadena encriptada</returns>
        public static string Encrypt(string input, string key, string iv)
        {
            using Aes aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = Encoding.UTF8.GetBytes(iv);

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            byte[] paramBytes = Encoding.UTF8.GetBytes(input);
            byte[] cryptoBytes = encryptor.TransformFinalBlock(paramBytes, 0, paramBytes.Length);
            return Convert.ToBase64String(cryptoBytes);
        }
        #endregion
    }
}
