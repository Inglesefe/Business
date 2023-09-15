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
        #endregion

        #region Constructors
        /// <summary>
        /// Inicializa la configuración de la prueba
        /// </summary>
        public PlanTest()
        {
            //Arrange
            Mock<IPersistentWithLog<Plan>> mock = new();
            List<Plan> plans = new()
            {
                new Plan() { Id = 1, Value = 3779100, InitialFee = 444600, InstallmentsNumber = 12, InstallmentValue = 444600, Active = true, Description = "PLAN COFREM 12 MESES" },
                new Plan() { Id = 2, Value = 3779100, InitialFee = 282600, InstallmentsNumber = 15, InstallmentValue = 233100, Active = true, Description = "PLAN COFREM 15 MESES" },
                new Plan() { Id = 3, Value = 3779100, InitialFee = 235350, InstallmentsNumber = 15, InstallmentValue = 236250, Active = false, Description = "PLAN COFREM 15 MESES ESPECIAL" }
            };
            mock.Setup(p => p.List("idplan = 1", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new ListResult<Plan>(plans.Where(y => y.Id == 1).ToList(), 1));
            mock.Setup(p => p.List("idplano = 1", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Throws<PersistentException>();
            mock.Setup(p => p.Read(It.IsAny<Plan>()))
                .Returns((Plan plan) => plans.Find(x => x.Id == plan.Id) ?? new Plan());
            mock.Setup(p => p.Insert(It.IsAny<Plan>(), It.IsAny<User>()))
                .Returns((Plan plan, User user) =>
                {
                    plan.Id = plans.Count + 1;
                    plans.Add(plan);
                    return plan;
                });
            mock.Setup(p => p.Update(It.IsAny<Plan>(), It.IsAny<User>()))
                .Returns((Plan plan, User user) =>
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
            mock.Setup(p => p.Delete(It.IsAny<Plan>(), It.IsAny<User>()))
                .Returns((Plan plan, User user) =>
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
        public void ListTest()
        {
            //Act
            ListResult<Plan> list = _business.List("idplan = 1", "value", 1, 0);

            //Assert
            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de planes con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void ListWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _business.List("idplano = 1", "value", 1, 0));
        }

        /// <summary>
        /// Prueba la consulta de un plan dado su identificador
        /// </summary>
        [Fact]
        public void ReadTest()
        {
            //Arrange
            Plan plan = new() { Id = 1 };

            //Act
            plan = _business.Read(plan);

            //Assert
            Assert.Equal(12, plan.InstallmentsNumber);
        }

        /// <summary>
        /// Prueba la consulta de un plan que no existe dado su identificador
        /// </summary>
        [Fact]
        public void ReadNotFoundTest()
        {
            //Arrange
            Plan plan = new() { Id = 10 };

            //Act
            plan = _business.Read(plan);

            //Assert
            Assert.Equal(0, plan.Id);
        }

        /// <summary>
        /// Prueba la inserción de un plan
        /// </summary>
        [Fact]
        public void InsertTest()
        {
            //Arrange
            Plan plan = new() { InitialFee = 5000, InstallmentsNumber = 5, InstallmentValue = 250, Value = 75000, Active = true, Description = "Plan de prueba de insercion" };

            //Act
            plan = _business.Insert(plan, new() { Id = 1 });

            //Assert
            Assert.NotEqual(0, plan.Id);
        }

        /// <summary>
        /// Prueba la actualización de un plan
        /// </summary>
        [Fact]
        public void UpdateTest()
        {
            //Arrange
            Plan plan = new() { Id = 2, InitialFee = 54321, InstallmentsNumber = 35, InstallmentValue = 750, Value = 85000, Active = true, Description = "Plan de prueba de actualización" };
            Plan plan2 = new() { Id = 2 };

            //Act
            _ = _business.Update(plan, new() { Id = 1 });
            plan2 = _business.Read(plan2);

            //Assert
            Assert.NotEqual(15, plan2.InstallmentsNumber);
        }

        /// <summary>
        /// Prueba la eliminación de un plan
        /// </summary>
        [Fact]
        public void DeleteTest()
        {
            //Arrange
            Plan plan = new() { Id = 3 };
            Plan plan2 = new() { Id = 3 };

            //Act
            _ = _business.Delete(plan, new() { Id = 1 });
            plan2 = _business.Read(plan2);

            //Assert
            Assert.Equal(0, plan2.Id);
        }
        #endregion
    }
}
