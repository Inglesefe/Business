using Dal;
using Entities.Config;

namespace Business.Cont
{
    /// <summary>
    /// Realiza la persistencia de los tipos de cuentas contables en la base de datos
    /// </summary>
    public class BusinessAccountType : BusinessBase<AccountType>
    {
        #region Constructors
        /// <summary>
        /// Inicializa la persistencia
        /// </summary>
        /// <param name="persistent">Persistencia en base de datos de los tipos de cuentas contable</param>
        public BusinessAccountType(IPersistentWithLog<AccountType> persistent) : base(persistent) { }
        #endregion
    }
}
