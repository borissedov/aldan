using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Aldan.Data.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GenericAttribute",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EntityId = table.Column<int>(nullable: false),
                    KeyGroup = table.Column<string>(maxLength: 400, nullable: false),
                    Key = table.Column<string>(maxLength: 400, nullable: false),
                    Value = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenericAttribute", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MessageTemplate",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 200, nullable: false),
                    BccEmailAddresses = table.Column<string>(maxLength: 200, nullable: true),
                    Subject = table.Column<string>(maxLength: 1000, nullable: true),
                    Body = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageTemplate", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QueuedEmail",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    From = table.Column<string>(maxLength: 500, nullable: false),
                    FromName = table.Column<string>(maxLength: 500, nullable: true),
                    To = table.Column<string>(maxLength: 500, nullable: false),
                    ToName = table.Column<string>(maxLength: 500, nullable: true),
                    ReplyTo = table.Column<string>(maxLength: 500, nullable: true),
                    ReplyToName = table.Column<string>(maxLength: 500, nullable: true),
                    CC = table.Column<string>(maxLength: 500, nullable: true),
                    Bcc = table.Column<string>(maxLength: 500, nullable: true),
                    Subject = table.Column<string>(maxLength: 1000, nullable: true),
                    Body = table.Column<string>(nullable: true),
                    CreatedOnUtc = table.Column<DateTime>(nullable: false),
                    SentTries = table.Column<int>(nullable: false),
                    SentOnUtc = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QueuedEmail", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ScheduleTask",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    Seconds = table.Column<int>(nullable: false),
                    Type = table.Column<string>(nullable: false),
                    Enabled = table.Column<bool>(nullable: false),
                    StopOnError = table.Column<bool>(nullable: false),
                    LastStartUtc = table.Column<DateTime>(nullable: true),
                    LastEndUtc = table.Column<DateTime>(nullable: true),
                    LastSuccessUtc = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleTask", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserGuid = table.Column<Guid>(nullable: false),
                    Email = table.Column<string>(maxLength: 1000, nullable: true),
                    Password = table.Column<string>(nullable: true),
                    PasswordSalt = table.Column<string>(nullable: true),
                    LastIpAddress = table.Column<string>(nullable: true),
                    CreatedOnUtc = table.Column<DateTime>(nullable: false),
                    LastLoginDateUtc = table.Column<DateTime>(nullable: true),
                    LastActivityDateUtc = table.Column<DateTime>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false),
                    Role = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Log",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LogLevelId = table.Column<int>(nullable: false),
                    ShortMessage = table.Column<string>(nullable: false),
                    FullMessage = table.Column<string>(nullable: true),
                    IpAddress = table.Column<string>(maxLength: 200, nullable: true),
                    UserId = table.Column<int>(nullable: true),
                    PageUrl = table.Column<string>(nullable: true),
                    ReferrerUrl = table.Column<string>(nullable: true),
                    CreatedOnUtc = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Log", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Log_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "MessageTemplate",
                columns: new[] { "Id", "BccEmailAddresses", "Body", "IsActive", "Name", "Subject" },
                values: new object[,]
                {
                    { 1, null, @"<a href=""%Platform.URL%"">%Platform.Name%</a>
                <br />
                <br />
                To change your password <a href=""%User.PasswordRecoveryURL%"">click here</a>.
                <br />
                <br />
                %Platform.Name%
                ", true, "User.PasswordRecovery", "%Platform.Name%. Password recovery" },
                    { 2, null, @"We welcome you to <a href=""%Platform.URL%""> %Platform.Name%</a>.
                <br />
                <br />
                You can now take part in the various services we have to offer you. Some of these services include:
                <br />
                <br />
                Permanent Cart - Any products added to your online cart remain there until you remove them, or check them out.
                <br />
                Address Book - We can now deliver your products to another address other than yours! This is perfect to send birthday gifts direct to the birthday-person themselves.
                <br />
                Order History - View your history of purchases that you have made with us.
                <br />
                Products Reviews - Share your opinions on products with our other users.
                <br />
                <br />
                For help with any of our online services, please email the platform-owner: <a href=""mailto:%Platform.Email%"">%Platform.Email%</a>.
                <br />
                <br />
                Note: This email address was provided on our registration page. If you own the email and did not register on our site, please send an email to <a href=""mailto:%Platform.Email%"">%Platform.Email%</a>.
                ", true, "User.WelcomeMessage", "Welcome to %Platform.Name%" },
                    { 3, null, @"<p>
                <a href=""%Platform.URL%"">%Platform.Name%</a>
                <br />
                <br />
                A new user registered with your platform. Below are the user's details:
                <br />
                Full name: %User.FullName%
                <br />
                Email: %User.Email%
                </p>
                ", true, "NewUser.Notification", "%Platform.Name%. New user registration" },
                    { 4, null, @"<p>
                %ContactUs.Body%
                </p>
                ", true, "Service.ContactUs", "%Platform.Name%. Contact us" }
                });

            migrationBuilder.InsertData(
                table: "ScheduleTask",
                columns: new[] { "Id", "Enabled", "LastEndUtc", "LastStartUtc", "LastSuccessUtc", "Name", "Seconds", "StopOnError", "Type" },
                values: new object[] { 1, true, null, null, null, "Send emails", 60, false, "Aldan.Services.Messages.QueuedMessagesSendTask, Aldan.Services" });

            migrationBuilder.CreateIndex(
                name: "IX_Log_UserId",
                table: "Log",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GenericAttribute");

            migrationBuilder.DropTable(
                name: "Log");

            migrationBuilder.DropTable(
                name: "MessageTemplate");

            migrationBuilder.DropTable(
                name: "QueuedEmail");

            migrationBuilder.DropTable(
                name: "ScheduleTask");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
