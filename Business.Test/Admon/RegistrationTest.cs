using Business.Admon;
using Dal.Admon;

namespace Business.Test.Admon
{
    /// <summary>
    /// Realiza las pruebas sobre la clase de persistencia de matrículas
    /// </summary>
    [Collection("Tests")]
    public class RegistrationTest
    {
        #region Methods
        /// <summary>
        /// Prueba la creación de una capa de negocio de matrículas
        /// </summary>
        [Fact]
        public void CreateRegistrationTest()
        {
            //Act, Assert
            Assert.IsType<BusinessRegistration>(new BusinessRegistration(new PersistentRegistration("")));
        }
        #endregion
    }
}
