using Business.Util;
using Microsoft.Extensions.Configuration;

namespace Business.Test.Util
{
    /// <summary>
    /// Realiza las pruebas unitarias sobre el descifrado de cadenas de caracteres
    /// </summary>
    [Collection("Tests")]
    public class CryptoTest
    {
        #region Attributes
        /// <summary>
        /// Configuración de la aplicación de pruebas
        /// </summary>
        private readonly IConfiguration _configuration;
        #endregion

        #region Constructors
        /// <summary>
        /// Inicializa la configuración de la prueba
        /// </summary>
        public CryptoTest()
        {
            //Arrange
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, false)
                .AddEnvironmentVariables()
                .Build();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Prueba la desencripción de una cadena de caracteres
        /// </summary>
        [Fact]
        public void DecryptTest()
        {
            //Act
            string plainText = Crypto.Decrypt("FLWnwyoEz/7tYsnS+vxTVg==", _configuration["Aes:Key"] ?? "", _configuration["Aes:IV"] ?? "");

            //Assert
            Assert.Equal("Prueba123", plainText);
        }

        /// <summary>
        /// Prueba la encripción de una cadena de caracteres
        /// </summary>
        [Fact]
        public void EncryptTest()
        {
            //Act
            string cryptedText = Crypto.Encrypt("Prueba123", _configuration["Aes:Key"] ?? "", _configuration["Aes:IV"] ?? "");

            //Assert
            Assert.Equal("FLWnwyoEz/7tYsnS+vxTVg==", cryptedText);
        }
        #endregion
    }
}
