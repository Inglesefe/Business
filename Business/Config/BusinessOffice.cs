using Business.Exceptions;
using Dal;
using Dal.Config;
using Dal.Dto;
using Dal.Exceptions;
using Entities.Admon;
using Entities.Auth;
using Entities.Config;

namespace Business.Config
{
    /// <summary>
    /// Realiza la persistencia de las oficinas en la base de datos
    /// </summary>
    public class BusinessOffice : BusinessBase<Office>, IBusinessOffice
    {
        #region Constructors
        /// <summary>
        /// Inicializa la persistencia
        /// </summary>
        /// <param name="persistent">Persistencia en base de datos de las oficinas</param>
        public BusinessOffice(IPersistentWithLog<Office> persistent) : base(persistent) { }
        #endregion

        #region Methods
        /// <summary>
        /// Trae un listado de ejecutivos de cuenta asignados a una oficina desde la base de datos
        /// </summary>
        /// <param name="filters">Filtros aplicados a la consulta</param>
        /// <param name="orders">Ordenamientos aplicados a la base de datos</param>
        /// <param name="limit">Límite de registros a traer</param>
        /// <param name="offset">Corrimiento desde el que se cuenta el número de registros</param>
        /// <param name="office">Oficina a la que se le consultan los ejecutivos de cuenta asignados</param>
        /// <returns>Listado de ejecutivos de cuenta asignados a la oficina</returns>
        /// <exception cref="BusinessException">Si hubo una excepción al consultar los ejecutivos de cuenta</exception>
        public ListResult<AccountExecutive> ListAccountExecutives(string filters, string orders, int limit, int offset, Office office)
        {
            try
            {
                return ((IPersistentOffice)_persistent).ListAccountExecutives(filters, orders, limit, offset, office);
            }
            catch (PersistentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BusinessException("Error al listar los ejecutivos de cuenta asociados a la oficina", ex);
            }
        }

        /// <summary>
        /// Trae un listado de ejecutivos de cuenta no asignados a una oficina desde la base de datos
        /// </summary>
        /// <param name="filters">Filtros aplicados a la consulta</param>
        /// <param name="orders">Ordenamientos aplicados a la base de datos</param>
        /// <param name="limit">Límite de registros a traer</param>
        /// <param name="offset">Corrimiento desde el que se cuenta el número de registros</param>
        /// <param name="office">Oficina a la que se le consultan los ejecutivos de cuenta no asignados</param>
        /// <returns>Listado de ejecutivos de cuenta no asignados a la oficina</returns>
        /// <exception cref="BusinessException">Si hubo una excepción al consultar los ejecutivos de cuenta</exception>
        public ListResult<AccountExecutive> ListNotAccountExecutives(string filters, string orders, int limit, int offset, Office office)
        {
            try
            {
                return ((IPersistentOffice)_persistent).ListNotAccountExecutives(filters, orders, limit, offset, office);
            }
            catch (PersistentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BusinessException("Error al listar los ejecutivos de cuenta no asociados a la oficina", ex);
            }
        }

        /// <summary>
        /// Asigna un ejecutivo de cuenta a una oficina en la base de datos
        /// </summary>
        /// <param name="executive">Ejecutivo de cuenta que se asigna a la oficina</param>
        /// <param name="office">Oficina a la que se le asigna el ejecutivo de cuenta</param>
        /// <param name="user">Usuario que realiza la inserción</param>
        /// <returns>Ejecutivo de cuenta asignado</returns>
        /// <exception cref="BusinessException">Si hubo una excepción al asignar el ejecutivo de cuenta a la oficina</exception>
        public AccountExecutive InsertAccountExecutive(AccountExecutive executive, Office office, User user)
        {
            try
            {
                return ((IPersistentOffice)_persistent).InsertAccountExecutive(executive, office, user);
            }
            catch (PersistentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BusinessException("Error al insertar el ejecutivo de cuenta asociado a la oficina", ex);
            }
        }

        /// <summary>
        /// Elimina un ejecutivo de cuenta de una oficina de la base de datos
        /// </summary>
        /// <param name="executive">Ejecutivo de cuenta a eliminarle a la oficina</param>
        /// <param name="office">Oficina al que se le elimina el ejecutivo de cuenta</param>
        /// <param name="user">Usuario que realiza la eliminación</param>
        /// <returns>Ejecutivo de cuenta eliminado</returns>
        /// <exception cref="BusinessException">Si hubo una excepción al eliminar el ejecutivo de cuenta de la oficina</exception>
        public AccountExecutive DeleteAccountExecutive(AccountExecutive executive, Office office, User user)
        {
            try
            {
                return ((IPersistentOffice)_persistent).DeleteAccountExecutive(executive, office, user);
            }
            catch (PersistentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BusinessException("Error al eliminar el ejecutivo de cuenta asociado a la oficina", ex);
            }
        }
        #endregion
    }
}
