using Business.Config;
using Dal.Config;

namespace Business.Test.Config
{
    /// <summary>
    /// Realiza las pruebas sobre la clase de persistencia de tipos de pago
    /// </summary>
    [Collection("Tests")]
    public class PaymentTypeTest
    {
        #region Methods
        /// <summary>
        /// Prueba la creación de una capa de tipos de pago
        /// </summary>
        [Fact]
        public void CreatePaymentTypeTest()
        {
            //Act, Assert
            Assert.IsType<BusinessPaymentType>(new BusinessPaymentType(new PersistentPaymentType("")));
        }
        #endregion
    }
}
