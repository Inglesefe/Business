using Dal;
using Entities.Config;

namespace Business.Config
{
    /// <summary>
    /// Realiza la persistencia de los tipos de identificación en la base de datos
    /// </summary>
    public class BusinessIdentificationType : BusinessBase<IdentificationType>
    {
        #region Constructors
        /// <summary>
        /// Inicializa la persistencia
        /// </summary>
        /// <param name="persistent">Persistencia en base de datos de los tipos de identificación</param>
        public BusinessIdentificationType(IPersistentWithLog<IdentificationType> persistent) : base(persistent) { }
        #endregion
    }
}
