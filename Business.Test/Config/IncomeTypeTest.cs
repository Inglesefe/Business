using Business.Config;
using Dal.Config;

namespace Business.Test.Config
{
    /// <summary>
    /// Realiza las pruebas sobre la clase de persistencia de tipos de ingreso
    /// </summary>
    [Collection("Tests")]
    public class IncomeTypeTest
    {
        #region Methods
        /// <summary>
        /// Prueba la creación de una capa de tipos de ingreso
        /// </summary>
        [Fact]
        public void CreateIncomeTypeTest()
        {
            //Act, Assert
            Assert.IsType<BusinessIncomeType>(new BusinessIncomeType(new PersistentIncomeType("")));
        }
        #endregion
    }
}
