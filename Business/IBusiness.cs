﻿using Dal.Dto;
using Entities;
using Entities.Auth;

namespace Business
{
    /// <summary>
    /// Define los métodos que debe tener toda capa de negocio
    /// </summary>
    /// <typeparam name="T">Entidad que se maneja en la capa de negocio</typeparam>
    public interface IBusiness<T> where T : IEntity
    {
        #region Methods
        /// <summary>
        /// Trae un listado de entidades desde la base de datos
        /// </summary>
        /// <param name="filters">Filtros aplicados a la consulta</param>
        /// <param name="orders">Ordenamientos aplicados a la base de datos</param>
        /// <param name="limit">Límite de registros a traer</param>
        /// <param name="offset">Corrimiento desde el que se cuenta el número de registros</param>
        /// <returns>Listado de usuarios</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al consultar los usuarios</exception>
        /// <exception cref="BusinessException">Si hubo una excepción al consultar los usuarios</exception>
        ListResult<T> List(string filters, string orders, int limit, int offset);

        /// <summary>
        /// Consulta una entidad dado su identificador
        /// </summary>
        /// <param name="entity">Entidad a consultar</param>
        /// <returns>Entidad con los datos cargados desde la base de datos o null si no la pudo encontrar</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al consultar la entidad</exception>
        T Read(T entity);

        /// <summary>
        /// Inserta una entidad en la base de datos
        /// </summary>
        /// <param name="entity">Entidad a insertar</param>
        /// <param name="user">Usuario que realiza la inserción</param>
        /// <returns>Entidad insertada con el id generado por la base de datos</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al insertar la entidad</exception>
        T Insert(T entity, User user);

        /// <summary>
        /// Actualiza una entidad en la base de datos
        /// </summary>
        /// <param name="entity">Entidad a actualizar</param>
        /// <param name="user">Usuario que realiza la actualización</param>
        /// <returns>Entidad actualizada</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al actualizar la entidad</exception>
        T Update(T entity, User user);

        /// <summary>
        /// Elimina una entidad de la base de datos
        /// </summary>
        /// <param name="entity">Entidad a eliminar</param>
        /// <param name="user">Usuario que realiza la eliminación</param>
        /// <returns>Entidad eliminada</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al eliminar la entidad</exception>
        T Delete(T entity, User user);
        #endregion
    }
}
