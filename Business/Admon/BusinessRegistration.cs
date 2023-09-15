using Dal;
using Entities.Admon;

namespace Business.Admon
{
    /// <summary>
    /// Realiza la persistencia de las matrículas en la base de datos
    /// </summary>
    public class BusinessRegistration : BusinessBase<Registration>
    {
        #region Constructors
        /// <summary>
        /// Inicializa la persistencia
        /// </summary>
        /// <param name="persistent">Persistencia en base de datos de las matrículas</param>
        public BusinessRegistration(IPersistentWithLog<Registration> persistent) : base(persistent) { }
        #endregion
    }
}
