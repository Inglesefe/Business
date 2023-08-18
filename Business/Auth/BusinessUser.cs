using Business.Exceptions;
using Dal.Auth;
using Dal.Dto;
using Dal.Exceptions;
using Entities.Auth;
using System.Data;
using System.Security.Cryptography;
using System.Text;

namespace Business.Auth
{
    /// <summary>
    /// Lógica aplicada a los usuarios del módulo de autenticación y autorización
    /// </summary>
    public class BusinessUser : BusinessBase<User>
    {
        #region Constructors
        /// <summary>
        /// Inicializa la persistencia
        /// </summary>
        /// <param name="connection"></param>
        public BusinessUser(IDbConnection connection) : base(new PersistentUser(connection)) { }
        #endregion

        #region Methods
        /// <summary>
        /// Trae un listado de usuarios desde la base de datos
        /// </summary>
        /// <param name="filters">Filtros aplicados a la consulta</param>
        /// <param name="orders">Ordenamientos aplicados a la base de datos</param>
        /// <param name="limit">Límite de registros a traer</param>
        /// <param name="offset">Corrimiento desde el que se cuenta el número de registros</param>
        /// <returns>Listado de usuarios</returns>
        /// <exception cref="BusinessException">Si hubo una excepción al consultar los usuarios</exception>
        public override ListResult<User> List(string filters, string orders, int limit, int offset)
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
                throw new BusinessException("Error al consultar el listado de usuarios", ex);
            }
        }

        /// <summary>
        /// Consulta un usuario dado su identificador
        /// </summary>
        /// <param name="entity">Usuario a consultar</param>
        /// <returns>Usuario con los datos cargados desde la base de datos o null si no lo pudo encontrar</returns>
        /// <exception cref="BusinessException">Si hubo una excepción al consultar los usuarios</exception>
        public override User Read(User entity)
        {
            try
            {
                return _persistent.Read(entity);
            }
            catch (PersistentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BusinessException("Error al consultar el usuario", ex);
            }
        }

        /// <summary>
        /// Inserta un usuario en la base de datos
        /// </summary>
        /// <param name="entity">Usuario a insertar</param>
        /// <param name="user">Usuario que realiza la inserción</param>
        /// <returns>Usuario insertado con el id generado por la base de datos</returns>
        /// <exception cref="BusinessException">Si hubo una excepción al insertar el usuario</exception>
        public override User Insert(User entity, User user)
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
                throw new BusinessException("Error al insertar el usuario", ex);
            }
        }

        /// <summary>
        /// Actualiza un usuario en la base de datos
        /// </summary>
        /// <param name="entity">Usuario a actualizar</param>
        /// <param name="user">Usuario que realiza la actualización</param>
        /// <returns>Usuario actualizado</returns>
        /// <exception cref="BusinessException">Si hubo una excepción al actualizar el usuario</exception>
        public override User Update(User entity, User user)
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
                throw new BusinessException("Error al actualizar el usuario", ex);
            }
        }

        /// <summary>
        /// Elimina un usuario de la base de datos
        /// </summary>
        /// <param name="entity">Usuario a eliminar</param>
        /// <param name="user">Usuario que realiza la eliminación</param>
        /// <returns>Usuario eliminado</returns>
        /// <exception cref="BusinessException">Si hubo una excepción al eliminar el usuario</exception>
        public override User Delete(User entity, User user)
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
                throw new BusinessException("Error al eliminar el usuario", ex);
            }
        }

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
                string plainPassword = "";
                using (Aes aes = Aes.Create())
                {
                    aes.Key = Encoding.UTF8.GetBytes(key);
                    aes.IV = Encoding.UTF8.GetBytes(iv);

                    ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                    using MemoryStream msDecrypt = new(Convert.FromBase64String(password));
                    using CryptoStream csDecrypt = new(msDecrypt, decryptor, CryptoStreamMode.Read);
                    using StreamReader srDecrypt = new(csDecrypt);
                    plainPassword = srDecrypt.ReadToEnd();
                }
                return ((PersistentUser)_persistent).ReadByLoginAndPassword(entity, plainPassword);
            }
            catch (PersistentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BusinessException("Error al consultar el usuario", ex);
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
                return ((PersistentUser)_persistent).ReadByLogin(entity);
            }
            catch (PersistentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BusinessException("Error al consultar el usuario", ex);
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
                string plainPassword = "";
                using (Aes aes = Aes.Create())
                {
                    aes.Key = Encoding.UTF8.GetBytes(key);
                    aes.IV = Encoding.UTF8.GetBytes(iv);

                    ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                    using MemoryStream msDecrypt = new(Convert.FromBase64String(password));
                    using CryptoStream csDecrypt = new(msDecrypt, decryptor, CryptoStreamMode.Read);
                    using StreamReader srDecrypt = new(csDecrypt);
                    plainPassword = srDecrypt.ReadToEnd();
                }
                return ((PersistentUser)_persistent).UpdatePassword(entity, plainPassword, user);
            }
            catch (PersistentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BusinessException("Error al actualizar el usuario", ex);
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
                return ((PersistentUser)_persistent).ListRoles(filters, orders, limit, offset, user);
            }
            catch (PersistentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BusinessException("Error al consultar el listado de roles asignados al usuario", ex);
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
                return ((PersistentUser)_persistent).ListNotRoles(filters, orders, limit, offset, user);
            }
            catch (PersistentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BusinessException("Error al consultar el listado de roles no asignados al usuario", ex);
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
                return ((PersistentUser)_persistent).InsertRole(role, user, user1);
            }
            catch (PersistentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BusinessException("Error al asignar el rol al usuario", ex);
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
                return ((PersistentUser)_persistent).DeleteRole(role, user, user1);
            }
            catch (PersistentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BusinessException("Error al eliminar el rol del usuario", ex);
            }
        }
        #endregion
    }
}
