using Business.Auth;
using Dal.Auth;
using Dal.Dto;
using Dal.Exceptions;
using Entities.Auth;
using Moq;

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
            Mock<IPersistentRole> mock = new();

            List<Application> apps = new()
            {
                new Application() { Id = 1, Name = "Autenticación" },
                new Application() { Id = 2, Name = "Actualízame" },
                new Application() { Id = 3, Name = "Bórrame" }
            };
            List<Role> roles = new()
            {
                new Role() { Id = 1, Name = "Administradores" },
                new Role() { Id = 2, Name = "Actualízame" },
                new Role() { Id = 3, Name = "Bórrame" },
                new Role() { Id = 4, Name = "Para probar user_role y application_role" },
            };
            List<User> users = new()
            {
                new User() { Id = 1, Login = "leandrobaena@gmail.com", Name = "Leandro Baena Torres", Active = true },
                new User() { Id = 2, Login = "actualizame@gmail.com", Name = "Karol Ximena Baena", Active = true },
                new User() { Id = 3, Login = "borrame@gmail.com", Name = "David Santiago Baena", Active = true },
                new User() { Id = 4, Login = "inactivo@gmail.com", Name = "Luz Marina Torres", Active = false }
            };
            List<Tuple<Application, Role>> apps_roles = new()
            {
                new Tuple<Application, Role>(apps[0], roles[0]),
                new Tuple<Application, Role>(apps[0], roles[1]),
                new Tuple<Application, Role>(apps[1], roles[0]),
                new Tuple<Application, Role>(apps[1], roles[1])
            };
            List<Tuple<User, Role>> users_roles = new()
            {
                new Tuple<User, Role>(users[0], roles[0]),
                new Tuple<User, Role>(users[0], roles[1]),
                new Tuple<User, Role>(users[1], roles[0]),
                new Tuple<User, Role>(users[1], roles[1])
            };

            mock.Setup(p => p.List("idrole = 1", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new ListResult<Role>(roles.Where(y => y.Id == 1).ToList(), 1));
            mock.Setup(p => p.List("idrol = 1", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Throws<PersistentException>();

            mock.Setup(p => p.Read(It.IsAny<Role>()))
                .Returns((Role role) => roles.Find(x => x.Id == role.Id) ?? new Role());

            mock.Setup(p => p.Insert(It.IsAny<Role>(), It.IsAny<User>()))
                .Returns((Role role, User user) =>
                {
                    if (roles.Exists(x => x.Name == role.Name))
                    {
                        throw new PersistentException();
                    }
                    else
                    {
                        role.Id = roles.Count + 1;
                        roles.Add(role);
                        return role;
                    }
                });

            mock.Setup(p => p.Update(It.IsAny<Role>(), It.IsAny<User>()))
                .Returns((Role role, User user) =>
                {
                    roles.Where(x => x.Id == role.Id).ToList().ForEach(x => x.Name = role.Name);
                    return role;
                });

            mock.Setup(p => p.Delete(It.IsAny<Role>(), It.IsAny<User>()))
                .Returns((Role role, User user) =>
                {
                    roles = roles.Where(x => x.Id != role.Id).ToList();
                    return role;
                });

            mock.Setup(p => p.ListApplications("", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Role>()))
                .Returns(new ListResult<Application>(apps_roles.Where(x => x.Item2.Id == 1).Select(x => x.Item1).ToList(), 1));

            mock.Setup(p => p.ListApplications("a.idapplication = 2", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Role>()))
                .Returns(new ListResult<Application>(new List<Application>(), 0));

            mock.Setup(p => p.ListApplications("idaplicacion = 1", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Role>()))
                .Throws<PersistentException>();

            mock.Setup(p => p.ListNotApplications(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Role>()))
                .Returns((string filters, string orders, int limit, int offset, Role role) =>
                {
                    List<Application> result = apps.Where(x => !apps_roles.Exists(y => y.Item2.Id == role.Id && y.Item1.Id == x.Id)).ToList();
                    return new ListResult<Application>(result, result.Count);
                });

            mock.Setup(p => p.InsertApplication(It.IsAny<Application>(), It.IsAny<Role>(), It.IsAny<User>())).
                Returns((Application app, Role role, User user) =>
                {
                    if (apps_roles.Exists(x => x.Item1.Id == app.Id && x.Item2.Id == role.Id))
                    {
                        throw new PersistentException();
                    }
                    else
                    {
                        apps_roles.Add(new Tuple<Application, Role>(app, role));
                        return app;
                    }
                });

            mock.Setup(p => p.ListUsers("", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Role>()))
                .Returns(new ListResult<User>(users_roles.Where(x => x.Item2.Id == 1).Select(x => x.Item1).ToList(), 1));

            mock.Setup(p => p.ListUsers("u.iduser = 2", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Role>()))
                .Returns(new ListResult<User>(new List<User>(), 0));

            mock.Setup(p => p.ListUsers("idusuario = 1", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Role>()))
                .Throws<PersistentException>();

            mock.Setup(p => p.ListNotUsers(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Role>()))
                .Returns((string filters, string orders, int limit, int offset, Role role) =>
                {
                    List<User> result = users.Where(x => !users_roles.Exists(y => y.Item2.Id == role.Id && y.Item1.Id == x.Id)).ToList();
                    return new ListResult<User>(result, result.Count);
                });

            mock.Setup(p => p.InsertUser(It.IsAny<User>(), It.IsAny<Role>(), It.IsAny<User>())).
                Returns((User user, Role role, User user1) =>
                {
                    if (users_roles.Exists(x => x.Item1.Id == user.Id && x.Item2.Id == role.Id))
                    {
                        throw new PersistentException();
                    }
                    else
                    {
                        users_roles.Add(new Tuple<User, Role>(user, role));
                        return user;
                    }
                });

            _business = new(mock.Object);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Prueba la consulta de un listado de roles con filtros, ordenamientos y límite
        /// </summary>
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
        [Fact]
        public void RoleListWithErrorTest()
        {
            _ = Assert.Throws<PersistentException>(() => _business.List("idrol = 1", "name", 1, 0));
        }

        /// <summary>
        /// Prueba la consulta de un rol dado su identificador
        /// </summary>
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
        [Fact]
        public void RoleReadNotFoundTest()
        {
            Role role = new() { Id = 10 };
            role = _business.Read(role);

            Assert.Equal(0, role.Id);
        }

        /// <summary>
        /// Prueba la inserción de un rol
        /// </summary>
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
        [Fact]
        public void RoleInsertDuplicateTest()
        {
            Role role = new() { Name = "Administradores" };

            _ = Assert.Throws<PersistentException>(() => _business.Insert(role, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la actualización de un rol
        /// </summary>
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
        [Fact]
        public void RoleDeleteTest()
        {
            Role role = new() { Id = 3 };
            _ = _business.Delete(role, new() { Id = 1 });

            Role role2 = new() { Id = 3 };
            role2 = _business.Read(role2);

            Assert.Equal(0, role2.Id);
        }

        /// <summary>
        /// Prueba la consulta de un listado de usuarios de un rol con filtros, ordenamientos y límite
        /// </summary>
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
        [Fact]
        public void RoleListUsersWithErrorTest()
        {
            _ = Assert.Throws<PersistentException>(() => _business.ListUsers("idusuario = 1", "name", 10, 0, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la consulta de un listado de usuarios no asignados a un rol con filtros, ordenamientos y límite
        /// </summary>
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
        [Fact]
        public void RoleInsertUserTest()
        {
            User role = _business.InsertUser(new() { Id = 2 }, new() { Id = 4 }, new() { Id = 1 });

            Assert.NotNull(role);
        }

        /// <summary>
        /// Prueba la inserción de un usuario de un rol duplicado
        /// </summary>
        [Fact]
        public void RoleInsertUserDuplicateTest()
        {
            _ = Assert.Throws<PersistentException>(() => _business.InsertUser(new() { Id = 2 }, new() { Id = 1 }, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la eliminación de un usuario de un rol
        /// </summary>
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
        [Fact]
        public void RoleListApplicationsWithErrorTest()
        {
            _ = Assert.Throws<PersistentException>(() => _business.ListApplications("idaplicacion = 1", "name", 10, 0, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la consulta de un listado de aplicaciones no asignadas a un rol con filtros, ordenamientos y límite
        /// </summary>
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
        [Fact]
        public void RoleInsertApplicationTest()
        {
            Application application = _business.InsertApplication(new() { Id = 2 }, new() { Id = 4 }, new() { Id = 1 });

            Assert.NotNull(application);
        }

        /// <summary>
        /// Prueba la inserción de una aplicación de un rol duplicado
        /// </summary>
        [Fact]
        public void RoleInsertApplicationDuplicateTest()
        {
            _ = Assert.Throws<PersistentException>(() => _business.InsertUser(new() { Id = 2 }, new() { Id = 1 }, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la eliminación de una aplicación de un rol
        /// </summary>
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
