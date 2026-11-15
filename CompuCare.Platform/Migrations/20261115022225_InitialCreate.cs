using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CompuCare.Platform.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "companies",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    tax_id = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_companies", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "equipment",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    company_id = table.Column<int>(type: "int", nullable: false),
                    code = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    name = table.Column<string>(type: "varchar(160)", maxLength: 160, nullable: false),
                    serial_number = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    type = table.Column<string>(type: "varchar(90)", maxLength: 90, nullable: false),
                    location_id = table.Column<int>(type: "int", nullable: true),
                    assigned_employee_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_equipment", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "locations",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    company_id = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    desk = table.Column<string>(type: "varchar(140)", maxLength: 140, nullable: false),
                    active = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_locations", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "maintenance_visits",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    company_id = table.Column<int>(type: "int", nullable: false),
                    subscription_id = table.Column<int>(type: "int", nullable: false),
                    date = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false),
                    time = table.Column<string>(type: "varchar(8)", maxLength: 8, nullable: false),
                    equipment_ids = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: false),
                    status = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_maintenance_visits", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "notifications",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    message = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false),
                    read = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_notifications", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "password_reset_tokens",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    token_hash = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false),
                    expires_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    used_at = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_password_reset_tokens", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "plans",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    code = table.Column<string>(type: "varchar(60)", maxLength: 60, nullable: false),
                    name = table.Column<string>(type: "varchar(90)", maxLength: 90, nullable: false),
                    price = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    hours = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    preventive_visits = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_plans", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "quotations",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    service_request_id = table.Column<int>(type: "int", nullable: false),
                    company_id = table.Column<int>(type: "int", nullable: false),
                    technician_id = table.Column<int>(type: "int", nullable: false),
                    total = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    status = table.Column<string>(type: "varchar(35)", maxLength: 35, nullable: false),
                    paid_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    receipt = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_quotations", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "service_requests",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    company_id = table.Column<int>(type: "int", nullable: false),
                    equipment_id = table.Column<int>(type: "int", nullable: false),
                    requester_id = table.Column<int>(type: "int", nullable: false),
                    technician_id = table.Column<int>(type: "int", nullable: true),
                    category = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: false),
                    diagnosis = table.Column<string>(type: "varchar(4000)", maxLength: 4000, nullable: false),
                    work_done = table.Column<string>(type: "varchar(4000)", maxLength: 4000, nullable: false),
                    status = table.Column<string>(type: "varchar(35)", maxLength: 35, nullable: false),
                    paused = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    work_hours = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_service_requests", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "subscriptions",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    company_id = table.Column<int>(type: "int", nullable: false),
                    plan_id = table.Column<int>(type: "int", nullable: false),
                    plan_name = table.Column<string>(type: "varchar(90)", maxLength: 90, nullable: false),
                    monthly_hours = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    used_hours = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    preventive_total = table.Column<int>(type: "int", nullable: false),
                    preventive_used = table.Column<int>(type: "int", nullable: false),
                    started_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    renews_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    status = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    version = table.Column<Guid>(type: "char(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_subscriptions", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    company_id = table.Column<int>(type: "int", nullable: true),
                    name = table.Column<string>(type: "varchar(160)", maxLength: 160, nullable: false),
                    email = table.Column<string>(type: "varchar(254)", maxLength: 254, nullable: false),
                    password_hash = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    role = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false),
                    phone = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    position = table.Column<string>(type: "varchar(140)", maxLength: 140, nullable: false),
                    active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_users", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "equipment_assignments",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    employee_id = table.Column<int>(type: "int", nullable: true),
                    assigned_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    equipment_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_equipment_assignments", x => x.id);
                    table.ForeignKey(
                        name: "f_k_equipment_assignments_equipment_equipment_id",
                        column: x => x.equipment_id,
                        principalTable: "equipment",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "quotation_lines",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false),
                    cost = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    quotation_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_quotation_lines", x => x.id);
                    table.ForeignKey(
                        name: "f_k_quotation_lines_quotations_quotation_id",
                        column: x => x.quotation_id,
                        principalTable: "quotations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "timeline_entries",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    status = table.Column<string>(type: "varchar(35)", maxLength: 35, nullable: false),
                    at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    service_request_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_timeline_entries", x => x.id);
                    table.ForeignKey(
                        name: "f_k_timeline_entries_service_requests_service_request_id",
                        column: x => x.service_request_id,
                        principalTable: "service_requests",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "coverage_consumptions",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    service_request_id = table.Column<int>(type: "int", nullable: false),
                    period_started_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    hours = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    subscription_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_coverage_consumptions", x => x.id);
                    table.ForeignKey(
                        name: "f_k_coverage_consumptions_subscriptions_subscription_id",
                        column: x => x.subscription_id,
                        principalTable: "subscriptions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.InsertData(
                table: "plans",
                columns: new[] { "id", "code", "hours", "name", "preventive_visits", "price" },
                values: new object[,]
                {
                    { 1, "basico", 4m, "Básico", 1, 240m },
                    { 2, "empresarial", 8m, "Empresarial", 2, 550m },
                    { 3, "integral", 16m, "Integral", 4, 600m }
                });

            migrationBuilder.CreateIndex(
                name: "i_x_companies_tax_id",
                table: "companies",
                column: "tax_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_coverage_consumptions_request_period",
                table: "coverage_consumptions",
                columns: new[] { "subscription_id", "service_request_id", "period_started_at" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_equipment_assigned_employee_id",
                table: "equipment",
                column: "assigned_employee_id");

            migrationBuilder.CreateIndex(
                name: "i_x_equipment_code",
                table: "equipment",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_equipment_company_id_serial_number",
                table: "equipment",
                columns: new[] { "company_id", "serial_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_equipment_assignments_equipment_id",
                table: "equipment_assignments",
                column: "equipment_id");

            migrationBuilder.CreateIndex(
                name: "i_x_locations_company_id_name_desk",
                table: "locations",
                columns: new[] { "company_id", "name", "desk" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_maintenance_visits_company_id_date",
                table: "maintenance_visits",
                columns: new[] { "company_id", "date" });

            migrationBuilder.CreateIndex(
                name: "i_x_notifications_user_id_created_at",
                table: "notifications",
                columns: new[] { "user_id", "created_at" });

            migrationBuilder.CreateIndex(
                name: "i_x_password_reset_tokens_token_hash",
                table: "password_reset_tokens",
                column: "token_hash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_password_reset_tokens_user_id",
                table: "password_reset_tokens",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "i_x_plans_code",
                table: "plans",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_quotation_lines_quotation_id",
                table: "quotation_lines",
                column: "quotation_id");

            migrationBuilder.CreateIndex(
                name: "i_x_quotations_company_id_status",
                table: "quotations",
                columns: new[] { "company_id", "status" });

            migrationBuilder.CreateIndex(
                name: "i_x_quotations_service_request_id_status",
                table: "quotations",
                columns: new[] { "service_request_id", "status" });

            migrationBuilder.CreateIndex(
                name: "i_x_service_requests_company_id_status",
                table: "service_requests",
                columns: new[] { "company_id", "status" });

            migrationBuilder.CreateIndex(
                name: "i_x_service_requests_equipment_id",
                table: "service_requests",
                column: "equipment_id");

            migrationBuilder.CreateIndex(
                name: "i_x_service_requests_technician_id",
                table: "service_requests",
                column: "technician_id");

            migrationBuilder.CreateIndex(
                name: "i_x_subscriptions_company_id",
                table: "subscriptions",
                column: "company_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_timeline_entries_service_request_id",
                table: "timeline_entries",
                column: "service_request_id");

            migrationBuilder.CreateIndex(
                name: "i_x_users_company_id_role",
                table: "users",
                columns: new[] { "company_id", "role" });

            migrationBuilder.CreateIndex(
                name: "i_x_users_email",
                table: "users",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "companies");

            migrationBuilder.DropTable(
                name: "coverage_consumptions");

            migrationBuilder.DropTable(
                name: "equipment_assignments");

            migrationBuilder.DropTable(
                name: "locations");

            migrationBuilder.DropTable(
                name: "maintenance_visits");

            migrationBuilder.DropTable(
                name: "notifications");

            migrationBuilder.DropTable(
                name: "password_reset_tokens");

            migrationBuilder.DropTable(
                name: "plans");

            migrationBuilder.DropTable(
                name: "quotation_lines");

            migrationBuilder.DropTable(
                name: "timeline_entries");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "subscriptions");

            migrationBuilder.DropTable(
                name: "equipment");

            migrationBuilder.DropTable(
                name: "quotations");

            migrationBuilder.DropTable(
                name: "service_requests");
        }
    }
}
