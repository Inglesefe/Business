using Business.Admon;
using Dal.Admon;

namespace Business.Test.Admon
{
    /// <summary>
    /// Realiza las pruebas sobre la clase de persistencia de cuotas de matrículas
    /// </summary>
    [Collection("Tests")]
    public class FeeTest
    {
        #region Methods
        /// <summary>
        /// Prueba la creación de una capa de negocio de cuotas de matrículas
        /// </summary>
        [Fact]
        public void CreateFeeTest()
        {
            //Act, Assert
            Assert.IsType<BusinessFee>(new BusinessFee(new PersistentFee("")));
        }
        #endregion
    }
}
