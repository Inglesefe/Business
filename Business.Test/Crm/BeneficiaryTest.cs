using Business.Crm;
using Dal.Crm;

namespace Business.Test.Crm
{
    /// <summary>
    /// Realiza las pruebas sobre la clase de persistencia de beneficiarios
    /// </summary>
    [Collection("Tests")]
    public class BeneficiaryTest
    {
        #region Methods
        /// <summary>
        /// Prueba la creación de una capa de tipos de beneficiarios
        /// </summary>
        [Fact]
        public void CreateBeneficiaryTest()
        {
            //Act, Assert
            Assert.IsType<BusinessBeneficiary>(new BusinessBeneficiary(new PersistentBeneficiary("")));
        }
        #endregion
    }
}
