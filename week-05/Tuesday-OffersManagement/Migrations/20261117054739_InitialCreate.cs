using Microsoft.EntityFrameworkCore.Migrations;

namespace OffersManagement.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Offers",
                columns: table => new
                {
                    OfferId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OfferName = table.Column<string>(nullable: true),
                    Discount = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Offers", x => x.OfferId);
                });

            migrationBuilder.CreateTable(
                name: "PlanOffers",
                columns: table => new
                {
                    PlanOfferId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlanName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanOffers", x => x.PlanOfferId);
                });

            migrationBuilder.CreateTable(
                name: "OfferPlanOffers",
                columns: table => new
                {
                    OfferId = table.Column<int>(nullable: false),
                    PlanOfferId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfferPlanOffers", x => new { x.OfferId, x.PlanOfferId });
                    table.ForeignKey(
                        name: "FK_OfferPlanOffers_Offers_OfferId",
                        column: x => x.OfferId,
                        principalTable: "Offers",
                        principalColumn: "OfferId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OfferPlanOffers_PlanOffers_PlanOfferId",
                        column: x => x.PlanOfferId,
                        principalTable: "PlanOffers",
                        principalColumn: "PlanOfferId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OfferPlanOffers_PlanOfferId",
                table: "OfferPlanOffers",
                column: "PlanOfferId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OfferPlanOffers");

            migrationBuilder.DropTable(
                name: "Offers");

            migrationBuilder.DropTable(
                name: "PlanOffers");
        }
    }
}
