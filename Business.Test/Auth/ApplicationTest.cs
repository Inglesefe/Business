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
    /// Realiza las pruebas sobre la clase de persistencia de aplicaciones
    /// </summary>
    [Collection("Tests")]
    public class ApplicationTest
    {
        #region Attributes
        /// <summary>
        /// Capa de negocio de las aplicaciones usando la interfaz IPersistenceWithLog
        /// </summary>
        private readonly BusinessApplication _business;
        #endregion

        #region Constructors
        /// <summary>
        /// Inicializa la configuración de la prueba
        /// </summary>
        public ApplicationTest()
        {
            //Arrange
            Mock<IPersistentApplication> mock = new();
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
                new Role() { Id = 4, Name = "Para probar user_role y application_role" }
            };
            List<Tuple<Application, Role>> apps_roles = new()
            {
                new Tuple<Application, Role>(apps[0], roles[0]),
                new Tuple<Application, Role>(apps[0], roles[1]),
                new Tuple<Application, Role>(apps[1], roles[0]),
                new Tuple<Application, Role>(apps[1], roles[1])
            };
            mock.Setup(p => p.List("idapplication = 1", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new ListResult<Application>(apps.Where(y => y.Id == 1).ToList(), 1));
            mock.Setup(p => p.List("idaplicacion = 1", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Throws<PersistentException>();
            mock.Setup(p => p.List("error", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Throws<BusinessException>();
            mock.Setup(p => p.Read(It.IsAny<Application>()))
                .Returns((Application app) =>
                {
                    if (app.Id == -1)
                    {
                        throw new BusinessException();
                    }
                    return apps.Find(x => x.Id == app.Id) ?? new Application();
                });
            mock.Setup(p => p.Insert(It.IsAny<Application>(), It.IsAny<User>()))
                .Returns((Application app, User user) =>
                {
                    if (app.Id == -1)
                    {
                        throw new BusinessException();
                    }
                    if (apps.Exists(x => x.Name == app.Name))
                    {
                        throw new PersistentException();
                    }
                    else
                    {
                        app.Id = apps.Count + 1;
                        apps.Add(app);
                        return app;
                    }
                });
            mock.Setup(p => p.Update(It.IsAny<Application>(), It.IsAny<User>()))
                .Returns((Application app, User user) =>
                {
                    if (app.Id == -1)
                    {
                        throw new BusinessException();
                    }
                    if (app.Id == -2)
                    {
                        throw new PersistentException();
                    }
                    apps.Where(x => x.Id == app.Id).ToList().ForEach(x => x.Name = app.Name);
                    return app;
                });
            mock.Setup(p => p.Delete(It.IsAny<Application>(), It.IsAny<User>()))
                .Returns((Application app, User user) =>
                {
                    if (app.Id == -1)
                    {
                        throw new BusinessException();
                    }
                    if (app.Id == -2)
                    {
                        throw new PersistentException();
                    }
                    apps = apps.Where(x => x.Id != app.Id).ToList();
                    return app;
                });
            mock.Setup(p => p.ListRoles("", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Application>()))
                .Returns(new ListResult<Role>(apps_roles.Where(x => x.Item1.Id == 1).Select(x => x.Item2).ToList(), 1));
            mock.Setup(p => p.ListRoles("idrole = 2", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Application>()))
                .Returns(new ListResult<Role>(new List<Role>(), 0));
            mock.Setup(p => p.ListRoles("idaplicación = 1", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Application>()))
                .Throws<PersistentException>();
            mock.Setup(p => p.ListRoles("error", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Application>()))
                .Throws<BusinessException>();
            mock.Setup(p => p.ListNotRoles(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Application>()))
                .Returns((string filters, string orders, int limit, int offset, Application app) =>
                {
                    if (app.Id == -1)
                    {
                        throw new BusinessException();
                    }
                    if (app.Id == -2)
                    {
                        throw new PersistentException();
                    }
                    List<Role> result = roles.Where(x => !apps_roles.Exists(y => y.Item1.Id == app.Id && y.Item2.Id == x.Id)).ToList();
                    return new ListResult<Role>(result, result.Count);
                });
            mock.Setup(p => p.InsertRole(It.IsAny<Role>(), It.IsAny<Application>(), It.IsAny<User>())).
                Returns((Role role, Application app, User user) =>
                {
                    if (app.Id == -1)
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
                        return role;
                    }
                });
            mock.Setup(p => p.DeleteRole(It.IsAny<Role>(), It.IsAny<Application>(), It.IsAny<User>())).
                Returns((Role role, Application app, User user) =>
                {
                    if (app.Id == -1)
                    {
                        throw new BusinessException();
                    }
                    if (app.Id == -2)
                    {
                        throw new PersistentException();
                    }
                    return role;
                });
            _business = new(mock.Object);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Prueba la consulta de un listado de aplicaciones con filtros, ordenamientos y límite
        /// </summary>
        [Fact]
        public void ListTest()
        {
            //Act
            ListResult<Application> list = _business.List("idapplication = 1", "name", 1, 0);

            //Assert
            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de aplicaciones con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void ListWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _business.List("idaplicacion = 1", "name", 1, 0));
        }

        /// <summary>
        /// Prueba la consulta de un listado de aplicaciones con filtros, ordenamientos y límite y con errores de lógica
        /// </summary>
        [Fact]
        public void ListWithError2Test()
        {
            //Act, Assert
            Assert.Throws<BusinessException>(() => _business.List("error", "name", 1, 0));
        }

        /// <summary>
        /// Prueba la consulta de una aplicación dada su identificador
        /// </summary>
        [Fact]
        public void ReadTest()
        {
            //Arrange
            Application application = new() { Id = 1 };

            //Act
            application = _business.Read(application);

            //Assert
            Assert.Equal("Autenticación", application.Name);
        }

        /// <summary>
        /// Prueba la consulta de una aplicación que no existe dado su identificador
        /// </summary>
        [Fact]
        public void ReadNotFoundTest()
        {
            //Arrange
            Application application = new() { Id = 10 };

            //Act
            application = _business.Read(application);

            //Assert
            Assert.Equal(0, application.Id);
        }

        /// <summary>
        /// Prueba la consulta de una aplicación con error
        /// </summary>
        [Fact]
        public void ReadWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<BusinessException>(() => _business.Read(new() { Id = -1 }));
        }

        /// <summary>
        /// Prueba la inserción de una aplicación
        /// </summary>
        [Fact]
        public void InsertTest()
        {
            //Arrange
            Application application = new() { Name = "Prueba 1" };

            //Act
            application = _business.Insert(application, new() { Id = 1 });

            //Assert
            Assert.NotEqual(0, application.Id);
        }

        /// <summary>
        /// Prueba la inserción de una aplicación con nombre duplicado
        /// </summary>
        [Fact]
        public void InsertDuplicateTest()
        {
            //Arrange
            Application application = new() { Name = "Autenticación" };

            //Act, Assert
            _ = Assert.Throws<PersistentException>(() => _business.Insert(application, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la inserción de una aplicación con error de negocio
        /// </summary>
        [Fact]
        public void InsertWithErrorTest()
        {
            //Act, Assert
            _ = Assert.Throws<BusinessException>(() => _business.Insert(new() { Id = -1 }, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la actualización de una aplicación
        /// </summary>
        [Fact]
        public void UpdateTest()
        {
            //Assert
            Application application = new() { Id = 2, Name = "Prueba actualizar" };
            Application application2 = new() { Id = 2 };

            //Act
            _ = _business.Update(application, new() { Id = 1 });
            application2 = _business.Read(application2);

            //Assert
            Assert.NotEqual("Actualizame", application2.Name);
        }

        /// <summary>
        /// Prueba la actualización de una aplicación con error de persistencia
        /// </summary>
        [Fact]
        public void UpdateWithErrorTest()
        {
            //Assert
            Assert.Throws<PersistentException>(() => _business.Update(new() { Id = -2 }, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la actualización de una aplicación con error de negocio
        /// </summary>
        [Fact]
        public void UpdateWithError2Test()
        {
            //Assert
            Assert.Throws<BusinessException>(() => _business.Update(new() { Id = -1 }, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la eliminación de una aplicación
        /// </summary>
        [Fact]
        public void DeleteTest()
        {
            //Arrange
            Application application = new() { Id = 3 };
            Application application2 = new() { Id = 3 };

            //Act
            _ = _business.Delete(application, new() { Id = 1 });
            application2 = _business.Read(application2);

            //Assert
            Assert.Equal(0, application2.Id);
        }

        /// <summary>
        /// Prueba la eliminación de una aplicación con error de persistencia
        /// </summary>
        [Fact]
        public void DeleteWithErrorTest()
        {
            //Assert
            Assert.Throws<PersistentException>(() => _business.Delete(new() { Id = -2 }, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la eliminación de una aplicación con error de negocio
        /// </summary>
        [Fact]
        public void DeleteWithError2Test()
        {
            //Assert
            Assert.Throws<BusinessException>(() => _business.Delete(new() { Id = -1 }, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la consulta de un listado de roles de una aplicación con filtros, ordenamientos y límite
        /// </summary>
        [Fact]
        public void ListRolesTest()
        {
            //Act
            ListResult<Role> list = _business.ListRoles("", "", 10, 0, new() { Id = 1 });

            //Assert
            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de roles de una aplicación con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void ListRolesWithErrorTest()
        {
            //Act, Assert
            _ = Assert.Throws<PersistentException>(() => _business.ListRoles("idaplicación = 1", "name", 10, 0, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la consulta de un listado de roles de una aplicación con filtros, ordenamientos y límite y con errores de negocio
        /// </summary>
        [Fact]
        public void ListRolesWithError2Test()
        {
            //Act, Assert
            _ = Assert.Throws<BusinessException>(() => _business.ListRoles("error", "name", 10, 0, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la consulta de un listado de roles no asignados a una aplicación con filtros, ordenamientos y límite
        /// </summary>
        [Fact]
        public void ListNotRolesTest()
        {
            //Act
            ListResult<Role> list = _business.ListNotRoles("", "", 10, 0, new() { Id = 1 });

            //Assert
            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de roles no asignados a una aplicación con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void ListNotRolesWithErrorTest()
        {
            //Act, Assert
            _ = Assert.Throws<PersistentException>(() => _business.ListNotRoles("", "", 10, 0, new() { Id = -2 }));
        }

        /// <summary>
        /// Prueba la consulta de un listado de roles no asociados a una aplicación con filtros, ordenamientos y límite y con errores de negocio
        /// </summary>
        [Fact]
        public void ListNotRolesWithError2Test()
        {
            //Act, Assert
            _ = Assert.Throws<BusinessException>(() => _business.ListNotRoles("", "", 10, 0, new() { Id = -1 }));
        }

        /// <summary>
        /// Prueba la inserción de un rol de una aplicación
        /// </summary>
        [Fact]
        public void InsertRoleTest()
        {
            //Act
            Role role = _business.InsertRole(new() { Id = 4 }, new() { Id = 1 }, new() { Id = 1 });

            //Assert
            Assert.NotEqual(0, role.Id);
        }

        /// <summary>
        /// Prueba la inserción de un rol de una aplicación duplicado
        /// </summary>
        [Fact]
        public void InsertRoleDuplicateTest()
        {
            //Act, Assert
            _ = Assert.Throws<PersistentException>(() => _business.InsertRole(new() { Id = 1 }, new() { Id = 1 }, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la inserción de un rol con error de negocio
        /// </summary>
        [Fact]
        public void InsertRoleWithErrorTest()
        {
            //Act, Assert
            _ = Assert.Throws<BusinessException>(() => _business.InsertRole(new() { Id = 1 }, new() { Id = -1 }, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la eliminación de un rol de una aplicación
        /// </summary>
        [Fact]
        public void DeleteRoleTest()
        {
            //Act
            _ = _business.DeleteRole(new() { Id = 2 }, new() { Id = 1 }, new() { Id = 1 });
            ListResult<Role> list = _business.ListRoles("idrole = 2", "", 10, 0, new() { Id = 1 });

            //Assert
            Assert.Equal(0, list.Total);
        }

        /// <summary>
        /// Prueba la eliminación de un rol de una aplicación con error de persistencia
        /// </summary>
        [Fact]
        public void DeleteRoleWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _business.DeleteRole(new() { Id = 1 }, new() { Id = -2 }, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la eliminación de un rol de una aplicación con error de negocio
        /// </summary>
        [Fact]
        public void DeleteRoleWithError2Test()
        {
            //Act, Assert
            Assert.Throws<BusinessException>(() => _business.DeleteRole(new() { Id = 1 }, new() { Id = -1 }, new() { Id = 1 }));
        }
        #endregion
    }
}
