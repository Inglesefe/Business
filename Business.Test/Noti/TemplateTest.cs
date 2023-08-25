using Business.Noti;
using Dal;
using Dal.Dto;
using Dal.Exceptions;
using Entities.Auth;
using Entities.Noti;
using Moq;
using System.Data;

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

        /// <summary>
        /// Conexión a la base de datos falsa
        /// </summary>
        private readonly IDbConnection connectionFake;
        #endregion

        #region Constructors
        /// <summary>
        /// Inicializa la configuración de la prueba
        /// </summary>
        public TemplateTest()
        {
            Mock<IPersistentWithLog<Template>> mock = new();
            Mock<IDbConnection> mockConnection = new();
            connectionFake = mockConnection.Object;

            List<Template> templates = new()
            {
                new Template() { Id = 1, Name = "Plantilla de prueba", Content = "<h1>Esta es una prueba hecha por #{user}#</h1>" },
                new Template() { Id = 2, Name = "Plantilla a actualizar", Content = "<h1>Esta es una prueba hecha para #{actualizar}#</h1>" },
                new Template() { Id = 3, Name = "Plantilla a eliminar", Content = "<h1>Esta es una prueba hecha para #{eliminar}#</h1>" }
            };

            mock.Setup(p => p.List("idtemplate = 1", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<IDbConnection>()))
                .Returns(new ListResult<Template>(templates.Where(y => y.Id == 1).ToList(), 1));
            mock.Setup(p => p.List("idplantilla = 1", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<IDbConnection>()))
                .Throws<PersistentException>();

            mock.Setup(p => p.Read(It.IsAny<Template>(), It.IsAny<IDbConnection>()))
                .Returns((Template template, IDbConnection connection) => templates.Find(x => x.Id == template.Id) ?? new Template());

            mock.Setup(p => p.Insert(It.IsAny<Template>(), It.IsAny<User>(), It.IsAny<IDbConnection>()))
                .Returns((Template template, User user, IDbConnection connection) =>
                {
                    template.Id = templates.Count + 1;
                    templates.Add(template);
                    return template;
                });

            mock.Setup(p => p.Update(It.IsAny<Template>(), It.IsAny<User>(), It.IsAny<IDbConnection>()))
                .Returns((Template template, User user, IDbConnection connection) =>
                {
                    templates.Where(x => x.Id == template.Id).ToList().ForEach(x =>
                    {
                        x.Name = template.Name;
                        x.Content = template.Content;
                    });
                    return template;
                });

            mock.Setup(p => p.Delete(It.IsAny<Template>(), It.IsAny<User>(), It.IsAny<IDbConnection>()))
                .Returns((Template template, User user, IDbConnection connection) =>
                {
                    templates = templates.Where(x => x.Id != template.Id).ToList();
                    return template;
                });

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
            ListResult<Template> list = _business.List("idtemplate = 1", "name", 1, 0, connectionFake);

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
            Assert.Throws<PersistentException>(() => _business.List("idplantilla = 1", "name", 1, 0, connectionFake));
        }

        /// <summary>
        /// Prueba la consulta de una plantilla dada su identificador
        /// </summary>
        /// <returns>N/A</returns>
        [Fact]
        public void TemplateReadTest()
        {
            Template application = new() { Id = 1 };
            application = _business.Read(application, connectionFake);

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
            application = _business.Read(application, connectionFake);

            Assert.Equal(0, application.Id);
        }

        /// <summary>
        /// Prueba la inserción de una plantilla
        /// </summary>
        /// <returns>N/A</returns>
        [Fact]
        public void TemplateInsertTest()
        {
            Template template = new() { Name = "Prueba 1" };
            template = _business.Insert(template, new() { Id = 1 }, connectionFake);

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
            _ = _business.Update(template, new() { Id = 1 }, connectionFake);

            Template template2 = new() { Id = 2 };
            template2 = _business.Read(template2, connectionFake);

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
            _ = _business.Delete(template, new() { Id = 1 }, connectionFake);

            Template template2 = new() { Id = 3 };
            template2 = _business.Read(template2, connectionFake);

            Assert.Equal(0, template2.Id);
        }

        /// <summary>
        /// Prueba el reemplazo de las variables en el contenido de la plantilla
        /// </summary>
        /// <returns>N/A</returns>
        [Fact]
        public void TemplateReplacedVariablesTest()
        {
            Template template = new() { Id = 1 };
            template = _business.Read(template, connectionFake);
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
