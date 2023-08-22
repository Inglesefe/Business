using Business.Exceptions;
using Dal;
using Dal.Dto;
using Dal.Exceptions;
using Entities.Auth;
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

        #region Methods
        /// <summary>
        /// Trae un listado de planes desde la base de datos
        /// </summary>
        /// <param name="filters">Filtros aplicados a la consulta</param>
        /// <param name="orders">Ordenamientos aplicados a la base de datos</param>
        /// <param name="limit">Límite de registros a traer</param>
        /// <param name="offset">Corrimiento desde el que se cuenta el número de registros</param>
        /// <returns>Listado de planes</returns>
        /// <exception cref="BusinessException">Si hubo una excepción al consultar las planes</exception>
        public ListResult<Plan> List(string filters, string orders, int limit, int offset)
        {
            try
            {
                return _persistent.List(filters, orders, limit, offset);
            }
            catch (PersistentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BusinessException("Error al consultar el listado de planes", ex);
            }
        }

        /// <summary>
        /// Consulta un plan dado su identificador
        /// </summary>
        /// <param name="entity">Plan a consultar</param>
        /// <returns>Plan con los datos cargados desde la base de datos o null si no lo pudo encontrar</returns>
        /// <exception cref="BusinessException">Si hubo una excepción al consultar el plan</exception>
        public Plan Read(Plan entity)
        {
            try
            {
                return _persistent.Read(entity);
            }
            catch (Exception ex)
            {
                throw new BusinessException("Error al consultar el plan", ex);
            }
        }

        /// <summary>
        /// Inserta un plan en la base de datos
        /// </summary>
        /// <param name="entity">Plan a insertar</param>
        /// <returns>Plan insertado con el id generado por la base de datos</returns>
        /// <param name="user">Usuario que realiza la inserción</param>
        /// <exception cref="BusinessException">Si hubo una excepción al insertar el plan</exception>
        public Plan Insert(Plan entity, User user)
        {
            try
            {
                return _persistent.Insert(entity, user);
            }
            catch (PersistentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BusinessException("Error al insertar el plan", ex);
            }
        }

        /// <summary>
        /// Actualiza un plan en la base de datos
        /// </summary>
        /// <param name="entity">Plan a actualizar</param>
        /// <param name="user">Usuario que realiza la actualización</param>
        /// <returns>Plan actualizada</returns>
        /// <exception cref="BusinessException">Si hubo una excepción al actualizar el plan</exception>
        public Plan Update(Plan entity, User user)
        {
            try
            {
                return _persistent.Update(entity, user);
            }
            catch (PersistentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BusinessException("Error al actualizar el plan", ex);
            }
        }

        /// <summary>
        /// Elimina un plan de la base de datos
        /// </summary>
        /// <param name="entity">Plan a eliminar</param>
        /// <param name="user">Usuario que realiza la eliminación</param>
        /// <returns>Plan eliminado</returns>
        /// <exception cref="BusinessException">Si hubo una excepción al eliminar el plan</exception>
        public Plan Delete(Plan entity, User user)
        {
            try
            {
                return _persistent.Delete(entity, user);
            }
            catch (PersistentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BusinessException("Error al eliminar el plan", ex);
            }
        }
        #endregion
    }
}
