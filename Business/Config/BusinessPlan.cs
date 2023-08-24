using Dal;
using Entities.Config;

namespace Business.Config
{
    /// <summary>
    /// Realiza la persistencia de los planes en la base de datos
    /// </summary>
    public class BusinessPlan : BusinessBase<Plan>
    {
        #region Constructors
        /// <summary>
        /// Inicializa la persistencia
        /// </summary>
        /// <param name="persistent">Persistencia en base de datos de los planes</param>
        public BusinessPlan(IPersistentWithLog<Plan> persistent) : base(persistent) { }
        #endregion
    }
}
