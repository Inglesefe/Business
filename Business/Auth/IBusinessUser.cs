using Business.Exceptions;
using Dal.Dto;
using Entities.Auth;
using System.Data;

namespace Business.Auth
{
    public interface IBusinessUser : IBusiness<User>
    {
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
        User ReadByLoginAndPassword(User entity, string password, string key, string iv, IDbConnection connection);

        /// <summary>
        /// Consulta un usuario dado su login y si est{a activo
        /// </summary>
        /// <param name="entity">Usuario a consultar</param>
        /// <returns>Usuario con los datos cargados desde la base de datos</returns>
        /// <exception cref="BusinessException">Si hubo una excepción al consultar el usuario</exception>
        User ReadByLogin(User entity, IDbConnection connection);

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
        User UpdatePassword(User entity, string password, string key, string iv, User user, IDbConnection connection);

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
        ListResult<Role> ListRoles(string filters, string orders, int limit, int offset, User user, IDbConnection connection);

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
        ListResult<Role> ListNotRoles(string filters, string orders, int limit, int offset, User user, IDbConnection connection);

        /// <summary>
        /// Asigna un rol a un usuario en la base de datos
        /// </summary>
        /// <param name="role">Rol que se asigna al usuario</param>
        /// <param name="user">Usuario al que se le asigna el rol</param>
        /// <param name="user1">Usuario que realiza la inserción</param>
        /// <returns>Rol asignado</returns>
        /// <exception cref="BusinessException">Si hubo una excepción al asignar el rol al usuario</exception>
        Role InsertRole(Role role, User user, User user1, IDbConnection connection);

        /// <summary>
        /// Elimina un rol de un usuario de la base de datos
        /// </summary>
        /// <param name="role">Rol a eliminarle al usuario</param>
        /// <param name="user">Usuario al que se le elimina el rol</param>
        /// <param name="user1">Usuario que realiza la eliminación</param>
        /// <returns>Rol eliminado</returns>
        /// <exception cref="BusinessException">Si hubo una excepción al eliminar el rol del usuario</exception>
        Role DeleteRole(Role role, User user, User user1, IDbConnection connection);
        #endregion
    }
}
