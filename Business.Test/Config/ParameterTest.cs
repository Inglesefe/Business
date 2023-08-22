using Business.Config;
using Dal;
using Dal.Dto;
using Dal.Exceptions;
using Entities.Auth;
using Entities.Config;
using Moq;

namespace Business.Test.Config
{
    /// <summary>
    /// Realiza las pruebas sobre la clase de persistencia de parámetros
    /// </summary>
    [Collection("Tests")]
    public class ParameterTest
    {
        #region Attributes
        /// <summary>
        /// Capa de negocio de los parámetros
        /// </summary>
        private readonly BusinessParameter _business;
        #endregion

        #region Constructors
        /// <summary>
        /// Inicializa la configuración de la prueba
        /// </summary>
        public ParameterTest()
        {
            Mock<IPersistentWithLog<Parameter>> mock = new();

            List<Parameter> parameters = new()
            {
                new Parameter() { Id = 1, Name = "Parámetro 1", Value = "Valor 1" },
                new Parameter() { Id = 2, Name = "Parámetro 2", Value = "Valor 2" },
                new Parameter() { Id = 3, Name = "Parámetro 3", Value = "Valor 3" }
            };

            mock.Setup(p => p.List("idparameter = 1", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new ListResult<Parameter>(parameters.Where(y => y.Id == 1).ToList(), 1));
            mock.Setup(p => p.List("idparametro = 1", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Throws<PersistentException>();

            mock.Setup(p => p.Read(It.IsAny<Parameter>()))
                .Returns((Parameter parameter) => parameters.Find(x => x.Id == parameter.Id) ?? new Parameter());

            mock.Setup(p => p.Insert(It.IsAny<Parameter>(), It.IsAny<User>()))
                .Returns((Parameter parameter, User user) =>
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

            mock.Setup(p => p.Update(It.IsAny<Parameter>(), It.IsAny<User>()))
                .Returns((Parameter parameter, User user) =>
                {
                    parameters.Where(x => x.Id == parameter.Id).ToList().ForEach(x => x.Name = parameter.Name);
                    return parameter;
                });

            mock.Setup(p => p.Delete(It.IsAny<Parameter>(), It.IsAny<User>()))
                .Returns((Parameter parameter, User user) =>
                {
                    parameters = parameters.Where(x => x.Id != parameter.Id).ToList();
                    return parameter;
                });

            _business = new(mock.Object);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Prueba la consulta de un listado de parámetros con filtros, ordenamientos y límite
        /// </summary>
        [Fact]
        public void ParameterListTest()
        {
            ListResult<Parameter> list = _business.List("idparameter = 1", "name", 1, 0);

            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de parámetros con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void ParameterListWithErrorTest()
        {
            Assert.Throws<PersistentException>(() => _business.List("idparametro = 1", "name", 1, 0));
        }

        /// <summary>
        /// Prueba la consulta de un parámetro dado su identificador
        /// </summary>
        [Fact]
        public void ParameterReadTest()
        {
            Parameter parameter = new() { Id = 1 };
            parameter = _business.Read(parameter);

            Assert.Equal("Parámetro 1", parameter.Name);
        }

        /// <summary>
        /// Prueba la consulta de un parámetro que no existe dado su identificador
        /// </summary>
        [Fact]
        public void ParameterReadNotFoundTest()
        {
            Parameter parameter = new() { Id = 10 };
            parameter = _business.Read(parameter);

            Assert.Equal(0, parameter.Id);
        }

        /// <summary>
        /// Prueba la inserción de un parámetro
        /// </summary>
        [Fact]
        public void ParameterInsertTest()
        {
            Parameter parameter = new() { Name = "Parametro 4", Value = "Valor 4" };
            parameter = _business.Insert(parameter, new() { Id = 1 });

            Assert.NotEqual(0, parameter.Id);
        }

        /// <summary>
        /// Prueba la inserción de un parámetro con nombre duplicado
        /// </summary>
        /// <returns>N/A</returns>
        [Fact]
        public void ParameterInsertDuplicateTest()
        {
            Parameter parameter = new() { Name = "Parámetro 1", Value = "Valor 5" };

            _ = Assert.Throws<PersistentException>(() => _business.Insert(parameter, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la actualización de un parámetro
        /// </summary>
        [Fact]
        public void ParameterUpdateTest()
        {
            Parameter parameter = new() { Id = 2, Name = "Parámetro 6", Value = "Valor 6" };
            _ = _business.Update(parameter, new() { Id = 1 });

            Parameter parameter2 = new() { Id = 2 };
            parameter2 = _business.Read(parameter2);

            Assert.NotEqual("Parámetro 2", parameter2.Name);
        }

        /// <summary>
        /// Prueba la eliminación de un parámetro
        /// </summary>
        [Fact]
        public void ParameterDeleteTest()
        {
            Parameter parameter = new() { Id = 3 };
            _ = _business.Delete(parameter, new() { Id = 1 });

            Parameter parameter2 = new() { Id = 3 };
            parameter2 = _business.Read(parameter2);

            Assert.Equal(0, parameter2.Id);
        }
        #endregion
    }
}
