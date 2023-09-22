using Business.Dto;
using Business.Noti;
using Dal.Noti;
using Entities.Noti;
using Microsoft.Extensions.Configuration;

namespace Business.Test.Noti
{
    /// <summary>
    /// Realiza las pruebas sobre la clase de persistencia de notificaciones
    /// </summary>
    [Collection("Tests")]
    public class NotificationTest
    {
        #region Attributes
        /// <summary>
        /// Configuración de la aplicación de pruebas
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Configuración del servidor SMTP
        /// </summary>
        private readonly SmtpConfig _smtpConfig;
        #endregion

        #region Constructors
        /// <summary>
        /// Inicializa la configuración de la prueba
        /// </summary>
        public NotificationTest()
        {
            //Arrange
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, false)
                .AddEnvironmentVariables()
                .Build();
            _smtpConfig = new SmtpConfig()
            {
                From = _configuration["Smtp:From"] ?? "",
                Host = _configuration["Smtp:Host"] ?? "",
                Password = _configuration["Smtp:Password"] ?? "",
                Port = int.Parse(_configuration["Smtp:Port"] ?? "0"),
                Ssl = bool.Parse(_configuration["Smtp:Ssl"] ?? "false"),
                Username = _configuration["Smtp:Username"] ?? ""
            };
        }
        #endregion

        #region Methods
        /// <summary>
        /// Prueba la creación de una capa de negocio de notificaciones
        /// </summary>
        [Fact]
        public void CreateOwnerTest()
        {
            //Act, Assert
            Assert.IsType<BusinessNotification>(new BusinessNotification(new PersistentNotification("")));
        }

        /// <summary>
        /// Prueba el envío de la notificación por correo
        /// </summary>
        [Fact]
        public void SendNotificationTest()
        {
            //Arrange
            Notification notification = new()
            {
                Id = 1,
                Subject = "Prueba de envío de correo en proyecto de pruebas",
                Date = DateTime.Now,
                To = "leandrobaena@gmail.com",
                Content = "<p>Prueba de envío de un correo desde el proyecto de pruebas</p>",
                User = 1
            };

            try
            {
                //Act
                BusinessNotification.SendNotification(notification, _smtpConfig);

                //Assert
                Assert.True(true);
            }
            catch
            {
                //Assert
                Assert.True(false);
            }

        }
        #endregion
    }
}
