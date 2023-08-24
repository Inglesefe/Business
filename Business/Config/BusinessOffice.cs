using Dal;
using Entities.Config;

namespace Business.Config
{
    /// <summary>
    /// Realiza la persistencia de las oficinas en la base de datos
    /// </summary>
    public class BusinessOffice : BusinessBase<Office>
    {
        #region Constructors
        /// <summary>
        /// Inicializa la persistencia
        /// </summary>
        /// <param name="persistent">Persistencia en base de datos de las oficinas</param>
        public BusinessOffice(IPersistentWithLog<Office> persistent) : base(persistent) { }
        #endregion
    }
}
