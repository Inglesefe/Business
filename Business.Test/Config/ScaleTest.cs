using Business.Config;
using Dal.Config;

namespace Business.Test.Config
{
    /// <summary>
    /// Realiza las pruebas sobre la clase de persistencia de escalas
    /// </summary>
    [Collection("Tests")]
    public class ScaleTest
    {
        #region Methods
        /// <summary>
        /// Prueba la creación de una capa de escalas
        /// </summary>
        [Fact]
        public void CreateScaleTest()
        {
            //Act, Assert
            Assert.IsType<BusinessScale>(new BusinessScale(new PersistentScale("")));
        }
        #endregion
    }
}
