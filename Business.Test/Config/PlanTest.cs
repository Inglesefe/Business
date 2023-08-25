using Business.Config;
using Dal;
using Dal.Dto;
using Dal.Exceptions;
using Entities.Auth;
using Entities.Config;
using Moq;
using System.Data;

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
        /// Capa de negocio de los planes
        /// </summary>
        private readonly BusinessPlan _business;

        /// <summary>
        /// Conexión a la base de datos falsa
        /// </summary>
        private readonly IDbConnection connectionFake;
        #endregion

        #region Constructors
        /// <summary>
        /// Inicializa la configuración de la prueba
        /// </summary>
        public PlanTest()
        {
            Mock<IPersistentWithLog<Plan>> mock = new();
            Mock<IDbConnection> mockConnection = new();
            connectionFake = mockConnection.Object;

            List<Plan> plans = new()
            {
                new Plan() { Id = 1, Value = 3779100, InitialFee = 444600, InstallmentsNumber = 12, InstallmentValue = 444600, Active = true, Description = "PLAN COFREM 12 MESES" },
                new Plan() { Id = 2, Value = 3779100, InitialFee = 282600, InstallmentsNumber = 15, InstallmentValue = 233100, Active = true, Description = "PLAN COFREM 15 MESES" },
                new Plan() { Id = 3, Value = 3779100, InitialFee = 235350, InstallmentsNumber = 15, InstallmentValue = 236250, Active = false, Description = "PLAN COFREM 15 MESES ESPECIAL" }
            };

            mock.Setup(p => p.List("idplan = 1", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<IDbConnection>()))
                .Returns(new ListResult<Plan>(plans.Where(y => y.Id == 1).ToList(), 1));
            mock.Setup(p => p.List("idplano = 1", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<IDbConnection>()))
                .Throws<PersistentException>();

            mock.Setup(p => p.Read(It.IsAny<Plan>(), It.IsAny<IDbConnection>()))
                .Returns((Plan plan, IDbConnection connection) => plans.Find(x => x.Id == plan.Id) ?? new Plan());

            mock.Setup(p => p.Insert(It.IsAny<Plan>(), It.IsAny<User>(), It.IsAny<IDbConnection>()))
                .Returns((Plan plan, User user, IDbConnection connection) =>
                {
                    plan.Id = plans.Count + 1;
                    plans.Add(plan);
                    return plan;
                });

            mock.Setup(p => p.Update(It.IsAny<Plan>(), It.IsAny<User>(), It.IsAny<IDbConnection>()))
                .Returns((Plan plan, User user, IDbConnection connection) =>
                {
                    plans.Where(x => x.Id == plan.Id).ToList().ForEach(x =>
                    {
                        x.Value = plan.Value;
                        x.InitialFee = plan.InitialFee;
                        x.InstallmentsNumber = plan.InstallmentsNumber;
                        x.InstallmentValue = plan.InstallmentValue;
                        x.Active = plan.Active;
                        x.Description = plan.Description;
                    });
                    return plan;
                });

            mock.Setup(p => p.Delete(It.IsAny<Plan>(), It.IsAny<User>(), It.IsAny<IDbConnection>()))
                .Returns((Plan plan, User user, IDbConnection connection) =>
                {
                    plans = plans.Where(x => x.Id != plan.Id).ToList();
                    return plan;
                });

            _business = new(mock.Object);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Prueba la consulta de un listado de planes con filtros, ordenamientos y límite
        /// </summary>
        [Fact]
        public void PlanListTest()
        {
            ListResult<Plan> list = _business.List("idplan = 1", "value", 1, 0, connectionFake);

            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de planes con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void PlanListWithErrorTest()
        {
            Assert.Throws<PersistentException>(() => _business.List("idplano = 1", "value", 1, 0, connectionFake));
        }

        /// <summary>
        /// Prueba la consulta de un plan dado su identificador
        /// </summary>
        [Fact]
        public void PlanReadTest()
        {
            Plan plan = new() { Id = 1 };
            plan = _business.Read(plan, connectionFake);

            Assert.Equal(12, plan.InstallmentsNumber);
        }

        /// <summary>
        /// Prueba la consulta de un plan que no existe dado su identificador
        /// </summary>
        [Fact]
        public void PlanReadNotFoundTest()
        {
            Plan plan = new() { Id = 10 };
            plan = _business.Read(plan, connectionFake);

            Assert.Equal(0, plan.Id);
        }

        /// <summary>
        /// Prueba la inserción de un plan
        /// </summary>
        [Fact]
        public void PlanInsertTest()
        {
            Plan plan = new() { InitialFee = 5000, InstallmentsNumber = 5, InstallmentValue = 250, Value = 75000, Active = true, Description = "Plan de prueba de insercion" };
            plan = _business.Insert(plan, new() { Id = 1 }, connectionFake);

            Assert.NotEqual(0, plan.Id);
        }

        /// <summary>
        /// Prueba la actualización de un plan
        /// </summary>
        [Fact]
        public void PlanUpdateTest()
        {
            Plan plan = new() { Id = 2, InitialFee = 54321, InstallmentsNumber = 35, InstallmentValue = 750, Value = 85000, Active = true, Description = "Plan de prueba de actualización" };
            _ = _business.Update(plan, new() { Id = 1 }, connectionFake);

            Plan plan2 = new() { Id = 2 };
            plan2 = _business.Read(plan2, connectionFake);

            Assert.NotEqual(15, plan2.InstallmentsNumber);
        }

        /// <summary>
        /// Prueba la eliminación de un plan
        /// </summary>
        [Fact]
        public void PlanDeleteTest()
        {
            Plan plan = new() { Id = 3 };
            _ = _business.Delete(plan, new() { Id = 1 }, connectionFake);

            Plan plan2 = new() { Id = 3 };
            plan2 = _business.Read(plan2, connectionFake);

            Assert.Equal(0, plan2.Id);
        }
        #endregion
    }
}
