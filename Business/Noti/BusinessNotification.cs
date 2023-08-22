using Business.Dto;
using Business.Exceptions;
using Dal;
using Dal.Dto;
using Dal.Exceptions;
using Entities.Auth;
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
        #region Attributes
        /// <summary>
        /// Persistencia de las plantillas
        /// </summary>
        private readonly IPersistentWithLog<Template> _persistentTemplate;
        #endregion

        #region Constructors
        /// <summary>
        /// Inicializa la persistencia
        /// </summary>
        /// <param name="persistent">Persistencia en base de datos de las notificaciones</param>
        public BusinessNotification(IPersistentWithLog<Notification> persistent, IPersistentWithLog<Template> persistentTemplate) : base(persistent)
        {
            _persistentTemplate = persistentTemplate;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Trae un listado de notificaciones desde la base de datos
        /// </summary>
        /// <param name="filters">Filtros aplicados a la consulta</param>
        /// <param name="orders">Ordenamientos aplicados a la base de datos</param>
        /// <param name="limit">Límite de registros a traer</param>
        /// <param name="offset">Corrimiento desde el que se cuenta el número de registros</param>
        /// <returns>Listado de notificaciones</returns>
        /// <exception cref="BusinessException">Si hubo una excepción al consultar las notificaciones</exception>
        public ListResult<Notification> List(string filters, string orders, int limit, int offset)
        {
            try
            {
                return _persistent.List(filters, orders, limit, offset);
            }
            catch (PersistentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BusinessException("Error al consultar el listado de notificaciones", ex);
            }
        }

        /// <summary>
        /// Consulta una notificación dado su identificador
        /// </summary>
        /// <param name="entity">Notificación a consultar</param>
        /// <returns>Notificación con los datos cargados desde la base de datos o null si no lo pudo encontrar</returns>
        /// <exception cref="BusinessException">Si hubo una excepción al consultar la notificación</exception>
        public Notification Read(Notification entity)
        {
            try
            {
                return _persistent.Read(entity);
            }
            catch (Exception ex)
            {
                throw new BusinessException("Error al consultar la notificación", ex);
            }
        }

        /// <summary>
        /// Inserta una notificación en la base de datos
        /// </summary>
        /// <param name="entity">Notificación a insertar</param>
        /// <returns>Notificación insertada con el id generado por la base de datos</returns>
        /// <param name="user">Usuario que realiza la inserción</param>
        /// <exception cref="BusinessException">Si hubo una excepción al insertar la notificación</exception>
        public Notification Insert(Notification entity, User user)
        {
            try
            {
                return _persistent.Insert(entity, user);
            }
            catch (PersistentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BusinessException("Error al insertar la notificación", ex);
            }
        }

        /// <summary>
        /// Actualiza una notificación en la base de datos
        /// </summary>
        /// <param name="entity">Notificación a actualizar</param>
        /// <param name="user">Usuario que realiza la actualización</param>
        /// <returns>Notificación actualizada</returns>
        /// <exception cref="BusinessException">Si hubo una excepción al actualizar la notificación</exception>
        public Notification Update(Notification entity, User user)
        {
            try
            {
                return _persistent.Update(entity, user);
            }
            catch (PersistentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BusinessException("Error al actualizar la notificación", ex);
            }
        }

        /// <summary>
        /// Elimina una notificación de la base de datos
        /// </summary>
        /// <param name="entity">Notificación a eliminar</param>
        /// <param name="user">Usuario que realiza la eliminación</param>
        /// <returns>Notificación eliminada</returns>
        /// <exception cref="BusinessException">Si hubo una excepción al eliminar la notificación</exception>
        public Notification Delete(Notification entity, User user)
        {
            try
            {
                return _persistent.Delete(entity, user);
            }
            catch (PersistentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BusinessException("Error al eliminar la notificación", ex);
            }
        }

        /// <summary>
        /// Crea el contenido de una notificación a partir de una plantilla y unos datos de reemplazo
        /// </summary>
        /// <param name="notification">Notificación inicial sin contenido</param>
        /// <param name="template">Plantilla a cargar desde la base de datos</param>
        /// <param name="data">Nombre de las variables y sus respectivos valores, a buscar y reemplazar en la plantilla</param>
        /// <returns>Notificación cuyo contenido es el de la plantilla con las variables reemplazadas</returns>
        public Notification ReplacedVariables(Notification notification, Template template, IDictionary<string, string> data)
        {
            template = _persistentTemplate.Read(template);
            foreach (KeyValuePair<string, string> item in data)
            {
                template.Content = template.Content.Replace("#{" + item.Key + "}#", item.Value);
            }
            notification.Content = template.Content;
            return notification;
        }

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
                client.Connect(smtpConfig.Host, smtpConfig.Port, smtpConfig.Ssl);
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
