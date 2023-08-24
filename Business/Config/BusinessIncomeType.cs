using Dal;
using Entities.Config;

namespace Business.Config
{
    /// <summary>
    /// Realiza la persistencia de los tipos de ingreso en la base de datos
    /// </summary>
    public class BusinessIncomeType : BusinessBase<IncomeType>
    {
        #region Constructors
        /// <summary>
        /// Inicializa la persistencia
        /// </summary>
        /// <param name="persistent">Persistencia en base de datos de los tipos de ingreso</param>
        public BusinessIncomeType(IPersistentWithLog<IncomeType> persistent) : base(persistent) { }
        #endregion
    }
}
