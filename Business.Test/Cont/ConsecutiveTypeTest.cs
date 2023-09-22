using Business.Cont;
using Dal.Cont;

namespace Business.Test.Cont
{
    /// <summary>
    /// Realiza las pruebas sobre la clase de persistencia de tipos de consecutivo
    /// </summary>
    [Collection("Tests")]
    public class ConsecutiveTypeTest
    {
        #region Methods
        /// <summary>
        /// Prueba la creación de una capa de tipos de consecutivos
        /// </summary>
        [Fact]
        public void CreateConsecutiveTypeTest()
        {
            //Act, Assert
            Assert.IsType<BusinessConsecutiveType>(new BusinessConsecutiveType(new PersistentConsecutiveType("")));
        }
        #endregion
    }
}
