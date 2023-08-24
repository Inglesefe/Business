using Dal;
using Entities.Config;

namespace Business.Config
{
    /// <summary>
    /// Realiza la persistencia de los paises en la base de datos
    /// </summary>
    public class BusinessCountry : BusinessBase<Country>
    {
        #region Constructors
        /// <summary>
        /// Inicializa la persistencia
        /// </summary>
        /// <param name="persistent">Persistencia en base de datos de los paises</param>
        public BusinessCountry(IPersistentWithLog<Country> persistent) : base(persistent) { }
        #endregion
    }
}
