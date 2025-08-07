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
                             FROM tsd.Animal
                             LEFT JOIN tsd.Genotype ON Genotype.ID = Animal.GID
                             LEFT JOIN tsd.Strain ON Strain.ID = Animal.SID 
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
                    Genotype = reader.IsDBNull(reader.GetOrdinal("Genotype")) ? null : reader.GetString(reader.GetOrdinal("Genotype")),
                    Strain = reader.IsDBNull(reader.GetOrdinal("Strain")) ? null : reader.GetString(reader.GetOrdinal("Strain"))
                }, parameters);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting animals by ExpID: {ExpID}", expID);
                return new List<Animal>();
            }
        }

        public async Task<List<Animal>> GetAnimalByRepIDAsync(int repID)
        {
            string query = @"SELECT Animal.*, Strain.Strain, Genotype.Genotype, AnimalRepository.RepID 
                             FROM tsd.Animal
                             LEFT JOIN tsd.Genotype ON Genotype.ID = Animal.GID
                             LEFT JOIN tsd.Strain ON Strain.ID = Animal.SID
                             INNER JOIN tsd.AnimalRepository ON AnimalRepository.AID = Animal.AnimalID 
                             WHERE RepID = @repID";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@repID", repID)
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
                    Genotype = reader.IsDBNull(reader.GetOrdinal("Genotype")) ? null : reader.GetString(reader.GetOrdinal("Genotype")),
                    Strain = reader.IsDBNull(reader.GetOrdinal("Strain")) ? null : reader.GetString(reader.GetOrdinal("Strain"))
                }, parameters);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting animals by ExpID: {ExpID}", repID);
                return new List<Animal>();
            }
        }

        public async Task<bool> DoesAnimalIDExistAsync(string userAnimalId, int expID)
        {
            string sql = "SELECT COUNT(*) FROM tsd.Animal WHERE LTRIM(RTRIM(UserAnimalID)) = @UserAnimalID AND ExpID = @ExpID";

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
            string sql = @"INSERT INTO tsd.Animal (ExpID, UserAnimalID, SID, GID, Sex) 
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
            string sql = @"UPDATE tsd.Animal 
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
            string sql = "SELECT COUNT(*) FROM tsd.Animal WHERE SID IS NOT NULL AND GID IS NOT NULL AND Sex != ''";

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
            string sql = @"Delete From tsd.RBT_TouchScreen_Features Where SessionID in (Select SessionID From tsd.SessionInfo Where AnimalID = @AnimalID);
                   Delete From tsd.rbt_data_cached_avg Where SessionID in (Select SessionID From tsd.SessionInfo Where AnimalID = @AnimalID);
                   Delete From tsd.rbt_data_cached_std Where SessionID in (Select SessionID From tsd.SessionInfo Where AnimalID = @AnimalID);
                   Delete From tsd.rbt_data_cached_cnt Where SessionID in (Select SessionID From tsd.SessionInfo Where AnimalID = @AnimalID);
                   Delete From tsd.rbt_data_cached_sum Where SessionID in (Select SessionID From tsd.SessionInfo Where AnimalID = @AnimalID);
                   Delete From tsd.SessionInfo_Dynamic Where SessionID in (Select SessionID From tsd.SessionInfo Where AnimalID = @AnimalID);
                   Delete From tsd.SessionInfo Where AnimalID = @AnimalID;
                   Delete From tsd.Upload Where AnimalID = @AnimalID;
                   Delete From tsd.Animal Where AnimalID = @AnimalID;";

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
            string query = "SELECT * FROM tsd.Strain";

            try
            {
                return await Dal.ExecuteQueryAsync(query, reader => new Strains
                {
                    ID = reader.GetInt32(reader.GetOrdinal("ID")),
                    Strain = reader.IsDBNull(reader.GetOrdinal("Strain")) ? null : reader.GetString(reader.GetOrdinal("Strain")),
                    Link = reader.IsDBNull(reader.GetOrdinal("Link")) ? null : reader.GetString(reader.GetOrdinal("Link"))
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

            string query = $"SELECT * FROM tsd.Genotype WHERE ID IN ({lstGenoIDCsv})";

            try
            {
                return await Dal.ExecuteQueryAsync(query, reader => new Geno
                {
                    ID = reader.GetInt32(reader.GetOrdinal("ID")),
                    Genotype = reader.IsDBNull(reader.GetOrdinal("Genotype")) ? null : reader.GetString(reader.GetOrdinal("Genotype")),
                    Link = reader.IsDBNull(reader.GetOrdinal("Link")) ? null : reader.GetString(reader.GetOrdinal("Link")),
                    Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description"))
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
            string sql = "SELECT COUNT(*) FROM tsd.Animal WHERE LTRIM(RTRIM(UserAnimalID)) = @UserAnimalID AND ExpID = @ExpID";

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
                FROM tsd.Animal 
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
                    bool isInfoCompleted = reader.IsDBNull(reader.GetOrdinal("IsInfoCompleted")) ? false : reader.GetInt32(reader.GetOrdinal("IsInfoCompleted")) == 1;
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
                UPDATE Upload SET AnimalId = @ExistingAnimalIdToUse WHERE tsd.AnimalId = @OldAnimalId;
                DELETE FROM RBT_TouchScreen_Features WHERE SessionID IN (SELECT SessionID FROM tsd.SessionInfo WHERE AnimalID = @OldAnimalId);
                DELETE FROM tsd.rbt_data_cached_avg WHERE SessionID IN (SELECT SessionID FROM tsd.SessionInfo WHERE AnimalID = @OldAnimalId);
                DELETE FROM tsd.rbt_data_cached_std WHERE SessionID IN (SELECT SessionID FROM tsd.SessionInfo WHERE AnimalID = @OldAnimalId);
                DELETE FROM tsd.rbt_data_cached_cnt WHERE SessionID IN (SELECT SessionID FROM tsd.SessionInfo WHERE AnimalID = @OldAnimalId);
                DELETE FROM tsd.rbt_data_cached_sum WHERE SessionID IN (SELECT SessionID FROM tsd.SessionInfo WHERE AnimalID = @OldAnimalId);
                DELETE FROM tsd.SessionInfo_Dynamic WHERE SessionID IN (SELECT SessionID FROM tsd.SessionInfo WHERE AnimalID = @OldAnimalId);
                DELETE FROM tsd.SessionInfo WHERE AnimalID = @OldAnimalId;
                DELETE FROM tsd.Upload WHERE AnimalID = @OldAnimalId;
                DELETE FROM tsd.Animal WHERE AnimalID = @OldAnimalId;
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

        public async Task DeleteAnimalsAsync(IEnumerable<int> animalIds)
        {
            string sql = $"Delete From tsd.RBT_TouchScreen_Features Where SessionID in (Select SessionID From tsd.SessionInfo Where AnimalID IN ({string.Join(",", animalIds)}));"
                       + $"Delete From tsd.rbt_data_cached_avg Where SessionID in (Select SessionID From tsd.SessionInfo Where AnimalID IN ({string.Join(",", animalIds)}));"
                       + $"Delete From tsd.rbt_data_cached_std Where SessionID in (Select SessionID From tsd.SessionInfo Where AnimalID IN ({string.Join(",", animalIds)}));"
                       + $"Delete From tsd.rbt_data_cached_cnt Where SessionID in (Select SessionID From tsd.SessionInfo Where AnimalID IN ({string.Join(",", animalIds)}));"
                       + $"Delete From tsd.rbt_data_cached_sum Where SessionID in (Select SessionID From tsd.SessionInfo Where AnimalID IN ({string.Join(",", animalIds)}));"
                       + $"Delete From tsd.SessionInfo_Dynamic Where SessionID in (Select SessionID From tsd.SessionInfo Where AnimalID IN ({string.Join(",", animalIds)}));"
                       + $"Delete From tsd.SessionInfo Where AnimalID IN ({string.Join(",", animalIds)});"
                       + $"Delete From tsd.Upload Where AnimalID IN ({string.Join(",", animalIds)});"
                       + $"Delete From tsd.Animal Where AnimalID IN ({string.Join(",", animalIds)});";

            try
            {
                await Dal.ExecuteNonQueryAsync(sql);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error deleting animals with AnimalIDs: {AnimalIDs}", animalIds);
                throw;
            }
        }

        public async Task DuplicateAnimalsAsync(IEnumerable<Animal> animals)
        {
            foreach (var animal in animals)
            {
                animal.UserAnimalID = $"{animal.UserAnimalID}_copy";
                await InsertAnimalAsync(animal);
            }
        }

    }
}
