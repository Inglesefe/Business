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
    /// Realiza las pruebas sobre la clase de persistencia de paises
    /// </summary>
    [Collection("Tests")]
    public class CountryTest
    {
        #region Attributes
        /// <summary>
        /// Capa de negocio de los paises
        /// </summary>
        private readonly BusinessCountry _business;

        /// <summary>
        /// Conexión a la base de datos falsa
        /// </summary>
        private readonly IDbConnection connectionFake;
        #endregion

        #region Constructors
        /// <summary>
        /// Inicializa la configuración de la prueba
        /// </summary>
        public CountryTest()
        {
            Mock<IPersistentWithLog<Country>> mock = new();
            Mock<IDbConnection> mockConnection = new();
            connectionFake = mockConnection.Object;

            List<Country> countries = new()
            {
                new Country() { Id = 1, Code = "CO", Name = "Colombia" },
                new Country() { Id = 2, Code = "US", Name = "Estados unidos" },
                new Country() { Id = 3, Code = "EN", Name = "Inglaterra" }
            };

            mock.Setup(p => p.List("idcountry = 1", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<IDbConnection>()))
                .Returns(new ListResult<Country>(countries.Where(y => y.Id == 1).ToList(), 1));
            mock.Setup(p => p.List("idpais = 1", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<IDbConnection>()))
                .Throws<PersistentException>();

            mock.Setup(p => p.Read(It.IsAny<Country>(), It.IsAny<IDbConnection>()))
                .Returns((Country country, IDbConnection connection) => countries.Find(x => x.Id == country.Id) ?? new Country());

            mock.Setup(p => p.Insert(It.IsAny<Country>(), It.IsAny<User>(), It.IsAny<IDbConnection>()))
                .Returns((Country country, User user, IDbConnection connection) =>
                {
                    if (countries.Exists(x => x.Code == country.Code))
                    {
                        throw new PersistentException();
                    }
                    else
                    {
                        country.Id = countries.Count + 1;
                        countries.Add(country);
                        return country;
                    }
                });

            mock.Setup(p => p.Update(It.IsAny<Country>(), It.IsAny<User>(), It.IsAny<IDbConnection>()))
                .Returns((Country country, User user, IDbConnection connection) =>
                {
                    countries.Where(x => x.Id == country.Id).ToList().ForEach(x => x.Code = country.Code);
                    return country;
                });

            mock.Setup(p => p.Delete(It.IsAny<Country>(), It.IsAny<User>(), It.IsAny<IDbConnection>()))
                .Returns((Country city, User user, IDbConnection connection) =>
                {
                    countries = countries.Where(x => x.Id != city.Id).ToList();
                    return city;
                });

            _business = new(mock.Object);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Prueba la consulta de un listado de paises con filtros, ordenamientos y límite
        /// </summary>
        [Fact]
        public void CountryListTest()
        {
            ListResult<Country> list = _business.List("idcountry = 1", "name", 1, 0, connectionFake);

            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de paises con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void CountryListWithErrorTest()
        {
            Assert.Throws<PersistentException>(() => _business.List("idpais = 1", "name", 1, 0, connectionFake));
        }

        /// <summary>
        /// Prueba la consulta de un país dado su identificador
        /// </summary>
        [Fact]
        public void CountryReadTest()
        {
            Country country = new() { Id = 1 };
            country = _business.Read(country, connectionFake);

            Assert.Equal("CO", country.Code);
        }

        /// <summary>
        /// Prueba la consulta de un país que no existe dado su identificador
        /// </summary>
        [Fact]
        public void CountryReadNotFoundTest()
        {
            Country country = new() { Id = 10 };
            country = _business.Read(country, connectionFake);

            Assert.Equal(0, country.Id);
        }

        /// <summary>
        /// Prueba la inserción de un país
        /// </summary>
        [Fact]
        public void CountryInsertTest()
        {
            Country country = new() { Code = "PR", Name = "Puerto Rico" };
            country = _business.Insert(country, new() { Id = 1 }, connectionFake);

            Assert.NotEqual(0, country.Id);
        }

        /// <summary>
        /// Prueba la inserción de un país con nombre duplicado
        /// </summary>
        /// <returns>N/A</returns>
        [Fact]
        public void CountryInsertDuplicateTest()
        {
            Country country = new() { Code = "CO", Name = "Colombia" };

            _ = Assert.Throws<PersistentException>(() => _business.Insert(country, new() { Id = 1 }, connectionFake));
        }

        /// <summary>
        /// Prueba la actualización de un país
        /// </summary>
        [Fact]
        public void CountryUpdateTest()
        {
            Country country = new() { Id = 2, Code = "PE", Name = "Perú" };
            _ = _business.Update(country, new() { Id = 1 }, connectionFake);

            Country country2 = new() { Id = 2 };
            country2 = _business.Read(country2, connectionFake);

            Assert.NotEqual("US", country2.Code);
        }

        /// <summary>
        /// Prueba la eliminación de un país
        /// </summary>
        [Fact]
        public void CountryDeleteTest()
        {
            Country country = new() { Id = 3 };
            _ = _business.Delete(country, new() { Id = 1 }, connectionFake);

            Country country2 = new() { Id = 3 };
            country2 = _business.Read(country2, connectionFake);

            Assert.Equal(0, country2.Id);
        }
        #endregion
    }
}
