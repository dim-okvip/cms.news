using System.Text.Json;

namespace CMS.News.DAL
{
    public class DataSeeder
    {
        public static void SeedData(NewsDbContext context)
        {
            Guid siteVN = new Guid("06baa6e3-e010-4307-b702-2740066aeeca");
            Guid siteEN = new Guid("48ed5b71-66dc-4725-9604-4c042e45fa3f");

            if (!context.Sites.Any())
            {
                var listSite = new List<Site>() {
                    new Site()
                    {
                        Id = siteVN,
                        Name = "Trang thông tin tiếng Việt",
                        Address = "https://localhost:7277",
                        CreatedTime = DateTime.Now,
                    },
                     new Site()
                    {
                        Id = siteEN,
                        Name = "Trang thông tin tiếng Anh",
                        Address = "http://localhost:5280",
                        CreatedTime = DateTime.Now,
                    },
                };

                context.Sites.AddRange(listSite);
                context.SaveChanges();
            }

            Guid roleAdmin = Guid.NewGuid();
            Guid roleBienTap = Guid.NewGuid();
            Guid roleCTV = Guid.NewGuid();

            if (!context.Roles.Any())
            {
                var listRole = new List<Role>
                {
                    new Role()
                    {
                        Id = roleAdmin,
                        Name = "ADMIN",
                        Description = "Quản trị hệ thống",
                        CreatedTime = DateTime.Now
                    },
                    new Role()
                    {
                        Id = roleBienTap,
                        Name = "Tổng biên tập",
                        Description = "Biên soạn bài viết",
                        CreatedTime = DateTime.Now
                    },
                    new Role()
                    {
                        Id = roleCTV,
                        Name = "Cộng tác viên/ Phóng  viên",
                        Description = "Cộng tác viên/ Phóng  viên",
                        CreatedTime = DateTime.Now
                    },
                };
                context.Roles.AddRange(listRole);
                context.SaveChanges();
            }

            Guid rightUploadFile = Guid.NewGuid();

            Guid rightRoleManagement = Guid.NewGuid();
            Guid rightUserManagement = Guid.NewGuid();
            Guid rightStorylineManagement = Guid.NewGuid();

            if (!context.Rights.Any())
            {
                var listRight = new List<Right>
                {
                    new Right()
                    {
                        Id = rightUploadFile,
                        Code = RightName.UPLOAD_FILE,
                        Name = "Upload file",
                        Description = "Tải file lên server"
                    },
                    new Right()
                    {
                        Id = rightRoleManagement,
                        Code = RightName.ROLE_MANAGEMENT,
                        Name = "Quản lí vai trò",
                        Description = "Quản lí vai trò"
                    },
                    new Right()
                    {
                        Id = rightUserManagement,
                        Code = RightName.USER_MANAGEMENT,
                        Name = "Quản lí người dùng",
                        Description = "Quản lí người dùng"
                    },
                    new Right()
                    {
                        Id = rightStorylineManagement,
                        Code = RightName.STORYLINE_MANAGEMENT,
                        Name = "Quản lí bài viết",
                        Description = "Quản lí bài viết"
                    },
                };
                context.Rights.AddRange(listRight);
                context.SaveChanges();
            }

            if (!context.RoleRights.Any())
            {
                List<Guid> listActionNormalRole = new() { rightUploadFile, rightStorylineManagement };
                List<Guid> listActionOfAdminRole = new();
                listActionOfAdminRole.AddRange(listActionNormalRole);
                listActionOfAdminRole.AddRange(new List<Guid> {
                    rightRoleManagement,
                    rightUserManagement,
                });
                List<Guid> listNormalRole = new() { roleBienTap, roleCTV };

                List<RoleRight> listRoleAction = new();
                #region Normal role
                foreach (var roleId in listNormalRole)
                {
                    foreach (var actionId in listActionNormalRole)
                    {
                        listRoleAction.Add(new RoleRight()
                        {
                            RoleId = roleId,
                            RightId = actionId,
                        });
                    }
                }
                #endregion

                #region Admin role
                foreach (var rightId in listActionOfAdminRole)
                {
                    listRoleAction.Add(new RoleRight()
                    {
                        RoleId = roleAdmin,
                        RightId = rightId,
                    });
                }
                #endregion

                context.RoleRights.AddRange(listRoleAction);
                context.SaveChanges();
            }

            Guid userAdmin = new Guid("db5eaa2b-014f-413c-b6c3-676020b51a5a");
            Guid userShbet = new Guid("ce2f4002-17aa-4a24-89aa-2e9c59f06d0b");
            Guid userHi88 = new Guid("206f154a-b681-4e6c-aea9-3196db29dc4d");
            Guid userTuktuk = new Guid("0f2fcf13-6241-4ac8-95b2-440cc561a6ea");
            Guid user789Bet = new Guid("3f442527-484f-437c-adf5-a0409b915bb9");
            Guid userSweetheart = new Guid("cddb7bd4-8e6f-43c4-af02-a42d2fe37ad0");
            Guid userNew88 = new Guid("e12b7d41-d233-4c5d-8f30-b1c5905836ac");
            Guid userF8bet = new Guid("a1f204b8-52df-4edf-9797-bab7da41c00a");
            Guid user123 = new Guid("3264f87f-4bec-4f32-9ac3-c693e3bb4a2e");
            Guid userJun88 = new Guid("4e0b43be-d7f2-4afe-b813-ea8baea5f82b");

            if (!context.Users.Any())
            {
                List<User> listUser = ReadFile<List<User>>("User.json");
                context.Users.AddRange(listUser);
                context.SaveChanges();
            }

            if (!context.UserRoles.Any())
            {
                var listUserRole = new List<UserRole>()
                {
                    new UserRole()
                    {
                        UserId = userAdmin,
                        RoleId = roleAdmin,
                        SiteId = siteVN
                    },
                    new UserRole()
                    {
                        UserId = userAdmin,
                        RoleId = roleAdmin,
                        SiteId = siteEN
                    },
                    new UserRole()
                    {
                        UserId = userShbet,
                        RoleId = roleBienTap,
                        SiteId = siteEN
                    },
                    new UserRole()
                    {
                        UserId = userHi88,
                        RoleId = roleBienTap,
                        SiteId = siteVN
                    },
                    new UserRole()
                    {
                        UserId = userTuktuk,
                        RoleId = roleBienTap,
                        SiteId = siteEN
                    },
                    new UserRole()
                    {
                        UserId = user789Bet,
                        RoleId = roleBienTap,
                        SiteId = siteVN
                    },
                    new UserRole()
                    {
                        UserId = userSweetheart,
                        RoleId = roleBienTap,
                        SiteId = siteEN
                    },
                    new UserRole()
                    {
                        UserId = userNew88,
                        RoleId = roleBienTap,
                        SiteId = siteVN
                    },
                    new UserRole()
                    {
                        UserId = userF8bet,
                        RoleId = roleBienTap,
                        SiteId = siteEN
                    },
                    new UserRole()
                    {
                        UserId = user123,
                        RoleId = roleBienTap,
                        SiteId = siteVN
                    },
                    new UserRole()
                    {
                        UserId = userJun88,
                        RoleId = roleBienTap,
                        SiteId = siteEN
                    },
                };
                context.UserRoles.AddRange(listUserRole);
                context.SaveChanges();
            }
        }

        private static T ReadFile<T>(string fileName)
        {
            string basePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Data");

            string path = Path.Combine(basePath, fileName);
            string value = File.ReadAllText(path);

            return JsonSerializer.Deserialize<T>(value, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        }
    }
}
