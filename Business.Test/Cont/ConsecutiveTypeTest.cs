using Business.Cont;
using Dal;
using Dal.Dto;
using Dal.Exceptions;
using Entities.Auth;
using Entities.Config;
using Moq;
using System.Data;

namespace Business.Test.Cont
{
    /// <summary>
    /// Realiza las pruebas sobre la clase de persistencia de tipos de consecutivo
    /// </summary>
    [Collection("Tests")]
    public class ConsecutiveTypeTest
    {
        #region Attributes
        /// <summary>
        /// Capa de negocio de los tipos de consecutivo
        /// </summary>
        private readonly BusinessConsecutiveType _business;
        #endregion

        #region Constructors
        /// <summary>
        /// Inicializa la configuración de la prueba
        /// </summary>
        public ConsecutiveTypeTest()
        {
            //Arrange
            Mock<IPersistentWithLog<ConsecutiveType>> mock = new();
            List<ConsecutiveType> types = new()
            {
                new ConsecutiveType() { Id = 1, Name = "Recibos de caja" },
                new ConsecutiveType() { Id = 2, Name = "RRegistro oficial" },
                new ConsecutiveType() { Id = 3, Name = "Otro registro" }
            };
            mock.Setup(p => p.List("idconsecutivetype = 1", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new ListResult<ConsecutiveType>(types.Where(y => y.Id == 1).ToList(), 1));
            mock.Setup(p => p.List("idtipoconsecutivo = 1", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Throws<PersistentException>();
            mock.Setup(p => p.Read(It.IsAny<ConsecutiveType>()))
                .Returns((ConsecutiveType type) => types.Find(x => x.Id == type.Id) ?? new ConsecutiveType());
            mock.Setup(p => p.Insert(It.IsAny<ConsecutiveType>(), It.IsAny<User>()))
                .Returns((ConsecutiveType type, User user) =>
                {
                    type.Id = types.Count + 1;
                    types.Add(type);
                    return type;
                });
            mock.Setup(p => p.Update(It.IsAny<ConsecutiveType>(), It.IsAny<User>()))
                .Returns((ConsecutiveType type, User user) =>
                {
                    types.Where(x => x.Id == type.Id).ToList().ForEach(x =>
                    {
                        x.Name = type.Name;
                    });
                    return type;
                });
            mock.Setup(p => p.Delete(It.IsAny<ConsecutiveType>(), It.IsAny<User>()))
                .Returns((ConsecutiveType type, User user) =>
                {
                    types = types.Where(x => x.Id != type.Id).ToList();
                    return type;
                });
            _business = new(mock.Object);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Prueba la consulta de un listado de tipos de consecutivo con filtros, ordenamientos y límite
        /// </summary>
        [Fact]
        public void ListTest()
        {
            //Act
            ListResult<ConsecutiveType> list = _business.List("idconsecutivetype = 1", "value", 1, 0);

            //Assert
            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de tipos de consecutivo con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void ListWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _business.List("idtipoconsecutivo = 1", "value", 1, 0));
        }

        /// <summary>
        /// Prueba la consulta de un tipo de consecutivo dado su identificador
        /// </summary>
        [Fact]
        public void ReadTest()
        {
            //Arrange
            ConsecutiveType type = new() { Id = 1 };

            //Act
            type = _business.Read(type);

            //Assert
            Assert.Equal("Recibos de caja", type.Name);
        }

        /// <summary>
        /// Prueba la consulta de un tipo de consecutivo que no existe dado su identificador
        /// </summary>
        [Fact]
        public void ReadNotFoundTest()
        {
            //Arrange
            ConsecutiveType type = new() { Id = 10 };

            //Act
            type = _business.Read(type);

            //Assert
            Assert.Equal(0, type.Id);
        }

        /// <summary>
        /// Prueba la inserción de un tipo de consecutivo
        /// </summary>
        [Fact]
        public void InsertTest()
        {
            //Arrange
            ConsecutiveType type = new() { Name = "Nueva" };

            //Act
            type = _business.Insert(type, new() { Id = 1 });

            //Assert
            Assert.NotEqual(0, type.Id);
        }

        /// <summary>
        /// Prueba la actualización de un tipo de consecutivo
        /// </summary>
        [Fact]
        public void UpdateTest()
        {
            //Arrange
            ConsecutiveType type = new() { Id = 2, Name = "Actualizado" };
            ConsecutiveType type2 = new() { Id = 2 };

            //Act
            _ = _business.Update(type, new() { Id = 1 });
            type2 = _business.Read(type2);

            //Assert
            Assert.NotEqual("Registro oficial", type2.Name);
        }

        /// <summary>
        /// Prueba la eliminación de un tipo de consecutivo
        /// </summary>
        [Fact]
        public void DeleteTest()
        {
            //Arrange
            ConsecutiveType type = new() { Id = 3 };
            ConsecutiveType type2 = new() { Id = 3 };

            //Act
            _ = _business.Delete(type, new() { Id = 1 });
            type2 = _business.Read(type2);

            //Assert
            Assert.Equal(0, type2.Id);
        }
        #endregion
    }
}
