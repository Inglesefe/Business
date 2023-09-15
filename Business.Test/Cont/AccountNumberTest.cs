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
    /// Realiza las pruebas sobre la clase de persistencia de números de cuenta
    /// </summary>
    [Collection("Tests")]
    public class AccountNumberTest
    {
        #region Attributes
        /// <summary>
        /// Capa de negocio de los números de cuenta
        /// </summary>
        private readonly BusinessAccountNumber _business;
        #endregion

        #region Constructors
        /// <summary>
        /// Inicializa la configuración de la prueba
        /// </summary>
        public AccountNumberTest()
        {
            //Arrange
            Mock<IPersistentWithLog<AccountNumber>> mock = new();
            List<AccountNumber> numbers = new()
            {
                new AccountNumber() { Id = 1, AccountType = new(){ Id = 1 }, City = new(){ Id = 1 }, Number = "123456789" },
                new AccountNumber() { Id = 2, AccountType = new(){ Id = 1 }, City = new(){ Id = 1 }, Number = "987654321" },
                new AccountNumber() { Id = 3, AccountType = new(){ Id = 1 }, City = new(){ Id = 1 }, Number = "147258369" }
            };
            mock.Setup(p => p.List("idaccountnumber = 1", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new ListResult<AccountNumber>(numbers.Where(y => y.Id == 1).ToList(), 1));
            mock.Setup(p => p.List("idnumerocuenta = 1", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Throws<PersistentException>();
            mock.Setup(p => p.Read(It.IsAny<AccountNumber>()))
                .Returns((AccountNumber number) => numbers.Find(x => x.Id == number.Id) ?? new AccountNumber());
            mock.Setup(p => p.Insert(It.IsAny<AccountNumber>(), It.IsAny<User>()))
                .Returns((AccountNumber number, User user) =>
                {
                    number.Id = numbers.Count + 1;
                    numbers.Add(number);
                    return number;
                });
            mock.Setup(p => p.Update(It.IsAny<AccountNumber>(), It.IsAny<User>()))
                .Returns((AccountNumber number, User user) =>
                {
                    numbers.Where(x => x.Id == number.Id).ToList().ForEach(x =>
                    {
                        x.AccountType = number.AccountType;
                        x.City = number.City;
                        x.Number = number.Number;
                    });
                    return number;
                });
            mock.Setup(p => p.Delete(It.IsAny<AccountNumber>(), It.IsAny<User>()))
                .Returns((AccountNumber number, User user) =>
                {
                    numbers = numbers.Where(x => x.Id != number.Id).ToList();
                    return number;
                });
            _business = new(mock.Object);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Prueba la consulta de un listado de números de cuenta con filtros, ordenamientos y límite
        /// </summary>
        [Fact]
        public void ListTest()
        {
            //Act
            ListResult<AccountNumber> list = _business.List("idaccountnumber = 1", "value", 1, 0);

            //Assert
            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de números de cuenta con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void ListWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _business.List("idnumerocuenta = 1", "value", 1, 0));
        }

        /// <summary>
        /// Prueba la consulta de un número de cuenta dado su identificador
        /// </summary>
        [Fact]
        public void ReadTest()
        {
            //Arrange
            AccountNumber number = new() { Id = 1 };

            //Act
            number = _business.Read(number);

            //Assert
            Assert.Equal("123456789", number.Number);
        }

        /// <summary>
        /// Prueba la consulta de un número de cuenta que no existe dado su identificador
        /// </summary>
        [Fact]
        public void ReadNotFoundTest()
        {
            //Arrange
            AccountNumber number = new() { Id = 10 };

            //Act
            number = _business.Read(number);

            //Assert
            Assert.Equal(0, number.Id);
        }

        /// <summary>
        /// Prueba la inserción de un número de cuenta
        /// </summary>
        [Fact]
        public void InsertTest()
        {
            //Arrange
            AccountNumber number = new() { AccountType = new() { Id = 1 }, City = new() { Id = 1 }, Number = "963258741" };

            //Act
            number = _business.Insert(number, new() { Id = 1 });

            //Assert
            Assert.NotEqual(0, number.Id);
        }

        /// <summary>
        /// Prueba la actualización de un número de cuenta
        /// </summary>
        [Fact]
        public void UpdateTest()
        {
            //Arrange
            AccountNumber number = new() { Id = 2, AccountType = new() { Id = 1 }, City = new() { Id = 1 }, Number = "741258963" };
            AccountNumber number2 = new() { Id = 2 };

            //Act
            _ = _business.Update(number, new() { Id = 1 });
            number2 = _business.Read(number2);

            //Assert
            Assert.NotEqual("987654321", number2.Number);
        }

        /// <summary>
        /// Prueba la eliminación de un número de cuenta
        /// </summary>
        [Fact]
        public void DeleteTest()
        {
            //Arrange
            AccountNumber number = new() { Id = 3 };
            AccountNumber number2 = new() { Id = 3 };

            //Act
            _ = _business.Delete(number, new() { Id = 1 });
            number2 = _business.Read(number2);

            //Assert
            Assert.Equal(0, number2.Id);
        }
        #endregion
    }
}
