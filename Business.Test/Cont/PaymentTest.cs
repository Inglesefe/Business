using Business.Cont;
using Dal.Cont;

namespace Business.Test.Cont
{
    /// <summary>
    /// Realiza las pruebas sobre la clase de persistencia de pagos
    /// </summary>
    [Collection("Tests")]
    public class PaymentTest
    {
        #region Methods
        /// <summary>
        /// Prueba la creación de una capa de negocio de un pago
        /// </summary>
        [Fact]
        public void CreatePaymentTest()
        {
            //Act, Assert
            Assert.IsType<BusinessPayment>(new BusinessPayment(new PersistentPayment("")));
        }
        #endregion
    }
}
