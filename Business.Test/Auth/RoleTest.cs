using Business.Auth;
using Dal.Dto;
using Dal.Exceptions;
using Entities.Auth;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace Business.Test.Auth
{
    /// <summary>
    /// Realiza las pruebas sobre la clase de persistencia de roles
    /// </summary>
    [Collection("Tests")]
    public class RoleTest
    {
        #region Attributes
        /// <summary>
        /// Configuración de la aplicación de pruebas
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Capa de negocio de los roles
        /// </summary>
        private readonly BusinessRole _business;
        #endregion

        #region Constructors
        /// <summary>
        /// Inicializa la configuración de la prueba
        /// </summary>
        public RoleTest()
        {
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, false)
                .AddEnvironmentVariables()
                .Build();
            _business = new(new MySqlConnection(_configuration.GetConnectionString("golden") ?? ""));
        }
        #endregion

        #region Methods
        /// <summary>
        /// Prueba la consulta de un listado de roles con filtros, ordenamientos y límite
        /// </summary>
        /// <returns>N/A</returns>
        [Fact]
        public void RoleListTest()
        {
            ListResult<Role> list = _business.List("idrole = 1", "name", 1, 0);

            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de roles con filtros, ordenamientos y límite y con errores
        /// </summary>
        /// <returns>N/A</returns>
        [Fact]
        public void RoleListWithErrorTest()
        {
            _ = Assert.Throws<PersistentException>(() => _business.List("idrol = 1", "name", 1, 0));
        }

        /// <summary>
        /// Prueba la consulta de un rol dado su identificador
        /// </summary>
        /// <returns>N/A</returns>
        [Fact]
        public void RoleReadTest()
        {
            Role role = new() { Id = 1 };
            role = _business.Read(role);

            Assert.Equal("Administradores", role.Name);
        }

        /// <summary>
        /// Prueba la consulta de un rol que no existe dado su identificador
        /// </summary>
        /// <returns>N/A</returns>
        [Fact]
        public void RoleReadNotFoundTest()
        {
            Role role = new() { Id = 10 };
            role = _business.Read(role);

            Assert.Null(role);
        }

        /// <summary>
        /// Prueba la inserción de un rol
        /// </summary>
        /// <returns>N/A</returns>
        [Fact]
        public void RoleInsertTest()
        {
            Role role = new() { Name = "Prueba insercion" };
            role = _business.Insert(role, new() { Id = 1 });

            Assert.NotEqual(0, role.Id);
        }

        /// <summary>
        /// Prueba la inserción de un rol con nombre duplicado
        /// </summary>
        /// <returns>N/A</returns>
        [Fact]
        public void RoleInsertDuplicateTest()
        {
            Role role = new() { Name = "Administradores" };

            _ = Assert.Throws<PersistentException>(() => _business.Insert(role, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la actualización de un rol
        /// </summary>
        /// <returns>N/A</returns>
        [Fact]
        public void RoleUpdateTest()
        {
            Role role = new() { Id = 2, Name = "Prueba actualizar" };
            _ = _business.Update(role, new() { Id = 1 });

            Role role2 = new() { Id = 2 };
            role2 = _business.Read(role2);

            Assert.NotEqual("Actualizame", role2.Name);
        }

        /// <summary>
        /// Prueba la eliminación de un rol
        /// </summary>
        /// <returns>N/A</returns>
        [Fact]
        public void RoleDeleteTest()
        {
            Role role = new() { Id = 3 };
            _ = _business.Delete(role, new() { Id = 1 });

            Role role2 = new() { Id = 3 };
            role2 = _business.Read(role2);

            Assert.Null(role2);
        }

        /// <summary>
        /// Prueba la consulta de un listado de usuarios de un rol con filtros, ordenamientos y límite
        /// </summary>
        /// <returns>N/A</returns>
        [Fact]
        public void RoleListUsersTest()
        {
            ListResult<User> list = _business.ListUsers("", "", 10, 0, new() { Id = 1 });

            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de usuarios de un rol con filtros, ordenamientos y límite y con errores
        /// </summary>
        /// <returns>N/A</returns>
        [Fact]
        public void RoleListUsersWithErrorTest()
        {
            _ = Assert.Throws<PersistentException>(() => _business.ListUsers("idusuario = 1", "name", 10, 0, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la consulta de un listado de usuarios no asignados a un rol con filtros, ordenamientos y límite
        /// </summary>
        /// <returns>N/A</returns>
        [Fact]
        public void RoleListNotUsersTest()
        {
            ListResult<User> list = _business.ListNotUsers("", "", 10, 0, new() { Id = 1 });

            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la inserción de un usuario de un rol
        /// </summary>
        /// <returns>N/A</returns>
        [Fact]
        public void RoleInsertUserTest()
        {
            User role = _business.InsertUser(new() { Id = 2 }, new() { Id = 4 }, new() { Id = 1 });

            Assert.NotNull(role);
        }

        /// <summary>
        /// Prueba la inserción de un usuario de un rol duplicado
        /// </summary>
        /// <returns>N/A</returns>
        [Fact]
        public void RoleInsertUserDuplicateTest()
        {
            _ = Assert.Throws<PersistentException>(() => _business.InsertUser(new() { Id = 2 }, new() { Id = 1 }, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la eliminación de un usuario de un rol
        /// </summary>
        /// <returns>N/A</returns>
        [Fact]
        public void RoleDeleteUserTest()
        {
            _ = _business.DeleteUser(new() { Id = 2 }, new() { Id = 2 }, new() { Id = 1 });
            ListResult<User> list = _business.ListUsers("u.iduser = 2", "", 10, 0, new() { Id = 2 });

            Assert.Equal(0, list.Total);
        }


        /// <summary>
        /// Prueba la consulta de un listado de aplicaciones de un rol con filtros, ordenamientos y límite
        /// </summary>
        /// <returns>N/A</returns>
        [Fact]
        public void RoleListApplicationsTest()
        {
            ListResult<Application> list = _business.ListApplications("", "", 10, 0, new() { Id = 1 });

            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de aplicaciones de un rol con filtros, ordenamientos y límite y con errores
        /// </summary>
        /// <returns>N/A</returns>
        [Fact]
        public void RoleListApplicationsWithErrorTest()
        {
            _ = Assert.Throws<PersistentException>(() => _business.ListApplications("idaplicacion = 1", "name", 10, 0, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la consulta de un listado de aplicaciones no asignadas a un rol con filtros, ordenamientos y límite
        /// </summary>
        /// <returns>N/A</returns>
        [Fact]
        public void RoleListNotApplicationsTest()
        {
            ListResult<Application> list = _business.ListNotApplications("", "", 10, 0, new() { Id = 1 });

            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la inserción de una aplicación de un rol
        /// </summary>
        /// <returns>N/A</returns>
        [Fact]
        public void RoleInsertApplicationTest()
        {
            Application application = _business.InsertApplication(new() { Id = 2 }, new() { Id = 4 }, new() { Id = 1 });

            Assert.NotNull(application);
        }

        /// <summary>
        /// Prueba la inserción de una aplicación de un rol duplicado
        /// </summary>
        /// <returns>N/A</returns>
        [Fact]
        public void RoleInsertApplicationDuplicateTest()
        {
            _ = Assert.Throws<PersistentException>(() => _business.InsertUser(new() { Id = 2 }, new() { Id = 1 }, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la eliminación de una aplicación de un rol
        /// </summary>
        /// <returns>N/A</returns>
        [Fact]
        public void RoleDeleteApplicationTest()
        {
            _ = _business.DeleteApplication(new() { Id = 2 }, new() { Id = 2 }, new() { Id = 1 });
            ListResult<Application> list = _business.ListApplications("a.idapplication = 2", "", 10, 0, new() { Id = 2 });

            Assert.Equal(0, list.Total);
        }
        #endregion
    }
}
