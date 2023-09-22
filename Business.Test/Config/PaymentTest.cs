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
    /// Realiza las pruebas sobre la clase de persistencia de tipos de pago
    /// </summary>
    [Collection("Tests")]
    public class PaymentTypeTest
    {
        #region Attributes
        /// <summary>
        /// Capa de negocio de los tipos de pago
        /// </summary>
        private readonly BusinessPaymentType _business;
        #endregion

        #region Constructors
        /// <summary>
        /// Inicializa la configuración de la prueba
        /// </summary>
        public PaymentTypeTest()
        {
            //Arrange
            Mock<IPersistentWithLog<PaymentType>> mock = new();
            List<PaymentType> parameters = new()
            {
                new PaymentType() { Id = 1, Name = "Efectivo" },
                new PaymentType() { Id = 2, Name = "Nequi" },
                new PaymentType() { Id = 3, Name = "PSE" }
            };
            mock.Setup(p => p.List("idpaymenttype = 1", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new ListResult<PaymentType>(parameters.Where(y => y.Id == 1).ToList(), 1));
            mock.Setup(p => p.List("idtipopago = 1", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Throws<PersistentException>();
            mock.Setup(p => p.Read(It.IsAny<PaymentType>()))
                .Returns((PaymentType parameter) => parameters.Find(x => x.Id == parameter.Id) ?? new PaymentType());
            mock.Setup(p => p.Insert(It.IsAny<PaymentType>(), It.IsAny<User>()))
                .Returns((PaymentType parameter, User user) =>
                {
                    if (parameters.Exists(x => x.Name == parameter.Name))
                    {
                        throw new PersistentException();
                    }
                    else
                    {
                        parameter.Id = parameters.Count + 1;
                        parameters.Add(parameter);
                        return parameter;
                    }
                });
            mock.Setup(p => p.Update(It.IsAny<PaymentType>(), It.IsAny<User>()))
                .Returns((PaymentType parameter, User user) =>
                {
                    parameters.Where(x => x.Id == parameter.Id).ToList().ForEach(x => x.Name = parameter.Name);
                    return parameter;
                });
            mock.Setup(p => p.Delete(It.IsAny<PaymentType>(), It.IsAny<User>()))
                .Returns((PaymentType parameter, User user) =>
                {
                    parameters = parameters.Where(x => x.Id != parameter.Id).ToList();
                    return parameter;
                });
            _business = new(mock.Object);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Prueba la consulta de un listado de tipos de pago con filtros, ordenamientos y límite
        /// </summary>
        [Fact]
        public void ListTest()
        {
            //Act
            ListResult<PaymentType> list = _business.List("idpaymenttype = 1", "name", 1, 0);

            //Assert
            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de tipos de pago con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void ListWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _business.List("idtipopago = 1", "name", 1, 0));
        }

        /// <summary>
        /// Prueba la consulta de un tipo de pago dado su identificador
        /// </summary>
        [Fact]
        public void ReadTest()
        {
            //Arrange
            PaymentType parameter = new() { Id = 1 };

            //Act
            parameter = _business.Read(parameter);

            //Assert
            Assert.Equal("Efectivo", parameter.Name);
        }

        /// <summary>
        /// Prueba la consulta de un tipo de pago que no existe dado su identificador
        /// </summary>
        [Fact]
        public void ReadNotFoundTest()
        {
            //Arrange
            PaymentType parameter = new() { Id = 10 };

            //Act
            parameter = _business.Read(parameter);

            //Assert
            Assert.Equal(0, parameter.Id);
        }

        /// <summary>
        /// Prueba la inserción de un tipo de pago
        /// </summary>
        [Fact]
        public void InsertTest()
        {
            //Arrange
            PaymentType parameter = new() { Name = "Daviplata" };

            //Act
            parameter = _business.Insert(parameter, new() { Id = 1 });

            //Assert
            Assert.NotEqual(0, parameter.Id);
        }

        /// <summary>
        /// Prueba la actualización de un tipo de pago
        /// </summary>
        [Fact]
        public void UpdateTest()
        {
            //Arrange
            PaymentType parameter = new() { Id = 2, Name = "Visa" };
            PaymentType parameter2 = new() { Id = 2 };

            //Act
            _ = _business.Update(parameter, new() { Id = 1 });
            parameter2 = _business.Read(parameter2);

            //Assert
            Assert.NotEqual("Nequi", parameter2.Name);
        }

        /// <summary>
        /// Prueba la eliminación de un tipo de pago
        /// </summary>
        [Fact]
        public void DeleteTest()
        {
            //Arrange
            PaymentType parameter = new() { Id = 3 };
            PaymentType parameter2 = new() { Id = 3 };

            //Act
            _ = _business.Delete(parameter, new() { Id = 1 });
            parameter2 = _business.Read(parameter2);

            //Assert
            Assert.Equal(0, parameter2.Id);
        }
        #endregion
    }
}
