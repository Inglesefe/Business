using Business.Cont;
using Dal;
using Dal.Dto;
using Dal.Exceptions;
using Entities.Auth;
using Entities.Cont;
using Moq;
using System.Data;

namespace Business.Test.Cont
{
    /// <summary>
    /// Realiza las pruebas sobre la clase de persistencia de tipos de cuenta
    /// </summary>
    [Collection("Tests")]
    public class AccountTypeTest
    {
        #region Attributes
        /// <summary>
        /// Capa de negocio de los tipos de cuenta
        /// </summary>
        private readonly BusinessAccountType _business;
        #endregion

        #region Constructors
        /// <summary>
        /// Inicializa la configuración de la prueba
        /// </summary>
        public AccountTypeTest()
        {
            //Arrange
            Mock<IPersistentWithLog<AccountType>> mock = new();
            List<AccountType> types = new()
            {
                new AccountType() { Id = 1, Name = "Caja" },
                new AccountType() { Id = 2, Name = "Bancos" },
                new AccountType() { Id = 3, Name = "Otra" }
            };
            mock.Setup(p => p.List("idaccounttype = 1", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new ListResult<AccountType>(types.Where(y => y.Id == 1).ToList(), 1));
            mock.Setup(p => p.List("idtipocuenta = 1", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Throws<PersistentException>();
            mock.Setup(p => p.Read(It.IsAny<AccountType>()))
                .Returns((AccountType type) => types.Find(x => x.Id == type.Id) ?? new AccountType());
            mock.Setup(p => p.Insert(It.IsAny<AccountType>(), It.IsAny<User>()))
                .Returns((AccountType type, User user) =>
                {
                    type.Id = types.Count + 1;
                    types.Add(type);
                    return type;
                });
            mock.Setup(p => p.Update(It.IsAny<AccountType>(), It.IsAny<User>()))
                .Returns((AccountType type, User user) =>
                {
                    types.Where(x => x.Id == type.Id).ToList().ForEach(x =>
                    {
                        x.Name = type.Name;
                    });
                    return type;
                });
            mock.Setup(p => p.Delete(It.IsAny<AccountType>(), It.IsAny<User>()))
                .Returns((AccountType type, User user) =>
                {
                    types = types.Where(x => x.Id != type.Id).ToList();
                    return type;
                });
            _business = new(mock.Object);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Prueba la consulta de un listado de tipos de cuenta con filtros, ordenamientos y límite
        /// </summary>
        [Fact]
        public void ListTest()
        {
            //Act
            ListResult<AccountType> list = _business.List("idaccounttype = 1", "value", 1, 0);

            //Assert
            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de tipos de cuenta con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void ListWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _business.List("idtipocuenta = 1", "value", 1, 0));
        }

        /// <summary>
        /// Prueba la consulta de un tipo de cuenta dado su identificador
        /// </summary>
        [Fact]
        public void ReadTest()
        {
            //Arrange
            AccountType type = new() { Id = 1 };

            //Act
            type = _business.Read(type);

            //Assert
            Assert.Equal("Caja", type.Name);
        }

        /// <summary>
        /// Prueba la consulta de un tipo de cuenta que no existe dado su identificador
        /// </summary>
        [Fact]
        public void ReadNotFoundTest()
        {
            //Arrange
            AccountType type = new() { Id = 10 };

            //Act
            type = _business.Read(type);

            //Assert
            Assert.Equal(0, type.Id);
        }

        /// <summary>
        /// Prueba la inserción de un tipo de cuenta
        /// </summary>
        [Fact]
        public void InsertTest()
        {
            //Arrange
            AccountType type = new() { Name = "Nueva" };

            //Act
            type = _business.Insert(type, new() { Id = 1 });

            //Assert
            Assert.NotEqual(0, type.Id);
        }

        /// <summary>
        /// Prueba la actualización de un tipo de cuenta
        /// </summary>
        [Fact]
        public void UpdateTest()
        {
            //Arrange
            AccountType type = new() { Id = 2, Name = "Actualizado" };
            AccountType type2 = new() { Id = 2 };

            //Act
            _ = _business.Update(type, new() { Id = 1 });
            type2 = _business.Read(type2);

            //Assert
            Assert.NotEqual("Bancos", type2.Name);
        }

        /// <summary>
        /// Prueba la eliminación de un tipo de cuenta
        /// </summary>
        [Fact]
        public void DeleteTest()
        {
            //Arrange
            AccountType type = new() { Id = 3 };
            AccountType type2 = new() { Id = 3 };

            //Act
            _ = _business.Delete(type, new() { Id = 1 });
            type2 = _business.Read(type2);

            //Assert
            Assert.Equal(0, type2.Id);
        }
        #endregion
    }
}
