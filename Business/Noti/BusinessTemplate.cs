using Business.Exceptions;
using Dal;
using Dal.Dto;
using Dal.Exceptions;
using Entities.Auth;
using Entities.Noti;

namespace Business.Noti
{
    /// <summary>
    /// Capa de negocio de las plantillas en la base de datos
    /// </summary>
    public class BusinessTemplate : BusinessBase<Template>, IBusiness<Template>
    {
        #region Constructors
        /// <summary>
        /// Inicializa la persistencia
        /// </summary>
        /// <param name="persistent">Persistencia en base de datos de las notificaciones</param>
        public BusinessTemplate(IPersistentWithLog<Template> persistent) : base(persistent) { }
        #endregion

        #region Methods
        /// <summary>
        /// Trae un listado de plantillas desde la base de datos
        /// </summary>
        /// <param name="filters">Filtros aplicados a la consulta</param>
        /// <param name="orders">Ordenamientos aplicados a la base de datos</param>
        /// <param name="limit">Límite de registros a traer</param>
        /// <param name="offset">Corrimiento desde el que se cuenta el número de registros</param>
        /// <returns>Listado de plantillas</returns>
        /// <exception cref="BusinessException">Si hubo una excepción al consultar las plantillas</exception>
        public ListResult<Template> List(string filters, string orders, int limit, int offset)
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
                throw new BusinessException("Error al consultar el listado de plantillas", ex);
            }
        }

        /// <summary>
        /// Consulta una plantila dado su identificador
        /// </summary>
        /// <param name="entity">Plantilla a consultar</param>
        /// <returns>Plantilla con los datos cargados desde la base de datos o null si no lo pudo encontrar</returns>
        /// <exception cref="BusinessException">Si hubo una excepción al consultar la plantilla</exception>
        public Template Read(Template entity)
        {
            try
            {
                return _persistent.Read(entity);
            }
            catch (Exception ex)
            {
                throw new BusinessException("Error al consultar la plantilla", ex);
            }
        }

        /// <summary>
        /// Inserta una plantilla en la base de datos
        /// </summary>
        /// <param name="entity">Plantilla a insertar</param>
        /// <returns>Plantilla insertada con el id generado por la base de datos</returns>
        /// <param name="user">Usuario que realiza la inserción</param>
        /// <exception cref="BusinessException">Si hubo una excepción al insertar la plantilla</exception>
        public Template Insert(Template entity, User user)
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
                throw new BusinessException("Error al insertar la plantilla", ex);
            }
        }

        /// <summary>
        /// Actualiza una plantilla en la base de datos
        /// </summary>
        /// <param name="entity">Plantilla a actualizar</param>
        /// <param name="user">Usuario que realiza la actualización</param>
        /// <returns>Plantilla actualizada</returns>
        /// <exception cref="BusinessException">Si hubo una excepción al actualizar la plantilla</exception>
        public Template Update(Template entity, User user)
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
                throw new BusinessException("Error al actualizar la plantilla", ex);
            }
        }

        /// <summary>
        /// Elimina una plantilla de la base de datos
        /// </summary>
        /// <param name="entity">Plantilla a eliminar</param>
        /// <param name="user">Usuario que realiza la eliminación</param>
        /// <returns>Plantilla eliminada</returns>
        /// <exception cref="BusinessException">Si hubo una excepción al eliminar la plantilla</exception>
        public Template Delete(Template entity, User user)
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
                throw new BusinessException("Error al eliminar la plantilla", ex);
            }
        }

        /// <summary>
        /// Obtiene el contenido html de la plantilla reemplazado con los valores que vienen en un diccionario
        /// de datos, buscando y reemplazando las variables de la siguiente manera:
        /// #{llave}# - Valor
        /// Ejemplo:<br />
        /// Original: <p>Contenido de prueba con variable #{datos}#</p>
        /// Reemplazado por diccionario con llave(variable) - Valor (de prueba): <p>Contenido de prueba con variable de prueba</p>
        /// </summary>
        /// <param name="template">Plantilla a cargar desde la base de datos</param>
        /// <param name="data">Nombre de las variables y sus respectivos valores, a buscar y reemplazar en la plantilla</param>
        /// <returns>Contenido de la plantilla con las variables reemplazadas</returns>
        public static Template ReplacedVariables(Template template, IDictionary<string, string> data)
        {
            foreach (KeyValuePair<string, string> item in data)
            {
                template.Content = template.Content.Replace("#{" + item.Key + "}#", item.Value);
            }
            return template;
        }
        #endregion
    }
}
