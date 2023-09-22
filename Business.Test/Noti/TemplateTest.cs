using Business.Noti;
using Dal;
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
            //Arrange
            Mock<IPersistentWithLog<Template>> mock = new();
            List<Template> templates = new()
            {
                new Template() { Id = 1, Name = "Plantilla de prueba", Content = "<h1>Esta es una prueba hecha por #{user}#</h1>" }
            };
            mock.Setup(p => p.Read(It.IsAny<Template>()))
                .Returns((Template template) => templates.Find(x => x.Id == template.Id) ?? new Template());
            _business = new(mock.Object);
        }
        #endregion

        #region Methods
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
