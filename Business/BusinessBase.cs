using Business.Exceptions;
using Dal;
using Dal.Dto;
using Dal.Exceptions;
using Entities;
using Entities.Auth;
using System.Data;

namespace Business
{
    /// <summary>
    /// Clase base para el manejo de la capa de negocios de las distintas
    /// entidades del módulo de autenticación y autorización
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BusinessBase<T> : IBusiness<T> where T : EntityBase
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

        #region Methods
        /// <summary>
        /// Trae un listado de entidades desde la base de datos
        /// </summary>
        /// <param name="filters">Filtros aplicados a la consulta</param>
        /// <param name="orders">Ordenamientos aplicados a la base de datos</param>
        /// <param name="limit">Límite de registros a traer</param>
        /// <param name="offset">Corrimiento desde el que se cuenta el número de registros</param>
        /// <param name="connection">Conexión a la base de datos</param>
        /// <returns>Listado de entidades</returns>
        /// <exception cref="BusinessException">Si hubo una excepción al consultar las entidades</exception>
        public ListResult<T> List(string filters, string orders, int limit, int offset, IDbConnection connection)
        {
            try
            {
                return _persistent.List(filters, orders, limit, offset, connection);
            }
            catch (PersistentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BusinessException("Error at get " + typeof(T).Name + " list", ex);
            }
        }

        /// <summary>
        /// Consulta una entidad dado su identificador
        /// </summary>
        /// <param name="entity">Entidad a consultar</param>
        /// <param name="connection">Conexión a la base de datos</param>
        /// <returns>Entidad con los datos cargados desde la base de datos o por defecto si no lo pudo encontrar</returns>
        /// <exception cref="BusinessException">Si hubo una excepción al consultar la entidad</exception>
        public T Read(T entity, IDbConnection connection)
        {
            try
            {
                return _persistent.Read(entity, connection);
            }
            catch (Exception ex)
            {
                throw new BusinessException("Error at get " + typeof(T).Name + " with id = " + entity.Id, ex);
            }
        }

        /// <summary>
        /// Inserta una entidad en la base de datos
        /// </summary>
        /// <param name="entity">Entidad a insertar</param>
        /// <param name="user">Usuario que realiza la inserción</param>
        /// <param name="connection">Conexión a la base de datos</param>
        /// <returns>Entidad insertada con el id generado por la base de datos</returns>
        /// <exception cref="BusinessException">Si hubo una excepción al insertar la entidad</exception>
        public T Insert(T entity, User user, IDbConnection connection)
        {
            try
            {
                return _persistent.Insert(entity, user, connection);
            }
            catch (PersistentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BusinessException("Error at insert " + typeof(T).Name, ex);
            }
        }

        /// <summary>
        /// Actualiza una entidad en la base de datos
        /// </summary>
        /// <param name="entity">Entidad a actualizar</param>
        /// <param name="user">Usuario que realiza la actualización</param>
        /// <param name="connection">Conexión a la base de datos</param>
        /// <returns>Entidad actualizada</returns>
        /// <exception cref="BusinessException">Si hubo una excepción al actualizar la entidad</exception>
        public T Update(T entity, User user, IDbConnection connection)
        {
            try
            {
                return _persistent.Update(entity, user, connection);
            }
            catch (PersistentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BusinessException("Error at update " + typeof(T).Name + " with id = " + entity.Id, ex);
            }
        }

        /// <summary>
        /// Elimina una entidad de la base de datos
        /// </summary>
        /// <param name="entity">Entidad a eliminar</param>
        /// <param name="user">Usuario que realiza la eliminación</param>
        /// <param name="connection">Conexión a la base de datos</param>
        /// <returns>Entidad eliminada</returns>
        /// <exception cref="BusinessException">Si hubo una excepción al eliminar la entidad</exception>
        public T Delete(T entity, User user, IDbConnection connection)
        {
            try
            {
                return _persistent.Delete(entity, user, connection);
            }
            catch (PersistentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BusinessException("Error at delete " + typeof(T).Name + " with id = " + entity.Id, ex);
            }
        }

        #endregion
    }
}
