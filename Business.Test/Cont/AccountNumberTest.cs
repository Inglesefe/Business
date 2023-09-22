using Business.Cont;
using Dal.Cont;

namespace Business.Test.Cont
{
    /// <summary>
    /// Realiza las pruebas sobre la clase de persistencia de números de cuenta
    /// </summary>
    [Collection("Tests")]
    public class AccountNumberTest
    {
        #region Methods
        /// <summary>
        /// Prueba la creación de una capa de números de cuenta
        /// </summary>
        [Fact]
        public void CreateAccountNumberTest()
        {
            //Act, Assert
            Assert.IsType<BusinessAccountNumber>(new BusinessAccountNumber(new PersistentAccountNumber("")));
        }
        #endregion
    }
}
