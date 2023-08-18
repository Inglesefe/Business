namespace Business.Dto
{
    /// <summary>
    /// Configuración del servidor SMTP
    /// </summary>
    public class SmtpConfig
    {
        #region Attributes
        /// <summary>
        /// Usuario con que se conecta al servidor SMTP
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Contraseña con que se conecta al servidor SMTP
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Servidor SMTP
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Puerto de conexión con el servidor SMTP
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Si se habilita o no SSL en la conexión con el servidor SMTP
        /// </summary>
        public bool Ssl { get; set; }

        /// <summary>
        /// Correo desde el cual se envía la notificación
        /// </summary>
        public string From { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Inicializa la configuración con datos por defecto
        /// </summary>
        public SmtpConfig()
        {
            Username = string.Empty;
            Password = string.Empty;
            Host = string.Empty;
            Port = 0;
            Ssl = false;
            From = string.Empty;
        }
        #endregion
    }
}
