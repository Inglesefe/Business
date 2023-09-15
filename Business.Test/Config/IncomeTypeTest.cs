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
    /// Realiza las pruebas sobre la clase de persistencia de tipos de ingreso
    /// </summary>
    [Collection("Tests")]
    public class IncomeTypeTest
    {
        #region Attributes
        /// <summary>
        /// Capa de negocio de los tipos de ingreso
        /// </summary>
        private readonly BusinessIncomeType _business;
        #endregion

        #region Constructors
        /// <summary>
        /// Inicializa la configuración de la prueba
        /// </summary>
        public IncomeTypeTest()
        {
            //Arrange
            Mock<IPersistentWithLog<IncomeType>> mock = new();
            List<IncomeType> incomeTypes = new()
            {
                new IncomeType() { Id = 1, Code = "CI", Name = "Cuota inicial" },
                new IncomeType() { Id = 2, Code = "CR", Name = "Crédito cartera" },
                new IncomeType() { Id = 3, Code = "FC", Name = "Factura" }
            };
            mock.Setup(p => p.List("idincometype = 1", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new ListResult<IncomeType>(incomeTypes.Where(y => y.Id == 1).ToList(), 1));
            mock.Setup(p => p.List("idtipoingreso = 1", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Throws<PersistentException>();
            mock.Setup(p => p.Read(It.IsAny<IncomeType>()))
                .Returns((IncomeType incomeType) => incomeTypes.Find(x => x.Id == incomeType.Id) ?? new IncomeType());
            mock.Setup(p => p.Insert(It.IsAny<IncomeType>(), It.IsAny<User>()))
                .Returns((IncomeType incomeType, User user) =>
                {
                    if (incomeTypes.Exists(x => x.Code == incomeType.Code))
                    {
                        throw new PersistentException();
                    }
                    else
                    {
                        incomeType.Id = incomeTypes.Count + 1;
                        incomeTypes.Add(incomeType);
                        return incomeType;
                    }
                });
            mock.Setup(p => p.Update(It.IsAny<IncomeType>(), It.IsAny<User>()))
                .Returns((IncomeType incomeType, User user) =>
                {
                    incomeTypes.Where(x => x.Id == incomeType.Id).ToList().ForEach(x => x.Name = incomeType.Name);
                    return incomeType;
                });
            mock.Setup(p => p.Delete(It.IsAny<IncomeType>(), It.IsAny<User>()))
                .Returns((IncomeType incomeType, User user) =>
                {
                    incomeTypes = incomeTypes.Where(x => x.Id != incomeType.Id).ToList();
                    return incomeType;
                });
            _business = new(mock.Object);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Prueba la consulta de un listado de tipos de ingreso con filtros, ordenamientos y límite
        /// </summary>
        [Fact]
        public void ListTest()
        {
            //Act
            ListResult<IncomeType> list = _business.List("idincometype = 1", "name", 1, 0);

            //Assert
            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de tipos de ingreso con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void ListWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _business.List("idtipoingreso = 1", "name", 1, 0));
        }

        /// <summary>
        /// Prueba la consulta de un tipo de ingreso dado su identificador
        /// </summary>
        [Fact]
        public void ReadTest()
        {
            //Arrange
            IncomeType incomeType = new() { Id = 1 };

            //Act
            incomeType = _business.Read(incomeType);

            //Assert
            Assert.Equal("CI", incomeType.Code);
        }

        /// <summary>
        /// Prueba la consulta de un tipo de ingreso que no existe dado su identificador
        /// </summary>
        [Fact]
        public void ReadNotFoundTest()
        {
            //Arrange
            IncomeType incomeType = new() { Id = 10 };

            //Act
            incomeType = _business.Read(incomeType);

            //Assert
            Assert.Equal(0, incomeType.Id);
        }

        /// <summary>
        /// Prueba la inserción de un tipo de ingreso
        /// </summary>
        [Fact]
        public void InsertTest()
        {
            //Arrange
            IncomeType incomeType = new() { Code = "CF", Name = "Cheques posfechados" };

            //Act
            incomeType = _business.Insert(incomeType, new() { Id = 1 });

            //Assert
            Assert.NotEqual(0, incomeType.Id);
        }

        /// <summary>
        /// Prueba la actualización de un tipo de ingreso
        /// </summary>
        [Fact]
        public void UpdateTest()
        {
            //Arrange
            IncomeType incomeType = new() { Id = 2, Code = "CT", Name = "Otro ingreso" };
            IncomeType incomeType2 = new() { Id = 2 };

            //Act
            _ = _business.Update(incomeType, new() { Id = 1 });
            incomeType2 = _business.Read(incomeType2);

            //Assert
            Assert.NotEqual("Credito cartera", incomeType2.Name);
        }

        /// <summary>
        /// Prueba la eliminación de un tipo de ingreso
        /// </summary>
        [Fact]
        public void DeleteTest()
        {
            //Arrange
            IncomeType incomeType = new() { Id = 3 };
            IncomeType incomeType2 = new() { Id = 3 };

            //Act
            _ = _business.Delete(incomeType, new() { Id = 1 });
            incomeType2 = _business.Read(incomeType2);

            //Assert
            Assert.Equal(0, incomeType2.Id);
        }
        #endregion
    }
}
