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
        /// <returns>CAdena desencriptada</returns>
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
        #endregion
    }
}
