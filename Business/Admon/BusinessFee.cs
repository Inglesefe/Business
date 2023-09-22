using Dal;
using Entities.Admon;

namespace Business.Admon
{
    /// <summary>
    /// Realiza la persistencia de ls cuotas de las matrículas en la base de datos
    /// </summary>
    public class BusinessFee : BusinessBase<Fee>
    {
        #region Constructors
        /// <summary>
        /// Inicializa la persistencia
        /// </summary>
        /// <param name="persistent">Persistencia en base de datos de ls cuotas de las matrículas</param>
        public BusinessFee(IPersistentWithLog<Fee> persistent) : base(persistent) { }
        #endregion
    }
}
