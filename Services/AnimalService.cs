using AngularSPAWebAPI.Controllers;
using AngularSPAWebAPI.Models;
using CBAS.Helpers;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace AngularSPAWebAPI.Services
{

    public class AnimalService
    {
        public async Task<List<Animal>> GetAnimalByExpIDAsync(int expID)
        {
            string query = @"SELECT Animal.*, Strain.Strain, Genotype.Genotype 
                             FROM Animal
                             LEFT JOIN Genotype ON Genotype.ID = Animal.GID
                             LEFT JOIN Strain ON Strain.ID = Animal.SID 
                             WHERE ExpID = @ExpID";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@ExpID", expID)
            };

            try
            {
                return await Dal.ExecuteQueryAsync(query, reader => new Animal
                {
                    ExpID = reader.GetInt32(reader.GetOrdinal("ExpID")),
                    AnimalID = reader.GetInt32(reader.GetOrdinal("AnimalID")),
                    UserAnimalID = reader.GetString(reader.GetOrdinal("UserAnimalID")),
                    SID = reader.IsDBNull(reader.GetOrdinal("SID")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("SID")),
                    GID = reader.IsDBNull(reader.GetOrdinal("GID")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("GID")),
                    Sex = reader.GetString(reader.GetOrdinal("Sex")),
                    Genotype = reader.GetString(reader.GetOrdinal("Genotype")),
                    Strain = reader.GetString(reader.GetOrdinal("Strain"))
                }, parameters);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting animals by ExpID: {ExpID}", expID);
                return new List<Animal>();
            }
        }

        public async Task<bool> DoesAnimalIDExistAsync(string userAnimalId, int expID)
        {
            string sql = "SELECT COUNT(*) FROM Animal WHERE LTRIM(RTRIM(UserAnimalID)) = @UserAnimalID AND ExpID = @ExpID";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@UserAnimalID", userAnimalId.Trim()),
                new SqlParameter("@ExpID", expID)
            };

            try
            {
                int countResult = Convert.ToInt32(await Dal.ExecScalarAsync(sql, parameters));
                return countResult > 0;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error checking if AnimalID exists for UserAnimalID: {UserAnimalID}, ExpID: {ExpID}", userAnimalId, expID);
                return false;
            }
        }

        public async Task<int> InsertAnimalAsync(Animal animal)
        {
            string sql = @"INSERT INTO Animal (ExpID, UserAnimalID, SID, GID, Sex) 
                   VALUES (@ExpID, @UserAnimalID, @SID, @GID, @Sex); 
                   SELECT CAST(scope_identity() AS int);";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@ExpID", animal.ExpID),
                new SqlParameter("@UserAnimalID", animal.UserAnimalID),
                new SqlParameter("@SID", animal.SID ?? (object)DBNull.Value),
                new SqlParameter("@GID", animal.GID ?? (object)DBNull.Value),
                new SqlParameter("@Sex", animal.Sex)
            };

            try
            {
                object result = await Dal.ExecScalarAsync(sql, parameters);
                return Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error inserting animal: {@Animal}", animal);
                throw;
            }
        }

        public async Task UpdateAnimalAsync(Animal animal)
        {
            string sql = @"UPDATE Animal 
                   SET Sex = @Sex, GID = @GID, SID = @SID 
                   WHERE AnimalID = @AnimalID";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Sex", animal.Sex),
                new SqlParameter("@GID", animal.GID ?? (object)DBNull.Value),
                new SqlParameter("@SID", animal.SID ?? (object)DBNull.Value),
                new SqlParameter("@AnimalID", animal.AnimalID)
            };

            try
            {
                await Dal.ExecuteNonQueryAsync(sql, parameters);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error updating animal: {@Animal}", animal);
                throw;
            }
        }

        public async Task<int> GetCountOfAnimalsAsync()
        {
            string sql = "SELECT COUNT(*) FROM Animal WHERE SID IS NOT NULL AND GID IS NOT NULL AND Sex != ''";

            try
            {
                object result = await Dal.ExecScalarAsync(sql);
                return Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting count of animals");
                throw;
            }
        }

        public async Task DeleteAnimalByAnimalIDAsync(int animalID)
        {
            string sql = @"Delete From RBT_TouchScreen_Features Where SessionID in (Select SessionID From SessionInfo Where AnimalID = @AnimalID);
                   Delete From rbt_data_cached_avg Where SessionID in (Select SessionID From SessionInfo Where AnimalID = @AnimalID);
                   Delete From rbt_data_cached_std Where SessionID in (Select SessionID From SessionInfo Where AnimalID = @AnimalID);
                   Delete From rbt_data_cached_cnt Where SessionID in (Select SessionID From SessionInfo Where AnimalID = @AnimalID);
                   Delete From rbt_data_cached_sum Where SessionID in (Select SessionID From SessionInfo Where AnimalID = @AnimalID);
                   Delete From SessionInfo_Dynamic Where SessionID in (Select SessionID From SessionInfo Where AnimalID = @AnimalID);
                   Delete From SessionInfo Where AnimalID = @AnimalID;
                   Delete From Upload Where AnimalID = @AnimalID;
                   Delete From Animal Where AnimalID = @AnimalID;";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@AnimalID", animalID)
            };

            try
            {
                await Dal.ExecuteNonQueryAsync(sql, parameters);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error deleting animal with AnimalID: {AnimalID}", animalID);
                throw;
            }
        }

        public async Task<List<Strains>> GetStrainListAsync()
        {
            string query = "SELECT * FROM Strain";

            try
            {
                return await Dal.ExecuteQueryAsync(query, reader => new Strains
                {
                    ID = reader.GetInt32(reader.GetOrdinal("ID")),
                    Strain = reader.GetString(reader.GetOrdinal("Strain")),
                    Link = reader.GetString(reader.GetOrdinal("Link"))
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting strain list");
                return new List<Strains>();
            }
        }

        public async Task<List<Geno>> GetGenoListAsync(int? strainID)
        {
            List<Geno> GenoList = new List<Geno>();

            if (strainID == null || strainID == 0)
            {
                return GenoList;
            }

            List<int> lstGenoID = HelperService.GetGenoID(strainID);
            string lstGenoIDCsv = String.Join(",", lstGenoID.Select(x => x.ToString()).ToArray());

            string query = $"SELECT * FROM Genotype WHERE ID IN ({lstGenoIDCsv})";

            try
            {
                return await Dal.ExecuteQueryAsync(query, reader => new Geno
                {
                    ID = reader.GetInt32(reader.GetOrdinal("ID")),
                    Genotype = reader.GetString(reader.GetOrdinal("Genotype")),
                    Link = reader.GetString(reader.GetOrdinal("Link")),
                    Description = reader.GetString(reader.GetOrdinal("Description"))
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting genotype list for strainID: {StrainID}", strainID);
                return new List<Geno>();
            }
        }

        public async Task<bool> IsUserAnimalIDExistAsync(string userAnimalID, int expID)
        {
            string sql = "SELECT COUNT(*) FROM Animal WHERE LTRIM(RTRIM(UserAnimalID)) = @UserAnimalID AND ExpID = @ExpID";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@UserAnimalID", userAnimalID.Trim()),
                new SqlParameter("@ExpID", expID)
            };

            try
            {
                int count = Convert.ToInt32(await Dal.ExecScalarAsync(sql, parameters));
                return count > 0;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error checking if UserAnimalID exists for UserAnimalID: {UserAnimalID}, ExpID: {ExpID}", userAnimalID, expID);
                return false;
            }
        }

        public async Task<(int, bool)> GetAnimalIDByUserAnimalIdAndExpIdAsync(string editedUserAnimalId, int expId)
        {
            string sql = @"
                SELECT 
                    AnimalID, 
                    CASE 
                        WHEN Sex IS NOT NULL AND GID IS NOT NULL AND SID IS NOT NULL THEN 1 
                        ELSE 0 
                    END AS IsInfoCompleted
                FROM Animal 
                WHERE LTRIM(RTRIM(UserAnimalID)) = @UserAnimalID AND ExpID = @ExpID";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@UserAnimalID", editedUserAnimalId.Trim()),
                new SqlParameter("@ExpID", expId)
            };

            try
            {
                return await Dal.ExecuteQuerySingleAsync(sql, reader =>
                {
                    int animalId = reader.GetInt32(reader.GetOrdinal("AnimalID"));
                    bool isInfoCompleted = reader.GetInt32(reader.GetOrdinal("IsInfoCompleted")) == 1;
                    return (animalId, isInfoCompleted);
                }, parameters);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting AnimalID and checking info completeness for UserAnimalID: {UserAnimalID}, ExpID: {ExpID}", editedUserAnimalId, expId);
                return (0, false);
            }
        }

        public async Task<bool> ReplaceAnimalIdAsync(int oldAnimalId, int existingAnimalIdToUse)
        {
            string sql = @"
                BEGIN TRANSACTION;
                UPDATE Upload SET AnimalId = @ExistingAnimalIdToUse WHERE AnimalId = @OldAnimalId;
                DELETE FROM RBT_TouchScreen_Features WHERE SessionID IN (SELECT SessionID FROM SessionInfo WHERE AnimalID = @OldAnimalId);
                DELETE FROM rbt_data_cached_avg WHERE SessionID IN (SELECT SessionID FROM SessionInfo WHERE AnimalID = @OldAnimalId);
                DELETE FROM rbt_data_cached_std WHERE SessionID IN (SELECT SessionID FROM SessionInfo WHERE AnimalID = @OldAnimalId);
                DELETE FROM rbt_data_cached_cnt WHERE SessionID IN (SELECT SessionID FROM SessionInfo WHERE AnimalID = @OldAnimalId);
                DELETE FROM rbt_data_cached_sum WHERE SessionID IN (SELECT SessionID FROM SessionInfo WHERE AnimalID = @OldAnimalId);
                DELETE FROM SessionInfo_Dynamic WHERE SessionID IN (SELECT SessionID FROM SessionInfo WHERE AnimalID = @OldAnimalId);
                DELETE FROM SessionInfo WHERE AnimalID = @OldAnimalId;
                DELETE FROM Upload WHERE AnimalID = @OldAnimalId;
                DELETE FROM Animal WHERE AnimalID = @OldAnimalId;
                COMMIT TRANSACTION;";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@ExistingAnimalIdToUse", existingAnimalIdToUse),
                new SqlParameter("@OldAnimalId", oldAnimalId)
            };

            try
            {
                await Dal.ExecuteNonQueryAsync(sql, parameters);
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error replacing AnimalID from {OldAnimalId} to {ExistingAnimalIdToUse}", oldAnimalId, existingAnimalIdToUse);
                return false;
            }
        }


        //public List<int> GetGenoID (int? strainID)
        //{
        //    List<int> lstGenoID = new List<int>();

        //    switch (strainID)
        //    {
        //        case 1:
        //            lstGenoID.Add(1);
        //            lstGenoID.Add(4);
        //            break;
        //        case 2:
        //            lstGenoID.Add(2);
        //            lstGenoID.Add(5);
        //            break;
        //        case 3:
        //            lstGenoID.Add(3);
        //            lstGenoID.Add(6);
        //            break;
        //        case 4:
        //            lstGenoID.Add(4);
        //            break;
        //        case 5:
        //            lstGenoID.Add(5);
        //            break;
        //        case 6:
        //            lstGenoID.Add(6);
        //            break;
        //        case 7:
        //            lstGenoID.Add(7);
        //            lstGenoID.Add(11);
        //            break;
        //        case 8:
        //            lstGenoID.Add(8);
        //            lstGenoID.Add(11);
        //            break;
        //        case 9:
        //            lstGenoID.Add(9);
        //            lstGenoID.Add(10);
        //            lstGenoID.Add(12);
        //            break;
        //        case 10:
        //            lstGenoID.Add(12);
        //            break;
        //        case 11:
        //            lstGenoID.Add(11);
        //            break;
        //        case 12:
        //            lstGenoID.Add(6);
        //            lstGenoID.Add(13);
        //            lstGenoID.Add(14);
        //            lstGenoID.Add(15);
        //            break;
        //        case 13:
        //            lstGenoID.Add(6);
        //            lstGenoID.Add(13);
        //            lstGenoID.Add(16);
        //            lstGenoID.Add(17);
        //            lstGenoID.Add(18);
        //            break;
        //        case 14:
        //            lstGenoID.Add(6);
        //            lstGenoID.Add(13);
        //            lstGenoID.Add(19);
        //            lstGenoID.Add(20);
        //            lstGenoID.Add(21);
        //            break;
        //        case 15:
        //            lstGenoID.Add(22);
        //            lstGenoID.Add(23);
        //            lstGenoID.Add(24);
        //            break;

        //    }

        //    return lstGenoID;
        //}




    }
}
