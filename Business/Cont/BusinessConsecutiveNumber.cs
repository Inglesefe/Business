using Dal;
using Entities.Cont;

namespace Business.Cont
{
    /// <summary>
    /// Realiza la persistencia de las número de consecutivos contables en la base de datos
    /// </summary>
    public class BusinessConsecutiveNumber : BusinessBase<ConsecutiveNumber>
    {
        #region Constructors
        /// <summary>
        /// Inicializa la persistencia
        /// </summary>
        /// <param name="persistent">Persistencia en base de datos de los consecutivos de cuenta contable</param>
        public BusinessConsecutiveNumber(IPersistentWithLog<ConsecutiveNumber> persistent) : base(persistent) { }
        #endregion
    }
}
