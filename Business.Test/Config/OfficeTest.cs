using Business.Config;
using Dal.Config;
using Dal.Dto;
using Dal.Exceptions;
using Entities.Admon;
using Entities.Auth;
using Entities.Config;
using Moq;
using System.Data;

namespace Business.Test.Config
{
    /// <summary>
    /// Realiza las pruebas sobre la clase de persistencia de oficinas
    /// </summary>
    [Collection("Tests")]
    public class OfficeTest
    {
        #region Attributes
        /// <summary>
        /// Capa de negocio de las oficinas
        /// </summary>
        private readonly BusinessOffice _business;
        #endregion

        #region Constructors
        /// <summary>
        /// Inicializa la configuración de la prueba
        /// </summary>
        public OfficeTest()
        {
            Mock<IPersistentOffice> mock = new();
            List<Office> offices = new()
            {
                new Office() { Id = 1, Name = "Castellana", Address = "Cl 95" },
                new Office() { Id = 2, Name = "Kennedy", Address = "Cl 56 sur" },
                new Office() { Id = 3, Name = "Venecia", Address = "Puente" }
            };
            List<AccountExecutive> executives = new()
            {
                new AccountExecutive() { Id = 1, Name = "Leandro Baena Torres", IdentificationType = new(){ Id = 1 }, Identification = "123456789" },
                new AccountExecutive() { Id = 2, Name = "David Santiago Baena Barreto", IdentificationType = new(){ Id = 1 }, Identification = "987654321" },
                new AccountExecutive() { Id = 3, Name = "Karol Ximena Baena Barreto", IdentificationType = new(){ Id = 1 }, Identification = "147852369" }
            };
            List<Tuple<Office, AccountExecutive>> executives_offices = new()
            {
                new Tuple<Office, AccountExecutive>(offices[0], executives[0]),
                new Tuple<Office, AccountExecutive>(offices[0], executives[1]),
                new Tuple<Office, AccountExecutive>(offices[1], executives[0]),
                new Tuple<Office, AccountExecutive>(offices[1], executives[1])
            };
            mock.Setup(p => p.List("o.idoffice = 1", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new ListResult<Office>(offices.Where(y => y.Id == 1).ToList(), 1));
            mock.Setup(p => p.List("idoficina = 1", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Throws<PersistentException>();
            mock.Setup(p => p.Read(It.IsAny<Office>()))
                .Returns((Office office) => offices.Find(x => x.Id == office.Id) ?? new Office());
            mock.Setup(p => p.Insert(It.IsAny<Office>(), It.IsAny<User>()))
                .Returns((Office office, User user) =>
                {
                    if (offices.Exists(x => x.Name == office.Name))
                    {
                        throw new PersistentException();
                    }
                    else
                    {
                        office.Id = offices.Count + 1;
                        offices.Add(office);
                        return office;
                    }
                });
            mock.Setup(p => p.Update(It.IsAny<Office>(), It.IsAny<User>()))
                .Returns((Office office, User user) =>
                {
                    offices.Where(x => x.Id == office.Id).ToList().ForEach(x => x.Name = office.Name);
                    return office;
                });
            mock.Setup(p => p.Delete(It.IsAny<Office>(), It.IsAny<User>()))
                .Returns((Office office, User user) =>
                {
                    offices = offices.Where(x => x.Id != office.Id).ToList();
                    return office;
                });
            mock.Setup(p => p.ListAccountExecutives("", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Office>()))
                .Returns(new ListResult<AccountExecutive>(executives_offices.Where(x => x.Item1.Id == 1).Select(x => x.Item2).ToList(), 1));
            mock.Setup(p => p.ListAccountExecutives("idaccountexecutive = 2", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Office>()))
                .Returns(new ListResult<AccountExecutive>(new List<AccountExecutive>(), 0));
            mock.Setup(p => p.ListAccountExecutives("idejecutivocuenta = 1", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Office>()))
                .Throws<PersistentException>();
            mock.Setup(p => p.ListNotAccountExecutives(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Office>()))
                .Returns((string filters, string orders, int limit, int offset, Office office) =>
                {
                    List<AccountExecutive> result = executives.Where(x => !executives_offices.Exists(y => y.Item1.Id == office.Id && y.Item2.Id == x.Id)).ToList();
                    return new ListResult<AccountExecutive>(result, result.Count);
                });
            mock.Setup(p => p.InsertAccountExecutive(It.IsAny<AccountExecutive>(), It.IsAny<Office>(), It.IsAny<User>())).
                Returns((AccountExecutive executive, Office office, User user) =>
                {
                    if (executives_offices.Exists(x => x.Item1.Id == office.Id && x.Item2.Id == executive.Id))
                    {
                        throw new PersistentException();
                    }
                    else
                    {
                        executives_offices.Add(new Tuple<Office, AccountExecutive>(office, executive));
                        return executive;
                    }
                });
            _business = new(mock.Object);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Prueba la consulta de un listado de oficinas con filtros, ordenamientos y límite
        /// </summary>
        [Fact]
        public void ListTest()
        {
            //Act
            ListResult<Office> list = _business.List("o.idoffice = 1", "o.name", 1, 0);

            //Assert
            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de oficinas con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void ListWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _business.List("idoficina = 1", "name", 1, 0));
        }

        /// <summary>
        /// Prueba la consulta de una oficina dado su identificador
        /// </summary>
        [Fact]
        public void ReadTest()
        {
            //Arrange
            Office office = new() { Id = 1 };

            //Act
            office = _business.Read(office);

            //Assert
            Assert.Equal("Castellana", office.Name);
        }

        /// <summary>
        /// Prueba la consulta de una oficina que no existe dado su identificador
        /// </summary>
        [Fact]
        public void ReadNotFoundTest()
        {
            //Assert
            Office office = new() { Id = 10 };

            //Assert
            office = _business.Read(office);

            //Assert
            Assert.Equal(0, office.Id);
        }

        /// <summary>
        /// Prueba la inserción de una oficina
        /// </summary>
        [Fact]
        public void InsertTest()
        {
            //Arrange
            Office office = new() { City = new() { Id = 1 }, Name = "Madelena", Address = "Calle 59 sur" };

            //Act
            office = _business.Insert(office, new() { Id = 1 });

            //Assert
            Assert.NotEqual(0, office.Id);
        }

        /// <summary>
        /// Prueba la actualización de una oficina
        /// </summary>
        [Fact]
        public void UpdateTest()
        {
            //Arrange
            Office office = new() { Id = 2, City = new() { Id = 1 }, Name = "Santa Librada", Address = "Calle 78 sur" };
            Office office2 = new() { Id = 2 };

            //Act
            office2 = _business.Read(office2);
            _ = _business.Update(office, new() { Id = 1 });

            //Assert
            Assert.NotEqual("Kennedy", office2.Name);
        }

        /// <summary>
        /// Prueba la eliminación de una oficina
        /// </summary>
        [Fact]
        public void DeleteTest()
        {
            //Arrange
            Office office = new() { Id = 3 };
            Office office2 = new() { Id = 3 };

            //Act
            _ = _business.Delete(office, new() { Id = 1 });
            office2 = _business.Read(office2);

            //Assert
            Assert.Equal(0, office2.Id);
        }

        /// <summary>
        /// Prueba la consulta de un listado de ejecutivos de cuenta de una oficina con filtros, ordenamientos y límite
        /// </summary>
        [Fact]
        public void ListAccountExecutivesTest()
        {
            //Act
            ListResult<AccountExecutive> list = _business.ListAccountExecutives("", "", 10, 0, new() { Id = 1 });

            //Assert
            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de ejecutivos de cuenta de una oficina con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void ListAccountExecutivesWithErrorTest()
        {
            //Act, Assert
            _ = Assert.Throws<PersistentException>(() => _business.ListAccountExecutives("idejecutivocuenta = 1", "name", 10, 0, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la consulta de un listado de ejecutivos de cuenta no asignados a una oficina con filtros, ordenamientos y límite
        /// </summary>
        [Fact]
        public void ListNotAccountExecutivesTest()
        {
            //Act
            ListResult<AccountExecutive> list = _business.ListNotAccountExecutives("", "", 10, 0, new() { Id = 1 });

            //Assert
            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la inserción de un ejecutivo de cuenta a una oficina
        /// </summary>
        [Fact]
        public void InsertAccountExecutiveTest()
        {
            //Act
            AccountExecutive executive = _business.InsertAccountExecutive(new() { Id = 4 }, new() { Id = 1 }, new() { Id = 1 });

            //Assert
            Assert.NotEqual(0, executive.Id);
        }

        /// <summary>
        /// Prueba la eliminación de un ejecutivo de cuenta de una oficina
        /// </summary>
        [Fact]
        public void DeleteAccountExecutiveTest()
        {
            //Act
            _ = _business.DeleteAccountExecutive(new() { Id = 2 }, new() { Id = 1 }, new() { Id = 1 });
            ListResult<AccountExecutive> list = _business.ListAccountExecutives("idaccountexecutive = 2", "", 10, 0, new() { Id = 1 });

            //Assert
            Assert.Equal(0, list.Total);
        }
        #endregion
    }
}
