using Dal;
using Entities.Noti;

namespace Business.Noti
{
    /// <summary>
    /// Capa de negocio de las plantillas en la base de datos
    /// </summary>
    public class BusinessTemplate : BusinessBase<Template>
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
