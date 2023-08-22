using Business.Noti;
using Dal;
using Dal.Dto;
using Dal.Exceptions;
using Entities.Noti;
using Moq;

namespace Business.Test.Noti
{
    /// <summary>
    /// Realiza las pruebas sobre la clase de persistencia de plantillas
    /// </summary>
    [Collection("Tests")]
    public class TemplateTest
    {
        #region Attributes
        /// <summary>
        /// Capa de negocio de las plantillas
        /// </summary>
        private readonly BusinessTemplate _business;
        #endregion

        #region Constructors
        /// <summary>
        /// Inicializa la configuración de la prueba
        /// </summary>
        public TemplateTest()
        {
            Mock<IPersistentWithLog<Template>> mock = new();

            _business = new(mock.Object);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Prueba la consulta de un listado de plantillas con filtros, ordenamientos y límite
        /// </summary>
        /// <returns>N/A</returns>
        [Fact]
        public void TemplateListTest()
        {
            ListResult<Template> list = _business.List("idtemplate = 1", "name", 1, 0);

            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de plantillas con filtros, ordenamientos y límite y con errores
        /// </summary>
        /// <returns>N/A</returns>
        [Fact]
        public void TemplateListWithErrorTest()
        {
            Assert.Throws<PersistentException>(() => _business.List("idplantilla = 1", "name", 1, 0));
        }

        /// <summary>
        /// Prueba la consulta de una plantilla dada su identificador
        /// </summary>
        /// <returns>N/A</returns>
        [Fact]
        public void TemplateReadTest()
        {
            Template application = new() { Id = 1 };
            application = _business.Read(application);

            Assert.Equal("Plantilla de prueba", application.Name);
        }

        /// <summary>
        /// Prueba la consulta de una plantilla que no existe dado su identificador
        /// </summary>
        /// <returns>N/A</returns>
        [Fact]
        public void TemplateReadNotFoundTest()
        {
            Template application = new() { Id = 10 };
            application = _business.Read(application);

            Assert.Null(application);
        }

        /// <summary>
        /// Prueba la inserción de una plantilla
        /// </summary>
        /// <returns>N/A</returns>
        [Fact]
        public void TemplateInsertTest()
        {
            Template template = new() { Name = "Prueba 1" };
            template = _business.Insert(template, new() { Id = 1 });

            Assert.NotEqual(0, template.Id);
        }

        /// <summary>
        /// Prueba la actualización de una plantilla
        /// </summary>
        /// <returns>N/A</returns>
        [Fact]
        public void TemplateUpdateTest()
        {
            Template template = new() { Id = 2, Name = "Prueba actualizar" };
            _ = _business.Update(template, new() { Id = 1 });

            Template template2 = new() { Id = 2 };
            template2 = _business.Read(template2);

            Assert.NotEqual("Plantilla a actualizar", template2.Name);
        }

        /// <summary>
        /// Prueba la eliminación de una plantilla
        /// </summary>
        /// <returns>N/A</returns>
        [Fact]
        public void TemplateDeleteTest()
        {
            Template template = new() { Id = 3 };
            _ = _business.Delete(template, new() { Id = 1 });

            Template template2 = new() { Id = 3 };
            template2 = _business.Read(template2);

            Assert.Null(template2);
        }

        /// <summary>
        /// Prueba el reemplazo de las variables en el contenido de la plantilla
        /// </summary>
        /// <returns>N/A</returns>
        [Fact]
        public void TemplateReplacedVariablesTest()
        {
            Template template = new() { Id = 1 };
            template = _business.Read(template);
            IDictionary<string, string> data = new Dictionary<string, string>
            {
                { "user", "leandrobaena@gmail.com" }
            };
            template = BusinessTemplate.ReplacedVariables(template, data);

            Assert.Equal("<h1>Esta es una prueba hecha por leandrobaena@gmail.com</h1>", template.Content);
        }
        #endregion
    }
}
