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
    /// Realiza las pruebas sobre la clase de persistencia de ciudades
    /// </summary>
    [Collection("Tests")]
    public class CityTest
    {
        #region Attributes
        /// <summary>
        /// Capa de negocio de las ciudades
        /// </summary>
        private readonly BusinessCity _business;

        /// <summary>
        /// Conexión a la base de datos falsa
        /// </summary>
        private readonly IDbConnection connectionFake;
        #endregion

        #region Constructors
        /// <summary>
        /// Inicializa la configuración de la prueba
        /// </summary>
        public CityTest()
        {
            Mock<IPersistentWithLog<City>> mock = new();
            Mock<IDbConnection> mockConnection = new();
            connectionFake = mockConnection.Object;

            List<City> cities = new()
            {
                new City() { Id = 1, Code = "BOG", Name = "Bogotá" },
                new City() { Id = 1, Code = "MED", Name = "Medellín" },
                new City() { Id = 1, Code = "CAL", Name = "Cali" }
            };

            mock.Setup(p => p.List("ci.idcity = 1", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<IDbConnection>()))
                .Returns(new ListResult<City>(cities.Where(y => y.Id == 1).ToList(), 1));
            mock.Setup(p => p.List("idpais = 1", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<IDbConnection>()))
                .Throws<PersistentException>();

            mock.Setup(p => p.Read(It.IsAny<City>(), It.IsAny<IDbConnection>()))
                .Returns((City city, IDbConnection connection) => cities.Find(x => x.Id == city.Id) ?? new City());

            mock.Setup(p => p.Insert(It.IsAny<City>(), It.IsAny<User>(), It.IsAny<IDbConnection>()))
                .Returns((City city, User user, IDbConnection connection) =>
                {
                    if (cities.Exists(x => x.Code == city.Code))
                    {
                        throw new PersistentException();
                    }
                    else
                    {
                        city.Id = cities.Count + 1;
                        cities.Add(city);
                        return city;
                    }
                });

            mock.Setup(p => p.Update(It.IsAny<City>(), It.IsAny<User>(), It.IsAny<IDbConnection>()))
                .Returns((City city, User user, IDbConnection connection) =>
                {
                    cities.Where(x => x.Id == city.Id).ToList().ForEach(x => x.Code = city.Code);
                    return city;
                });

            mock.Setup(p => p.Delete(It.IsAny<City>(), It.IsAny<User>(), It.IsAny<IDbConnection>()))
                .Returns((City city, User user, IDbConnection connection) =>
                {
                    cities = cities.Where(x => x.Id != city.Id).ToList();
                    return city;
                });

            _business = new(mock.Object);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Prueba la consulta de un listado de ciudades con filtros, ordenamientos y límite
        /// </summary>
        [Fact]
        public void CityListTest()
        {
            ListResult<City> list = _business.List("ci.idcity = 1", "ci.name", 1, 0, connectionFake);

            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de ciudades con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void CityListWithErrorTest()
        {
            Assert.Throws<PersistentException>(() => _business.List("idpais = 1", "name", 1, 0, connectionFake));
        }

        /// <summary>
        /// Prueba la consulta de una ciudad dado su identificador
        /// </summary>
        [Fact]
        public void CityReadTest()
        {
            City city = new() { Id = 1 };
            city = _business.Read(city, connectionFake);

            Assert.Equal("BOG", city.Code);
        }

        /// <summary>
        /// Prueba la consulta de una ciudad que no existe dado su identificador
        /// </summary>
        [Fact]
        public void CityReadNotFoundTest()
        {
            City city = new() { Id = 10 };
            city = _business.Read(city, connectionFake);

            Assert.Equal(0, city.Id);
        }

        /// <summary>
        /// Prueba la inserción de una ciudad
        /// </summary>
        [Fact]
        public void CityInsertTest()
        {
            City city = new() { Country = new() { Id = 1 }, Code = "BUC", Name = "Bucaramanga" };
            city = _business.Insert(city, new() { Id = 1 }, connectionFake);

            Assert.NotEqual(0, city.Id);
        }

        /// <summary>
        /// Prueba la inserción de una ciudad con código duplicado
        /// </summary>
        /// <returns>N/A</returns>
        [Fact]
        public void CityInsertDuplicateTest()
        {
            City city = new() { Country = new() { Id = 1 }, Code = "BOG", Name = "Prueba 1" };

            _ = Assert.Throws<PersistentException>(() => _business.Insert(city, new() { Id = 1 }, connectionFake));
        }

        /// <summary>
        /// Prueba la actualización de una ciudad
        /// </summary>
        [Fact]
        public void CityUpdateTest()
        {
            City city = new() { Id = 2, Country = new() { Id = 1 }, Code = "BAQ", Name = "Barranquilla" };
            _ = _business.Update(city, new() { Id = 1 }, connectionFake);

            City city2 = new() { Id = 2 };
            city2 = _business.Read(city2, connectionFake);

            Assert.NotEqual("MED", city2.Code);
        }

        /// <summary>
        /// Prueba la eliminación de una ciudad
        /// </summary>
        [Fact]
        public void CityDeleteTest()
        {
            City city = new() { Id = 3 };
            _ = _business.Delete(city, new() { Id = 1 }, connectionFake);

            City city2 = new() { Id = 3 };
            city2 = _business.Read(city2, connectionFake);

            Assert.Equal(0, city2.Id);
        }
        #endregion
    }
}
