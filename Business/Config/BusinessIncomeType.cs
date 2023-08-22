using Business.Exceptions;
using Dal;
using Dal.Dto;
using Dal.Exceptions;
using Entities.Auth;
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

        #region Methods
        /// <summary>
        /// Trae un listado de tipos de ingreso desde la base de datos
        /// </summary>
        /// <param name="filters">Filtros aplicados a la consulta</param>
        /// <param name="orders">Ordenamientos aplicados a la base de datos</param>
        /// <param name="limit">Límite de registros a traer</param>
        /// <param name="offset">Corrimiento desde el que se cuenta el número de registros</param>
        /// <returns>Listado de tipos de ingreso</returns>
        /// <exception cref="BusinessException">Si hubo una excepción al consultar las tipos de ingreso</exception>
        public ListResult<IncomeType> List(string filters, string orders, int limit, int offset)
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
                throw new BusinessException("Error al consultar el listado de tipos de ingreso", ex);
            }
        }

        /// <summary>
        /// Consulta un tipo de ingreso dado su identificador
        /// </summary>
        /// <param name="entity">Tipo de ingreso a consultar</param>
        /// <returns>Tipo de ingreso con los datos cargados desde la base de datos o null si no lo pudo encontrar</returns>
        /// <exception cref="BusinessException">Si hubo una excepción al consultar el tipo de ingreso</exception>
        public IncomeType Read(IncomeType entity)
        {
            try
            {
                return _persistent.Read(entity);
            }
            catch (Exception ex)
            {
                throw new BusinessException("Error al consultar el tipo de ingreso", ex);
            }
        }

        /// <summary>
        /// Inserta un tipo de ingreso en la base de datos
        /// </summary>
        /// <param name="entity">Tipo de ingreso a insertar</param>
        /// <returns>Tipo de ingreso insertado con el id generado por la base de datos</returns>
        /// <param name="user">Usuario que realiza la inserción</param>
        /// <exception cref="BusinessException">Si hubo una excepción al insertar el tipo de ingreso</exception>
        public IncomeType Insert(IncomeType entity, User user)
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
                throw new BusinessException("Error al insertar el tipo de ingreso", ex);
            }
        }

        /// <summary>
        /// Actualiza un tipo de ingreso en la base de datos
        /// </summary>
        /// <param name="entity">Tipo de ingreso a actualizar</param>
        /// <param name="user">Usuario que realiza la actualización</param>
        /// <returns>Tipo de ingreso actualizada</returns>
        /// <exception cref="BusinessException">Si hubo una excepción al actualizar el tipo de ingreso</exception>
        public IncomeType Update(IncomeType entity, User user)
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
                throw new BusinessException("Error al actualizar el tipo de ingreso", ex);
            }
        }

        /// <summary>
        /// Elimina un tipo de ingreso de la base de datos
        /// </summary>
        /// <param name="entity">Tipo de ingreso a eliminar</param>
        /// <param name="user">Usuario que realiza la eliminación</param>
        /// <returns>Tipo de ingreso eliminado</returns>
        /// <exception cref="BusinessException">Si hubo una excepción al eliminar el tipo de ingreso</exception>
        public IncomeType Delete(IncomeType entity, User user)
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
                throw new BusinessException("Error al eliminar el tipo de ingreso", ex);
            }
        }
        #endregion
    }
}
