using Business.Cont;
using Dal.Cont;

namespace Business.Test.Cont
{
    /// <summary>
    /// Realiza las pruebas sobre la clase de persistencia de números de consecutivos
    /// </summary>
    [Collection("Tests")]
    public class ConsecutiveNumberTest
    {
        #region Methods
        /// <summary>
        /// Prueba la creación de una capa de números de consecutivos
        /// </summary>
        [Fact]
        public void CreateConsecutiveNumberTest()
        {
            //Act, Assert
            Assert.IsType<BusinessConsecutiveNumber>(new BusinessConsecutiveNumber(new PersistentConsecutiveNumber("")));
        }
        #endregion
    }
}
