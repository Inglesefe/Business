using Dal;
using Entities.Crm;

namespace Business.Crm
{
    /// <summary>
    /// Realiza la persistencia de los titulares en la base de datos
    /// </summary>
    public class BusinessOwner : BusinessBase<Owner>
    {
        #region Constructors
        /// <summary>
        /// Inicializa la persistencia
        /// </summary>
        /// <param name="persistent">Persistencia en base de datos de los titulares</param>
        public BusinessOwner(IPersistentWithLog<Owner> persistent) : base(persistent) { }
        #endregion
    }
}
