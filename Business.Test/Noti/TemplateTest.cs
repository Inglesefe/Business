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
        #endregion

        #region Constructors
        /// <summary>
        /// Inicializa la configuración de la prueba
        /// </summary>
        public TemplateTest()
        {
            //Arrange
            Mock<IPersistentWithLog<Template>> mock = new();
            List<Template> templates = new()
            {
                new Template() { Id = 1, Name = "Plantilla de prueba", Content = "<h1>Esta es una prueba hecha por #{user}#</h1>" },
                new Template() { Id = 2, Name = "Plantilla a actualizar", Content = "<h1>Esta es una prueba hecha para #{actualizar}#</h1>" },
                new Template() { Id = 3, Name = "Plantilla a eliminar", Content = "<h1>Esta es una prueba hecha para #{eliminar}#</h1>" }
            };
            mock.Setup(p => p.List("idtemplate = 1", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new ListResult<Template>(templates.Where(y => y.Id == 1).ToList(), 1));
            mock.Setup(p => p.List("idplantilla = 1", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Throws<PersistentException>();
            mock.Setup(p => p.Read(It.IsAny<Template>()))
                .Returns((Template template) => templates.Find(x => x.Id == template.Id) ?? new Template());
            mock.Setup(p => p.Insert(It.IsAny<Template>(), It.IsAny<User>()))
                .Returns((Template template, User user) =>
                {
                    template.Id = templates.Count + 1;
                    templates.Add(template);
                    return template;
                });
            mock.Setup(p => p.Update(It.IsAny<Template>(), It.IsAny<User>()))
                .Returns((Template template, User user) =>
                {
                    templates.Where(x => x.Id == template.Id).ToList().ForEach(x =>
                    {
                        x.Name = template.Name;
                        x.Content = template.Content;
                    });
                    return template;
                });
            mock.Setup(p => p.Delete(It.IsAny<Template>(), It.IsAny<User>()))
                .Returns((Template template, User user) =>
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
        [Fact]
        public void ListTest()
        {
            //Act
            ListResult<Template> list = _business.List("idtemplate = 1", "name", 1, 0);

            //Assert
            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de plantillas con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void ListWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _business.List("idplantilla = 1", "name", 1, 0));
        }

        /// <summary>
        /// Prueba la consulta de una plantilla dada su identificador
        /// </summary>
        [Fact]
        public void ReadTest()
        {
            //Arrange
            Template application = new() { Id = 1 };

            //Act
            application = _business.Read(application);

            //Assert
            Assert.Equal("Plantilla de prueba", application.Name);
        }

        /// <summary>
        /// Prueba la consulta de una plantilla que no existe dado su identificador
        /// </summary>
        [Fact]
        public void ReadNotFoundTest()
        {
            //Arrange
            Template application = new() { Id = 10 };

            //Act
            application = _business.Read(application);

            //Assert
            Assert.Equal(0, application.Id);
        }

        /// <summary>
        /// Prueba la inserción de una plantilla
        /// </summary>
        [Fact]
        public void InsertTest()
        {
            //Arrange
            Template template = new() { Name = "Prueba 1" };

            //Act
            template = _business.Insert(template, new() { Id = 1 });

            //Assert
            Assert.NotEqual(0, template.Id);
        }

        /// <summary>
        /// Prueba la actualización de una plantilla
        /// </summary>
        [Fact]
        public void UpdateTest()
        {
            //Arrange
            Template template = new() { Id = 2, Name = "Prueba actualizar" };
            Template template2 = new() { Id = 2 };

            //Act
            _ = _business.Update(template, new() { Id = 1 });
            template2 = _business.Read(template2);

            //Assert
            Assert.NotEqual("Plantilla a actualizar", template2.Name);
        }

        /// <summary>
        /// Prueba la eliminación de una plantilla
        /// </summary>
        [Fact]
        public void DeleteTest()
        {
            //Arrange
            Template template = new() { Id = 3 };
            Template template2 = new() { Id = 3 };

            //Act
            _ = _business.Delete(template, new() { Id = 1 });
            template2 = _business.Read(template2);

            //Assert
            Assert.Equal(0, template2.Id);
        }

        /// <summary>
        /// Prueba el reemplazo de las variables en el contenido de la plantilla
        /// </summary>
        [Fact]
        public void ReplacedVariablesTest()
        {
            //Arrange
            Template template = new() { Id = 1 };
            template = _business.Read(template);
            IDictionary<string, string> data = new Dictionary<string, string>
            {
                { "user", "leandrobaena@gmail.com" }
            };

            //Act
            template = BusinessTemplate.ReplacedVariables(template, data);

            //Assert
            Assert.Equal("<h1>Esta es una prueba hecha por leandrobaena@gmail.com</h1>", template.Content);
        }
        #endregion
    }
}
