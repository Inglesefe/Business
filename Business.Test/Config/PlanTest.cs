using Business.Config;
using Dal.Config;

namespace Business.Test.Config
{
    /// <summary>
    /// Realiza las pruebas sobre la clase de persistencia de planes
    /// </summary>
    [Collection("Tests")]
    public class PlanTest
    {
        #region Methods
        /// <summary>
        /// Prueba la creación de una capa de planes
        /// </summary>
        [Fact]
        public void CreatePlanTest()
        {
            //Act, Assert
            Assert.IsType<BusinessPlan>(new BusinessPlan(new PersistentPlan("")));
        }
        #endregion
    }
}
