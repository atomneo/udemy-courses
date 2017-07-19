namespace Vidly.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SeedUsers : DbMigration
    {
        public override void Up()
        {
            Sql(@"INSERT INTO [dbo].[AspNetUsers] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName]) VALUES (N'223570a4-263a-4f07-b7b7-fd2ef03c8e3d', N'neo1k4+vidly_guest@gmail.com', 0, N'AIrpnh5bQY4JoASWHqKQ5KxBsT2qt/dH1rLNo9zfPF7C8yg9fbaX85uTTCckJBlUIQ==', N'451bb474-8bd3-49ce-9432-86dc258d4fc7', NULL, 0, 0, NULL, 1, 0, N'neo1k4+vidly_guest@gmail.com')
INSERT INTO [dbo].[AspNetUsers] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName]) VALUES (N'aa26ef85-a1f6-48e6-862d-cd9b33e56f2a', N'neo1k4+vidly_admin@gmail.com', 0, N'AG8xRDw53XUhKiCVhH8UUkz30KuC/ecneiDD51snKip9PLhG9DoT32JrcgoJ8U1FWw==', N'4689162f-5570-436c-8493-600d48e3462d', NULL, 0, 0, NULL, 1, 0, N'neo1k4+vidly_admin@gmail.com')

INSERT INTO [dbo].[AspNetRoles] ([Id], [Name]) VALUES (N'423f7220-de14-40f9-bc41-75e733630818', N'CanManageMovies')

INSERT INTO [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'aa26ef85-a1f6-48e6-862d-cd9b33e56f2a', N'423f7220-de14-40f9-bc41-75e733630818')
");
        }
        
        public override void Down()
        {
        }
    }
}
