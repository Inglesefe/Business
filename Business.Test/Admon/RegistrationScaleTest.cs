using Business.Admon;
using Dal.Admon;

namespace Business.Test.Admon
{
    /// <summary>
    /// Realiza las pruebas sobre la clase de persistencia de escalas asociadas a las matrículas
    /// </summary>
    [Collection("Tests")]
    public class RegistrationScaleTest
    {
        #region Methods
        /// <summary>
        /// Prueba la creación de una capa de negocio de escalas asociadas a las matrículas
        /// </summary>
        [Fact]
        public void CreateRegistrationScaleTest()
        {
            //Act, Assert
            Assert.IsType<BusinessRegistrationScale>(new BusinessRegistrationScale(new PersistentRegistrationScale("")));
        }
        #endregion
    }
}
