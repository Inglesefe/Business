using Business.Crm;
using Dal.Crm;

namespace Business.Test.Crm
{
    /// <summary>
    /// Realiza las pruebas sobre la clase de persistencia de titulares
    /// </summary>
    [Collection("Tests")]
    public class OwnerTest
    {
        #region Methods
        /// <summary>
        /// Prueba la creación de una capa de negocio de titulares
        /// </summary>
        [Fact]
        public void CreateOwnerTest()
        {
            //Act, Assert
            Assert.IsType<BusinessOwner>(new BusinessOwner(new PersistentOwner("")));
        }
        #endregion
    }
}
