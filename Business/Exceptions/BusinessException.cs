using System.Runtime.Serialization;

namespace Business.Exceptions
{
    /// <summary>
    /// Excepción personalizada para la lógica de entidades
    /// </summary>
    [Serializable]
    public class BusinessException : ArgumentException
    {
        #region Methods
        /// <summary>
        /// Constructor por defecto
        /// </summary>
        public BusinessException() { }

        /// <summary>
        /// Crea una excepción con un mensaje
        /// </summary>
        /// <param name="message">Mensaje de la excepción</param>
        public BusinessException(string message) : base(message) { }

        /// <summary>
        /// Crea una excepción con un mensaje y una excepción interna
        /// </summary>
        /// <param name="message">Mensaje de la excepción</param>
        /// <param name="inner">Excepción interna</param>
        public BusinessException(string message, Exception inner) : base(message, inner) { }

        /// <summary>
        /// Crea una excepción a partir de una serialización realizada
        /// </summary>
        /// <param name="info">Información de la serialización</param>
        /// <param name="context">Contexto de la serialización</param>
        protected BusinessException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        #endregion
    }
}
