using Business.Exceptions;
using Dal;
using Dal.Dto;
using Dal.Exceptions;
using Entities.Auth;
using Entities.Config;

namespace Business.Config
{
    /// <summary>
    /// Realiza la persistencia de los parámetros en la base de datos
    /// </summary>
    public class BusinessParameter : BusinessBase<Parameter>, IBusiness<Parameter>
    {
        #region Constructors
        /// <summary>
        /// Inicializa la persistencia
        /// </summary>
        /// <param name="persistent">Persistencia en base de datos de los parámetros</param>
        public BusinessParameter(IPersistentWithLog<Parameter> persistent) : base(persistent) { }
        #endregion

        #region Methods
        /// <summary>
        /// Trae un listado de parámetros desde la base de datos
        /// </summary>
        /// <param name="filters">Filtros aplicados a la consulta</param>
        /// <param name="orders">Ordenamientos aplicados a la base de datos</param>
        /// <param name="limit">Límite de registros a traer</param>
        /// <param name="offset">Corrimiento desde el que se cuenta el número de registros</param>
        /// <returns>Listado de parámetros</returns>
        /// <exception cref="BusinessException">Si hubo una excepción al consultar las parámetros</exception>
        public ListResult<Parameter> List(string filters, string orders, int limit, int offset)
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
                throw new BusinessException("Error al consultar el listado de parámetros", ex);
            }
        }

        /// <summary>
        /// Consulta un parámetro dado su identificador
        /// </summary>
        /// <param name="entity">Parámetro a consultar</param>
        /// <returns>Parámetro con los datos cargados desde la base de datos o null si no lo pudo encontrar</returns>
        /// <exception cref="BusinessException">Si hubo una excepción al consultar el parámetro</exception>
        public Parameter Read(Parameter entity)
        {
            try
            {
                return _persistent.Read(entity);
            }
            catch (Exception ex)
            {
                throw new BusinessException("Error al consultar el parámetro", ex);
            }
        }

        /// <summary>
        /// Inserta un parámetro en la base de datos
        /// </summary>
        /// <param name="entity">Parámetro a insertar</param>
        /// <returns>Parámetro insertado con el id generado por la base de datos</returns>
        /// <param name="user">Usuario que realiza la inserción</param>
        /// <exception cref="BusinessException">Si hubo una excepción al insertar el parámetro</exception>
        public Parameter Insert(Parameter entity, User user)
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
                throw new BusinessException("Error al insertar el parámetro", ex);
            }
        }

        /// <summary>
        /// Actualiza un parámetro en la base de datos
        /// </summary>
        /// <param name="entity">Parámetro a actualizar</param>
        /// <param name="user">Usuario que realiza la actualización</param>
        /// <returns>Parámetro actualizada</returns>
        /// <exception cref="BusinessException">Si hubo una excepción al actualizar el parámetro</exception>
        public Parameter Update(Parameter entity, User user)
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
                throw new BusinessException("Error al actualizar el parámetro", ex);
            }
        }

        /// <summary>
        /// Elimina un parámetro de la base de datos
        /// </summary>
        /// <param name="entity">Parámetro a eliminar</param>
        /// <param name="user">Usuario que realiza la eliminación</param>
        /// <returns>Parámetro eliminado</returns>
        /// <exception cref="BusinessException">Si hubo una excepción al eliminar el parámetro</exception>
        public Parameter Delete(Parameter entity, User user)
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
                throw new BusinessException("Error al eliminar el parámetro", ex);
            }
        }
        #endregion
    }
}
