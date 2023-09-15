using Dal;
using Entities.Config;

namespace Business.Config
{
    /// <summary>
    /// Realiza la persistencia de las escalas en la base de datos
    /// </summary>
    public class BusinessScale : BusinessBase<Scale>
    {
        #region Constructors
        /// <summary>
        /// Inicializa la persistencia
        /// </summary>
        /// <param name="persistent">Persistencia en base de datos de las escalas</param>
        public BusinessScale(IPersistentWithLog<Scale> persistent) : base(persistent) { }
        #endregion
    }
}
