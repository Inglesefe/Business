using Business.Exceptions;
using Dal.Auth;
using Dal.Dto;
using Dal.Exceptions;
using Entities.Auth;

namespace Business.Auth
{
    /// <summary>
    /// Realiza la persistencia de las aplicaciones en la base de datos
    /// </summary>
    public class BusinessApplication : BusinessBase<Application>, IBusinessApplication
    {
        #region Constructors
        /// <summary>
        /// Inicializa la persistencia
        /// </summary>
        /// <param name="persistent">Persistencia en base de datos de las aplicaciones</param>
        public BusinessApplication(IPersistentApplication persistent) : base(persistent) { }
        #endregion

        #region Methods
        /// <summary>
        /// Trae un listado de aplicaciones desde la base de datos
        /// </summary>
        /// <param name="filters">Filtros aplicados a la consulta</param>
        /// <param name="orders">Ordenamientos aplicados a la base de datos</param>
        /// <param name="limit">Límite de registros a traer</param>
        /// <param name="offset">Corrimiento desde el que se cuenta el número de registros</param>
        /// <returns>Listado de aplicaciones</returns>
        /// <exception cref="BusinessException">Si hubo una excepción al consultar las aplicaciones</exception>
        public ListResult<Application> List(string filters, string orders, int limit, int offset)
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
                throw new BusinessException("Error al consultar el listado de aplicaciones", ex);
            }
        }

        /// <summary>
        /// Consulta una aplicación dado su identificador
        /// </summary>
        /// <param name="entity">Aplicación a consultar</param>
        /// <returns>Aplicación con los datos cargados desde la base de datos o null si no lo pudo encontrar</returns>
        /// <exception cref="BusinessException">Si hubo una excepción al consultar la aplicación</exception>
        public Application Read(Application entity)
        {
            try
            {
                return _persistent.Read(entity);
            }
            catch (Exception ex)
            {
                throw new BusinessException("Error al consultar la aplicación", ex);
            }
        }

        /// <summary>
        /// Inserta una aplicación en la base de datos
        /// </summary>
        /// <param name="entity">Aplicación a insertar</param>
        /// <returns>Aplicación insertada con el id generado por la base de datos</returns>
        /// <param name="user">Usuario que realiza la inserción</param>
        /// <exception cref="BusinessException">Si hubo una excepción al insertar la aplicación</exception>
        public Application Insert(Application entity, User user)
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
                throw new BusinessException("Error al insertar la aplicación", ex);
            }
        }

        /// <summary>
        /// Actualiza una aplicación en la base de datos
        /// </summary>
        /// <param name="entity">Aplicación a actualizar</param>
        /// <param name="user">Usuario que realiza la actualización</param>
        /// <returns>Aplicación actualizada</returns>
        /// <exception cref="BusinessException">Si hubo una excepción al actualizar la aplicación</exception>
        public Application Update(Application entity, User user)
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
                throw new BusinessException("Error al actualizar la aplicación", ex);
            }
        }

        /// <summary>
        /// Elimina una aplicación de la base de datos
        /// </summary>
        /// <param name="entity">Aplicación a eliminar</param>
        /// <param name="user">Usuario que realiza la eliminación</param>
        /// <returns>Aplicación eliminada</returns>
        /// <exception cref="BusinessException">Si hubo una excepción al eliminar la aplicación</exception>
        public Application Delete(Application entity, User user)
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
                throw new BusinessException("Error al eliminar la aplicación", ex);
            }
        }

        /// <summary>
        /// Trae un listado de roles asignados a una aplicación desde la base de datos
        /// </summary>
        /// <param name="filters">Filtros aplicados a la consulta</param>
        /// <param name="orders">Ordenamientos aplicados a la base de datos</param>
        /// <param name="limit">Límite de registros a traer</param>
        /// <param name="offset">Corrimiento desde el que se cuenta el número de registros</param>
        /// <param name="application">Aplicación al que se le consultan los roles asignados</param>
        /// <returns>Listado de roles asignados a la aplicación</returns>
        /// <exception cref="BusinessException">Si hubo una excepción al consultar los roles</exception>
        public ListResult<Role> ListRoles(string filters, string orders, int limit, int offset, Application application)
        {
            try
            {
                return ((IPersistentApplication)_persistent).ListRoles(filters, orders, limit, offset, application);
            }
            catch (PersistentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BusinessException("Error al consultar el listado de roles asignados a la aplicación", ex);
            }
        }

        /// <summary>
        /// Trae un listado de roles no asignados a una aplicación desde la base de datos
        /// </summary>
        /// <param name="filters">Filtros aplicados a la consulta</param>
        /// <param name="orders">Ordenamientos aplicados a la base de datos</param>
        /// <param name="limit">Límite de registros a traer</param>
        /// <param name="offset">Corrimiento desde el que se cuenta el número de registros</param>
        /// <param name="application">Aplicación a la que se le consultan los roles no asignados</param>
        /// <returns>Listado de roles no asignados a la aplicación</returns>
        /// <exception cref="BusinessException">Si hubo una excepción al consultar los roles</exception>
        public ListResult<Role> ListNotRoles(string filters, string orders, int limit, int offset, Application application)
        {
            try
            {
                return ((IPersistentApplication)_persistent).ListNotRoles(filters, orders, limit, offset, application);
            }
            catch (PersistentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BusinessException("Error al consultar el listado de roles no asignados a la aplicación", ex);
            }
        }

        /// <summary>
        /// Asigna un rol a una aplicación en la base de datos
        /// </summary>
        /// <param name="role">Rol que se asigna a la aplicación</param>
        /// <param name="application">Aplicación al que se le asigna el rol</param>
        /// <param name="user">Usuario que realiza la inserción</param>
        /// <returns>Rol asignado</returns>
        /// <exception cref="BusinessException">Si hubo una excepción al asignar el rol a la aplicación</exception>
        public Role InsertRole(Role role, Application application, User user)
        {
            try
            {
                return ((IPersistentApplication)_persistent).InsertRole(role, application, user);
            }
            catch (PersistentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BusinessException("Error al asignar el rol a la aplicación", ex);
            }
        }

        /// <summary>
        /// Elimina un rol de una aplicación de la base de datos
        /// </summary>
        /// <param name="role">Rol a eliminarle a la aplicación</param>
        /// <param name="application">Aplicación al que se le elimina el rol</param>
        /// <param name="user">Usuario que realiza la inserción</param>
        /// <returns>Rol eliminado</returns>
        /// <exception cref="BusinessException">Si hubo una excepción al eliminar el rol de la aplicación</exception>
        public Role DeleteRole(Role role, Application application, User user)
        {
            try
            {
                return ((IPersistentApplication)_persistent).DeleteRole(role, application, user);
            }
            catch (PersistentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BusinessException("Error al eliminar el rol de la aplicación", ex);
            }
        }
        #endregion
    }
}
