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
    /// Realiza la persistencia de los tipos de identificación en la base de datos
    /// </summary>
    public class BusinessIdentificationType : BusinessBase<IdentificationType>
    {
        #region Constructors
        /// <summary>
        /// Inicializa la persistencia
        /// </summary>
        /// <param name="connection">Conexión a la base de datos</param>
        public BusinessIdentificationType(IDbConnection connection) : base(new PersistentIdentificationType(connection)) { }
        #endregion

        #region Methods
        /// <summary>
        /// Trae un listado de tipos de identificación desde la base de datos
        /// </summary>
        /// <param name="filters">Filtros aplicados a la consulta</param>
        /// <param name="orders">Ordenamientos aplicados a la base de datos</param>
        /// <param name="limit">Límite de registros a traer</param>
        /// <param name="offset">Corrimiento desde el que se cuenta el número de registros</param>
        /// <returns>Listado de tipos de identificación</returns>
        /// <exception cref="BusinessException">Si hubo una excepción al consultar las tipos de identificación</exception>
        public override ListResult<IdentificationType> List(string filters, string orders, int limit, int offset)
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
                throw new BusinessException("Error al consultar el listado de tipos de identificación", ex);
            }
        }

        /// <summary>
        /// Consulta un tipo de identificación dado su identificador
        /// </summary>
        /// <param name="entity">Tipo de identificación a consultar</param>
        /// <returns>Tipo de identificación con los datos cargados desde la base de datos o null si no lo pudo encontrar</returns>
        /// <exception cref="BusinessException">Si hubo una excepción al consultar el tipo de identificación</exception>
        public override IdentificationType Read(IdentificationType entity)
        {
            try
            {
                return _persistent.Read(entity);
            }
            catch (Exception ex)
            {
                throw new BusinessException("Error al consultar el tipo de identificación", ex);
            }
        }

        /// <summary>
        /// Inserta un tipo de identificación en la base de datos
        /// </summary>
        /// <param name="entity">Tipo de identificación a insertar</param>
        /// <returns>Tipo de identificación insertado con el id generado por la base de datos</returns>
        /// <param name="user">Usuario que realiza la inserción</param>
        /// <exception cref="BusinessException">Si hubo una excepción al insertar el tipo de identificación</exception>
        public override IdentificationType Insert(IdentificationType entity, User user)
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
                throw new BusinessException("Error al insertar el tipo de identificación", ex);
            }
        }

        /// <summary>
        /// Actualiza un tipo de identificación en la base de datos
        /// </summary>
        /// <param name="entity">Tipo de identificación a actualizar</param>
        /// <param name="user">Usuario que realiza la actualización</param>
        /// <returns>Tipo de identificación actualizada</returns>
        /// <exception cref="BusinessException">Si hubo una excepción al actualizar el tipo de identificación</exception>
        public override IdentificationType Update(IdentificationType entity, User user)
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
                throw new BusinessException("Error al actualizar el tipo de identificación", ex);
            }
        }

        /// <summary>
        /// Elimina un tipo de identificación de la base de datos
        /// </summary>
        /// <param name="entity">Tipo de identificación a eliminar</param>
        /// <param name="user">Usuario que realiza la eliminación</param>
        /// <returns>Tipo de identificación eliminado</returns>
        /// <exception cref="BusinessException">Si hubo una excepción al eliminar el tipo de identificación</exception>
        public override IdentificationType Delete(IdentificationType entity, User user)
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
                throw new BusinessException("Error al eliminar el tipo de identificación", ex);
            }
        }
        #endregion
    }
}
