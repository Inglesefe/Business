using Business.Config;
using Dal.Config;

namespace Business.Test.Config
{
    /// <summary>
    /// Realiza las pruebas sobre la clase de persistencia de parámetros
    /// </summary>
    [Collection("Tests")]
    public class ParameterTest
    {
        #region Methods
        /// <summary>
        /// Prueba la creación de una capa de parámetros
        /// </summary>
        [Fact]
        public void CreateParameterTest()
        {
            //Act, Assert
            Assert.IsType<BusinessParameter>(new BusinessParameter(new PersistentParameter("")));
        }
        #endregion
    }
}
