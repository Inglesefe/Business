using Business.Config;
using Dal.Dto;
using Dal.Exceptions;
using Entities.Config;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

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
        /// Configuración de la aplicación de pruebas
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Capa de negocio de las ciudades
        /// </summary>
        private readonly BusinessCity _business;
        #endregion

        #region Constructors
        /// <summary>
        /// Inicializa la configuración de la prueba
        /// </summary>
        public CityTest()
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
        /// Prueba la consulta de un listado de ciudades con filtros, ordenamientos y límite
        /// </summary>
        [Fact]
        public void CityListTest()
        {
            ListResult<City> list = _business.List("ci.idcity = 1", "ci.name", 1, 0);

            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de ciudades con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void CityListWithErrorTest()
        {
            Assert.Throws<PersistentException>(() => _business.List("idpais = 1", "name", 1, 0));
        }

        /// <summary>
        /// Prueba la consulta de una ciudad dado su identificador
        /// </summary>
        [Fact]
        public void CityReadTest()
        {
            City city = new() { Id = 1 };
            city = _business.Read(city);

            Assert.Equal("BOG", city.Code);
        }

        /// <summary>
        /// Prueba la consulta de una ciudad que no existe dado su identificador
        /// </summary>
        [Fact]
        public void CityReadNotFoundTest()
        {
            City city = new() { Id = 10 };
            city = _business.Read(city);

            Assert.Null(city);
        }

        /// <summary>
        /// Prueba la inserción de una ciudad
        /// </summary>
        [Fact]
        public void CityInsertTest()
        {
            City city = new() { Country = new() { Id = 1 }, Code = "BUC", Name = "Bucaramanga" };
            city = _business.Insert(city, new() { Id = 1 });

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

            _ = Assert.Throws<PersistentException>(() => _business.Insert(city, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la actualización de una ciudad
        /// </summary>
        [Fact]
        public void CityUpdateTest()
        {
            City city = new() { Id = 2, Country = new() { Id = 1 }, Code = "BAQ", Name = "Barranquilla" };
            _ = _business.Update(city, new() { Id = 1 });

            City city2 = new() { Id = 2 };
            city2 = _business.Read(city2);

            Assert.NotEqual("MED", city2.Code);
        }

        /// <summary>
        /// Prueba la eliminación de una ciudad
        /// </summary>
        [Fact]
        public void CityDeleteTest()
        {
            City city = new() { Id = 3 };
            _ = _business.Delete(city, new() { Id = 1 });

            City city2 = new() { Id = 3 };
            city2 = _business.Read(city2);

            Assert.Null(city2);
        }
        #endregion
    }
}
