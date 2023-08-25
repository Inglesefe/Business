using Business.Dto;
using Business.Noti;
using Dal;
using Dal.Dto;
using Dal.Exceptions;
using Entities.Auth;
using Entities.Noti;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Data;

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
        /// Capa de negocio de las notificaciones
        /// </summary>
        private readonly BusinessNotification _business;

        /// <summary>
        /// Configuración del servidor SMTP
        /// </summary>
        private readonly SmtpConfig _smtpConfig;

        /// <summary>
        /// Conexión a la base de datos falsa
        /// </summary>
        private readonly IDbConnection connectionFake;
        #endregion

        #region Constructors
        /// <summary>
        /// Inicializa la configuración de la prueba
        /// </summary>
        public NotificationTest()
        {
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, false)
                .AddEnvironmentVariables()
                .Build();
            Mock<IPersistentWithLog<Notification>> mock = new();
            Mock<IDbConnection> mockConnection = new();
            connectionFake = mockConnection.Object;

            List<Notification> notifications = new()
            {
                new Notification() { Id = 1, Date = DateTime.Now, To = "leandrobaena@gmail.com", Subject = "Correo de prueba", Content = "<h1>Esta es una prueba hecha por leandrobaena@gmail.com</h1>", User = 1 }
            };

            mock.Setup(p => p.List("idnotification = 1", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<IDbConnection>()))
                .Returns(new ListResult<Notification>(notifications.Where(y => y.Id == 1).ToList(), 1));
            mock.Setup(p => p.List("idnotificacion = 1", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<IDbConnection>()))
                .Throws<PersistentException>();

            mock.Setup(p => p.Read(It.IsAny<Notification>(), It.IsAny<IDbConnection>()))
                .Returns((Notification notification, IDbConnection connection) => notifications.Find(x => x.Id == notification.Id) ?? new Notification());

            mock.Setup(p => p.Insert(It.IsAny<Notification>(), It.IsAny<User>(), It.IsAny<IDbConnection>()))
                .Returns((Notification notification, User user, IDbConnection connection) =>
                {
                    notification.Id = notifications.Count + 1;
                    notifications.Add(notification);
                    return notification;
                });

            _business = new(mock.Object);

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
        /// Prueba la consulta de un listado de notificaciones con filtros, ordenamientos y límite
        /// </summary>
        /// <returns>N/A</returns>
        [Fact]
        public void NotificationListTest()
        {
            ListResult<Notification> list = _business.List("idnotification = 1", "date", 1, 0, connectionFake);

            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de notificaciones con filtros, ordenamientos y límite y con errores
        /// </summary>
        /// <returns>N/A</returns>
        [Fact]
        public void NotificationListWithErrorTest()
        {
            Assert.Throws<PersistentException>(() => _business.List("idnotificacion = 1", "name", 1, 0, connectionFake));
        }

        /// <summary>
        /// Prueba la consulta de una notificación dada su identificador
        /// </summary>
        /// <returns>N/A</returns>
        [Fact]
        public void NotificationReadTest()
        {
            Notification notification = new() { Id = 1 };
            notification = _business.Read(notification, connectionFake);

            Assert.Equal("leandrobaena@gmail.com", notification.To);
        }

        /// <summary>
        /// Prueba la consulta de una notificación que no existe dado su identificador
        /// </summary>
        /// <returns>N/A</returns>
        [Fact]
        public void NotificationReadNotFoundTest()
        {
            Notification notification = new() { Id = 10 };
            notification = _business.Read(notification, connectionFake);

            Assert.Equal(0, notification.Id);
        }

        /// <summary>
        /// Prueba la inserción de una notificación
        /// </summary>
        /// <returns>N/A</returns>
        [Fact]
        public void NotificationInsertTest()
        {
            Notification notification = new() { To = "leandrobaena@gmail.com", Subject = "Prueba de inserción", Content = "<p>Prueba de inserción de notificación</p>", User = 1 };
            notification = _business.Insert(notification, new() { Id = 1 }, connectionFake);

            Assert.NotEqual(0, notification.Id);
        }

        /// <summary>
        /// Prueba el envío de la notificación por correo
        /// </summary>
        [Fact]
        public void SendNotificationTest()
        {
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
                BusinessNotification.SendNotification(notification, _smtpConfig);
                Assert.True(true);
            }
            catch
            {
                Assert.True(false);
            }

        }
        #endregion
    }
}
