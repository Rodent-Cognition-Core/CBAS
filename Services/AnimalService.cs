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
        public List<Animal> GetAnimalByExpID(int expID)
        {
            List<Animal> lstAnimal = new List<Animal>();

            using (DataTable dt = Dal.GetDataTable($@"SELECT Animal.*, Strain.Strain, Genotype.Genotype From Animal
                                                        left join Genotype on Genotype.ID = Animal.GID
                                                        left join Strain on Strain.ID = Animal.SID 
                                                        WHERE ExpID = {expID}"))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    lstAnimal.Add(new Animal
                    {
                        ExpID = Int32.Parse(dr["ExpID"].ToString()),
                        AnimalID = Int32.Parse(dr["AnimalID"].ToString()),
                        UserAnimalID = Convert.ToString(dr["UserAnimalID"].ToString()),
                        SID = HelperService.ConvertToNullableInt(dr["SID"].ToString()),
                        GID = HelperService.ConvertToNullableInt(dr["GID"].ToString()),
                        Sex = Convert.ToString(dr["Sex"].ToString()),
                        Genotype = Convert.ToString(dr["Genotype"].ToString()),
                        Strain = Convert.ToString(dr["Strain"].ToString()),

                    });
                }

            }

            return lstAnimal;

        }

        public bool DoesAnimalIDExist(string userAnimalId, int expID)
        {
            string sql = $"select count(*) from Animal where ltrim(rtrim(UserAnimalID)) = '{userAnimalId.Trim()}' and  ExpID = {expID}";

            int countResult = Int32.Parse(Dal.ExecScalar(sql).ToString());

            bool flag = (countResult == 0) ? false : true;
            return flag;
        }

        // Function definition for inserting animal info
        public int InsertAnimal(Animal animal)
        {
            string sql = $"Insert into Animal " +
              $"(ExpID, UserAnimalID, SID, GID, Sex) Values " +
              $"({animal.ExpID}, '{animal.UserAnimalID}', {animal.SID}, {animal.GID}, '{animal.Sex}'); SELECT @@IDENTITY AS 'Identity';";

            return Int32.Parse(Dal.ExecScalar(sql).ToString());
        }

        //Function Definition for updating Animal info
        public void UpdateAnimal(Animal animal)
        {
            // For Daniel  (UserAnimalID = '{animal.UserAnimalID}' should be removed from the sql query)
            string sql = $"UPDATE Animal " +
                 $"SET Sex = '{animal.Sex}'," +
                 $"GID = '{animal.GID}', SID = '{animal.SID}' WHERE AnimalID = {animal.AnimalID} ;";
            
            Dal.ExecuteNonQuery(sql);

        }

        public Int32 GetCountOfAnimals()
        {
            string sql = "select count(*) from animal Where SID is not Null and GID is not Null and Sex!=''";

            return Int32.Parse(Dal.ExecScalar(sql).ToString());
        }

        public void DeleteAnimalByAnimalID(int animalID)
        {
            string sql = $@"Delete From RBT_TouchScreen_Features Where SessionID in (Select SessionID From SessionInfo Where AnimalID = {animalID});
                            Delete From rbt_data_cached_avg Where SessionID in (Select SessionID From SessionInfo Where AnimalID = {animalID});
                            Delete From rbt_data_cached_std Where SessionID in (Select SessionID From SessionInfo Where AnimalID = {animalID});
                            Delete From rbt_data_cached_cnt Where SessionID in (Select SessionID From SessionInfo Where AnimalID = {animalID});
                            Delete From rbt_data_cached_sum Where SessionID in (Select SessionID From SessionInfo Where AnimalID = {animalID});
                            Delete From SessionInfo_Dynamic Where SessionID in (Select SessionID From SessionInfo Where AnimalID = {animalID});
                            Delete From SessionInfo Where AnimalID = {animalID};
                            Delete From Upload Where AnimalID = {animalID};
							Delete From Animal Where AnimalID = {animalID};";

            Dal.ExecuteNonQuery(sql);
        }


        // Function definition to extract list of all strains from strain tbl in DB
        public List<Strains> GetStrainList()
        {
            List<Strains> StrainList = new List<Strains>();
            using (DataTable dt = Dal.GetDataTable($@"Select * From Strain"))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    StrainList.Add(new Strains
                    {
                        ID = Int32.Parse(dr["ID"].ToString()),
                        Strain = Convert.ToString(dr["Strain"].ToString()),
                        Link = Convert.ToString(dr["Link"].ToString()),

                    });
                }
            }

            return StrainList;
        }

        // Function definition to extract list of all strains from Genotype tbl in DB
        public List<Geno> GetGenoList(int? strainID)
        {
            List<int> lstGenoID = new List<int>();
            List<Geno> GenoList = new List<Geno>();
            string lstGenoIDCsv;

            if (strainID == null || strainID == 0)
            {
                return GenoList;
            }
            
            lstGenoID = HelperService.GetGenoID(strainID);
            lstGenoIDCsv = String.Join(",", lstGenoID.Select(x => x.ToString()).ToArray());
           

            using (DataTable dt = Dal.GetDataTable($@"Select * From Genotype where ID in ({lstGenoIDCsv})"))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    GenoList.Add(new Geno
                    {
                        ID = Int32.Parse(dr["ID"].ToString()),
                        Genotype = Convert.ToString(dr["Genotype"].ToString()),
                        Link = Convert.ToString(dr["Link"].ToString()),
                        Description = Convert.ToString(dr["Description"].ToString()),

                    });
                }
            }

            return GenoList;
        }

        public bool IsUserAnimalIDExist(string UserAnimalID, int ExpID)
        {
            bool flag = false;
            string sql = $"Select count(*) From Animal Where UserAnimalID= '{UserAnimalID}' and ExpID={ExpID}; ";
            int count = Int32.Parse(Dal.ExecScalar(sql).ToString());

            if(count>0) { flag = true; }

            return flag;
            
        }

        public async Task<(int, bool)> GetAnimalIDByUserAnimalIdAndExpIdAsync(string editedUserAnimalId, int expId)
        {
            string sql = @"
                SELECT AnimalID, 
                       CASE 
                           WHEN Sex IS NOT NULL AND gid IS NOT NULL AND sid IS NOT NULL THEN 1 
                           ELSE 0 
                       END AS IsInfoCompleted
                FROM Animal 
                WHERE UserAnimalID = @editedUserAnimalId AND ExpId = @expId;
            ";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@editedUserAnimalId", editedUserAnimalId),
                new SqlParameter("@expId", expId)
            };

            return await Dal.ExecuteQueryAsync(sql, parameters.ToArray(), async reader =>
            {
                if (await reader.ReadAsync())
                {
                    int animalId = reader.GetInt32(reader.GetOrdinal("AnimalID"));
                    bool isInfoCompleted = reader.GetInt32(reader.GetOrdinal("IsInfoCompleted")) == 1;
                    return (animalId, isInfoCompleted);
                }
                return (-1, false); // Default return if no data is found
            });
        }

        public bool ReplaceAnimalId(int oldAnimalId, int existingAnimalIdToUse)
        {
            string sql = $"Update Upload Set AnimalId = {existingAnimalIdToUse} Where AnimalId= {oldAnimalId}; ";
            Dal.ExecuteNonQuery(sql);

            DeleteAnimalByAnimalID(oldAnimalId); 

            return true;
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
