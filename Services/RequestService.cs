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

    public class RequestService
    {

        public void AddNewTask(Request request)
        {         

            
            string sql = $@"Insert into Request (Type, FullName, Email, TaskName, ScheduleName) Values
                            ('AddTask', '{HelperService.EscapeSql(request.FullName)}', '{request.Email}', '{HelperService.EscapeSql(request.taskName)}',
                             '{HelperService.EscapeSql(request.ScheduleName)}'); SELECT @@IDENTITY AS 'Identity';";

            Int32.Parse(Dal.ExecScalar(sql).ToString());

            
        }

        public void AddNewPI(Request request)
        {


            string sql = $@"Insert into Request (Type, FullName, Email, PIFullName, PIEmail, PIInstitution) Values
                            ('AddPI', '{HelperService.EscapeSql(request.FullName)}', '{request.Email}', '{HelperService.EscapeSql(request.PIFullName)}',
                             '{HelperService.EscapeSql(request.PIEmail)}', '{HelperService.EscapeSql(request.PIInstitution)}'); SELECT @@IDENTITY AS 'Identity';";

            Int32.Parse(Dal.ExecScalar(sql).ToString());
            
        }


        public void AddNewAge(Request request)
        {


            string sql = $@"Insert into Request (Type, FullName, Email, Age) Values
                            ('AddAge', '{HelperService.EscapeSql(request.FullName)}', '{request.Email}', '{HelperService.EscapeSql(request.Age)}'); SELECT @@IDENTITY AS 'Identity';";

            Int32.Parse(Dal.ExecScalar(sql).ToString());

        }


        public void AddNewMouseLine(Request request)
        {


            string sql = $@"Insert into Request (Type, FullName, Email, MouseStrain, Genotype, GeneticModification, StrainReference, ControlSuggestion) Values
                            ('AddMouseLine', '{HelperService.EscapeSql(request.FullName)}', '{request.Email}', '{HelperService.EscapeSql(request.MouseStrain)}', '{HelperService.EscapeSql(request.Genotype)}',
                             '{HelperService.EscapeSql(request.GeneticModification)}', '{HelperService.EscapeSql(request.StrainReference)}',
                             '{HelperService.EscapeSql(request.ControlSuggestion)}'); SELECT @@IDENTITY AS 'Identity';";

            Int32.Parse(Dal.ExecScalar(sql).ToString());

        }

    }
}
