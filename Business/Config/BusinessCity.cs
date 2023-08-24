using Dal;
using Entities.Config;

namespace Business.Config
{
    /// <summary>
    /// Realiza la persistencia de las ciudades en la base de datos
    /// </summary>
    public class BusinessCity : BusinessBase<City>
    {
        #region Constructors
        /// <summary>
        /// Inicializa la persistencia
        /// </summary>
        /// <param name="persistent">Persistencia en base de datos de las ciudades</param>
        public BusinessCity(IPersistentWithLog<City> persistent) : base(persistent) { }
        #endregion
    }
}
