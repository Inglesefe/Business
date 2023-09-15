﻿using Business.Auth;
using Dal.Auth;
using Dal.Dto;
using Dal.Exceptions;
using Entities.Auth;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Data;

namespace Business.Test.Auth
{
    /// <summary>
    /// Realiza las pruebas sobre la clase de persistencia de usuarios
    /// </summary>
    [Collection("Tests")]
    public class UserTest
    {
        #region Attributes
        /// <summary>
        /// Configuración de la aplicación de pruebas
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Capa de negocio de los usuarios
        /// </summary>
        private readonly BusinessUser _business;
        #endregion

        #region Constructors
        /// <summary>
        /// Inicializa la configuración de la prueba
        /// </summary>
        public UserTest()
        {
            //Arrange
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, false)
                .AddEnvironmentVariables()
                .Build();
            Mock<IPersistentUser> mock = new();
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
            List<Tuple<User, Role>> users_roles = new()
            {
                new Tuple<User, Role>(users[0], roles[0]),
                new Tuple<User, Role>(users[0], roles[1]),
                new Tuple<User, Role>(users[1], roles[0]),
                new Tuple<User, Role>(users[1], roles[1])
            };
            mock.Setup(p => p.List("iduser = 1", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new ListResult<User>(users.Where(y => y.Id == 1).ToList(), 1));
            mock.Setup(p => p.List("idusuario = 1", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Throws<PersistentException>();
            mock.Setup(p => p.Read(It.IsAny<User>()))
                .Returns((User user) => users.Find(x => x.Id == user.Id) ?? new User());
            mock.Setup(p => p.ReadByLoginAndPassword(It.IsAny<User>(), It.IsAny<string>()))
                .Returns((User user, string password) => users.Find(x => x.Login == user.Login && password == "Prueba123" && x.Active) ?? new User());
            mock.Setup(p => p.ReadByLogin(It.IsAny<User>()))
                .Returns((User user) => users.Find(x => x.Login == user.Login) ?? new User());
            mock.Setup(p => p.Insert(It.IsAny<User>(), It.IsAny<User>()))
                .Returns((User user, User user1) =>
                {
                    if (users.Exists(x => x.Login == user.Login))
                    {
                        throw new PersistentException();
                    }
                    else
                    {
                        user.Id = users.Count + 1;
                        users.Add(user);
                        return user;
                    }
                });
            mock.Setup(p => p.Update(It.IsAny<User>(), It.IsAny<User>()))
                .Returns((User user, User user1) =>
                {
                    users.Where(x => x.Id == user.Id).ToList().ForEach(x => { x.Login = user.Login; x.Name = user.Name; x.Active = user.Active; });
                    return user;
                });
            mock.Setup(p => p.UpdatePassword(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<User>()))
                .Returns((User user, string password, User user1) =>
                {
                    return user;
                });
            mock.Setup(p => p.Delete(It.IsAny<User>(), It.IsAny<User>()))
                .Returns((User user, User user1) =>
                {
                    users = users.Where(x => x.Id != user.Id).ToList();
                    return user;
                });
            mock.Setup(p => p.ListRoles("", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<User>()))
                .Returns(new ListResult<Role>(users_roles.Where(x => x.Item1.Id == 1).Select(x => x.Item2).ToList(), 1));
            mock.Setup(p => p.ListRoles("r.idrole = 2", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<User>()))
                .Returns(new ListResult<Role>(new List<Role>(), 0));
            mock.Setup(p => p.ListRoles("idusuario = 1", It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<User>()))
                .Throws<PersistentException>();
            mock.Setup(p => p.ListNotRoles(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<User>()))
                .Returns((string filters, string orders, int limit, int offset, User user) =>
                {
                    List<Role> result = roles.Where(x => !users_roles.Exists(y => y.Item1.Id == user.Id && y.Item2.Id == x.Id)).ToList();
                    return new ListResult<Role>(result, result.Count);
                });
            mock.Setup(p => p.InsertRole(It.IsAny<Role>(), It.IsAny<User>(), It.IsAny<User>())).
                Returns((Role role, User user, User user1) =>
                {
                    if (users_roles.Exists(x => x.Item1.Id == user.Id && x.Item2.Id == role.Id))
                    {
                        throw new PersistentException();
                    }
                    else
                    {
                        users_roles.Add(new Tuple<User, Role>(user, role));
                        return role;
                    }
                });
            _business = new(mock.Object);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Prueba la consulta de un listado de usuarios con filtros, ordenamientos y límite
        /// </summary>
        [Fact]
        public void ListTest()
        {
            //Act
            ListResult<User> list = _business.List("iduser = 1", "name", 1, 0);

            //Assert
            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de usuarios con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void ListWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _business.List("idusuario = 1", "name", 1, 0));
        }

        /// <summary>
        /// Prueba la consulta de un usuario dado su identificador
        /// </summary>
        [Fact]
        public void ReadTest()
        {
            //Arrange
            User user = new() { Id = 1 };

            //Act
            user = _business.Read(user);

            //Assert
            Assert.Equal("leandrobaena@gmail.com", user.Login);
        }

        /// <summary>
        /// Prueba la consulta de un usuario que no existe dado su identificador
        /// </summary>
        [Fact]
        public void ReadNotFoundTest()
        {
            //Arrange
            User user = new() { Id = 10 };

            //Act
            user = _business.Read(user);

            //Assert
            Assert.Equal(0, user.Id);
        }

        /// <summary>
        /// Prueba la inserción de un usuario
        /// </summary>
        [Fact]
        public void InsertTest()
        {
            //Arrange
            User user = new() { Login = "insertado@prueba.com", Name = "Prueba 1", Active = true };

            //Act
            user = _business.Insert(user, new() { Id = 1 });

            //Assert
            Assert.NotEqual(0, user.Id);
        }

        /// <summary>
        /// Prueba la inserción de un usuario con login duplicado
        /// </summary>
        [Fact]
        public void InsertDuplicateTest()
        {
            //Act
            User user = new() { Login = "leandrobaena@gmail.com", Name = "Prueba insertar", Active = true };

            //Assert
            _ = Assert.Throws<PersistentException>(() => _business.Insert(user, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la actualización de un usuario
        /// </summary>
        [Fact]
        public void UpdateTest()
        {
            //Assert
            User user = new() { Id = 2, Login = "otrologin@gmail.com", Name = "Prueba actualizar", Active = false };
            User user2 = new() { Id = 2 };

            //Act
            _ = _business.Update(user, new() { Id = 1 });
            user2 = _business.Read(user2);

            //Assert
            Assert.NotEqual("actualizame@gmail.com", user2.Name);
            Assert.False(user2.Active);
        }

        /// <summary>
        /// Prueba la eliminación de un usuario
        /// </summary>
        [Fact]
        public void DeleteTest()
        {
            //Assert
            User user = new() { Id = 3 };
            User user2 = new() { Id = 3 };

            //Assert
            _ = _business.Delete(user, new() { Id = 1 });
            user2 = _business.Read(user2);

            //Assert
            Assert.Equal(0, user2.Id);
        }

        /// <summary>
        /// Prueba la consulta de un usuario dado su login y contraseña
        /// </summary>
        [Fact]
        public void ReadByLoginAndPasswordTest()
        {
            //Arrange
            User user = new() { Login = "leandrobaena@gmail.com" };

            //Act
            user = _business.ReadByLoginAndPassword(user, "FLWnwyoEz/7tYsnS+vxTVg==", _configuration["Aes:Key"] ?? "", _configuration["Aes:IV"] ?? "");

            //Assert
            Assert.NotEqual(0, user.Id);
        }

        /// <summary>
        /// Prueba la consulta de un usuario que no existe dado su login y password
        /// </summary>
        [Fact]
        public void ReadByLoginAndPasswordWithErrorTest()
        {
            //Arrange
            User user = new() { Login = "actualizame@gmail.com" };

            //Act
            user = _business.ReadByLoginAndPassword(user, "o2qMay2SrdjaZLxLFW1yQA==", _configuration["Aes:Key"] ?? "", _configuration["Aes:IV"] ?? "");

            //Assert
            Assert.Equal(0, user.Id);
        }

        /// <summary>
        /// Prueba la consulta de un usuario dado su login
        /// </summary>
        [Fact]
        public void ReadByLoginTest()
        {
            //Arrange
            User user = new() { Login = "leandrobaena@gmail.com" };

            //Act
            user = _business.ReadByLogin(user);

            //Assert
            Assert.NotEqual(0, user.Id);
        }

        /// <summary>
        /// Prueba la consulta de un usuario inactivo dado su login y password
        /// </summary>
        [Fact]
        public void ReadByLoginAndPasswordInactiveTest()
        {
            //Arrange
            User user = new() { Login = "inactivo@gmail.com" };

            //Act
            user = _business.ReadByLoginAndPassword(user, "FLWnwyoEz/7tYsnS+vxTVg==", _configuration["Aes:Key"] ?? "", _configuration["Aes:IV"] ?? "");

            //Assert
            Assert.Equal(0, user.Id);
        }

        /// <summary>
        /// Prueba la actualización de la contraseña de un usuario
        /// </summary>
        [Fact]
        public void UpdatePasswordTest()
        {
            //Arrange
            User user = new() { Id = 1, Login = "leandrobaena@gmail.com" };

            //Act
            _ = _business.UpdatePassword(user, "FLWnwyoEz/7tYsnS+vxTVg==", _configuration["Aes:Key"] ?? "", _configuration["Aes:IV"] ?? "", new() { Id = 1 });
            user = _business.ReadByLoginAndPassword(user, "FLWnwyoEz/7tYsnS+vxTVg==", _configuration["Aes:Key"] ?? "", _configuration["Aes:IV"] ?? "");

            //Assert
            Assert.NotEqual(0, user.Id);
        }

        /// <summary>
        /// Prueba la consulta de un listado de roles de un usuario con filtros, ordenamientos y límite
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
        /// Prueba la consulta de un listado de roles de un usuario con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void ListRolesWithErrorTest()
        {
            //Act, Assert
            _ = Assert.Throws<PersistentException>(() => _business.ListRoles("idusuario = 1", "name", 10, 0, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la consulta de un listado de roles no asignados a un usuario con filtros, ordenamientos y límite
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
        /// Prueba la inserción de un rol de un usuario
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
        /// Prueba la inserción de un rol de un usuario duplicado
        /// </summary>
        [Fact]
        public void InsertRoleDuplicateTest()
        {
            //Act, Assert
            _ = Assert.Throws<PersistentException>(() => _business.InsertRole(new() { Id = 1 }, new() { Id = 1 }, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la eliminación de un rol de un usuario
        /// </summary>
        [Fact]
        public void DeleteRoleTest()
        {
            //Act
            _ = _business.DeleteRole(new() { Id = 2 }, new() { Id = 1 }, new() { Id = 1 });
            ListResult<Role> list = _business.ListRoles("r.idrole = 2", "", 10, 0, new() { Id = 1 });

            //Assert
            Assert.Equal(0, list.Total);
        }
        #endregion
    }
}
