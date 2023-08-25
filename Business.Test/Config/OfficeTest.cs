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

        /// <summary>
        /// Conexión a la base de datos falsa
        /// </summary>
        private readonly IDbConnection connectionFake;
        #endregion

        #region Constructors
        /// <summary>
        /// Inicializa la configuración de la prueba
        /// </summary>
        public OfficeTest()
        {
            Mock<IPersistentWithLog<Office>> mock = new();
            Mock<IDbConnection> mockConnection = new();
            connectionFake = mockConnection.Object;

            List<Office> offices = new()
            {
                new Office() { Id = 1, Name = "Castellana", Address = "Cl 95" },
                new Office() { Id = 2, Name = "Kennedy", Address = "Cl 56 sur" },
                new Office() { Id = 3, Name = "Venecia", Address = "Puente" }
            };

            mock.Setup(p => p.List("o.idoffice = 1", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<IDbConnection>()))
                .Returns(new ListResult<Office>(offices.Where(y => y.Id == 1).ToList(), 1));
            mock.Setup(p => p.List("idoficina = 1", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<IDbConnection>()))
                .Throws<PersistentException>();

            mock.Setup(p => p.Read(It.IsAny<Office>(), It.IsAny<IDbConnection>()))
                .Returns((Office office, IDbConnection connection) => offices.Find(x => x.Id == office.Id) ?? new Office());

            mock.Setup(p => p.Insert(It.IsAny<Office>(), It.IsAny<User>(), It.IsAny<IDbConnection>()))
                .Returns((Office office, User user, IDbConnection connection) =>
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

            mock.Setup(p => p.Update(It.IsAny<Office>(), It.IsAny<User>(), It.IsAny<IDbConnection>()))
                .Returns((Office office, User user, IDbConnection connection) =>
                {
                    offices.Where(x => x.Id == office.Id).ToList().ForEach(x => x.Name = office.Name);
                    return office;
                });

            mock.Setup(p => p.Delete(It.IsAny<Office>(), It.IsAny<User>(), It.IsAny<IDbConnection>()))
                .Returns((Office office, User user, IDbConnection connection) =>
                {
                    offices = offices.Where(x => x.Id != office.Id).ToList();
                    return office;
                });

            _business = new(mock.Object);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Prueba la consulta de un listado de oficinas con filtros, ordenamientos y límite
        /// </summary>
        [Fact]
        public void OfficeListTest()
        {
            ListResult<Office> list = _business.List("o.idoffice = 1", "o.name", 1, 0, connectionFake);

            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de oficinas con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void OfficeListWithErrorTest()
        {
            Assert.Throws<PersistentException>(() => _business.List("idoficina = 1", "name", 1, 0, connectionFake));
        }

        /// <summary>
        /// Prueba la consulta de una oficina dado su identificador
        /// </summary>
        [Fact]
        public void OfficeReadTest()
        {
            Office office = new() { Id = 1 };
            office = _business.Read(office, connectionFake);

            Assert.Equal("Castellana", office.Name);
        }

        /// <summary>
        /// Prueba la consulta de una oficina que no existe dado su identificador
        /// </summary>
        [Fact]
        public void OfficeReadNotFoundTest()
        {
            Office office = new() { Id = 10 };
            office = _business.Read(office, connectionFake);

            Assert.Equal(0, office.Id);
        }

        /// <summary>
        /// Prueba la inserción de una oficina
        /// </summary>
        [Fact]
        public void OfficeInsertTest()
        {
            Office office = new() { City = new() { Id = 1 }, Name = "Madelena", Address = "Calle 59 sur" };
            office = _business.Insert(office, new() { Id = 1 }, connectionFake);

            Assert.NotEqual(0, office.Id);
        }

        /// <summary>
        /// Prueba la actualización de una oficina
        /// </summary>
        [Fact]
        public void OfficeUpdateTest()
        {
            Office office = new() { Id = 2, City = new() { Id = 1 }, Name = "Santa Librada", Address = "Calle 78 sur" };
            _ = _business.Update(office, new() { Id = 1 }, connectionFake);

            Office office2 = new() { Id = 2 };
            office2 = _business.Read(office2, connectionFake);

            Assert.NotEqual("Kennedy", office2.Name);
        }

        /// <summary>
        /// Prueba la eliminación de una oficina
        /// </summary>
        [Fact]
        public void OfficeDeleteTest()
        {
            Office office = new() { Id = 3 };
            _ = _business.Delete(office, new() { Id = 1 }, connectionFake);

            Office office2 = new() { Id = 3 };
            office2 = _business.Read(office2, connectionFake);

            Assert.Equal(0, office2.Id);
        }
        #endregion
    }
}
