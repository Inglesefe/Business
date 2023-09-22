using Business.Config;
using Dal.Config;

namespace Business.Test.Config
{
    /// <summary>
    /// Realiza las pruebas sobre la clase de persistencia de paises
    /// </summary>
    [Collection("Tests")]
    public class CountryTest
    {
        #region Methods
        /// <summary>
        /// Prueba la creación de una capa de negocio de paises
        /// </summary>
        [Fact]
        public void CreateCountryTest()
        {
            //Act, Assert
            Assert.IsType<BusinessCountry>(new BusinessCountry(new PersistentCountry("")));
        }
        #endregion
    }
}
