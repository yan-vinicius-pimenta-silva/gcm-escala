using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EscalaGcm.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "equipes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Ativo = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_equipes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "horarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Inicio = table.Column<TimeOnly>(type: "TEXT", nullable: false),
                    Fim = table.Column<TimeOnly>(type: "TEXT", nullable: false),
                    Descricao = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Ativo = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_horarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "posicoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Ativo = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_posicoes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "setores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Tipo = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    Ativo = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_setores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "turnos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Ativo = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_turnos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NomeUsuario = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    SenhaHash = table.Column<string>(type: "TEXT", nullable: false),
                    NomeCompleto = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Perfil = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Ativo = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "viaturas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Identificador = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Ativo = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_viaturas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "guardas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Telefone = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    PosicaoId = table.Column<int>(type: "INTEGER", nullable: false),
                    Ativo = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_guardas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_guardas_posicoes_PosicaoId",
                        column: x => x.PosicaoId,
                        principalTable: "posicoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "escalas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Ano = table.Column<int>(type: "INTEGER", nullable: false),
                    Mes = table.Column<int>(type: "INTEGER", nullable: false),
                    Quinzena = table.Column<int>(type: "INTEGER", nullable: false),
                    SetorId = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_escalas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_escalas_setores_SetorId",
                        column: x => x.SetorId,
                        principalTable: "setores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ausencias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GuardaId = table.Column<int>(type: "INTEGER", nullable: false),
                    DataInicio = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    DataFim = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    Motivo = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    Observacoes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ausencias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ausencias_guardas_GuardaId",
                        column: x => x.GuardaId,
                        principalTable: "guardas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "equipe_membros",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EquipeId = table.Column<int>(type: "INTEGER", nullable: false),
                    GuardaId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_equipe_membros", x => x.Id);
                    table.ForeignKey(
                        name: "FK_equipe_membros_equipes_EquipeId",
                        column: x => x.EquipeId,
                        principalTable: "equipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_equipe_membros_guardas_GuardaId",
                        column: x => x.GuardaId,
                        principalTable: "guardas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ferias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GuardaId = table.Column<int>(type: "INTEGER", nullable: false),
                    DataInicio = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    DataFim = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    Observacao = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ferias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ferias_guardas_GuardaId",
                        column: x => x.GuardaId,
                        principalTable: "guardas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "escala_itens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EscalaId = table.Column<int>(type: "INTEGER", nullable: false),
                    Data = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    TurnoId = table.Column<int>(type: "INTEGER", nullable: false),
                    HorarioId = table.Column<int>(type: "INTEGER", nullable: false),
                    Observacao = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_escala_itens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_escala_itens_escalas_EscalaId",
                        column: x => x.EscalaId,
                        principalTable: "escalas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_escala_itens_horarios_HorarioId",
                        column: x => x.HorarioId,
                        principalTable: "horarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_escala_itens_turnos_TurnoId",
                        column: x => x.TurnoId,
                        principalTable: "turnos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "escala_alocacoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EscalaItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    GuardaId = table.Column<int>(type: "INTEGER", nullable: true),
                    EquipeId = table.Column<int>(type: "INTEGER", nullable: true),
                    Funcao = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    ViaturaId = table.Column<int>(type: "INTEGER", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UpdatedBy = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_escala_alocacoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_escala_alocacoes_equipes_EquipeId",
                        column: x => x.EquipeId,
                        principalTable: "equipes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_escala_alocacoes_escala_itens_EscalaItemId",
                        column: x => x.EscalaItemId,
                        principalTable: "escala_itens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_escala_alocacoes_guardas_GuardaId",
                        column: x => x.GuardaId,
                        principalTable: "guardas",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_escala_alocacoes_viaturas_ViaturaId",
                        column: x => x.ViaturaId,
                        principalTable: "viaturas",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ausencias_GuardaId_DataInicio_DataFim",
                table: "ausencias",
                columns: new[] { "GuardaId", "DataInicio", "DataFim" });

            migrationBuilder.CreateIndex(
                name: "IX_equipe_membros_EquipeId_GuardaId",
                table: "equipe_membros",
                columns: new[] { "EquipeId", "GuardaId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_equipe_membros_GuardaId",
                table: "equipe_membros",
                column: "GuardaId");

            migrationBuilder.CreateIndex(
                name: "IX_equipes_Nome",
                table: "equipes",
                column: "Nome",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_escala_alocacoes_EquipeId_EscalaItemId",
                table: "escala_alocacoes",
                columns: new[] { "EquipeId", "EscalaItemId" });

            migrationBuilder.CreateIndex(
                name: "IX_escala_alocacoes_EscalaItemId",
                table: "escala_alocacoes",
                column: "EscalaItemId");

            migrationBuilder.CreateIndex(
                name: "IX_escala_alocacoes_GuardaId_EscalaItemId",
                table: "escala_alocacoes",
                columns: new[] { "GuardaId", "EscalaItemId" });

            migrationBuilder.CreateIndex(
                name: "IX_escala_alocacoes_ViaturaId",
                table: "escala_alocacoes",
                column: "ViaturaId");

            migrationBuilder.CreateIndex(
                name: "IX_escala_itens_EscalaId_Data",
                table: "escala_itens",
                columns: new[] { "EscalaId", "Data" });

            migrationBuilder.CreateIndex(
                name: "IX_escala_itens_EscalaId_Data_TurnoId_HorarioId",
                table: "escala_itens",
                columns: new[] { "EscalaId", "Data", "TurnoId", "HorarioId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_escala_itens_HorarioId",
                table: "escala_itens",
                column: "HorarioId");

            migrationBuilder.CreateIndex(
                name: "IX_escala_itens_TurnoId",
                table: "escala_itens",
                column: "TurnoId");

            migrationBuilder.CreateIndex(
                name: "IX_escalas_Ano_Mes_Quinzena_SetorId",
                table: "escalas",
                columns: new[] { "Ano", "Mes", "Quinzena", "SetorId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_escalas_SetorId_Ano_Mes_Quinzena",
                table: "escalas",
                columns: new[] { "SetorId", "Ano", "Mes", "Quinzena" });

            migrationBuilder.CreateIndex(
                name: "IX_ferias_GuardaId_DataInicio_DataFim",
                table: "ferias",
                columns: new[] { "GuardaId", "DataInicio", "DataFim" });

            migrationBuilder.CreateIndex(
                name: "IX_guardas_Nome",
                table: "guardas",
                column: "Nome");

            migrationBuilder.CreateIndex(
                name: "IX_guardas_PosicaoId",
                table: "guardas",
                column: "PosicaoId");

            migrationBuilder.CreateIndex(
                name: "IX_horarios_Inicio_Fim",
                table: "horarios",
                columns: new[] { "Inicio", "Fim" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_posicoes_Nome",
                table: "posicoes",
                column: "Nome",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_setores_Nome",
                table: "setores",
                column: "Nome",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_turnos_Nome",
                table: "turnos",
                column: "Nome",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_NomeUsuario",
                table: "usuarios",
                column: "NomeUsuario",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_viaturas_Identificador",
                table: "viaturas",
                column: "Identificador",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ausencias");

            migrationBuilder.DropTable(
                name: "equipe_membros");

            migrationBuilder.DropTable(
                name: "escala_alocacoes");

            migrationBuilder.DropTable(
                name: "ferias");

            migrationBuilder.DropTable(
                name: "usuarios");

            migrationBuilder.DropTable(
                name: "equipes");

            migrationBuilder.DropTable(
                name: "escala_itens");

            migrationBuilder.DropTable(
                name: "viaturas");

            migrationBuilder.DropTable(
                name: "guardas");

            migrationBuilder.DropTable(
                name: "escalas");

            migrationBuilder.DropTable(
                name: "horarios");

            migrationBuilder.DropTable(
                name: "turnos");

            migrationBuilder.DropTable(
                name: "posicoes");

            migrationBuilder.DropTable(
                name: "setores");
        }
    }
}
