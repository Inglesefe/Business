using Dal;
using Entities.Cont;

namespace Business.Cont
{
    /// <summary>
    /// Realiza la persistencia de las número de cuentas contables en la base de datos
    /// </summary>
    public class BusinessAccountNumber : BusinessBase<AccountNumber>
    {
        #region Constructors
        /// <summary>
        /// Inicializa la persistencia
        /// </summary>
        /// <param name="persistent">Persistencia en base de datos de los números de cuenta contable</param>
        public BusinessAccountNumber(IPersistentWithLog<AccountNumber> persistent) : base(persistent) { }
        #endregion
    }
}
