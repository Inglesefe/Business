using Business.Auth;
using Business.Exceptions;
using Dal.Auth;
using Dal.Dto;
using Dal.Exceptions;
using Entities.Auth;
using Moq;
using System.Data;

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
            //Arrange
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
            mock.Setup(p => p.ListApplications("", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Role>()))
                .Returns(new ListResult<Application>(apps_roles.Where(x => x.Item2.Id == 1).Select(x => x.Item1).ToList(), 1));
            mock.Setup(p => p.ListApplications("idapplication = 2", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Role>()))
                .Returns(new ListResult<Application>(new List<Application>(), 0));
            mock.Setup(p => p.ListApplications("idaplicacion = 1", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Role>()))
                .Throws<PersistentException>();
            mock.Setup(p => p.ListApplications("error", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Role>()))
                .Throws<BusinessException>();
            mock.Setup(p => p.ListNotApplications(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Role>()))
                .Returns((string filters, string orders, int limit, int offset, Role role) =>
                {
                    if (role.Id == -1)
                    {
                        throw new BusinessException();
                    }
                    if (role.Id == -2)
                    {
                        throw new PersistentException();
                    }
                    List<Application> result = apps.Where(x => !apps_roles.Exists(y => y.Item2.Id == role.Id && y.Item1.Id == x.Id)).ToList();
                    return new ListResult<Application>(result, result.Count);
                });
            mock.Setup(p => p.InsertApplication(It.IsAny<Application>(), It.IsAny<Role>(), It.IsAny<User>())).
                Returns((Application app, Role role, User user) =>
                {
                    if (role.Id == -1)
                    {
                        throw new BusinessException();
                    }
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
            mock.Setup(p => p.DeleteApplication(It.IsAny<Application>(), It.IsAny<Role>(), It.IsAny<User>())).
                Returns((Application app, Role role, User user1) =>
                {
                    if (role.Id == -1)
                    {
                        throw new BusinessException();
                    }
                    if (role.Id == -2)
                    {
                        throw new PersistentException();
                    }
                    return app;
                });
            mock.Setup(p => p.ListUsers("", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Role>()))
                .Returns(new ListResult<User>(users_roles.Where(x => x.Item2.Id == 1).Select(x => x.Item1).ToList(), 1));
            mock.Setup(p => p.ListUsers("iduser = 2", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Role>()))
                .Returns(new ListResult<User>(new List<User>(), 0));
            mock.Setup(p => p.ListUsers("idusuario = 1", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Role>()))
                .Throws<PersistentException>();
            mock.Setup(p => p.ListUsers("error", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Role>()))
                .Throws<BusinessException>();
            mock.Setup(p => p.ListNotUsers(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Role>()))
                .Returns((string filters, string orders, int limit, int offset, Role role) =>
                {
                    if (role.Id == -1)
                    {
                        throw new BusinessException();
                    }
                    if (role.Id == -2)
                    {
                        throw new PersistentException();
                    }
                    List<User> result = users.Where(x => !users_roles.Exists(y => y.Item2.Id == role.Id && y.Item1.Id == x.Id)).ToList();
                    return new ListResult<User>(result, result.Count);
                });
            mock.Setup(p => p.InsertUser(It.IsAny<User>(), It.IsAny<Role>(), It.IsAny<User>())).
                Returns((User user, Role role, User user1) =>
                {
                    if (role.Id == -1)
                    {
                        throw new BusinessException();
                    }
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
            mock.Setup(p => p.DeleteUser(It.IsAny<User>(), It.IsAny<Role>(), It.IsAny<User>())).
                Returns((User user, Role role, User user1) =>
                {
                    if (role.Id == -1)
                    {
                        throw new BusinessException();
                    }
                    if (role.Id == -2)
                    {
                        throw new PersistentException();
                    }
                    return user;
                });
            _business = new(mock.Object);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Prueba la consulta de un listado de usuarios de un rol con filtros, ordenamientos y límite
        /// </summary>
        [Fact]
        public void ListUsersTest()
        {
            //Act
            ListResult<User> list = _business.ListUsers("", "", 10, 0, new() { Id = 1 });

            //Assert
            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de usuarios de un rol con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void ListUsersWithErrorTest()
        {
            //Act, Assert
            _ = Assert.Throws<PersistentException>(() => _business.ListUsers("idusuario = 1", "name", 10, 0, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la consulta de un listado de usuarios de un rol con filtros, ordenamientos y límite y con errores de negocio
        /// </summary>
        [Fact]
        public void ListUsersWithError2Test()
        {
            //Act, Assert
            _ = Assert.Throws<BusinessException>(() => _business.ListUsers("error", "name", 10, 0, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la consulta de un listado de usuarios no asignados a un rol con filtros, ordenamientos y límite
        /// </summary>
        [Fact]
        public void ListNotUsersTest()
        {
            //Act
            ListResult<User> list = _business.ListNotUsers("", "", 10, 0, new() { Id = 1 });

            //Assert
            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de usuarios no asignados a un rol con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void ListNotUsersWithErrorTest()
        {
            //Act, Assert
            _ = Assert.Throws<PersistentException>(() => _business.ListNotUsers("", "", 10, 0, new() { Id = -2 }));
        }

        /// <summary>
        /// Prueba la consulta de un listado de usuarios no asignados a un rol con filtros, ordenamientos y límite y con errores de negocio
        /// </summary>
        [Fact]
        public void ListNotUsersWithError2Test()
        {
            //Act, Assert
            _ = Assert.Throws<BusinessException>(() => _business.ListNotUsers("", "", 10, 0, new() { Id = -1 }));
        }


        /// <summary>
        /// Prueba la inserción de un usuario de un rol
        /// </summary>
        [Fact]
        public void InsertUserTest()
        {
            //Act
            User role = _business.InsertUser(new() { Id = 2 }, new() { Id = 4 }, new() { Id = 1 });

            //Assert
            Assert.NotEqual(0, role.Id);
        }

        /// <summary>
        /// Prueba la inserción de un usuario de un rol duplicado
        /// </summary>
        [Fact]
        public void InsertUserDuplicateTest()
        {
            //Act, Assert
            _ = Assert.Throws<PersistentException>(() => _business.InsertUser(new() { Id = 2 }, new() { Id = 1 }, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la inserción de un usuario con error de negocio
        /// </summary>
        [Fact]
        public void InsertUserWithErrorTest()
        {
            //Act, Assert
            _ = Assert.Throws<BusinessException>(() => _business.InsertUser(new() { Id = 2 }, new() { Id = -1 }, new() { Id = -1 }));
        }

        /// <summary>
        /// Prueba la eliminación de un usuario de un rol
        /// </summary>
        [Fact]
        public void DeleteUserTest()
        {
            //Act
            _ = _business.DeleteUser(new() { Id = 2 }, new() { Id = 2 }, new() { Id = 1 });
            ListResult<User> list = _business.ListUsers("iduser = 2", "", 10, 0, new() { Id = 2 });

            //Assert
            Assert.Equal(0, list.Total);
        }

        /// <summary>
        /// Prueba la eliminación de un usuario de un rol con error de persistencia
        /// </summary>
        [Fact]
        public void DeleteUserWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _business.DeleteUser(new() { Id = 2 }, new() { Id = -2 }, new() { Id = -1 }));
        }

        /// <summary>
        /// Prueba la eliminación de un usuario de un rol con error de negocio
        /// </summary>
        [Fact]
        public void DeleteUserWithError2Test()
        {
            //Act, Assert
            Assert.Throws<BusinessException>(() => _business.DeleteUser(new() { Id = 2 }, new() { Id = -1 }, new() { Id = -1 }));
        }

        /// <summary>
        /// Prueba la consulta de un listado de aplicaciones de un rol con filtros, ordenamientos y límite
        /// </summary>
        [Fact]
        public void ListApplicationsTest()
        {
            //Act
            ListResult<Application> list = _business.ListApplications("", "", 10, 0, new() { Id = 1 });

            //Assert
            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de aplicaciones de un rol con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void ListApplicationsWithErrorTest()
        {
            //Act, Assert
            _ = Assert.Throws<PersistentException>(() => _business.ListApplications("idaplicacion = 1", "name", 10, 0, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la consulta de un listado de aplicaciones de un rol con filtros, ordenamientos y límite y con errores de negocio
        /// </summary>
        [Fact]
        public void ListApplicationsWithError2Test()
        {
            //Act, Assert
            _ = Assert.Throws<BusinessException>(() => _business.ListApplications("error", "name", 10, 0, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la consulta de un listado de aplicaciones no asignadas a un rol con filtros, ordenamientos y límite
        /// </summary>
        [Fact]
        public void ListNotApplicationsTest()
        {
            //Act
            ListResult<Application> list = _business.ListNotApplications("", "", 10, 0, new() { Id = 1 });

            //Assert
            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de aplicaciones no asociadas a un rol con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void ListNotApplicationsWithErrorTest()
        {
            //Act, Assert
            _ = Assert.Throws<PersistentException>(() => _business.ListNotApplications("", "", 10, 0, new() { Id = -2 }));
        }

        /// <summary>
        /// Prueba la consulta de un listado de aplicaciones no asociadas a un rol con filtros, ordenamientos y límite y con errores de negocio
        /// </summary>
        [Fact]
        public void ListNotApplicationsWithError2Test()
        {
            //Act, Assert
            _ = Assert.Throws<BusinessException>(() => _business.ListNotApplications("", "", 10, 0, new() { Id = -1 }));
        }

        /// <summary>
        /// Prueba la inserción de una aplicación de un rol
        /// </summary>
        [Fact]
        public void InsertApplicationTest()
        {
            //Act
            Application application = _business.InsertApplication(new() { Id = 2 }, new() { Id = 4 }, new() { Id = 1 });

            //Assert
            Assert.NotNull(application);
        }

        /// <summary>
        /// Prueba la inserción de una aplicación de un rol duplicado
        /// </summary>
        [Fact]
        public void InsertApplicationDuplicateTest()
        {
            //Act, Assert
            _ = Assert.Throws<PersistentException>(() => _business.InsertApplication(new() { Id = 1 }, new() { Id = 1 }, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la inserción de una aplicación con error de negocio
        /// </summary>
        [Fact]
        public void InsertApplicationWithErrorTest()
        {
            //Act, Assert
            _ = Assert.Throws<BusinessException>(() => _business.InsertApplication(new() { Id = 1 }, new() { Id = -1 }, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la eliminación de una aplicación de un rol
        /// </summary>
        [Fact]
        public void DeleteApplicationTest()
        {
            //Act
            _ = _business.DeleteApplication(new() { Id = 2 }, new() { Id = 2 }, new() { Id = 1 });
            ListResult<Application> list = _business.ListApplications("idapplication = 2", "", 10, 0, new() { Id = 2 });

            //Assert
            Assert.Equal(0, list.Total);
        }

        /// <summary>
        /// Prueba la eliminación de una aplicación de un rol con error de persistencia
        /// </summary>
        [Fact]
        public void DeleteApplicationWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _business.DeleteUser(new() { Id = 1 }, new() { Id = -2 }, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la eliminación de una aplicación de un rol con error de negocio
        /// </summary>
        [Fact]
        public void DeleteApplicationWithError2Test()
        {
            //Act, Assert
            Assert.Throws<BusinessException>(() => _business.DeleteUser(new() { Id = 1 }, new() { Id = -1 }, new() { Id = 1 }));
        }
        #endregion
    }
}
