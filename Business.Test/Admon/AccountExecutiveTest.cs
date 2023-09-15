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
    /// Realiza las pruebas sobre la clase de persistencia de ejecutivos de cuenta
    /// </summary>
    [Collection("Tests")]
    public class AccountExecutiveTest
    {
        #region Attributes
        /// <summary>
        /// Capa de negocio de los ejecutivos de cuenta
        /// </summary>
        private readonly BusinessAccountExecutive _business;
        #endregion

        #region Constructors
        /// <summary>
        /// Inicializa la configuración de la prueba
        /// </summary>
        public AccountExecutiveTest()
        {
            //Arrange
            Mock<IPersistentWithLog<AccountExecutive>> mock = new();
            List<AccountExecutive> executives = new()
            {
                new AccountExecutive() { Id = 1, Name = "Leandro Baena Torres", IdentificationType = new(){ Id = 1 }, Identification = "123456789" },
                new AccountExecutive() { Id = 2, Name = "David Santiago Baena Barreto", IdentificationType = new(){ Id = 1 }, Identification = "987654321" },
                new AccountExecutive() { Id = 3, Name = "Karol Ximena Baena Barreto", IdentificationType = new(){ Id = 1 }, Identification = "147852369" },
                new AccountExecutive() { Id = 4, Name = "Luz Marina Torres", IdentificationType = new(){ Id = 1 }, Identification = "852963741" }
            };
            mock.Setup(p => p.List("idaccountexecutive = 1", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new ListResult<AccountExecutive>(executives.Where(y => y.Id == 1).ToList(), 1));
            mock.Setup(p => p.List("idejecutivocuenta = 1", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Throws<PersistentException>();
            mock.Setup(p => p.Read(It.IsAny<AccountExecutive>()))
                .Returns((AccountExecutive executive) => executives.Find(x => x.Id == executive.Id) ?? new AccountExecutive());
            mock.Setup(p => p.Insert(It.IsAny<AccountExecutive>(), It.IsAny<User>()))
                .Returns((AccountExecutive executive, User user) =>
                {
                    if (executives.Exists(x => x.IdentificationType.Id == executive.IdentificationType.Id && x.Identification == executive.Identification))
                    {
                        throw new PersistentException();
                    }
                    else
                    {
                        executive.Id = executives.Count + 1;
                        executives.Add(executive);
                        return executive;
                    }
                });
            mock.Setup(p => p.Update(It.IsAny<AccountExecutive>(), It.IsAny<User>()))
                .Returns((AccountExecutive executive, User user) =>
                {
                    executives.Where(x => x.Id == executive.Id).ToList().ForEach(x =>
                    {
                        x.Name = executive.Name;
                        x.IdentificationType = executive.IdentificationType;
                        x.Identification = executive.Identification;
                    });
                    return executive;
                });
            mock.Setup(p => p.Delete(It.IsAny<AccountExecutive>(), It.IsAny<User>()))
                .Returns((AccountExecutive executive, User user) =>
                {
                    executives = executives.Where(x => x.Id != executive.Id).ToList();
                    return executive;
                });
            _business = new(mock.Object);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Prueba la consulta de un listado de ejecutivos de cuenta con filtros, ordenamientos y límite
        /// </summary>
        [Fact]
        public void ListTest()
        {
            //Act
            ListResult<AccountExecutive> list = _business.List("idaccountexecutive = 1", "name", 1, 0);

            //Assert
            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de ejecutivos de cuenta con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void ListWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _business.List("idejecutivocuenta = 1", "name", 1, 0));
        }

        /// <summary>
        /// Prueba la consulta de un ejecutivo de cuenta dada su identificador
        /// </summary>
        [Fact]
        public void ReadTest()
        {
            //Arrange
            AccountExecutive executive = new() { Id = 1 };

            //Act
            executive = _business.Read(executive);

            //Assert
            Assert.Equal("Leandro Baena Torres", executive.Name);
        }

        /// <summary>
        /// Prueba la consulta de un ejecutivo de cuenta que no existe dado su identificador
        /// </summary>
        [Fact]
        public void ReadNotFoundTest()
        {
            //Arrange
            AccountExecutive executive = new() { Id = 10 };

            //Act
            executive = _business.Read(executive);

            //Assert
            Assert.Equal(0, executive.Id);
        }

        /// <summary>
        /// Prueba la inserción de un ejecutivo de cuenta
        /// </summary>
        [Fact]
        public void InsertTest()
        {
            //Arrange
            AccountExecutive executive = new() { Name = "Patricia Torres", IdentificationType = new() { Id = 1 }, Identification = "753146289" };

            //Act
            executive = _business.Insert(executive, new() { Id = 1 });

            //Assert
            Assert.NotEqual(0, executive.Id);
        }

        /// <summary>
        /// Prueba la inserción de un ejecutivo de cuenta con identificación duplicada
        /// </summary>
        [Fact]
        public void InsertDuplicateTest()
        {
            //Arrange
            AccountExecutive executive = new() { Name = "Prueba errónea", IdentificationType = new() { Id = 1 }, Identification = "123456789" };

            //Act, Assert
            _ = Assert.Throws<PersistentException>(() => _business.Insert(executive, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la actualización de un ejecutivo de cuenta
        /// </summary>
        [Fact]
        public void UpdateTest()
        {
            //Arrange
            AccountExecutive executive = new() { Id = 2, Name = "Prueba actualizar", IdentificationType = new() { Id = 1 }, Identification = "321456987" };
            AccountExecutive executive2 = new() { Id = 2 };

            //Act
            _ = _business.Update(executive, new() { Id = 1 });
            executive2 = _business.Read(executive2);

            //Assert
            Assert.NotEqual("David Santiago Baena Barreto", executive2.Name);
        }

        /// <summary>
        /// Prueba la eliminación de un ejecutivo de cuenta
        /// </summary>
        [Fact]
        public void DeleteTest()
        {
            //Arrange
            AccountExecutive executive = new() { Id = 3 };
            AccountExecutive executive2 = new() { Id = 3 };

            //Act
            _ = _business.Delete(executive, new() { Id = 1 });
            executive2 = _business.Read(executive2);

            //Assert
            Assert.Equal(0, executive2.Id);
        }
        #endregion
    }
}
