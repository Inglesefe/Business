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

        /// <summary>
        /// Conexión a la base de datos falsa
        /// </summary>
        private readonly IDbConnection connectionFake;
        #endregion

        #region Constructors
        /// <summary>
        /// Inicializa la configuración de la prueba
        /// </summary>
        public IdentificationTypeTest()
        {
            Mock<IPersistentWithLog<IdentificationType>> mock = new();
            Mock<IDbConnection> mockConnection = new();
            connectionFake = mockConnection.Object;

            List<IdentificationType> identificationTypes = new()
            {
                new IdentificationType() { Id = 1, Name = "Cédula ciudadanía" },
                new IdentificationType() { Id = 2, Name = "Cédula extranjería" },
                new IdentificationType() { Id = 3, Name = "Pasaporte" }
            };

            mock.Setup(p => p.List("ididentificationtype = 1", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<IDbConnection>()))
                .Returns(new ListResult<IdentificationType>(identificationTypes.Where(y => y.Id == 1).ToList(), 1));
            mock.Setup(p => p.List("idtipoidentificacion = 1", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<IDbConnection>()))
                .Throws<PersistentException>();

            mock.Setup(p => p.Read(It.IsAny<IdentificationType>(), It.IsAny<IDbConnection>()))
                .Returns((IdentificationType identificationType, IDbConnection connection) => identificationTypes.Find(x => x.Id == identificationType.Id) ?? new IdentificationType());

            mock.Setup(p => p.Insert(It.IsAny<IdentificationType>(), It.IsAny<User>(), It.IsAny<IDbConnection>()))
                .Returns((IdentificationType identificationType, User user, IDbConnection connection) =>
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

            mock.Setup(p => p.Update(It.IsAny<IdentificationType>(), It.IsAny<User>(), It.IsAny<IDbConnection>()))
                .Returns((IdentificationType identificationType, User user, IDbConnection connection) =>
                {
                    identificationTypes.Where(x => x.Id == identificationType.Id).ToList().ForEach(x => x.Name = identificationType.Name);
                    return identificationType;
                });

            mock.Setup(p => p.Delete(It.IsAny<IdentificationType>(), It.IsAny<User>(), It.IsAny<IDbConnection>()))
                .Returns((IdentificationType identificationType, User user, IDbConnection connection) =>
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
        public void IdentificationTypeListTest()
        {
            ListResult<IdentificationType> list = _business.List("ididentificationtype = 1", "name", 1, 0, connectionFake);

            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de tipos de identificación con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void IdentificationTypeListWithErrorTest()
        {
            Assert.Throws<PersistentException>(() => _business.List("idtipoidentificacion = 1", "name", 1, 0, connectionFake));
        }

        /// <summary>
        /// Prueba la consulta de un tipo de identificación dado su identificador
        /// </summary>
        [Fact]
        public void IdentificationTypeReadTest()
        {
            IdentificationType identificationType = new() { Id = 1 };
            identificationType = _business.Read(identificationType, connectionFake);

            Assert.Equal("Cédula ciudadanía", identificationType.Name);
        }

        /// <summary>
        /// Prueba la consulta de un tipo de identificación que no existe dado su identificador
        /// </summary>
        [Fact]
        public void IdentificationTypeReadNotFoundTest()
        {
            IdentificationType identificationType = new() { Id = 10 };
            identificationType = _business.Read(identificationType, connectionFake);

            Assert.Equal(0, identificationType.Id);
        }

        /// <summary>
        /// Prueba la inserción de un tipo de identificación
        /// </summary>
        [Fact]
        public void IdentificationTypeInsertTest()
        {
            IdentificationType identificationType = new() { Name = "Prueba 1" };
            identificationType = _business.Insert(identificationType, new() { Id = 1 }, connectionFake);

            Assert.NotEqual(0, identificationType.Id);
        }

        /// <summary>
        /// Prueba la actualización de un tipo de identificación
        /// </summary>
        [Fact]
        public void IdentificationTypeUpdateTest()
        {
            IdentificationType identificationType = new() { Id = 2, Name = "Tarjeta de identidad" };
            _ = _business.Update(identificationType, new() { Id = 1 }, connectionFake);

            IdentificationType identificationType2 = new() { Id = 2 };
            identificationType2 = _business.Read(identificationType2, connectionFake);

            Assert.NotEqual("Cédula extranjería", identificationType2.Name);
        }

        /// <summary>
        /// Prueba la eliminación de un tipo de identificación
        /// </summary>
        [Fact]
        public void IdentificationTypeDeleteTest()
        {
            IdentificationType identificationType = new() { Id = 3 };
            _ = _business.Delete(identificationType, new() { Id = 1 }, connectionFake);

            IdentificationType identificationType2 = new() { Id = 3 };
            identificationType2 = _business.Read(identificationType2, connectionFake);

            Assert.Equal(0, identificationType2.Id);
        }
        #endregion
    }
}
