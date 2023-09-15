using Dal;
using Entities.Admon;

namespace Business.Admon
{
    /// <summary>
    /// Realiza la persistencia de los ejecutivos de cuenta en la base de datos
    /// </summary>
    public class BusinessAccountExecutive : BusinessBase<AccountExecutive>
    {
        #region Constructors
        /// <summary>
        /// Inicializa la persistencia
        /// </summary>
        /// <param name="persistent">Persistencia en base de datos de los ejecutivos de cuenta</param>
        public BusinessAccountExecutive(IPersistentWithLog<AccountExecutive> persistent) : base(persistent) { }
        #endregion
    }
}
