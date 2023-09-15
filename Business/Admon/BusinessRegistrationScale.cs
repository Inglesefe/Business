using Dal;
using Entities.Admon;

namespace Business.Admon
{
    /// <summary>
    /// Realiza la persistencia de las escalas asociadas a las matrículas en la base de datos
    /// </summary>
    public class BusinessRegistrationScale : BusinessBase<RegistrationScale>
    {
        #region Constructors
        /// <summary>
        /// Inicializa la persistencia
        /// </summary>
        /// <param name="persistent">Persistencia en base de datos de las escalas asociadas a alas matrículas</param>
        public BusinessRegistrationScale(IPersistentWithLog<RegistrationScale> persistent) : base(persistent) { }
        #endregion
    }
}
