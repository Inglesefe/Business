using Business.Exceptions;
using Dal.Auth;
using Dal.Dto;
using Dal.Exceptions;
using Entities.Auth;

namespace Business.Auth
{
    /// <summary>
    /// Realiza la persistencia de los roles en la base de datos
    /// </summary>
    public class BusinessRole : BusinessBase<Role>, IBusinessRole
    {
        #region Constructors
        /// <summary>
        /// Inicializa la conexión a la bse de datos
        /// </summary>
        /// <param name="persistent">Persistencia en base de datos de los roles</param>
        public BusinessRole(IPersistentRole persistent) : base(persistent) { }
        #endregion

        #region Methods
        /// <summary>
        /// Trae un listado de usuarios asignados a un rol desde la base de datos
        /// </summary>
        /// <param name="filters">Filtros aplicados a la consulta</param>
        /// <param name="orders">Ordenamientos aplicados a la base de datos</param>
        /// <param name="limit">Límite de registros a traer</param>
        /// <param name="offset">Corrimiento desde el que se cuenta el número de registros</param>
        /// <param name="role">Rol al que se le consultan los usuarios asignados</param>
        /// <returns>Listado de usuarios asignados al rol</returns>
        /// <exception cref="BusinessException">Si hubo una excepción al consultar los usuarios</exception>
        public ListResult<User> ListUsers(string filters, string orders, int limit, int offset, Role role)
        {
            try
            {
                return ((IPersistentRole)_persistent).ListUsers(filters, orders, limit, offset, role);
            }
            catch (PersistentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BusinessException("Error al listar los usuarios asociados al rol", ex);
            }
        }

        /// <summary>
        /// Trae un listado de usuarios no asignados a un rol desde la base de datos
        /// </summary>
        /// <param name="filters">Filtros aplicados a la consulta</param>
        /// <param name="orders">Ordenamientos aplicados a la base de datos</param>
        /// <param name="limit">Límite de registros a traer</param>
        /// <param name="offset">Corrimiento desde el que se cuenta el número de registros</param>
        /// <param name="role">Rol al que se le consultan los usuarios no asignados</param>
        /// <returns>Listado de usuarios no asignados al rol</returns>
        /// <exception cref="BusinessException">Si hubo una excepción al consultar los usuarios</exception>
        public ListResult<User> ListNotUsers(string filters, string orders, int limit, int offset, Role role)
        {
            try
            {
                return ((IPersistentRole)_persistent).ListNotUsers(filters, orders, limit, offset, role);
            }
            catch (PersistentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BusinessException("Error al listar los usuarios no asociados al rol", ex);
            }
        }

        /// <summary>
        /// Asigna un usuario a un rol en la base de datos
        /// </summary>
        /// <param name="user">Usuario que se asigna al rol</param>
        /// <param name="role">Rol al que se le asigna el usuario</param>
        /// <param name="user1">Usuario que realiza la inserción</param>
        /// <returns>Usuario asignado</returns>
        /// <exception cref="BusinessException">Si hubo una excepción al asignar el usuario al rol</exception>
        public User InsertUser(User user, Role role, User user1)
        {
            try
            {
                return ((IPersistentRole)_persistent).InsertUser(user, role, user1);
            }
            catch (PersistentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BusinessException("Error al insertar un usuario asociado al rol", ex);
            }
        }

        /// <summary>
        /// Elimina un usuario de un rol de la base de datos
        /// </summary>
        /// <param name="user">Usuario a eliminarle al rol</param>
        /// <param name="role">Rol al que se le elimina el usuario</param>
        /// <param name="user1">Usuario que realiza la eliminación</param>
        /// <returns>Usuario eliminado</returns>
        /// <exception cref="BusinessException">Si hubo una excepción al eliminar el usuario del rol</exception>
        public User DeleteUser(User user, Role role, User user1)
        {
            try
            {
                return ((IPersistentRole)_persistent).DeleteUser(user, role, user1);
            }
            catch (PersistentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BusinessException("Error al eliminar un usuario asociado al rol", ex);
            }
        }

        /// <summary>
        /// Trae un listado de aplicaciones asignados a un rol desde la base de datos
        /// </summary>
        /// <param name="filters">Filtros aplicados a la consulta</param>
        /// <param name="orders">Ordenamientos aplicados a la base de datos</param>
        /// <param name="limit">Límite de registros a traer</param>
        /// <param name="offset">Corrimiento desde el que se cuenta el número de registros</param>
        /// <param name="role">Rol al que se le consultan las aplicaciones asignadas</param>
        /// <returns>Listado de aplicaciones asignadas al rol</returns>
        /// <exception cref="BusinessException">Si hubo una excepción al consultar las aplicaciones</exception>
        public ListResult<Application> ListApplications(string filters, string orders, int limit, int offset, Role role)
        {
            try
            {
                return ((IPersistentRole)_persistent).ListApplications(filters, orders, limit, offset, role);
            }
            catch (PersistentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BusinessException("Error al listar las aplicaciones asociadas al rol", ex);
            }
        }

        /// <summary>
        /// Trae un listado de aplicaciones no asignados a un rol desde la base de datos
        /// </summary>
        /// <param name="filters">Filtros aplicados a la consulta</param>
        /// <param name="orders">Ordenamientos aplicados a la base de datos</param>
        /// <param name="limit">Límite de registros a traer</param>
        /// <param name="offset">Corrimiento desde el que se cuenta el número de registros</param>
        /// <param name="role">Rol al que se le consultan las aplicaciones no asignadas</param>
        /// <returns>Listado de aplicaciones no asignadas al rol</returns>
        /// <exception cref="BusinessException">Si hubo una excepción al consultar los usuarios</exception>
        public ListResult<Application> ListNotApplications(string filters, string orders, int limit, int offset, Role role)
        {
            try
            {
                return ((IPersistentRole)_persistent).ListNotApplications(filters, orders, limit, offset, role);
            }
            catch (PersistentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BusinessException("Error al listar las aplicaciones no asociadas al rol", ex);
            }
        }

        /// <summary>
        /// Asigna una aplicación a un rol en la base de datos
        /// </summary>
        /// <param name="application">Aplicación que se asigna al rol</param>
        /// <param name="role">Rol al que se le asigna la aplicación</param>
        /// <param name="user">Usuario que realiza la inserción</param>
        /// <returns>Aplicación asignada</returns>
        /// <exception cref="BusinessException">Si hubo una excepción al asignar la aplicación al rol</exception>
        public Application InsertApplication(Application application, Role role, User user)
        {
            try
            {
                return ((IPersistentRole)_persistent).InsertApplication(application, role, user);
            }
            catch (PersistentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BusinessException("Error al insertar una aplicación asociada al rol", ex);
            }
        }

        /// <summary>
        /// Elimina una aplicación de un rol de la base de datos
        /// </summary>
        /// <param name="application">Aplicación a eliminarle al rol</param>
        /// <param name="role">Rol al que se le elimina la aplicación</param>
        /// <param name="user">Usuario que realiza la eliminación</param>
        /// <returns>Aplicación eliminada</returns>
        /// <exception cref="BusinessException">Si hubo una excepción al eliminar la aplicación del rol</exception>
        public Application DeleteApplication(Application application, Role role, User user)
        {
            try
            {
                return ((IPersistentRole)_persistent).DeleteApplication(application, role, user);
            }
            catch (PersistentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BusinessException("Error al eliminar una aplicción asociada al rol", ex);
            }
        }
        #endregion
    }
}
