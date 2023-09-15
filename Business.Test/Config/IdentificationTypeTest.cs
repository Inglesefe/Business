﻿using Business.Config;
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
    /// Realiza las pruebas sobre la clase de persistencia de tipos de identificación
    /// </summary>
    [Collection("Tests")]
    public class IdentificationTypeTest
    {
        #region Attributes
        /// <summary>
        /// Capa de negocio de los tipos de identificación
        /// </summary>
        private readonly BusinessIdentificationType _business;
        #endregion

        #region Constructors
        /// <summary>
        /// Inicializa la configuración de la prueba
        /// </summary>
        public IdentificationTypeTest()
        {
            //Arrange
            Mock<IPersistentWithLog<IdentificationType>> mock = new();
            List<IdentificationType> identificationTypes = new()
            {
                new IdentificationType() { Id = 1, Name = "Cédula ciudadanía" },
                new IdentificationType() { Id = 2, Name = "Cédula extranjería" },
                new IdentificationType() { Id = 3, Name = "Pasaporte" }
            };
            mock.Setup(p => p.List("ididentificationtype = 1", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new ListResult<IdentificationType>(identificationTypes.Where(y => y.Id == 1).ToList(), 1));
            mock.Setup(p => p.List("idtipoidentificacion = 1", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Throws<PersistentException>();
            mock.Setup(p => p.Read(It.IsAny<IdentificationType>()))
                .Returns((IdentificationType identificationType) => identificationTypes.Find(x => x.Id == identificationType.Id) ?? new IdentificationType());
            mock.Setup(p => p.Insert(It.IsAny<IdentificationType>(), It.IsAny<User>()))
                .Returns((IdentificationType identificationType, User user) =>
                {
                    if (identificationTypes.Exists(x => x.Name == identificationType.Name))
                    {
                        throw new PersistentException();
                    }
                    else
                    {
                        identificationType.Id = identificationTypes.Count + 1;
                        identificationTypes.Add(identificationType);
                        return identificationType;
                    }
                });
            mock.Setup(p => p.Update(It.IsAny<IdentificationType>(), It.IsAny<User>()))
                .Returns((IdentificationType identificationType, User user) =>
                {
                    identificationTypes.Where(x => x.Id == identificationType.Id).ToList().ForEach(x => x.Name = identificationType.Name);
                    return identificationType;
                });
            mock.Setup(p => p.Delete(It.IsAny<IdentificationType>(), It.IsAny<User>()))
                .Returns((IdentificationType identificationType, User user) =>
                {
                    identificationTypes = identificationTypes.Where(x => x.Id != identificationType.Id).ToList();
                    return identificationType;
                });
            _business = new(mock.Object);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Prueba la consulta de un listado de tipos de identificación con filtros, ordenamientos y límite
        /// </summary>
        [Fact]
        public void ListTest()
        {
            //Act
            ListResult<IdentificationType> list = _business.List("ididentificationtype = 1", "name", 1, 0);

            //Assert
            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de tipos de identificación con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void ListWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _business.List("idtipoidentificacion = 1", "name", 1, 0));
        }

        /// <summary>
        /// Prueba la consulta de un tipo de identificación dado su identificador
        /// </summary>
        [Fact]
        public void ReadTest()
        {
            //Arrange
            IdentificationType identificationType = new() { Id = 1 };

            //Act
            identificationType = _business.Read(identificationType);

            //Assert
            Assert.Equal("Cédula ciudadanía", identificationType.Name);
        }

        /// <summary>
        /// Prueba la consulta de un tipo de identificación que no existe dado su identificador
        /// </summary>
        [Fact]
        public void ReadNotFoundTest()
        {
            //Arrange
            IdentificationType identificationType = new() { Id = 10 };

            //Act
            identificationType = _business.Read(identificationType);

            //Assert
            Assert.Equal(0, identificationType.Id);
        }

        /// <summary>
        /// Prueba la inserción de un tipo de identificación
        /// </summary>
        [Fact]
        public void InsertTest()
        {
            //Arrange
            IdentificationType identificationType = new() { Name = "Prueba 1" };

            //Act
            identificationType = _business.Insert(identificationType, new() { Id = 1 });

            //Assert
            Assert.NotEqual(0, identificationType.Id);
        }

        /// <summary>
        /// Prueba la actualización de un tipo de identificación
        /// </summary>
        [Fact]
        public void UpdateTest()
        {
            //Arrange
            IdentificationType identificationType = new() { Id = 2, Name = "Tarjeta de identidad" };
            IdentificationType identificationType2 = new() { Id = 2 };

            //Act
            identificationType2 = _business.Read(identificationType2);
            _ = _business.Update(identificationType, new() { Id = 1 });

            //Assert
            Assert.NotEqual("Cédula extranjería", identificationType2.Name);
        }

        /// <summary>
        /// Prueba la eliminación de un tipo de identificación
        /// </summary>
        [Fact]
        public void DeleteTest()
        {
            //Arrange
            IdentificationType identificationType = new() { Id = 3 };
            IdentificationType identificationType2 = new() { Id = 3 };

            //Act
            _ = _business.Delete(identificationType, new() { Id = 1 });
            identificationType2 = _business.Read(identificationType2);

            //Assert
            Assert.Equal(0, identificationType2.Id);
        }
        #endregion
    }
}
