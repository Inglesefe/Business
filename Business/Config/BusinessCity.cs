﻿using Business.Exceptions;
using Dal;
using Dal.Dto;
using Dal.Exceptions;
using Entities.Auth;
using Entities.Config;

namespace Business.Config
{
    /// <summary>
    /// Realiza la persistencia de las ciudades en la base de datos
    /// </summary>
    public class BusinessCity : BusinessBase<City>
    {
        #region Constructors
        /// <summary>
        /// Inicializa la persistencia
        /// </summary>
        /// <param name="persistent">Persistencia en base de datos de las ciudades</param>
        public BusinessCity(IPersistentWithLog<City> persistent) : base(persistent) { }
        #endregion

        #region Methods
        /// <summary>
        /// Trae un listado de ciudades desde la base de datos
        /// </summary>
        /// <param name="filters">Filtros aplicados a la consulta</param>
        /// <param name="orders">Ordenamientos aplicados a la base de datos</param>
        /// <param name="limit">Límite de registros a traer</param>
        /// <param name="offset">Corrimiento desde el que se cuenta el número de registros</param>
        /// <returns>Listado de ciudades</returns>
        /// <exception cref="BusinessException">Si hubo una excepción al consultar las ciudades</exception>
        public ListResult<City> List(string filters, string orders, int limit, int offset)
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
                throw new BusinessException("Error al consultar el listado de ciudades", ex);
            }
        }

        /// <summary>
        /// Consulta una ciudad dado su identificador
        /// </summary>
        /// <param name="entity">Ciudad a consultar</param>
        /// <returns>Ciudad con los datos cargados desde la base de datos o null si no lo pudo encontrar</returns>
        /// <exception cref="BusinessException">Si hubo una excepción al consultar la ciudad</exception>
        public City Read(City entity)
        {
            try
            {
                return _persistent.Read(entity);
            }
            catch (Exception ex)
            {
                throw new BusinessException("Error al consultar la ciudad", ex);
            }
        }

        /// <summary>
        /// Inserta una ciudad en la base de datos
        /// </summary>
        /// <param name="entity">Ciudad a insertar</param>
        /// <returns>Ciudad insertado con el id generado por la base de datos</returns>
        /// <param name="user">Usuario que realiza la inserción</param>
        /// <exception cref="BusinessException">Si hubo una excepción al insertar la ciudad</exception>
        public City Insert(City entity, User user)
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
                throw new BusinessException("Error al insertar la ciudad", ex);
            }
        }

        /// <summary>
        /// Actualiza una ciudad en la base de datos
        /// </summary>
        /// <param name="entity">Ciudad a actualizar</param>
        /// <param name="user">Usuario que realiza la actualización</param>
        /// <returns>Ciudad actualizada</returns>
        /// <exception cref="BusinessException">Si hubo una excepción al actualizar la ciudad</exception>
        public City Update(City entity, User user)
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
                throw new BusinessException("Error al actualizar la ciudad", ex);
            }
        }

        /// <summary>
        /// Elimina una ciudad de la base de datos
        /// </summary>
        /// <param name="entity">Ciudad a eliminar</param>
        /// <param name="user">Usuario que realiza la eliminación</param>
        /// <returns>Ciudad eliminado</returns>
        /// <exception cref="BusinessException">Si hubo una excepción al eliminar la ciudad</exception>
        public City Delete(City entity, User user)
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
                throw new BusinessException("Error al eliminar la ciudad", ex);
            }
        }
        #endregion
    }
}