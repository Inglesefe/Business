using Business.Admon;
using Dal.Admon;

namespace Business.Test.Admon
{
    /// <summary>
    /// Realiza las pruebas sobre la clase de persistencia de ejecutivos de cuenta
    /// </summary>
    [Collection("Tests")]
    public class AccountExecutiveTest
    {
        #region Methods
        /// <summary>
        /// Prueba la creación de una capa de negocio de un ejecutivo de cuenta
        /// </summary>
        [Fact]
        public void CreateAccountExecutiveTest()
        {
            //Act, Assert
            Assert.IsType<BusinessAccountExecutive>(new BusinessAccountExecutive(new PersistentAccountExecutive("")));
        }
        #endregion
    }
}
