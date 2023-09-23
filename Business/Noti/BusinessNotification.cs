using Business.Dto;
using Business.Exceptions;
using Dal;
using Entities.Noti;
using MailKit.Net.Smtp;
using MimeKit;

namespace Business.Noti
{
    /// <summary>
    /// Capa de negocio de las notificaciones en la base de datos
    /// </summary>
    public class BusinessNotification : BusinessBase<Notification>
    {
        #region Constructors
        /// <summary>
        /// Inicializa la persistencia
        /// </summary>
        /// <param name="persistent">Persistencia en base de datos de las notificaciones</param>
        public BusinessNotification(IPersistentWithLog<Notification> persistent) : base(persistent) { }
        #endregion

        #region Methods
        /// <summary>
        /// Envía una notificación por correo, a través de un servidor Smtp
        /// </summary>
        /// <param name="notification">Notificación a enviar</param>
        /// <param name="smtpConfig">Configuración del servidor Smtp</param>
        /// <exception cref="BusinessException">Si ocurre un error al enviar el correo</exception>
        public static void SendNotification(Notification notification, SmtpConfig smtpConfig)
        {
            try
            {
                MimeMessage message = new();
                message.From.Add(new MailboxAddress(smtpConfig.From, smtpConfig.From));
                message.Subject = notification.Subject;
                message.To.Add(new MailboxAddress(notification.To, notification.To));

                message.Body = new TextPart("html")
                {
                    Text = notification.Content
                };

                using SmtpClient client = new();
                if (smtpConfig.StartTls)
                {
                    client.Connect(smtpConfig.Host, smtpConfig.Port, MailKit.Security.SecureSocketOptions.StartTls);
                }
                else
                {
                    client.Connect(smtpConfig.Host, smtpConfig.Port, smtpConfig.Ssl);
                }
                client.Authenticate(smtpConfig.Username, smtpConfig.Password);
                client.Send(message);
                client.Disconnect(true);
            }
            catch (Exception ex)
            {
                throw new BusinessException("Error al enviar la notificación", ex);
            }
        }
        #endregion
    }
}
