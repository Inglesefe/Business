﻿using Business.Auth;
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

        /// <summary>
        /// Conexión a la base de datos falsa
        /// </summary>
        private readonly IDbConnection connectionFake;
        #endregion

        #region Constructors
        /// <summary>
        /// Inicializa la configuración de la prueba
        /// </summary>
        public ApplicationTest()
        {
            Mock<IPersistentApplication> mock = new();
            Mock<IDbConnection> mockConnection = new();
            connectionFake = mockConnection.Object;

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
            List<Tuple<Application, Role>> apps_roles = new()
            {
                new Tuple<Application, Role>(apps[0], roles[0]),
                new Tuple<Application, Role>(apps[0], roles[1]),
                new Tuple<Application, Role>(apps[1], roles[0]),
                new Tuple<Application, Role>(apps[1], roles[1])
            };

            mock.Setup(p => p.List("idapplication = 1", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<IDbConnection>()))
                .Returns(new ListResult<Application>(apps.Where(y => y.Id == 1).ToList(), 1));
            mock.Setup(p => p.List("idaplicacion = 1", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<IDbConnection>()))
                .Throws<PersistentException>();

            mock.Setup(p => p.Read(It.IsAny<Application>(), It.IsAny<IDbConnection>()))
                .Returns((Application app, IDbConnection connection) => apps.Find(x => x.Id == app.Id) ?? new Application());

            mock.Setup(p => p.Insert(It.IsAny<Application>(), It.IsAny<User>(), It.IsAny<IDbConnection>()))
                .Returns((Application app, User user, IDbConnection connection) =>
                {
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

            mock.Setup(p => p.Update(It.IsAny<Application>(), It.IsAny<User>(), It.IsAny<IDbConnection>()))
                .Returns((Application app, User user, IDbConnection connection) =>
                {
                    apps.Where(x => x.Id == app.Id).ToList().ForEach(x => x.Name = app.Name);
                    return app;
                });

            mock.Setup(p => p.Delete(It.IsAny<Application>(), It.IsAny<User>(), It.IsAny<IDbConnection>()))
                .Returns((Application app, User user, IDbConnection connection) =>
                {
                    apps = apps.Where(x => x.Id != app.Id).ToList();
                    return app;
                });

            mock.Setup(p => p.ListRoles("", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Application>(), It.IsAny<IDbConnection>()))
                .Returns(new ListResult<Role>(apps_roles.Where(x => x.Item1.Id == 1).Select(x => x.Item2).ToList(), 1));

            mock.Setup(p => p.ListRoles("r.idrole = 2", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Application>(), It.IsAny<IDbConnection>()))
                .Returns(new ListResult<Role>(new List<Role>(), 0));

            mock.Setup(p => p.ListRoles("idaplicación = 1", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Application>(), It.IsAny<IDbConnection>()))
                .Throws<PersistentException>();

            mock.Setup(p => p.ListNotRoles(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Application>(), It.IsAny<IDbConnection>()))
                .Returns((string filters, string orders, int limit, int offset, Application app, IDbConnection connection) =>
                {
                    List<Role> result = roles.Where(x => !apps_roles.Exists(y => y.Item1.Id == app.Id && y.Item2.Id == x.Id)).ToList();
                    return new ListResult<Role>(result, result.Count);
                });

            mock.Setup(p => p.InsertRole(It.IsAny<Role>(), It.IsAny<Application>(), It.IsAny<User>(), It.IsAny<IDbConnection>())).
                Returns((Role role, Application app, User user, IDbConnection connection) =>
                {
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

            _business = new(mock.Object);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Prueba la consulta de un listado de aplicaciones con filtros, ordenamientos y límite
        /// </summary>
        [Fact]
        public void ApplicationListTest()
        {
            ListResult<Application> list = _business.List("idapplication = 1", "name", 1, 0, connectionFake);

            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de aplicaciones con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void ApplicationListWithErrorTest()
        {
            Assert.Throws<PersistentException>(() => _business.List("idaplicacion = 1", "name", 1, 0, connectionFake));
        }

        /// <summary>
        /// Prueba la consulta de una aplicación dada su identificador
        /// </summary>
        [Fact]
        public void ApplicationReadTest()
        {
            Application application = new() { Id = 1 };
            application = _business.Read(application, connectionFake);

            Assert.Equal("Autenticación", application.Name);
        }

        /// <summary>
        /// Prueba la consulta de una aplicación que no existe dado su identificador
        /// </summary>
        [Fact]
        public void ApplicationReadNotFoundTest()
        {
            Application application = new() { Id = 10 };
            application = _business.Read(application, connectionFake);

            Assert.Equal(0, application.Id);
        }

        /// <summary>
        /// Prueba la inserción de una aplicación
        /// </summary>
        [Fact]
        public void ApplicationInsertTest()
        {
            Application application = new() { Name = "Prueba 1" };
            application = _business.Insert(application, new() { Id = 1 }, connectionFake);

            Assert.NotEqual(0, application.Id);
        }

        /// <summary>
        /// Prueba la inserción de una aplicación con nombre duplicado
        /// </summary>
        [Fact]
        public void ApplicationInsertDuplicateTest()
        {
            Application application = new() { Name = "Autenticación" };

            _ = Assert.Throws<PersistentException>(() => _business.Insert(application, new() { Id = 1 }, connectionFake));
        }

        /// <summary>
        /// Prueba la actualización de una aplicación
        /// </summary>
        [Fact]
        public void ApplicationUpdateTest()
        {
            Application application = new() { Id = 2, Name = "Prueba actualizar" };
            _ = _business.Update(application, new() { Id = 1 }, connectionFake);

            Application application2 = new() { Id = 2 };
            application2 = _business.Read(application2, connectionFake);

            Assert.NotEqual("Actualizame", application2.Name);
        }

        /// <summary>
        /// Prueba la eliminación de una aplicación
        /// </summary>
        [Fact]
        public void ApplicationDeleteTest()
        {
            Application application = new() { Id = 3 };
            _ = _business.Delete(application, new() { Id = 1 }, connectionFake);

            Application application2 = new() { Id = 3 };
            application2 = _business.Read(application2, connectionFake);

            Assert.Equal(0, application2.Id);
        }
        /// <summary>
        /// Prueba la consulta de un listado de roles de una aplicación con filtros, ordenamientos y límite
        /// </summary>
        [Fact]
        public void ApplicationListRolesTest()
        {
            ListResult<Role> list = _business.ListRoles("", "", 10, 0, new() { Id = 1 }, connectionFake);

            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de roles de una aplicación con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void ApplicationListRolesWithErrorTest()
        {
            _ = Assert.Throws<PersistentException>(() => _business.ListRoles("idaplicación = 1", "name", 10, 0, new() { Id = 1 }, connectionFake));
        }

        /// <summary>
        /// Prueba la consulta de un listado de roles no asignados a una aplicación con filtros, ordenamientos y límite
        /// </summary>
        [Fact]
        public void ApplicationListNotRolesTest()
        {
            ListResult<Role> list = _business.ListNotRoles("", "", 10, 0, new() { Id = 1 }, connectionFake);

            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la inserción de un rol de una aplicación
        /// </summary>
        [Fact]
        public void ApplicationInsertRoleTest()
        {
            Role role = _business.InsertRole(new() { Id = 4 }, new() { Id = 1 }, new() { Id = 1 }, connectionFake);

            Assert.NotNull(role);
        }

        /// <summary>
        /// Prueba la inserción de un rol de una aplicación duplicado
        /// </summary>
        [Fact]
        public void ApplicationInsertRoleDuplicateTest()
        {
            _ = Assert.Throws<PersistentException>(() => _business.InsertRole(new() { Id = 1 }, new() { Id = 1 }, new() { Id = 1 }, connectionFake));
        }

        /// <summary>
        /// Prueba la eliminación de un rol de una aplicación
        /// </summary>
        [Fact]
        public void ApplicationDeleteRoleTest()
        {
            _ = _business.DeleteRole(new() { Id = 2 }, new() { Id = 1 }, new() { Id = 1 }, connectionFake);
            ListResult<Role> list = _business.ListRoles("r.idrole = 2", "", 10, 0, new() { Id = 1 }, connectionFake);

            Assert.Equal(0, list.Total);
        }
        #endregion
    }
}
