using Business.Exceptions;
using Dal;
using Dal.Dto;
using Dal.Exceptions;
using Entities.Auth;
using Entities.Config;

namespace Business.Config
{
    /// <summary>
    /// Realiza la persistencia de las oficinas en la base de datos
    /// </summary>
    public class BusinessOffice : BusinessBase<Office>, IBusiness<Office>
    {
        #region Constructors
        /// <summary>
        /// Inicializa la persistencia
        /// </summary>
        /// <param name="persistent">Persistencia en base de datos de las oficinas</param>
        public BusinessOffice(IPersistentWithLog<Office> persistent) : base(persistent) { }
        #endregion

        #region Methods
        /// <summary>
        /// Trae un listado de oficinas desde la base de datos
        /// </summary>
        /// <param name="filters">Filtros aplicados a la consulta</param>
        /// <param name="orders">Ordenamientos aplicados a la base de datos</param>
        /// <param name="limit">Límite de registros a traer</param>
        /// <param name="offset">Corrimiento desde el que se cuenta el número de registros</param>
        /// <returns>Listado de oficinas</returns>
        /// <exception cref="BusinessException">Si hubo una excepción al consultar las oficinas</exception>
        public ListResult<Office> List(string filters, string orders, int limit, int offset)
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
                throw new BusinessException("Error al consultar el listado de oficinas", ex);
            }
        }

        /// <summary>
        /// Consulta una oficina dado su identificador
        /// </summary>
        /// <param name="entity">Oficina a consultar</param>
        /// <returns>Oficina con los datos cargados desde la base de datos o null si no lo pudo encontrar</returns>
        /// <exception cref="BusinessException">Si hubo una excepción al consultar la oficina</exception>
        public Office Read(Office entity)
        {
            try
            {
                return _persistent.Read(entity);
            }
            catch (Exception ex)
            {
                throw new BusinessException("Error al consultar la oficina", ex);
            }
        }

        /// <summary>
        /// Inserta una oficina en la base de datos
        /// </summary>
        /// <param name="entity">Oficina a insertar</param>
        /// <returns>Oficina insertado con el id generado por la base de datos</returns>
        /// <param name="user">Usuario que realiza la inserción</param>
        /// <exception cref="BusinessException">Si hubo una excepción al insertar la oficina</exception>
        public Office Insert(Office entity, User user)
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
                throw new BusinessException("Error al insertar la oficina", ex);
            }
        }

        /// <summary>
        /// Actualiza una oficina en la base de datos
        /// </summary>
        /// <param name="entity">Oficina a actualizar</param>
        /// <param name="user">Usuario que realiza la actualización</param>
        /// <returns>Oficina actualizada</returns>
        /// <exception cref="BusinessException">Si hubo una excepción al actualizar la oficina</exception>
        public Office Update(Office entity, User user)
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
                throw new BusinessException("Error al actualizar la oficina", ex);
            }
        }

        /// <summary>
        /// Elimina una oficina de la base de datos
        /// </summary>
        /// <param name="entity">Oficina a eliminar</param>
        /// <param name="user">Usuario que realiza la eliminación</param>
        /// <returns>Oficina eliminado</returns>
        /// <exception cref="BusinessException">Si hubo una excepción al eliminar la oficina</exception>
        public Office Delete(Office entity, User user)
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
                throw new BusinessException("Error al eliminar la oficina", ex);
            }
        }
        #endregion
    }
}
