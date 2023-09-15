using Business.Admon;
using Dal;
using Dal.Dto;
using Dal.Exceptions;
using Entities.Admon;
using Entities.Auth;
using Moq;
using System.Data;

namespace Business.Test.Admon
{
    /// <summary>
    /// Realiza las pruebas sobre la clase de persistencia de matrículas
    /// </summary>
    [Collection("Tests")]
    public class RegistrationTest
    {
        #region Attributes
        /// <summary>
        /// Capa de negocio de los matrículas
        /// </summary>
        private readonly BusinessRegistration _business;
        #endregion

        #region Constructors
        /// <summary>
        /// Inicializa la configuración de la prueba
        /// </summary>
        public RegistrationTest()
        {
            //Arrange
            Mock<IPersistentWithLog<Registration>> mock = new();
            List<Registration> registrations = new()
            {
                new Registration() {
                    Id = 1,
                    Office = new(){ Id = 1 },
                    Date = DateTime.Now,
                    ContractNumber = "255657",
                    Owner = new(){ Id = 1 },
                    Beneficiary1 = new(){ Id = 1 },
                    Beneficiary2 = new(){ Id = 2 },
                    Plan = new(){ Id = 1 }
                },
                new Registration() {
                    Id = 2,
                    Office = new(){ Id = 1 },
                    Date = DateTime.Now,
                    ContractNumber = "256566",
                    Owner = new(){ Id = 1 },
                    Beneficiary1 = new(){ Id = 3 },
                    Plan = new(){ Id = 1 }
                },
                new Registration() {
                    Id = 3,
                    Office = new(){ Id = 1 },
                    Date = DateTime.Now,
                    ContractNumber = "255658",
                    Owner = new(){ Id = 1 },
                    Plan = new(){ Id = 2 }
                }
            };
            mock.Setup(p => p.List("idregistration = 1", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new ListResult<Registration>(registrations.Where(y => y.Id == 1).ToList(), 1));
            mock.Setup(p => p.List("idmatricula = 1", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Throws<PersistentException>();
            mock.Setup(p => p.Read(It.IsAny<Registration>()))
                .Returns((Registration registration) => registrations.Find(x => x.Id == registration.Id) ?? new Registration());
            mock.Setup(p => p.Insert(It.IsAny<Registration>(), It.IsAny<User>()))
                .Returns((Registration registration, User user) =>
                {
                    registration.Id = registrations.Count + 1;
                    registrations.Add(registration);
                    return registration;
                });
            mock.Setup(p => p.Update(It.IsAny<Registration>(), It.IsAny<User>()))
                .Returns((Registration registration, User user) =>
                {
                    registrations.Where(x => x.Id == registration.Id).ToList().ForEach(x =>
                    {
                        x.Office = registration.Office;
                        x.Date = registration.Date;
                        x.ContractNumber = registration.ContractNumber;
                        x.Owner = registration.Owner;
                        x.Beneficiary1 = registration.Beneficiary1;
                        x.Beneficiary2 = registration.Beneficiary2;
                        x.Plan = registration.Plan;
                    });
                    return registration;
                });
            mock.Setup(p => p.Delete(It.IsAny<Registration>(), It.IsAny<User>()))
                .Returns((Registration executive, User user) =>
                {
                    registrations = registrations.Where(x => x.Id != executive.Id).ToList();
                    return executive;
                });
            _business = new(mock.Object);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Prueba la consulta de un listado de matrículas con filtros, ordenamientos y límite
        /// </summary>
        [Fact]
        public void ListTest()
        {
            //Act
            ListResult<Registration> list = _business.List("idregistration = 1", "name", 1, 0);

            //Assert
            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de matrículas con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void ListWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _business.List("idmatricula = 1", "name", 1, 0));
        }

        /// <summary>
        /// Prueba la consulta de una matrícula dada su identificador
        /// </summary>
        [Fact]
        public void ReadTest()
        {
            //Arrange
            Registration registration = new() { Id = 1 };

            //Act
            registration = _business.Read(registration);

            //Assert
            Assert.Equal(1, registration.Office.Id);
            Assert.Equal("255657", registration.ContractNumber);
        }

        /// <summary>
        /// Prueba la consulta de una matrícula que no existe dado su identificador
        /// </summary>
        [Fact]
        public void ReadNotFoundTest()
        {
            //Arrange
            Registration registration = new() { Id = 10 };

            //Act
            registration = _business.Read(registration);

            //Assert
            Assert.Equal(0, registration.Id);
        }

        /// <summary>
        /// Prueba la inserción de una matrícula
        /// </summary>
        [Fact]
        public void InsertTest()
        {
            //Arrange
            Registration registration = new() { Office = new() { Id = 1 }, Date = DateTime.Now, ContractNumber = "123456", Owner = new() { Id = 1 }, Plan = new() { Id = 5 } };

            //Act
            registration = _business.Insert(registration, new() { Id = 1 });

            //Assert
            Assert.NotEqual(0, registration.Id);
        }

        /// <summary>
        /// Prueba la actualización de una matrícula
        /// </summary>
        [Fact]
        public void UpdateTest()
        {
            //Arrange
            Registration registration = new() { Id = 2, Office = new() { Id = 2 }, Date = DateTime.Now, ContractNumber = "987654", Owner = new() { Id = 2 }, Plan = new() { Id = 10 } };
            Registration registration2 = new() { Id = 2 };

            //Act
            _ = _business.Update(registration, new() { Id = 1 });
            registration2 = _business.Read(registration2);

            //Assert
            Assert.NotEqual(1, registration2.Office.Id);
            Assert.NotEqual("256566", registration2.ContractNumber);
        }

        /// <summary>
        /// Prueba la eliminación de una matrícula
        /// </summary>
        [Fact]
        public void DeleteTest()
        {
            //Arrange
            Registration registration = new() { Id = 3 };
            Registration registration2 = new() { Id = 3 };

            //Act
            _ = _business.Delete(registration, new() { Id = 1 });
            registration2 = _business.Read(registration2);

            //Assert
            Assert.Equal(0, registration2.Id);
        }
        #endregion
    }
}
