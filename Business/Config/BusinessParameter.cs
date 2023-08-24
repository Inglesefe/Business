using Dal;
using Entities.Config;

namespace Business.Config
{
    /// <summary>
    /// Realiza la persistencia de los parámetros en la base de datos
    /// </summary>
    public class BusinessParameter : BusinessBase<Parameter>
    {
        #region Constructors
        /// <summary>
        /// Inicializa la persistencia
        /// </summary>
        /// <param name="persistent">Persistencia en base de datos de los parámetros</param>
        public BusinessParameter(IPersistentWithLog<Parameter> persistent) : base(persistent) { }
        #endregion
    }
}
