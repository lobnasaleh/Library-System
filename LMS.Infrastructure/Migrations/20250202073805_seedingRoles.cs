using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class seedingRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //adding Roles
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[]
                {
                    "Id","Name","NormalizedName","ConcurrencyStamp"
                },
                values: new object[]
                {
                    Guid.NewGuid().ToString(),"Member","Member".ToUpper(),Guid.NewGuid().ToString()
                }
                );
                migrationBuilder.InsertData(
                 table: "AspNetRoles",
                 columns: new[]
                {
                    "Id","Name","NormalizedName","ConcurrencyStamp"
                },
                  values: new object[]
                {
                    "0e94d03d-3f1c-444d-93c7-bfa16aef605f","Librarian","Librarian".ToUpper(),Guid.NewGuid().ToString()
                }
                );
            //Inserting a Librarian user
            var hasher = new PasswordHasher<IdentityUser>();
            //hasher.HashPassword(null, "This@librarian12")

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[]
                {
            "Id", "UserName", "NormalizedUserName", "Email", "NormalizedEmail", "EmailConfirmed",
            "PasswordHash", "SecurityStamp", "ConcurrencyStamp", "PhoneNumber", "PhoneNumberConfirmed",
            "TwoFactorEnabled", "LockoutEnabled", "AccessFailedCount",
            "Name", "Image", "DateOfBirth", "Gender"
                },
                values: new object[]
                {
            "fce1b435-c815-4fbf-aab3-b4aedf241737", "librarian1", "LIBRARIAN1", "librarian@gmail.com", "LIBRARIAN@GMAIL.COM", true,
            "AQAAAAIAAYagAAAAEAjxH1IvtznjkJp0dGjYFlhWJ8VFbB9T2Xmu/GSxNakkgKU01C6qUOjcCfnZSeKGIw==", Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), null, false,
            false, true, 0,
            "Librarian1", null, new DateTime(1985, 1, 1), 1 // Assuming Gender is an enum (1 =Female)
                }
            );
            // Assign Librarian User to Librarian Role 
            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "UserId", "RoleId" },
                values: new object[] { "fce1b435-c815-4fbf-aab3-b4aedf241737", "0e94d03d-3f1c-444d-93c7-bfa16aef605f" }
            );



        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("Delete form [AspNetRoles]");
        }
    }
}
