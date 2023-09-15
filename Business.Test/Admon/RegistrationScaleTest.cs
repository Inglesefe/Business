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
    /// Realiza las pruebas sobre la clase de persistencia de escalas asociadas a las matrículas
    /// </summary>
    [Collection("Tests")]
    public class RegistrationScaleTest
    {
        #region Attributes
        /// <summary>
        /// Capa de negocio de las escalas asociadas a las matrículas
        /// </summary>
        private readonly BusinessRegistrationScale _business;
        #endregion

        #region Constructors
        /// <summary>
        /// Inicializa la configuración de la prueba
        /// </summary>
        public RegistrationScaleTest()
        {
            //Arrange
            Mock<IPersistentWithLog<RegistrationScale>> mock = new();
            List<RegistrationScale> registrationScales = new()
            {
                new RegistrationScale() { Id = 1, Registration = new(){Id = 1}, Scale = new(){ Id = 1 }, AccountExecutive = new(){ Id = 1} },
                new RegistrationScale() { Id = 2, Registration = new(){Id = 1}, Scale = new(){ Id = 1 }, AccountExecutive = new(){ Id = 2} },
                new RegistrationScale() { Id = 3, Registration = new(){Id = 1}, Scale = new(){ Id = 2 }, AccountExecutive = new(){ Id = 1} }
            };
            mock.Setup(p => p.List("idregistrationscale = 1", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new ListResult<RegistrationScale>(registrationScales.Where(y => y.Id == 1).ToList(), 1));
            mock.Setup(p => p.List("idescalamatricula = 1", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Throws<PersistentException>();
            mock.Setup(p => p.Read(It.IsAny<RegistrationScale>()))
                .Returns((RegistrationScale executive) => registrationScales.Find(x => x.Id == executive.Id) ?? new RegistrationScale());
            mock.Setup(p => p.Insert(It.IsAny<RegistrationScale>(), It.IsAny<User>()))
                .Returns((RegistrationScale executive, User user) =>
                {
                    executive.Id = registrationScales.Count + 1;
                    registrationScales.Add(executive);
                    return executive;
                });
            mock.Setup(p => p.Update(It.IsAny<RegistrationScale>(), It.IsAny<User>()))
                .Returns((RegistrationScale executive, User user) =>
                {
                    registrationScales.Where(x => x.Id == executive.Id).ToList().ForEach(x =>
                    {
                        x.Registration = executive.Registration;
                        x.Scale = executive.Scale;
                        x.AccountExecutive = executive.AccountExecutive;
                    });
                    return executive;
                });
            mock.Setup(p => p.Delete(It.IsAny<RegistrationScale>(), It.IsAny<User>()))
                .Returns((RegistrationScale executive, User user) =>
                {
                    registrationScales = registrationScales.Where(x => x.Id != executive.Id).ToList();
                    return executive;
                });
            _business = new(mock.Object);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Prueba la consulta de un listado de escalas asociadas a matrículas con filtros, ordenamientos y límite
        /// </summary>
        [Fact]
        public void ListTest()
        {
            //Act
            ListResult<RegistrationScale> list = _business.List("idregistrationscale = 1", "name", 1, 0);

            //Assert
            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de escalas asociadas a matrículas con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void ListWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _business.List("idescalamatricula = 1", "name", 1, 0));
        }

        /// <summary>
        /// Prueba la consulta de una escala asociada a matrícula dada su identificador
        /// </summary>
        [Fact]
        public void ReadTest()
        {
            //Arrange
            RegistrationScale regScale = new() { Id = 1 };

            //Act
            regScale = _business.Read(regScale);

            //Assert
            Assert.Equal(1, regScale.Registration.Id);
            Assert.Equal(1, regScale.Scale.Id);
            Assert.Equal(1, regScale.AccountExecutive.Id);
        }

        /// <summary>
        /// Prueba la consulta de un ejecutivo de cuenta que no existe dado su identificador
        /// </summary>
        [Fact]
        public void ReadNotFoundTest()
        {
            //Arrange
            RegistrationScale regScale = new() { Id = 10 };

            //Act
            regScale = _business.Read(regScale);

            //Assert
            Assert.Equal(0, regScale.Id);
        }

        /// <summary>
        /// Prueba la inserción de un ejecutivo de cuenta
        /// </summary>
        [Fact]
        public void InsertTest()
        {
            //Arrange
            RegistrationScale regScale = new() { Registration = new() { Id = 1 }, Scale = new() { Id = 2 }, AccountExecutive = new() { Id = 2 } };

            //Act
            regScale = _business.Insert(regScale, new() { Id = 1 });

            //Assert
            Assert.NotEqual(0, regScale.Id);
        }

        /// <summary>
        /// Prueba la actualización de un ejecutivo de cuenta
        /// </summary>
        [Fact]
        public void UpdateTest()
        {
            //Arrange
            RegistrationScale regScale = new() { Id = 2, Registration = new() { Id = 1 }, Scale = new() { Id = 3 }, AccountExecutive = new() { Id = 3 } };
            RegistrationScale regScale2 = new() { Id = 2 };

            //Act
            _ = _business.Update(regScale, new() { Id = 1 });
            regScale2 = _business.Read(regScale2);

            //Assert
            Assert.NotEqual(1, regScale2.Scale.Id);
            Assert.NotEqual(2, regScale2.AccountExecutive.Id);
        }

        /// <summary>
        /// Prueba la eliminación de un ejecutivo de cuenta
        /// </summary>
        [Fact]
        public void DeleteTest()
        {
            //Arrange
            RegistrationScale regScale = new() { Id = 3 };
            RegistrationScale regScale2 = new() { Id = 3 };

            //Act
            _ = _business.Delete(regScale, new() { Id = 1 });
            regScale2 = _business.Read(regScale2);

            //Assert
            Assert.Equal(0, regScale2.Id);
        }
        #endregion
    }
}
