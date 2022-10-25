using AngularSPAWebAPI.Controllers;
using AngularSPAWebAPI.Models;
using CBAS.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace AngularSPAWebAPI.Services
{

    public class SearchExpService
    {
        public List<SearchExp> GetAllExpList()
        {
            List<SearchExp> lstExp = new List<SearchExp>();

            using (DataTable dt = Dal.GetDataTable($@"select Experiment.ExpID, Experiment.ExpName, task.name as CognitiveTask, Case Experiment.Status
													   When 1 Then 'Public'
													   When 0 Then 'Private'
											  	
													   End as Status,
													    
														STUFF((SELECT distinct ' <br/>' +  Age.AgeInMonth 
                                                                FROM SubExperiment
																inner join Age on Age.ID = SubExperiment.AgeID
                                                                Where SubExperiment.ExpID = Experiment.ExpID  
                                                                --order by Age.AgeInMonth
                                                                FOR XML PATH(''), type
                                                        ).value('.', 'nvarchar(max)'),1,6,'') As Age,

														STUFF((SELECT distinct ' <br/>' + Strain.Strain
                                                                FROM Animal 
																inner join Strain on Strain.ID = Animal.SID
                                                                Where Animal.ExpID = Experiment.ExpID
                                                                FOR XML PATH(''), type
                                                        ).value('.', 'nvarchar(max)'),1,6,'') As Strain,

														STUFF((SELECT distinct ', ' + Genotype.Genotype 
                                                                FROM Animal
																inner join Genotype on Genotype.ID = Animal.GID
                                                                Where Animal.ExpID = Experiment.ExpID
                                                                FOR XML PATH(''), type
                                                        ).value('.', 'nvarchar(max)'),1,2,'') As Genotype,


														Experiment.TaskDescription, Concat(Experiment.StartExpDate, '-', Experiment.EndExpDate) as Period ,tt2.PISiteName, tt2.UserName as Username, tt2.Email,
														Experiment.DOI
				
														From Experiment 
 
                                                        inner join task on task.ID = Experiment.TaskID
														
                                                        inner join
                                                        
                                                        (Select PUSID, PIUserSite.PSID, tt.PISiteName, CONCAT(AspNetUsers.GivenName, ' ', AspNetUsers.FamilyName) as UserName, AspNetUsers.Email as Email From PIUserSite
                                                        inner join
                                                        (Select PSID, PI.PName, Site.Institution, CONCAT(PName, ' - ', Institution) as PISiteName From PISite
                                                        inner join PI on PI.PID = PISite.PID
                                                        inner join Site on Site.SiteID = PISite.SiteID
														
														) as tt on tt.PSID = PIUserSite.PSID
														inner join AspNetUsers on AspNetUsers.Id = PIUserSite.UserID) 
														 as tt2 on tt2.PUSID = Experiment.PUSID"))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    lstExp.Add(new SearchExp
                    {
                        ExpId = Convert.ToString(dr["ExpID"].ToString()),
                        ExpName = Convert.ToString(dr["ExpName"].ToString()),
                        CognitiveTask = Convert.ToString(dr["CognitiveTask"].ToString()),
                        Status = Convert.ToString(dr["Status"].ToString()),
                        Age = Convert.ToString(dr["Age"].ToString()),
                        Strain = Convert.ToString(dr["Strain"].ToString()),
                        Genotype = Convert.ToString(dr["Genotype"].ToString()),
                        TaskDescription = Convert.ToString(dr["TaskDescription"].ToString()),
                        Period = Convert.ToString(dr["Period"].ToString()),
                        PISiteName = Convert.ToString(dr["PISiteName"].ToString()),
                        Username = Convert.ToString(dr["Username"].ToString()),
                        Email = Convert.ToString(dr["Email"].ToString()),
                        DOI = Convert.ToString(dr["DOI"].ToString()),

                    });
                }

            }

            return lstExp;
        }



    }
}
