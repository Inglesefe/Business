using Business.Config;
using Dal.Config;

namespace Business.Test.Config
{
    /// <summary>
    /// Realiza las pruebas sobre la clase de persistencia de ciudades
    /// </summary>
    [Collection("Tests")]
    public class CityTest
    {
        #region Methods
        /// <summary>
        /// Prueba la creación de una capa de negocio de ciudades
        /// </summary>
        [Fact]
        public void CreateCityTest()
        {
            //Act, Assert
            Assert.IsType<BusinessCity>(new BusinessCity(new PersistentCity("")));
        }
        #endregion
    }
}
