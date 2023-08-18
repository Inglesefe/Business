﻿using Business.Config;
using Dal.Dto;
using Dal.Exceptions;
using Entities.Config;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

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
        /// Configuración de la aplicación de pruebas
        /// </summary>
        private readonly IConfiguration _configuration;

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
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, false)
                .AddEnvironmentVariables()
                .Build();
            _business = new(new MySqlConnection(_configuration.GetConnectionString("golden") ?? ""));
        }
        #endregion

        #region Methods
        /// <summary>
        /// Prueba la consulta de un listado de oficinas con filtros, ordenamientos y límite
        /// </summary>
        [Fact]
        public void OfficeListTest()
        {
            ListResult<Office> list = _business.List("o.idoffice = 1", "o.name", 1, 0);

            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de oficinas con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void OfficeListWithErrorTest()
        {
            Assert.Throws<PersistentException>(() => _business.List("idoficina = 1", "name", 1, 0));
        }

        /// <summary>
        /// Prueba la consulta de una oficina dado su identificador
        /// </summary>
        [Fact]
        public void OfficeReadTest()
        {
            Office office = new() { Id = 1 };
            office = _business.Read(office);

            Assert.Equal("Castellana", office.Name);
        }

        /// <summary>
        /// Prueba la consulta de una oficina que no existe dado su identificador
        /// </summary>
        [Fact]
        public void OfficeReadNotFoundTest()
        {
            Office office = new() { Id = 10 };
            office = _business.Read(office);

            Assert.Null(office);
        }

        /// <summary>
        /// Prueba la inserción de una oficina
        /// </summary>
        [Fact]
        public void OfficeInsertTest()
        {
            Office office = new() { City = new() { Id = 1 }, Name = "Madelena", Address = "Calle 59 sur" };
            office = _business.Insert(office, new() { Id = 1 });

            Assert.NotEqual(0, office.Id);
        }

        /// <summary>
        /// Prueba la actualización de una oficina
        /// </summary>
        [Fact]
        public void OfficeUpdateTest()
        {
            Office office = new() { Id = 2, City = new() { Id = 1 }, Name = "Santa Librada", Address = "Calle 78 sur" };
            _ = _business.Update(office, new() { Id = 1 });

            Office office2 = new() { Id = 2 };
            office2 = _business.Read(office2);

            Assert.NotEqual("Kennedy", office2.Name);
        }

        /// <summary>
        /// Prueba la eliminación de una oficina
        /// </summary>
        [Fact]
        public void OfficeDeleteTest()
        {
            Office office = new() { Id = 3 };
            _ = _business.Delete(office, new() { Id = 1 });

            Office office2 = new() { Id = 3 };
            office2 = _business.Read(office2);

            Assert.Null(office2);
        }
        #endregion
    }
}
