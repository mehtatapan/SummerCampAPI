using Microsoft.EntityFrameworkCore.Migrations;

namespace SummerCampAPI.Data
{
    public static class ExtraMigration
    {
        public static void Steps(MigrationBuilder migrationBuilder)
        {
            //Camper Table Triggers for Concurrency
            migrationBuilder.Sql(
                @"
                    CREATE TRIGGER SetCamperTimestampOnUpdate
                    AFTER UPDATE ON Campers
                    BEGIN
                        UPDATE Campers
                        SET RowVersion = randomblob(8)
                        WHERE rowid = NEW.rowid;
                    END
                ");
            migrationBuilder.Sql(
                @"
                    CREATE TRIGGER SetCamperTimestampOnInsert
                    AFTER INSERT ON Campers
                    BEGIN
                        UPDATE Campers
                        SET RowVersion = randomblob(8)
                        WHERE rowid = NEW.rowid;
                    END
                ");

            //Compound Table Triggers for Concurrency
            migrationBuilder.Sql(
                @"
                    CREATE TRIGGER SetCompoundTimestampOnUpdate
                    AFTER UPDATE ON Compounds
                    BEGIN
                        UPDATE Compounds
                        SET RowVersion = randomblob(8)
                        WHERE rowid = NEW.rowid;
                    END
                ");
            migrationBuilder.Sql(
                @"
                    CREATE TRIGGER SetCompundTimestampOnInsert
                    AFTER INSERT ON Compounds
                    BEGIN
                        UPDATE Compounds
                        SET RowVersion = randomblob(8)
                        WHERE rowid = NEW.rowid;
                    END
                ");
        }
    }
}
