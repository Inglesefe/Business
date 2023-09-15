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
        #endregion

        #region Constructors
        /// <summary>
        /// Inicializa la configuración de la prueba
        /// </summary>
        public CountryTest()
        {
            //Arrange
            Mock<IPersistentWithLog<Country>> mock = new();
            List<Country> countries = new()
            {
                new Country() { Id = 1, Code = "CO", Name = "Colombia" },
                new Country() { Id = 2, Code = "US", Name = "Estados unidos" },
                new Country() { Id = 3, Code = "EN", Name = "Inglaterra" }
            };
            mock.Setup(p => p.List("idcountry = 1", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new ListResult<Country>(countries.Where(y => y.Id == 1).ToList(), 1));
            mock.Setup(p => p.List("idpais = 1", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Throws<PersistentException>();
            mock.Setup(p => p.Read(It.IsAny<Country>()))
                .Returns((Country country) => countries.Find(x => x.Id == country.Id) ?? new Country());
            mock.Setup(p => p.Insert(It.IsAny<Country>(), It.IsAny<User>()))
                .Returns((Country country, User user) =>
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
            mock.Setup(p => p.Update(It.IsAny<Country>(), It.IsAny<User>()))
                .Returns((Country country, User user) =>
                {
                    countries.Where(x => x.Id == country.Id).ToList().ForEach(x => x.Code = country.Code);
                    return country;
                });
            mock.Setup(p => p.Delete(It.IsAny<Country>(), It.IsAny<User>()))
                .Returns((Country city, User user) =>
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
        public void ListTest()
        {
            //Act
            ListResult<Country> list = _business.List("idcountry = 1", "name", 1, 0);

            //Assert
            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de paises con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void ListWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _business.List("idpais = 1", "name", 1, 0));
        }

        /// <summary>
        /// Prueba la consulta de un país dado su identificador
        /// </summary>
        [Fact]
        public void ReadTest()
        {
            //Arrange
            Country country = new() { Id = 1 };

            //Act
            country = _business.Read(country);

            //Assert
            Assert.Equal("CO", country.Code);
        }

        /// <summary>
        /// Prueba la consulta de un país que no existe dado su identificador
        /// </summary>
        [Fact]
        public void ReadNotFoundTest()
        {
            //Arrange
            Country country = new() { Id = 10 };

            //Act
            country = _business.Read(country);

            //Assert
            Assert.Equal(0, country.Id);
        }

        /// <summary>
        /// Prueba la inserción de un país
        /// </summary>
        [Fact]
        public void InsertTest()
        {
            //Arrange
            Country country = new() { Code = "PR", Name = "Puerto Rico" };

            //Act
            country = _business.Insert(country, new() { Id = 1 });

            //Assert
            Assert.NotEqual(0, country.Id);
        }

        /// <summary>
        /// Prueba la inserción de un país con nombre duplicado
        /// </summary>
        /// <returns>N/A</returns>
        [Fact]
        public void InsertDuplicateTest()
        {
            //Arrange
            Country country = new() { Code = "CO", Name = "Colombia" };

            //Act, Assert
            _ = Assert.Throws<PersistentException>(() => _business.Insert(country, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la actualización de un país
        /// </summary>
        [Fact]
        public void UpdateTest()
        {
            //Arrange
            Country country = new() { Id = 2, Code = "PE", Name = "Perú" };
            Country country2 = new() { Id = 2 };

            //Act
            _ = _business.Update(country, new() { Id = 1 });
            country2 = _business.Read(country2);

            //Assert
            Assert.NotEqual("US", country2.Code);
        }

        /// <summary>
        /// Prueba la eliminación de un país
        /// </summary>
        [Fact]
        public void DeleteTest()
        {
            //Arrange
            Country country = new() { Id = 3 };
            Country country2 = new() { Id = 3 };

            //Act
            _ = _business.Delete(country, new() { Id = 1 });
            country2 = _business.Read(country2);

            //Assert
            Assert.Equal(0, country2.Id);
        }
        #endregion
    }
}
