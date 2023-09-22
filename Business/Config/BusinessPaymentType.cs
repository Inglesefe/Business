using Dal;
using Entities.Config;

namespace Business.Config
{
    /// <summary>
    /// Realiza la persistencia de los tipos de pagos en la base de datos
    /// </summary>
    public class BusinessPaymentType : BusinessBase<PaymentType>
    {
        #region Constructors
        /// <summary>
        /// Inicializa la persistencia
        /// </summary>
        /// <param name="persistent">Persistencia en base de datos de los tipos de pagos</param>
        public BusinessPaymentType(IPersistentWithLog<PaymentType> persistent) : base(persistent) { }
        #endregion
    }
}
