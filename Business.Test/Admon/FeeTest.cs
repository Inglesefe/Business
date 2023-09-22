using Business.Admon;
using Dal;
using Dal.Dto;
using Dal.Exceptions;
using Entities.Admon;
using Entities.Auth;
using Moq;
using System.Data;

namespace Business.Test.Admon
{
    /// <summary>
    /// Realiza las pruebas sobre la clase de persistencia de cuotas de matrículas
    /// </summary>
    [Collection("Tests")]
    public class FeeTest
    {
        #region Attributes
        /// <summary>
        /// Capa de negocio de las cuotas de matrículas
        /// </summary>
        private readonly BusinessFee _business;
        #endregion

        #region Constructors
        /// <summary>
        /// Inicializa la configuración de la prueba
        /// </summary>
        public FeeTest()
        {
            //Arrange
            Mock<IPersistentWithLog<Fee>> mock = new();
            List<Fee> fees = new()
            {
                new Fee() { Id = 1, Registration = new() { Id = 1 }, Value = 1000, Number = 1, IncomeType = new(){ Id = 1}, DueDate = DateTime.Now },
                new Fee() { Id = 1, Registration = new() { Id = 2 }, Value = 2000, Number = 2, IncomeType = new(){ Id = 1}, DueDate = DateTime.Now },
                new Fee() { Id = 1, Registration = new() { Id = 3 }, Value = 3000, Number = 3, IncomeType = new(){ Id = 1}, DueDate = DateTime.Now }
            };
            mock.Setup(p => p.List("idfee = 1", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new ListResult<Fee>(fees.Where(y => y.Id == 1).ToList(), 1));
            mock.Setup(p => p.List("idcuota = 1", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Throws<PersistentException>();
            mock.Setup(p => p.Read(It.IsAny<Fee>()))
                .Returns((Fee fee) => fees.Find(x => x.Id == fee.Id) ?? new Fee());
            mock.Setup(p => p.Insert(It.IsAny<Fee>(), It.IsAny<User>()))
                .Returns((Fee fee, User user) =>
                {
                    if (fees.Exists(x => x.Registration.Id == fee.Registration.Id && x.Number == fee.Number && x.IncomeType.Id == fee.IncomeType.Id))
                    {
                        throw new PersistentException();
                    }
                    fee.Id = fees.Count + 1;
                    fees.Add(fee);
                    return fee;
                });
            mock.Setup(p => p.Update(It.IsAny<Fee>(), It.IsAny<User>()))
                .Returns((Fee fee, User user) =>
                {
                    fees.Where(x => x.Id == fee.Id).ToList().ForEach(x =>
                    {
                        x.Registration = fee.Registration;
                        x.Value = fee.Value;
                        x.Number = fee.Number;
                        x.IncomeType = fee.IncomeType;
                        x.DueDate = fee.DueDate;
                    });
                    return fee;
                });
            mock.Setup(p => p.Delete(It.IsAny<Fee>(), It.IsAny<User>()))
                .Returns((Fee fee, User user) =>
                {
                    fees = fees.Where(x => x.Id != fee.Id).ToList();
                    return fee;
                });
            _business = new(mock.Object);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Prueba la consulta de un listado de cuotas de matrículas con filtros, ordenamientos y límite
        /// </summary>
        [Fact]
        public void ListTest()
        {
            //Act
            ListResult<Fee> list = _business.List("idaccountfee = 1", "name", 1, 0);

            //Assert
            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de cuotas de matrículas con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void ListWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _business.List("idejecutivocuenta = 1", "name", 1, 0));
        }

        /// <summary>
        /// Prueba la consulta de una cuota de matrícula dada su identificador
        /// </summary>
        [Fact]
        public void ReadTest()
        {
            //Arrange
            Fee fee = new() { Id = 1 };

            //Act
            fee = _business.Read(fee);

            //Assert
            Assert.Equal(100, fee.Value);
        }

        /// <summary>
        /// Prueba la consulta de una cuota de matrícula que no existe dado su identificador
        /// </summary>
        [Fact]
        public void ReadNotFoundTest()
        {
            //Arrange
            Fee fee = new() { Id = 10 };

            //Act
            fee = _business.Read(fee);

            //Assert
            Assert.Equal(0, fee.Id);
        }

        /// <summary>
        /// Prueba la inserción de una cuota de matrícula
        /// </summary>
        [Fact]
        public void InsertTest()
        {
            //Arrange
            Fee fee = new() { Registration = new() { Id = 1 }, Value = 4000, Number = 4, IncomeType = new() { Id = 1 }, DueDate = DateTime.Now };

            //Act
            fee = _business.Insert(fee, new() { Id = 1 });

            //Assert
            Assert.NotEqual(0, fee.Id);
        }

        /// <summary>
        /// Prueba la inserción de una cuota de matrícula con identificación duplicada
        /// </summary>
        [Fact]
        public void InsertDuplicateTest()
        {
            //Arrange
            Fee fee = new() { Registration = new() { Id = 1 }, Value = 6000, Number = 1, IncomeType = new() { Id = 1 }, DueDate = DateTime.Now };

            //Act, Assert
            _ = Assert.Throws<PersistentException>(() => _business.Insert(fee, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la actualización de una cuota de matrícula
        /// </summary>
        [Fact]
        public void UpdateTest()
        {
            //Arrange
            Fee fee = new() { Id = 2, Registration = new() { Id = 1 }, Value = 5000, Number = 2, IncomeType = new() { Id = 1 }, DueDate = DateTime.Now };
            Fee fee2 = new() { Id = 2 };

            //Act
            _ = _business.Update(fee, new() { Id = 1 });
            fee2 = _business.Read(fee2);

            //Assert
            Assert.NotEqual(2000, fee2.Value);
        }

        /// <summary>
        /// Prueba la eliminación de una cuota de matrícula
        /// </summary>
        [Fact]
        public void DeleteTest()
        {
            //Arrange
            Fee fee = new() { Id = 3 };
            Fee fee2 = new() { Id = 3 };

            //Act
            _ = _business.Delete(fee, new() { Id = 1 });
            fee2 = _business.Read(fee2);

            //Assert
            Assert.Equal(0, fee2.Id);
        }
        #endregion
    }
}
