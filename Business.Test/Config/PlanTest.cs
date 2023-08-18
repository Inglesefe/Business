using Business.Config;
using Dal.Dto;
using Dal.Exceptions;
using Entities.Config;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace Business.Test.Config
{
    /// <summary>
    /// Realiza las pruebas sobre la clase de persistencia de planes
    /// </summary>
    [Collection("Tests")]
    public class PlanTest
    {
        #region Attributes
        /// <summary>
        /// Configuración de la aplicación de pruebas
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Capa de negocio de los planes
        /// </summary>
        private readonly BusinessPlan _business;
        #endregion

        #region Constructors
        /// <summary>
        /// Inicializa la configuración de la prueba
        /// </summary>
        public PlanTest()
        {
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, false)
                .AddEnvironmentVariables()
                .Build();
            _business = new(new MySqlConnection(_configuration.GetConnectionString("golden") ?? ""));
        }
        #endregion

        #region Methods
        /// <summary>
        /// Prueba la consulta de un listado de planes con filtros, ordenamientos y límite
        /// </summary>
        [Fact]
        public void PlanListTest()
        {
            ListResult<Plan> list = _business.List("idplan = 1", "value", 1, 0);

            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de planes con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void PlanListWithErrorTest()
        {
            Assert.Throws<PersistentException>(() => _business.List("idplan = 1", "valor", 1, 0));
        }

        /// <summary>
        /// Prueba la consulta de un plan dado su identificador
        /// </summary>
        [Fact]
        public void PlanReadTest()
        {
            Plan plan = new() { Id = 1 };
            plan = _business.Read(plan);

            Assert.Equal(12, plan.InstallmentsNumber);
        }

        /// <summary>
        /// Prueba la consulta de un plan que no existe dado su identificador
        /// </summary>
        [Fact]
        public void PlanReadNotFoundTest()
        {
            Plan plan = new() { Id = 10 };
            plan = _business.Read(plan);

            Assert.Null(plan);
        }

        /// <summary>
        /// Prueba la inserción de un plan
        /// </summary>
        [Fact]
        public void PlanInsertTest()
        {
            Plan plan = new() { InitialFee = 5000, InstallmentsNumber = 5, InstallmentValue = 250, Value = 75000, Active = true, Description = "Plan de prueba de insercion" };
            plan = _business.Insert(plan, new() { Id = 1 });

            Assert.NotEqual(0, plan.Id);
        }

        /// <summary>
        /// Prueba la actualización de un plan
        /// </summary>
        [Fact]
        public void PlanUpdateTest()
        {
            Plan plan = new() { Id = 2, InitialFee = 54321, InstallmentsNumber = 35, InstallmentValue = 750, Value = 85000, Active = true, Description = "Plan de prueba de actualización" };
            _ = _business.Update(plan, new() { Id = 1 });

            Plan plan2 = new() { Id = 2 };
            plan2 = _business.Read(plan2);

            Assert.NotEqual(15, plan2.InstallmentsNumber);
        }

        /// <summary>
        /// Prueba la eliminación de un plan
        /// </summary>
        [Fact]
        public void PlanDeleteTest()
        {
            Plan plan = new() { Id = 3 };
            _ = _business.Delete(plan, new() { Id = 1 });

            Plan plan2 = new() { Id = 3 };
            plan2 = _business.Read(plan2);

            Assert.Null(plan2);
        }
        #endregion
    }
}
