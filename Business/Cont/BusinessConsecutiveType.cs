using Dal;
using Entities.Config;

namespace Business.Cont
{
    /// <summary>
    /// Realiza la persistencia de los tipos de consecutivos contables en la base de datos
    /// </summary>
    public class BusinessConsecutiveType : BusinessBase<ConsecutiveType>
    {
        #region Constructors
        /// <summary>
        /// Inicializa la persistencia
        /// </summary>
        /// <param name="persistent">Persistencia en base de datos de los tipos de consecutivos contable</param>
        public BusinessConsecutiveType(IPersistentWithLog<ConsecutiveType> persistent) : base(persistent) { }
        #endregion
    }
}
