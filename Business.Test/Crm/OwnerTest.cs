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
    /// Realiza las pruebas sobre la clase de persistencia de titulares
    /// </summary>
    [Collection("Tests")]
    public class OwnerTest
    {
        #region Attributes
        /// <summary>
        /// Capa de negocio de los titulares
        /// </summary>
        private readonly BusinessOwner _business;
        #endregion

        #region Constructors
        /// <summary>
        /// Inicializa la configuración de la prueba
        /// </summary>
        public OwnerTest()
        {
            //Arrange
            Mock<IPersistentWithLog<Owner>> mock = new();
            List<Owner> owners = new()
            {
                new Owner() {
                    Id = 1,
                    Name = "Leandro Baena Torres",
                    IdentificationType = new(){ Id = 1 },
                    Identification = "123456789",
                    AddressHome = "CL 1 # 2 - 3",
                    AddressOffice = "CL 4 # 5 - 6",
                    PhoneHome = "3121234567",
                    PhoneOffice = "3127654321",
                    Email = "leandrobaena@gmail.com"
                },
                new Owner() {
                    Id = 2,
                    Name = "David Santiago Baena Barreto",
                    IdentificationType = new(){ Id = 1 },
                    Identification = "987654321",
                    AddressHome = "CL 7 # 8 - 9",
                    AddressOffice = "CL 10 # 11 - 12",
                    PhoneHome = "3151234567",
                    PhoneOffice = "3157654321",
                    Email = "dsantiagobaena@gmail.com"
                },
                new Owner() {
                    Id = 3,
                    Name = "Karol Ximena Baena Brreto",
                    IdentificationType = new(){ Id = 1 },
                    Identification = "147258369",
                    AddressHome = "CL 13 # 14 - 15",
                    AddressOffice = "CL 16 # 17 - 18",
                    PhoneHome = "3201234567",
                    PhoneOffice = "3207654321",
                    Email = "kximenabaena@gmail.com"
                }
            };
            mock.Setup(p => p.List("idowner = 1", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new ListResult<Owner>(owners.Where(y => y.Id == 1).ToList(), 1));
            mock.Setup(p => p.List("idtitular = 1", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Throws<PersistentException>();
            mock.Setup(p => p.Read(It.IsAny<Owner>()))
                .Returns((Owner owner) => owners.Find(x => x.Id == owner.Id) ?? new Owner());
            mock.Setup(p => p.Insert(It.IsAny<Owner>(), It.IsAny<User>()))
                .Returns((Owner owner, User user) =>
                {
                    owner.Id = owners.Count + 1;
                    owners.Add(owner);
                    return owner;
                });
            mock.Setup(p => p.Update(It.IsAny<Owner>(), It.IsAny<User>()))
                .Returns((Owner owner, User user) =>
                {
                    owners.Where(x => x.Id == owner.Id).ToList().ForEach(x =>
                    {
                        x.Name = owner.Name;
                    });
                    return owner;
                });
            mock.Setup(p => p.Delete(It.IsAny<Owner>(), It.IsAny<User>()))
                .Returns((Owner owner, User user) =>
                {
                    owners = owners.Where(x => x.Id != owner.Id).ToList();
                    return owner;
                });
            _business = new(mock.Object);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Prueba la consulta de un listado de titulares con filtros, ordenamientos y límite
        /// </summary>
        [Fact]
        public void ListTest()
        {
            //Act
            ListResult<Owner> list = _business.List("idowner = 1", "value", 1, 0);

            //Assert
            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de titulares con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void ListWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _business.List("idtitular = 1", "value", 1, 0));
        }

        /// <summary>
        /// Prueba la consulta de un titular dado su identificador
        /// </summary>
        [Fact]
        public void ReadTest()
        {
            //Arrange
            Owner owner = new() { Id = 1 };

            //Act
            owner = _business.Read(owner);

            //Assert
            Assert.Equal("Leandro Baena Torres", owner.Name);
        }

        /// <summary>
        /// Prueba la consulta de un titular que no existe dado su identificador
        /// </summary>
        [Fact]
        public void ReadNotFoundTest()
        {
            //Arrange
            Owner owner = new() { Id = 10 };

            //Act
            owner = _business.Read(owner);

            //Assert
            Assert.Equal(0, owner.Id);
        }

        /// <summary>
        /// Prueba la inserción de un titular
        /// </summary>
        [Fact]
        public void InsertTest()
        {
            //Arrange
            Owner owner = new() { Name = "Nuevo" };

            //Act
            owner = _business.Insert(owner, new() { Id = 1 });

            //Assert
            Assert.NotEqual(0, owner.Id);
        }

        /// <summary>
        /// Prueba la actualización de un titular
        /// </summary>
        [Fact]
        public void UpdateTest()
        {
            //Arrange
            Owner owner = new() { Id = 2, Name = "Actualizado" };
            Owner owner2 = new() { Id = 2 };

            //Act
            _ = _business.Update(owner, new() { Id = 1 });
            owner2 = _business.Read(owner2);

            //Assert
            Assert.NotEqual("David Santiago Baena Barreto", owner2.Name);
        }

        /// <summary>
        /// Prueba la eliminación de un titular
        /// </summary>
        [Fact]
        public void DeleteTest()
        {
            //Arrange
            Owner owner = new() { Id = 3 };
            Owner owner2 = new() { Id = 3 };

            //Act
            _ = _business.Delete(owner, new() { Id = 1 });
            owner2 = _business.Read(owner2);

            //Assert
            Assert.Equal(0, owner2.Id);
        }
        #endregion
    }
}
