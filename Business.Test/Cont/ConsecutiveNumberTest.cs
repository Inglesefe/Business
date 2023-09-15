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
    /// Realiza las pruebas sobre la clase de persistencia de números de consecutivos
    /// </summary>
    [Collection("Tests")]
    public class ConsecutiveNumberTest
    {
        #region Attributes
        /// <summary>
        /// Capa de negocio de los números de consecutivos
        /// </summary>
        private readonly BusinessConsecutiveNumber _business;
        #endregion

        #region Constructors
        /// <summary>
        /// Inicializa la configuración de la prueba
        /// </summary>
        public ConsecutiveNumberTest()
        {
            //Arrange
            Mock<IPersistentWithLog<ConsecutiveNumber>> mock = new();
            List<ConsecutiveNumber> numbers = new()
            {
                new ConsecutiveNumber() { Id = 1, ConsecutiveType = new(){ Id = 1 }, City = new(){ Id = 1 }, Number = "100" },
                new ConsecutiveNumber() { Id = 2, ConsecutiveType = new(){ Id = 1 }, City = new(){ Id = 1 }, Number = "200" },
                new ConsecutiveNumber() { Id = 3, ConsecutiveType = new(){ Id = 1 }, City = new(){ Id = 1 }, Number = "300" }
            };
            mock.Setup(p => p.List("idconsecutivenumber = 1", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new ListResult<ConsecutiveNumber>(numbers.Where(y => y.Id == 1).ToList(), 1));
            mock.Setup(p => p.List("idnumeroconsecutivo = 1", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Throws<PersistentException>();
            mock.Setup(p => p.Read(It.IsAny<ConsecutiveNumber>()))
                .Returns((ConsecutiveNumber number) => numbers.Find(x => x.Id == number.Id) ?? new ConsecutiveNumber());
            mock.Setup(p => p.Insert(It.IsAny<ConsecutiveNumber>(), It.IsAny<User>()))
                .Returns((ConsecutiveNumber number, User user) =>
                {
                    number.Id = numbers.Count + 1;
                    numbers.Add(number);
                    return number;
                });
            mock.Setup(p => p.Update(It.IsAny<ConsecutiveNumber>(), It.IsAny<User>()))
                .Returns((ConsecutiveNumber number, User user) =>
                {
                    numbers.Where(x => x.Id == number.Id).ToList().ForEach(x =>
                    {
                        x.ConsecutiveType = number.ConsecutiveType;
                        x.City = number.City;
                        x.Number = number.Number;
                    });
                    return number;
                });
            mock.Setup(p => p.Delete(It.IsAny<ConsecutiveNumber>(), It.IsAny<User>()))
                .Returns((ConsecutiveNumber number, User user) =>
                {
                    numbers = numbers.Where(x => x.Id != number.Id).ToList();
                    return number;
                });
            _business = new(mock.Object);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Prueba la consulta de un listado de números de consecutivos con filtros, ordenamientos y límite
        /// </summary>
        [Fact]
        public void ListTest()
        {
            //Act
            ListResult<ConsecutiveNumber> list = _business.List("idconsecutivenumber = 1", "value", 1, 0);

            //Assert
            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de números de consecutivos con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void ListWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _business.List("idnumeroconsecutivo = 1", "value", 1, 0));
        }

        /// <summary>
        /// Prueba la consulta de un número de consecutivo dado su identificador
        /// </summary>
        [Fact]
        public void ReadTest()
        {
            //Arrange
            ConsecutiveNumber number = new() { Id = 1 };

            //Act
            number = _business.Read(number);

            //Assert
            Assert.Equal("100", number.Number);
        }

        /// <summary>
        /// Prueba la consulta de un número de consecutivo que no existe dado su identificador
        /// </summary>
        [Fact]
        public void ReadNotFoundTest()
        {
            //Arrange
            ConsecutiveNumber number = new() { Id = 10 };

            //Act
            number = _business.Read(number);

            //Assert
            Assert.Equal(0, number.Id);
        }

        /// <summary>
        /// Prueba la inserción de un número de consecutivo
        /// </summary>
        [Fact]
        public void InsertTest()
        {
            //Arrange
            ConsecutiveNumber number = new() { ConsecutiveType = new() { Id = 1 }, City = new() { Id = 1 }, Number = "400" };

            //Act
            number = _business.Insert(number, new() { Id = 1 });

            //Assert
            Assert.NotEqual(0, number.Id);
        }

        /// <summary>
        /// Prueba la actualización de un número de consecutivo
        /// </summary>
        [Fact]
        public void UpdateTest()
        {
            //Arrange
            ConsecutiveNumber number = new() { Id = 2, ConsecutiveType = new() { Id = 1 }, City = new() { Id = 1 }, Number = "500" };
            ConsecutiveNumber number2 = new() { Id = 2 };

            //Act
            _ = _business.Update(number, new() { Id = 1 });
            number2 = _business.Read(number2);

            //Assert
            Assert.NotEqual("200", number2.Number);
        }

        /// <summary>
        /// Prueba la eliminación de un número de consecutivo
        /// </summary>
        [Fact]
        public void DeleteTest()
        {
            //Arrange
            ConsecutiveNumber number = new() { Id = 3 };
            ConsecutiveNumber number2 = new() { Id = 3 };

            //Act
            _ = _business.Delete(number, new() { Id = 1 });
            number2 = _business.Read(number2);

            //Assert
            Assert.Equal(0, number2.Id);
        }
        #endregion
    }
}
