using Dal;
using Entities.Crm;

namespace Business.Crm
{
    /// <summary>
    /// Realiza la persistencia de los beneficiarios en la base de datos
    /// </summary>
    public class BusinessBeneficiary : BusinessBase<Beneficiary>
    {
        #region Constructors
        /// <summary>
        /// Inicializa la persistencia
        /// </summary>
        /// <param name="persistent">Persistencia en base de datos de los beneficiarios</param>
        public BusinessBeneficiary(IPersistentWithLog<Beneficiary> persistent) : base(persistent) { }
        #endregion
    }
}
