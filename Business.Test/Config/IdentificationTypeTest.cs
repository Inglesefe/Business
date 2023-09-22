using Business.Config;
using Dal.Config;

namespace Business.Test.Config
{
    /// <summary>
    /// Realiza las pruebas sobre la clase de persistencia de tipos de identificación
    /// </summary>
    [Collection("Tests")]
    public class IdentificationTypeTest
    {
        #region Methods
        /// <summary>
        /// Prueba la creación de una capa de tipos de identificación
        /// </summary>
        [Fact]
        public void CreateIdentificationTypeTest()
        {
            //Act, Assert
            Assert.IsType<BusinessIdentificationType>(new BusinessIdentificationType(new PersistentIdentificationType("")));
        }
        #endregion
    }
}
