using Dal;
using Entities.Cont;

namespace Business.Cont
{
    /// <summary>
    /// Realiza la persistencia de los pagos en la base de datos
    /// </summary>
    public class BusinessPayment : BusinessBase<Payment>
    {
        #region Constructors
        /// <summary>
        /// Inicializa la persistencia
        /// </summary>
        /// <param name="persistent">Persistencia en base de datos de los pagos</param>
        public BusinessPayment(IPersistentWithLog<Payment> persistent) : base(persistent) { }
        #endregion
    }
}
