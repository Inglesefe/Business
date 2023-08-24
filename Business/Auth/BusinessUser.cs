using Business.Exceptions;
using Business.Util;
using Dal.Auth;
using Dal.Dto;
using Dal.Exceptions;
using Entities.Auth;

namespace Business.Auth
{
    /// <summary>
    /// Lógica aplicada a los usuarios del módulo de autenticación y autorización
    /// </summary>
    public class BusinessUser : BusinessBase<User>, IBusinessUser
    {
        #region Constructors
        /// <summary>
        /// Inicializa la persistencia
        /// </summary>
        /// <param name="persistent">Persistencia en base de datos de los usuarios</param>
        public BusinessUser(IPersistentUser persistent) : base(persistent) { }
        #endregion

        #region Methods
        /// <summary>
        /// Consulta un usuario dado su login y contraseña
        /// </summary>
        /// <param name="entity">Usuario a consultar</param>
        /// <param name="password">Contraseña del usuario</param>
        /// <param name="key">Llave para el algoritmo AES</param>
        /// <param name="iv">Vector de inicializaci{on para el algoritmo AES</param>
        /// <returns>Usuario con los datos cargados desde la base de datos</returns>
        /// <exception cref="BusinessException">Si hubo una excepción al consultar el usuario</exception>
        public User ReadByLoginAndPassword(User entity, string password, string key, string iv)
        {
            try
            {
                return ((IPersistentUser)_persistent).ReadByLoginAndPassword(entity, Crypto.Decrypt(password, key, iv));
            }
            catch (PersistentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BusinessException("Error at get an user", ex);
            }
        }

        /// <summary>
        /// Consulta un usuario dado su login y si est{a activo
        /// </summary>
        /// <param name="entity">Usuario a consultar</param>
        /// <returns>Usuario con los datos cargados desde la base de datos</returns>
        /// <exception cref="BusinessException">Si hubo una excepción al consultar el usuario</exception>
        public User ReadByLogin(User entity)
        {
            try
            {
                return ((IPersistentUser)_persistent).ReadByLogin(entity);
            }
            catch (PersistentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BusinessException("Error at get an user", ex);
            }
        }

        /// <summary>
        /// Actualiza la contraseña de un usuario en la base de datos
        /// </summary>
        /// <param name="entity">Usuario a actualizar</param>
        /// <param name="password">Nueva contraseña del usuario</param>
        /// <param name="user">Usuario que realiza la actualización</param>
        /// <param name="key">Llave para el algoritmo AES</param>
        /// <param name="iv">Vector de inicializaci{on para el algoritmo AES</param>
        /// <returns>Usuario actualizado</returns>
        /// <exception cref="BusinessException">Si hubo una excepción al actualizar el usuario</exception>
        public User UpdatePassword(User entity, string password, string key, string iv, User user)
        {
            try
            {
                return ((IPersistentUser)_persistent).UpdatePassword(entity, Crypto.Decrypt(password, key, iv), user);
            }
            catch (PersistentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BusinessException("Error at update password of an user", ex);
            }
        }

        /// <summary>
        /// Trae un listado de roles asignados a un usuario desde la base de datos
        /// </summary>
        /// <param name="filters">Filtros aplicados a la consulta</param>
        /// <param name="orders">Ordenamientos aplicados a la base de datos</param>
        /// <param name="limit">Límite de registros a traer</param>
        /// <param name="offset">Corrimiento desde el que se cuenta el número de registros</param>
        /// <param name="user">Usuario al que se le consultan los roles asignados</param>
        /// <returns>Listado de roles asignados al usuario</returns>
        /// <exception cref="BusinessException">Si hubo una excepción al consultar los roles</exception>
        public ListResult<Role> ListRoles(string filters, string orders, int limit, int offset, User user)
        {
            try
            {
                return ((IPersistentUser)_persistent).ListRoles(filters, orders, limit, offset, user);
            }
            catch (PersistentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BusinessException("Error at get roles related with an user", ex);
            }
        }

        /// <summary>
        /// Trae un listado de roles no asignados a un usuario desde la base de datos
        /// </summary>
        /// <param name="filters">Filtros aplicados a la consulta</param>
        /// <param name="orders">Ordenamientos aplicados a la base de datos</param>
        /// <param name="limit">Límite de registros a traer</param>
        /// <param name="offset">Corrimiento desde el que se cuenta el número de registros</param>
        /// <param name="user">Usuario al que se le consultan los roles no asignados</param>
        /// <returns>Listado de roles no asignados al usuario</returns>
        /// <exception cref="BusinessException">Si hubo una excepción al consultar los roles</exception>
        public ListResult<Role> ListNotRoles(string filters, string orders, int limit, int offset, User user)
        {
            try
            {
                return ((IPersistentUser)_persistent).ListNotRoles(filters, orders, limit, offset, user);
            }
            catch (PersistentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BusinessException("Error at get roles not related with an user", ex);
            }
        }

        /// <summary>
        /// Asigna un rol a un usuario en la base de datos
        /// </summary>
        /// <param name="role">Rol que se asigna al usuario</param>
        /// <param name="user">Usuario al que se le asigna el rol</param>
        /// <param name="user1">Usuario que realiza la inserción</param>
        /// <returns>Rol asignado</returns>
        /// <exception cref="BusinessException">Si hubo una excepción al asignar el rol al usuario</exception>
        public Role InsertRole(Role role, User user, User user1)
        {
            try
            {
                return ((IPersistentUser)_persistent).InsertRole(role, user, user1);
            }
            catch (PersistentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BusinessException("Error at insert role related with an user", ex);
            }
        }

        /// <summary>
        /// Elimina un rol de un usuario de la base de datos
        /// </summary>
        /// <param name="role">Rol a eliminarle al usuario</param>
        /// <param name="user">Usuario al que se le elimina el rol</param>
        /// <param name="user1">Usuario que realiza la eliminación</param>
        /// <returns>Rol eliminado</returns>
        /// <exception cref="BusinessException">Si hubo una excepción al eliminar el rol del usuario</exception>
        public Role DeleteRole(Role role, User user, User user1)
        {
            try
            {
                return ((IPersistentUser)_persistent).DeleteRole(role, user, user1);
            }
            catch (PersistentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BusinessException("Error at delete role related with an user", ex);
            }
        }
        #endregion
    }
}
