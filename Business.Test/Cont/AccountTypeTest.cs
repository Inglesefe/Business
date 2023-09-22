using Business.Cont;
using Dal.Cont;

namespace Business.Test.Cont
{
    /// <summary>
    /// Realiza las pruebas sobre la clase de persistencia de tipos de cuenta
    /// </summary>
    [Collection("Tests")]
    public class AccountTypeTest
    {
        #region Methods
        /// <summary>
        /// Prueba la creación de una capa de tipo de cuenta
        /// </summary>
        [Fact]
        public void CreateAccountTypeTest()
        {
            //Act, Assert
            Assert.IsType<BusinessAccountType>(new BusinessAccountType(new PersistentAccountType("")));
        }
        #endregion
    }
}
