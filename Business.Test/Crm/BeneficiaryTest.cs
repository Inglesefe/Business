using Business.Crm;
using Dal;
using Dal.Dto;
using Dal.Exceptions;
using Entities.Auth;
using Entities.Crm;
using Moq;
using System.Data;

namespace Business.Test.Crm
{
    /// <summary>
    /// Realiza las pruebas sobre la clase de persistencia de beneficiarios
    /// </summary>
    [Collection("Tests")]
    public class BeneficiaryTest
    {
        #region Attributes
        /// <summary>
        /// Capa de negocio de los beneficiarios
        /// </summary>
        private readonly BusinessBeneficiary _business;
        #endregion

        #region Constructors
        /// <summary>
        /// Inicializa la configuración de la prueba
        /// </summary>
        public BeneficiaryTest()
        {
            //Arrange
            Mock<IPersistentWithLog<Beneficiary>> mock = new();
            List<Beneficiary> beneficiaries = new()
            {
                new Beneficiary() { Id = 1, Name = "Pedro Perez", IdentificationType = new(){ Id = 1 }, Identification = "111111111", Relationship = "hijo" },
                new Beneficiary() { Id = 1, Name = "Maria Martinez", IdentificationType = new(){ Id = 1 }, Identification = "222222222", Relationship = "hija" },
                new Beneficiary() { Id = 1, Name = "Hernan Hernandez", IdentificationType = new(){ Id = 1 }, Identification = "333333333", Relationship = "esposa" },
                new Beneficiary() { Id = 1, Name = "Para eliminar", IdentificationType = new(){ Id = 1 }, Identification = "1111122222", Relationship = "primo" }
            };
            mock.Setup(p => p.List("idbeneficiary = 1", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new ListResult<Beneficiary>(beneficiaries.Where(y => y.Id == 1).ToList(), 1));
            mock.Setup(p => p.List("idbeneficiario = 1", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Throws<PersistentException>();
            mock.Setup(p => p.Read(It.IsAny<Beneficiary>()))
                .Returns((Beneficiary beneficiary) => beneficiaries.Find(x => x.Id == beneficiary.Id) ?? new Beneficiary());
            mock.Setup(p => p.Insert(It.IsAny<Beneficiary>(), It.IsAny<User>()))
                .Returns((Beneficiary beneficiary, User user) =>
                {
                    beneficiary.Id = beneficiaries.Count + 1;
                    beneficiaries.Add(beneficiary);
                    return beneficiary;
                });
            mock.Setup(p => p.Update(It.IsAny<Beneficiary>(), It.IsAny<User>()))
                .Returns((Beneficiary beneficiary, User user) =>
                {
                    beneficiaries.Where(x => x.Id == beneficiary.Id).ToList().ForEach(x =>
                    {
                        x.Name = beneficiary.Name;
                    });
                    return beneficiary;
                });
            mock.Setup(p => p.Delete(It.IsAny<Beneficiary>(), It.IsAny<User>()))
                .Returns((Beneficiary beneficiary, User user) =>
                {
                    beneficiaries = beneficiaries.Where(x => x.Id != beneficiary.Id).ToList();
                    return beneficiary;
                });
            _business = new(mock.Object);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Prueba la consulta de un listado de beneficiarios con filtros, ordenamientos y límite
        /// </summary>
        [Fact]
        public void ListTest()
        {
            //Act
            ListResult<Beneficiary> list = _business.List("idbeneficiary = 1", "value", 1, 0);

            //Assert
            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de beneficiarios con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void ListWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _business.List("idbeneficiario = 1", "value", 1, 0));
        }

        /// <summary>
        /// Prueba la consulta de un beneficiario dado su identificador
        /// </summary>
        [Fact]
        public void ReadTest()
        {
            //Arrange
            Beneficiary beneficiary = new() { Id = 1 };

            //Act
            beneficiary = _business.Read(beneficiary);

            //Assert
            Assert.Equal("Pedro Perez", beneficiary.Name);
        }

        /// <summary>
        /// Prueba la consulta de un beneficiario que no existe dado su identificador
        /// </summary>
        [Fact]
        public void ReadNotFoundTest()
        {
            //Arrange
            Beneficiary beneficiary = new() { Id = 10 };

            //Act
            beneficiary = _business.Read(beneficiary);

            //Assert
            Assert.Equal(0, beneficiary.Id);
        }

        /// <summary>
        /// Prueba la inserción de un beneficiario
        /// </summary>
        [Fact]
        public void InsertTest()
        {
            //Arrange
            Beneficiary beneficiary = new() { Name = "Nuevo" };

            //Act
            beneficiary = _business.Insert(beneficiary, new() { Id = 1 });

            //Assert
            Assert.NotEqual(0, beneficiary.Id);
        }

        /// <summary>
        /// Prueba la actualización de un beneficiario
        /// </summary>
        [Fact]
        public void UpdateTest()
        {
            //Arrange
            Beneficiary beneficiary = new() { Id = 2, Name = "Actualizado" };
            Beneficiary beneficiary2 = new() { Id = 2 };

            //Act
            _ = _business.Update(beneficiary, new() { Id = 1 });
            beneficiary2 = _business.Read(beneficiary2);

            //Assert
            Assert.NotEqual("Maria Martinez", beneficiary2.Name);
        }

        /// <summary>
        /// Prueba la eliminación de un beneficiario
        /// </summary>
        [Fact]
        public void DeleteTest()
        {
            //Arrange
            Beneficiary beneficiary = new() { Id = 3 };
            Beneficiary beneficiary2 = new() { Id = 3 };

            //Act
            _ = _business.Delete(beneficiary, new() { Id = 1 });
            beneficiary2 = _business.Read(beneficiary2);

            //Assert
            Assert.Equal(0, beneficiary2.Id);
        }
        #endregion
    }
}
