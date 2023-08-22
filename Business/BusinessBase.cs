using Dal;
using Entities;

namespace Business
{
    /// <summary>
    /// Clase base para el manejo de la capa de negocios de las distintas
    /// entidades del módulo de autenticación y autorización
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BusinessBase<T> where T : EntityBase
    {
        #region Attributes
        /// <summary>
        /// Administrador de persistencia de la entidad
        /// </summary>
        protected IPersistentWithLog<T> _persistent;
        #endregion

        #region Constructors
        /// <summary>
        /// Inicializa la persistencia
        /// </summary>
        /// <param name="persistent">Persistencia de la entidad</param>
        protected BusinessBase(IPersistentWithLog<T> persistent)
        {
            _persistent = persistent;
        }
        #endregion
    }
}
