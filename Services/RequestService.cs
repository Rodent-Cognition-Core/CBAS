using AngularSPAWebAPI.Controllers;
using AngularSPAWebAPI.Models;
using CBAS.Helpers;
using Serilog;
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

            try
            {
                Int32.Parse(Dal.ExecScalar(sql).ToString());
            }
            catch (Exception ex)
            {
                Log.Logger.Error($@"ADDNEWTASK: Failed to add task: {request.taskName} in {request.TaskCategory} category to database. Request was sent by {request.Email}");
            }



        }

        public void AddNewPI(Request request)
        {


            string sql = $@"Insert into Request (Type, FullName, Email, PIFullName, PIEmail, PIInstitution) Values
                            ('AddPI', '{HelperService.EscapeSql(request.FullName)}', '{request.Email}', '{HelperService.EscapeSql(request.PIFullName)}',
                             '{HelperService.EscapeSql(request.PIEmail)}', '{HelperService.EscapeSql(request.PIInstitution)}'); SELECT @@IDENTITY AS 'Identity';";
            try
            {
                Int32.Parse(Dal.ExecScalar(sql).ToString());
            }
            catch(Exception ex)
            {
                Log.Logger.Error($@"ADDNEWPI: Failed to add {request.PIFullName} with {request.PIEmail} to database. Request was sent by {request.Email}");
            }

            
        }


        public void AddNewAge(Request request)
        {


            string sql = $@"Insert into Request (Type, FullName, Email, Age) Values
                            ('AddAge', '{HelperService.EscapeSql(request.FullName)}', '{request.Email}', '{HelperService.EscapeSql(request.Age)}'); SELECT @@IDENTITY AS 'Identity';";

            try
            {
                Int32.Parse(Dal.ExecScalar(sql).ToString());
            }
            catch (Exception ex)
            {
                Log.Logger.Error($@"ADDNEWAGE: Failed to add age: {request.Age}  to database. Request was sent by {request.Email}");
            };

        }


        public void AddNewMouseLine(Request request)
        {


            string sql = $@"Insert into Request (Type, FullName, Email, MouseStrain, Genotype, GeneticModification, StrainReference, ControlSuggestion) Values
                            ('AddMouseLine', '{HelperService.EscapeSql(request.FullName)}', '{request.Email}', '{HelperService.EscapeSql(request.MouseStrain)}', '{HelperService.EscapeSql(request.Genotype)}',
                             '{HelperService.EscapeSql(request.GeneticModification)}', '{HelperService.EscapeSql(request.StrainReference)}',
                             '{HelperService.EscapeSql(request.ControlSuggestion)}'); SELECT @@IDENTITY AS 'Identity';";


            try
            {
                Int32.Parse(Dal.ExecScalar(sql).ToString());
            }
            catch (Exception ex)
            {
                Log.Logger.Error($@"ADDNEWMOUSELINE: Failed to add new mouse line for mouse strain: {request.MouseStrain}, genotype: {request.Genotype}, genetic modification: {request.GeneticModification}, strain reference:{request.StrainReference}, and control suggestions: {request.ControlSuggestion}  to database. Request was sent by {request.Email}");
            };

        }

    }
}
