using Business.Exceptions;
using Dal.Config;
using Dal.Dto;
using Dal.Exceptions;
using Entities.Auth;
using Entities.Config;
using System.Data;

namespace Business.Config
{
    /// <summary>
    /// Realiza la persistencia de los paises en la base de datos
    /// </summary>
    public class BusinessCountry : BusinessBase<Country>
    {
        #region Constructors
        /// <summary>
        /// Inicializa la persistencia
        /// </summary>
        /// <param name="connection">Conexión a la base de datos</param>
        public BusinessCountry(IDbConnection connection) : base(new PersistentCountry(connection)) { }
        #endregion

        #region Methods
        /// <summary>
        /// Trae un listado de paises desde la base de datos
        /// </summary>
        /// <param name="filters">Filtros aplicados a la consulta</param>
        /// <param name="orders">Ordenamientos aplicados a la base de datos</param>
        /// <param name="limit">Límite de registros a traer</param>
        /// <param name="offset">Corrimiento desde el que se cuenta el número de registros</param>
        /// <returns>Listado de paises</returns>
        /// <exception cref="BusinessException">Si hubo una excepción al consultar las paises</exception>
        public override ListResult<Country> List(string filters, string orders, int limit, int offset)
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
                throw new BusinessException("Error al consultar el listado de paises", ex);
            }
        }

        /// <summary>
        /// Consulta un país dado su identificador
        /// </summary>
        /// <param name="entity">País a consultar</param>
        /// <returns>País con los datos cargados desde la base de datos o null si no lo pudo encontrar</returns>
        /// <exception cref="BusinessException">Si hubo una excepción al consultar el país</exception>
        public override Country Read(Country entity)
        {
            try
            {
                return _persistent.Read(entity);
            }
            catch (Exception ex)
            {
                throw new BusinessException("Error al consultar el país", ex);
            }
        }

        /// <summary>
        /// Inserta un país en la base de datos
        /// </summary>
        /// <param name="entity">País a insertar</param>
        /// <returns>País insertado con el id generado por la base de datos</returns>
        /// <param name="user">Usuario que realiza la inserción</param>
        /// <exception cref="BusinessException">Si hubo una excepción al insertar el país</exception>
        public override Country Insert(Country entity, User user)
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
                throw new BusinessException("Error al insertar el país", ex);
            }
        }

        /// <summary>
        /// Actualiza un país en la base de datos
        /// </summary>
        /// <param name="entity">País a actualizar</param>
        /// <param name="user">Usuario que realiza la actualización</param>
        /// <returns>País actualizada</returns>
        /// <exception cref="BusinessException">Si hubo una excepción al actualizar el país</exception>
        public override Country Update(Country entity, User user)
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
                throw new BusinessException("Error al actualizar el país", ex);
            }
        }

        /// <summary>
        /// Elimina un país de la base de datos
        /// </summary>
        /// <param name="entity">País a eliminar</param>
        /// <param name="user">Usuario que realiza la eliminación</param>
        /// <returns>País eliminado</returns>
        /// <exception cref="BusinessException">Si hubo una excepción al eliminar el país</exception>
        public override Country Delete(Country entity, User user)
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
                throw new BusinessException("Error al eliminar el país", ex);
            }
        }
        #endregion
    }
}
