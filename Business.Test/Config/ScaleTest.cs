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
    /// Realiza las pruebas sobre la clase de persistencia de escalas
    /// </summary>
    [Collection("Tests")]
    public class ScaleTest
    {
        #region Attributes
        /// <summary>
        /// Capa de negocio de los escalas
        /// </summary>
        private readonly BusinessScale _business;
        #endregion

        #region Constructors
        /// <summary>
        /// Inicializa la configuración de la prueba
        /// </summary>
        public ScaleTest()
        {
            //Arrange
            Mock<IPersistentWithLog<Scale>> mock = new();
            List<Scale> scales = new()
            {
                new Scale() { Id = 1, Name = "Comisión 1", Comission = 1000, Validity = DateTime.Now },
                new Scale() { Id = 2, Name = "Comisión 2", Comission = 2000, Validity = DateTime.Now },
                new Scale() { Id = 3, Name = "Comisión 3", Comission = 3000, Validity = DateTime.Now }
            };
            mock.Setup(p => p.List("idscale = 1", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new ListResult<Scale>(scales.Where(y => y.Id == 1).ToList(), 1));
            mock.Setup(p => p.List("idescala = 1", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Throws<PersistentException>();
            mock.Setup(p => p.Read(It.IsAny<Scale>()))
                .Returns((Scale scale) => scales.Find(x => x.Id == scale.Id) ?? new Scale());
            mock.Setup(p => p.Insert(It.IsAny<Scale>(), It.IsAny<User>()))
                .Returns((Scale scale, User user) =>
                {
                    scale.Id = scales.Count + 1;
                    scales.Add(scale);
                    return scale;
                });
            mock.Setup(p => p.Update(It.IsAny<Scale>(), It.IsAny<User>()))
                .Returns((Scale scale, User user) =>
                {
                    scales.Where(x => x.Id == scale.Id).ToList().ForEach(x =>
                    {
                        x.Name = scale.Name;
                        x.Comission = scale.Comission;
                        x.Validity = scale.Validity;
                    });
                    return scale;
                });
            mock.Setup(p => p.Delete(It.IsAny<Scale>(), It.IsAny<User>()))
                .Returns((Scale scale, User user) =>
                {
                    scales = scales.Where(x => x.Id != scale.Id).ToList();
                    return scale;
                });
            _business = new(mock.Object);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Prueba la consulta de un listado de escalas con filtros, ordenamientos y límite
        /// </summary>
        [Fact]
        public void ListTest()
        {
            //Act
            ListResult<Scale> list = _business.List("idscale = 1", "value", 1, 0);

            //Assert
            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de escalas con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void ListWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _business.List("idescala = 1", "value", 1, 0));
        }

        /// <summary>
        /// Prueba la consulta de una escala dado su identificador
        /// </summary>
        [Fact]
        public void ReadTest()
        {
            //Arrange
            Scale scale = new() { Id = 1 };

            //Act
            scale = _business.Read(scale);

            //Assert
            Assert.Equal(1000, scale.Comission);
        }

        /// <summary>
        /// Prueba la consulta de una escala que no existe dado su identificador
        /// </summary>
        [Fact]
        public void ReadNotFoundTest()
        {
            //Arrange
            Scale scale = new() { Id = 10 };

            //Act
            scale = _business.Read(scale);

            //Assert
            Assert.Equal(0, scale.Id);
        }

        /// <summary>
        /// Prueba la inserción de una escala
        /// </summary>
        [Fact]
        public void InsertTest()
        {
            //Arrange
            Scale scale = new() { Name = "Nueva escala", Comission = 4000, Validity = DateTime.Now };

            //Act
            scale = _business.Insert(scale, new() { Id = 1 });

            //Assert
            Assert.NotEqual(0, scale.Id);
        }

        /// <summary>
        /// Prueba la actualización de una escala
        /// </summary>
        [Fact]
        public void UpdateTest()
        {
            //Arrange
            Scale scale = new() { Id = 2, Name = "Comisión actualizada", Comission = 5000, Validity = DateTime.Now };
            Scale scale2 = new() { Id = 2 };

            //Act
            _ = _business.Update(scale, new() { Id = 1 });
            scale2 = _business.Read(scale2);

            //Assert
            Assert.NotEqual("Comisión 2", scale2.Name);
        }

        /// <summary>
        /// Prueba la eliminación de una escala
        /// </summary>
        [Fact]
        public void DeleteTest()
        {
            //Arrange
            Scale scale = new() { Id = 3 };
            Scale scale2 = new() { Id = 3 };

            //Act
            _ = _business.Delete(scale, new() { Id = 1 });
            scale2 = _business.Read(scale2);

            //Assert
            Assert.Equal(0, scale2.Id);
        }
        #endregion
    }
}
