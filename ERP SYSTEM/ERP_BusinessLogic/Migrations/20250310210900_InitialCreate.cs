using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ERP_BusinessLogic.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TB_Category",
                columns: table => new
                {
                    categoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    categoryName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    categoryDescription = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_Category", x => x.categoryId);
                });

            migrationBuilder.CreateTable(
                name: "TB_Customer",
                columns: table => new
                {
                    CustomerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sex = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Age = table.Column<decimal>(type: "decimal(18,0)", nullable: false),
                    Image = table.Column<byte[]>(type: "image", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TB_Custo__A4AE64D85267C97A", x => x.CustomerId);
                });

            migrationBuilder.CreateTable(
                name: "TB_Distributor",
                columns: table => new
                {
                    distributorId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    distributorName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    phoneNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    address = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_Distributor", x => x.distributorId);
                });

            migrationBuilder.CreateTable(
                name: "TB_FMS_Account",
                columns: table => new
                {
                    AccID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    AccDebit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    AccCredit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    IncreaseMode = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TB_FMS_A__91CBC39804A449CE", x => x.AccID);
                });

            migrationBuilder.CreateTable(
                name: "TB_FMS_Category",
                columns: table => new
                {
                    CatID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CatName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CatDescription = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TB_FMS_C__6A1C8ADAB474143C", x => x.CatID);
                });

            migrationBuilder.CreateTable(
                name: "TB_FMS_Statement",
                columns: table => new
                {
                    StaID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StaName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StaBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    StaDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TB_FMS_S__BA875A61CF47BBBC", x => x.StaID);
                });

            migrationBuilder.CreateTable(
                name: "TB_FMS_StatementTemplate",
                columns: table => new
                {
                    TempID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TempName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TempDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TB_FMS_S__06C703E10B86E807", x => x.TempID);
                });

            migrationBuilder.CreateTable(
                name: "TB_HRManagerDetails",
                columns: table => new
                {
                    HRId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HRFullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HREmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Age = table.Column<int>(type: "int", nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Salary = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_HRManagerDetails", x => x.HRId);
                });

            migrationBuilder.CreateTable(
                name: "TB_Question",
                columns: table => new
                {
                    QuestionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Question = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TB_Quest__0DC06FAC6B4573AC", x => x.QuestionId);
                });

            migrationBuilder.CreateTable(
                name: "TB_RawMaterial",
                columns: table => new
                {
                    materialId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    materialName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    materialDescription = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_Supplier", x => x.materialId);
                });

            migrationBuilder.CreateTable(
                name: "TB_Reporter",
                columns: table => new
                {
                    ReporterID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReporterFirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReporterLastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReporterEntryDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TB_Repor__4406548BB7FD1991", x => x.ReporterID);
                });

            migrationBuilder.CreateTable(
                name: "TB_Supplier",
                columns: table => new
                {
                    supplierId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    supplierName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    supplierDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    phoneNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdverageDeliveryTimeInDays = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_Supplier_1", x => x.supplierId);
                });

            migrationBuilder.CreateTable(
                name: "TbDistributionOrderStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TbDistributionOrderStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TbManufacturingStatus",
                columns: table => new
                {
                    statusId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    statusName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TbManufacturingStatus", x => x.statusId);
                });

            migrationBuilder.CreateTable(
                name: "TbOrderStatus_Suppliers",
                columns: table => new
                {
                    OrderStatusId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderStatusName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TbOrderStatus_Suppliers", x => x.OrderStatusId);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TB_Product",
                columns: table => new
                {
                    productId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    productName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    productDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    purchasePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    salesPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    categoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tb_Product", x => x.productId);
                    table.ForeignKey(
                        name: "FK_Product_PK_Category",
                        column: x => x.categoryId,
                        principalTable: "TB_Category",
                        principalColumn: "categoryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TB_Task",
                columns: table => new
                {
                    TaskId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    TaskName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TaskDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    TaskDesc = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TB_Task__7C6949B1BE263896", x => x.TaskId);
                    table.ForeignKey(
                        name: "FK_TB_Task_TB_Customer",
                        column: x => x.CustomerId,
                        principalTable: "TB_Customer",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TB_ToDoList",
                columns: table => new
                {
                    ToDoListId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ToDoListName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ToDoListDesc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TB_ToDoL__1BEFD56CDF0D5E6E", x => x.ToDoListId);
                    table.ForeignKey(
                        name: "FK__TB_ToDoLi__Custo__45F365D3",
                        column: x => x.CustomerId,
                        principalTable: "TB_Customer",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TB_FMS_JournalEntry",
                columns: table => new
                {
                    JEID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JEName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JEDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JECredit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    JEDebit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    JEDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    JEAccount1 = table.Column<int>(type: "int", nullable: true),
                    JEAccount2 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TB_FMS_J__703C596C510B154B", x => x.JEID);
                    table.ForeignKey(
                        name: "FK__TB_FMS_Jo__JEAcc__10566F31",
                        column: x => x.JEAccount1,
                        principalTable: "TB_FMS_Account",
                        principalColumn: "AccID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK__TB_FMS_Jo__JEAcc__114A936A",
                        column: x => x.JEAccount2,
                        principalTable: "TB_FMS_Account",
                        principalColumn: "AccID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TB_FMS_AccCat",
                columns: table => new
                {
                    AccID = table.Column<int>(type: "int", nullable: false),
                    CatID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("composite_pk category_account", x => new { x.AccID, x.CatID });
                    table.ForeignKey(
                        name: "FK__TB_FMS_Ac__AccID__114A936A",
                        column: x => x.AccID,
                        principalTable: "TB_FMS_Account",
                        principalColumn: "AccID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK__TB_FMS_Ac__CatID__123EB7A3",
                        column: x => x.CatID,
                        principalTable: "TB_FMS_Category",
                        principalColumn: "CatID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TB_FMS_StatementAccounts",
                columns: table => new
                {
                    AccName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StaID = table.Column<int>(type: "int", nullable: false),
                    AccBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("composite primary key", x => new { x.AccName, x.StaID });
                    table.ForeignKey(
                        name: "FK__TB_FMS_St__StaID__18EBB532",
                        column: x => x.StaID,
                        principalTable: "TB_FMS_Statement",
                        principalColumn: "StaID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TB_FMS_TemplateAccounts",
                columns: table => new
                {
                    AccID = table.Column<int>(type: "int", nullable: false),
                    TempID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("composite_pk template_account", x => new { x.AccID, x.TempID });
                    table.ForeignKey(
                        name: "FK__TB_FMS_Te__AccID__160F4887",
                        column: x => x.AccID,
                        principalTable: "TB_FMS_Account",
                        principalColumn: "AccID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK__TB_FMS_Te__TempI__17036CC0",
                        column: x => x.TempID,
                        principalTable: "TB_FMS_StatementTemplate",
                        principalColumn: "TempID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TB_EmployeeDetails",
                columns: table => new
                {
                    EmployeeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeFullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TaxWithholding = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    HoursWorked = table.Column<int>(type: "int", nullable: true),
                    PhotoFileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOfJoining = table.Column<DateTime>(type: "datetime", nullable: true),
                    HRManagerId = table.Column<int>(type: "int", nullable: false),
                    AttendenceTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    Holidays = table.Column<DateTime>(type: "date", nullable: true),
                    EmployeeSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_EmployeeDetails", x => x.EmployeeID);
                    table.ForeignKey(
                        name: "FK_TB_EmployeeDetails_TB_HRManagerDetails",
                        column: x => x.HRManagerId,
                        principalTable: "TB_HRManagerDetails",
                        principalColumn: "HRId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TB_Survey",
                columns: table => new
                {
                    SurveyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SurveyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SurveyDesc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    QuestionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TB_Surve__A5481F7D0DEB2CDB", x => x.SurveyId);
                    table.ForeignKey(
                        name: "FK__TB_Survey__Custo__48CFD27E",
                        column: x => x.CustomerId,
                        principalTable: "TB_Customer",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__TB_Survey__Quest__49C3F6B7",
                        column: x => x.QuestionId,
                        principalTable: "TB_Question",
                        principalColumn: "QuestionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TB_RawMaterialsInventory",
                columns: table => new
                {
                    materialId = table.Column<int>(type: "int", nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    shippingDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    monthlyCosts = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    area = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    reorderingPoint = table.Column<int>(type: "int", nullable: false),
                    HasReachedROP = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TB_RawMa__99B653FDB26AF845", x => x.materialId);
                    table.ForeignKey(
                        name: "FK_RawMaterialsInventory_PK_RawMaterials",
                        column: x => x.materialId,
                        principalTable: "TB_RawMaterial",
                        principalColumn: "materialId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TB_Adminstrator",
                columns: table => new
                {
                    AdminID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReporterID_FK = table.Column<int>(type: "int", nullable: false),
                    AdminFirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdminLastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdminEntryDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TB_Admin__719FE4E800A4E6F2", x => x.AdminID);
                    table.ForeignKey(
                        name: "FK_TB_Adminstrator_TB_Reporter",
                        column: x => x.ReporterID_FK,
                        principalTable: "TB_Reporter",
                        principalColumn: "ReporterID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TB_SupplyingMaterialDetails",
                columns: table => new
                {
                    supplierId = table.Column<int>(type: "int", nullable: false),
                    materialId = table.Column<int>(type: "int", nullable: false),
                    pricePerUnit = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("COM_PK_supplierId_materialId", x => new { x.supplierId, x.materialId });
                    table.ForeignKey(
                        name: "FK_SupplyingMaterialDetails_PK_RawMaterial",
                        column: x => x.materialId,
                        principalTable: "TB_RawMaterial",
                        principalColumn: "materialId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SupplyingMaterialDetails_PK_Supplier",
                        column: x => x.supplierId,
                        principalTable: "TB_Supplier",
                        principalColumn: "supplierId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TB_DistributionOrder",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DistributorId = table.Column<int>(type: "int", nullable: false),
                    totalQty = table.Column<int>(type: "int", nullable: false),
                    SubTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    totalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    orderingDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    expectedArrivalDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    orderStatusId = table.Column<int>(type: "int", nullable: false, defaultValueSql: "((1))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_DistributionOrder", x => x.Id);
                    table.ForeignKey(
                        name: "FK_distributionOrder_PK_Distributor",
                        column: x => x.DistributorId,
                        principalTable: "TB_Distributor",
                        principalColumn: "distributorId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TB_DistributionOrder_TbDistributionOrderStatus_orderStatusId",
                        column: x => x.orderStatusId,
                        principalTable: "TbDistributionOrderStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TbOrder_Suppliers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupplierId = table.Column<int>(type: "int", nullable: false),
                    TotalQty = table.Column<int>(type: "int", nullable: false),
                    SubTotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ShippingCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OrderStatusId = table.Column<int>(type: "int", nullable: false),
                    OrderingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpectedArrivalDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TbOrder_Suppliers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TbOrder_Suppliers_TB_Supplier_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "TB_Supplier",
                        principalColumn: "supplierId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TbOrder_Suppliers_TbOrderStatus_Suppliers_OrderStatusId",
                        column: x => x.OrderStatusId,
                        principalTable: "TbOrderStatus_Suppliers",
                        principalColumn: "OrderStatusId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TB_FinishedProductsInventory",
                columns: table => new
                {
                    productId = table.Column<int>(type: "int", nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    shippingDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    monthlyCosts = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    area = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    reorderingPoint = table.Column<int>(type: "int", nullable: false),
                    HasReachedROP = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_FinishedProductsInventory", x => x.productId);
                    table.ForeignKey(
                        name: "FK__TB_Finish__produ__3C69FB99",
                        column: x => x.productId,
                        principalTable: "TB_Product",
                        principalColumn: "productId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TbManufacturingOrders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductManufacturedId = table.Column<int>(type: "int", nullable: false),
                    QtyToManufacture = table.Column<int>(type: "int", nullable: false),
                    ManufacturingCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StartingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FinishingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ManufacturingStatusId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TbManufacturingOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TbManufacturingOrders_TB_Product_ProductManufacturedId",
                        column: x => x.ProductManufacturedId,
                        principalTable: "TB_Product",
                        principalColumn: "productId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TbManufacturingOrders_TbManufacturingStatus_ManufacturingStatusId",
                        column: x => x.ManufacturingStatusId,
                        principalTable: "TbManufacturingStatus",
                        principalColumn: "statusId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TB_EmployeeTaskDetails",
                columns: table => new
                {
                    TaskId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaskDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TaskAssignedTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    TaskDeadlineTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    BounsHours = table.Column<int>(type: "int", nullable: true),
                    EmplyeeId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_EmployeeTaskDetails", x => x.TaskId);
                    table.ForeignKey(
                        name: "FK_TB_EmployeeTaskDetails_TB_EmployeeDetails",
                        column: x => x.EmplyeeId,
                        principalTable: "TB_EmployeeDetails",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TB_EmployeeTrainning",
                columns: table => new
                {
                    TrainnningId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TrainningType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrainningDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    HRMangerId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_EmployeeTrainning", x => x.TrainnningId);
                    table.ForeignKey(
                        name: "FK_TB_EmployeeTrainning_TB_EmployeeDetails",
                        column: x => x.EmployeeId,
                        principalTable: "TB_EmployeeDetails",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TB_EmployeeTrainning_TB_HRManagerDetails",
                        column: x => x.HRMangerId,
                        principalTable: "TB_HRManagerDetails",
                        principalColumn: "HRId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TB_Recuirement",
                columns: table => new
                {
                    RecuirementId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RecuirementCode = table.Column<int>(type: "int", nullable: true),
                    RecuirementPosition = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecuirementDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecuirementDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    HRManagerId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_Recuirement", x => x.RecuirementId);
                    table.ForeignKey(
                        name: "FK_TB_Recuirement_TB_EmployeeDetails",
                        column: x => x.EmployeeId,
                        principalTable: "TB_EmployeeDetails",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TB_Recuirement_TB_HRManagerDetails",
                        column: x => x.HRManagerId,
                        principalTable: "TB_HRManagerDetails",
                        principalColumn: "HRId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TB_VisualReport",
                columns: table => new
                {
                    ReportID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    R_ReporterID = table.Column<int>(type: "int", nullable: false),
                    R_AdminID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TB_Visua__D5BD48E54B3BA761", x => x.ReportID);
                    table.ForeignKey(
                        name: "FK_TB_VisualReport_TB_Adminstrator",
                        column: x => x.R_AdminID,
                        principalTable: "TB_Adminstrator",
                        principalColumn: "AdminID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TB_VisualReport_TB_Reporter",
                        column: x => x.R_ReporterID,
                        principalTable: "TB_Reporter",
                        principalColumn: "ReporterID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TB_DistributionOrderDetails",
                columns: table => new
                {
                    distributionOrderId = table.Column<int>(type: "int", nullable: false),
                    productId = table.Column<int>(type: "int", nullable: false),
                    qty = table.Column<int>(type: "int", nullable: false),
                    price = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("COM_PK_distributionOrderId_productId", x => new { x.distributionOrderId, x.productId });
                    table.ForeignKey(
                        name: "FK_DistributionOrderDetails_PK_DistributionOrder",
                        column: x => x.distributionOrderId,
                        principalTable: "TB_DistributionOrder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DistributionOrderDetails_PK_Products",
                        column: x => x.productId,
                        principalTable: "TB_Product",
                        principalColumn: "productId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TbOrderDetails_Suppliers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderedRawMaterials_MaterialId = table.Column<int>(type: "int", nullable: true),
                    OrderedRawMaterials_MaterialName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrderedRawMaterials_SalesPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TbOrder_SupplierId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TbOrderDetails_Suppliers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TbOrderDetails_Suppliers_TbOrder_Suppliers_TbOrder_SupplierId",
                        column: x => x.TbOrder_SupplierId,
                        principalTable: "TbOrder_Suppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TB_ManufacturingOrderDetails",
                columns: table => new
                {
                    manfactoringOrderId = table.Column<int>(type: "int", nullable: false),
                    rawMaterialId = table.Column<int>(type: "int", nullable: false),
                    rawMaterialQtyUsed = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_ManufacturingOrderDetails", x => new { x.manfactoringOrderId, x.rawMaterialId });
                    table.ForeignKey(
                        name: "FK_TB_ManufacturingOrderDetails_TB_ManufacturingOrder",
                        column: x => x.manfactoringOrderId,
                        principalTable: "TbManufacturingOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TB_ManufacturingOrderDetails_TB_RawMaterial",
                        column: x => x.rawMaterialId,
                        principalTable: "TB_RawMaterial",
                        principalColumn: "materialId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TB_InterviewDetails",
                columns: table => new
                {
                    InterviewId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InterviewDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    InterviewResult = table.Column<bool>(type: "bit", nullable: false),
                    RecuriementId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_InterviewDetails", x => x.InterviewId);
                    table.ForeignKey(
                        name: "FK_TB_InterviewDetails_TB_Recuirement",
                        column: x => x.RecuriementId,
                        principalTable: "TB_Recuirement",
                        principalColumn: "RecuirementId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TB_Adminstrator_ReporterID_FK",
                table: "TB_Adminstrator",
                column: "ReporterID_FK");

            migrationBuilder.CreateIndex(
                name: "IX_TB_DistributionOrder_DistributorId",
                table: "TB_DistributionOrder",
                column: "DistributorId");

            migrationBuilder.CreateIndex(
                name: "IX_TB_DistributionOrder_orderStatusId",
                table: "TB_DistributionOrder",
                column: "orderStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_TB_DistributionOrderDetails_productId",
                table: "TB_DistributionOrderDetails",
                column: "productId");

            migrationBuilder.CreateIndex(
                name: "IX_TB_EmployeeDetails_HRManagerId",
                table: "TB_EmployeeDetails",
                column: "HRManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_TB_EmployeeTaskDetails_EmplyeeId",
                table: "TB_EmployeeTaskDetails",
                column: "EmplyeeId");

            migrationBuilder.CreateIndex(
                name: "IX_TB_EmployeeTrainning_EmployeeId",
                table: "TB_EmployeeTrainning",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_TB_EmployeeTrainning_HRMangerId",
                table: "TB_EmployeeTrainning",
                column: "HRMangerId");

            migrationBuilder.CreateIndex(
                name: "IX_TB_FMS_AccCat_CatID",
                table: "TB_FMS_AccCat",
                column: "CatID");

            migrationBuilder.CreateIndex(
                name: "IX_TB_FMS_JournalEntry_JEAccount1",
                table: "TB_FMS_JournalEntry",
                column: "JEAccount1");

            migrationBuilder.CreateIndex(
                name: "IX_TB_FMS_JournalEntry_JEAccount2",
                table: "TB_FMS_JournalEntry",
                column: "JEAccount2");

            migrationBuilder.CreateIndex(
                name: "IX_TB_FMS_StatementAccounts_StaID",
                table: "TB_FMS_StatementAccounts",
                column: "StaID");

            migrationBuilder.CreateIndex(
                name: "IX_TB_FMS_TemplateAccounts_TempID",
                table: "TB_FMS_TemplateAccounts",
                column: "TempID");

            migrationBuilder.CreateIndex(
                name: "IX_TB_InterviewDetails_RecuriementId",
                table: "TB_InterviewDetails",
                column: "RecuriementId");

            migrationBuilder.CreateIndex(
                name: "IX_TB_ManufacturingOrderDetails_rawMaterialId",
                table: "TB_ManufacturingOrderDetails",
                column: "rawMaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_TB_Product_categoryId",
                table: "TB_Product",
                column: "categoryId");

            migrationBuilder.CreateIndex(
                name: "IX_TB_Recuirement_EmployeeId",
                table: "TB_Recuirement",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_TB_Recuirement_HRManagerId",
                table: "TB_Recuirement",
                column: "HRManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_TB_SupplyingMaterialDetails_materialId",
                table: "TB_SupplyingMaterialDetails",
                column: "materialId");

            migrationBuilder.CreateIndex(
                name: "IX_TB_Survey_CustomerId",
                table: "TB_Survey",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_TB_Survey_QuestionId",
                table: "TB_Survey",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_TB_Task_CustomerId",
                table: "TB_Task",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_TB_ToDoList_CustomerId",
                table: "TB_ToDoList",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_TB_VisualReport_R_AdminID",
                table: "TB_VisualReport",
                column: "R_AdminID");

            migrationBuilder.CreateIndex(
                name: "IX_TB_VisualReport_R_ReporterID",
                table: "TB_VisualReport",
                column: "R_ReporterID");

            migrationBuilder.CreateIndex(
                name: "IX_TbManufacturingOrders_ManufacturingStatusId",
                table: "TbManufacturingOrders",
                column: "ManufacturingStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_TbManufacturingOrders_ProductManufacturedId",
                table: "TbManufacturingOrders",
                column: "ProductManufacturedId");

            migrationBuilder.CreateIndex(
                name: "IX_TbOrder_Suppliers_OrderStatusId",
                table: "TbOrder_Suppliers",
                column: "OrderStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_TbOrder_Suppliers_SupplierId",
                table: "TbOrder_Suppliers",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_TbOrderDetails_Suppliers_TbOrder_SupplierId",
                table: "TbOrderDetails_Suppliers",
                column: "TbOrder_SupplierId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "TB_DistributionOrderDetails");

            migrationBuilder.DropTable(
                name: "TB_EmployeeTaskDetails");

            migrationBuilder.DropTable(
                name: "TB_EmployeeTrainning");

            migrationBuilder.DropTable(
                name: "TB_FinishedProductsInventory");

            migrationBuilder.DropTable(
                name: "TB_FMS_AccCat");

            migrationBuilder.DropTable(
                name: "TB_FMS_JournalEntry");

            migrationBuilder.DropTable(
                name: "TB_FMS_StatementAccounts");

            migrationBuilder.DropTable(
                name: "TB_FMS_TemplateAccounts");

            migrationBuilder.DropTable(
                name: "TB_InterviewDetails");

            migrationBuilder.DropTable(
                name: "TB_ManufacturingOrderDetails");

            migrationBuilder.DropTable(
                name: "TB_RawMaterialsInventory");

            migrationBuilder.DropTable(
                name: "TB_SupplyingMaterialDetails");

            migrationBuilder.DropTable(
                name: "TB_Survey");

            migrationBuilder.DropTable(
                name: "TB_Task");

            migrationBuilder.DropTable(
                name: "TB_ToDoList");

            migrationBuilder.DropTable(
                name: "TB_VisualReport");

            migrationBuilder.DropTable(
                name: "TbOrderDetails_Suppliers");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "TB_DistributionOrder");

            migrationBuilder.DropTable(
                name: "TB_FMS_Category");

            migrationBuilder.DropTable(
                name: "TB_FMS_Statement");

            migrationBuilder.DropTable(
                name: "TB_FMS_Account");

            migrationBuilder.DropTable(
                name: "TB_FMS_StatementTemplate");

            migrationBuilder.DropTable(
                name: "TB_Recuirement");

            migrationBuilder.DropTable(
                name: "TbManufacturingOrders");

            migrationBuilder.DropTable(
                name: "TB_RawMaterial");

            migrationBuilder.DropTable(
                name: "TB_Question");

            migrationBuilder.DropTable(
                name: "TB_Customer");

            migrationBuilder.DropTable(
                name: "TB_Adminstrator");

            migrationBuilder.DropTable(
                name: "TbOrder_Suppliers");

            migrationBuilder.DropTable(
                name: "TB_Distributor");

            migrationBuilder.DropTable(
                name: "TbDistributionOrderStatus");

            migrationBuilder.DropTable(
                name: "TB_EmployeeDetails");

            migrationBuilder.DropTable(
                name: "TB_Product");

            migrationBuilder.DropTable(
                name: "TbManufacturingStatus");

            migrationBuilder.DropTable(
                name: "TB_Reporter");

            migrationBuilder.DropTable(
                name: "TB_Supplier");

            migrationBuilder.DropTable(
                name: "TbOrderStatus_Suppliers");

            migrationBuilder.DropTable(
                name: "TB_HRManagerDetails");

            migrationBuilder.DropTable(
                name: "TB_Category");
        }
    }
}
