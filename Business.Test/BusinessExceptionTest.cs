using Business.Exceptions;

namespace Business.Test
{
    /// <summary>
    /// Realiza las pruebas unitarias sobre la excepción en la capa de negocio
    /// </summary>
    [Collection("Tests")]
    public class BusinessExceptionTest
    {
        /// <summary>
        /// Prueba la creación de una excepción de persistencia por defecto
        /// </summary>
        [Fact]
        public void CreateExceptionDefault()
        {
            Assert.IsType<BusinessException>(new BusinessException());
        }

        /// <summary>
        /// Prueba la creación de una excepción de persistencia con un mensaje
        /// </summary>
        [Fact]
        public void CreateExceptionWithMessage()
        {
            Assert.IsType<BusinessException>(new BusinessException("Excepción de prueba"));
        }

        /// <summary>
        /// Prueba la creación de una excepción de persistencia con un mensaje y una excepción interna
        /// </summary>
        [Fact]
        public void CreateExceptionWithMessageAndInnerException()
        {
            Assert.IsType<BusinessException>(new BusinessException("Excepción de prueba", new Exception()));
        }
    }
}
