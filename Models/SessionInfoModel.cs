using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace AngularSPAWebAPI.Models
{
    public class SessionInfo
    {
        public int SessionID { get; set; }
        public int ExpID { get; set; }
        public int UploadID { get; set; }
        public string UserID { get; set; }
        public int AnimalID { get; set; }
        public string UserAnimalID { get; set; }
        public string Database_Name { get; set; }
        public string Date_Time { get; set; }
        public string Environment { get; set; }
        public string Machine_Name { get; set; }
        public string SessionName { get; set; }
        public string Analysis_Name { get; set; }
        public string Schedule_Name { get; set; }
        public string Guid { get; set; }
        public string Schedule_Run_ID { get; set; }
        public string Version { get; set; }
        public string Version_Name { get; set; }
        public string Application_Version { get; set; }
        public int Max_Number_Trials { get; set; }
        public string Max_Schedule_Time { get; set; }
        public string Schedule_Description { get; set; }
        public string Schedule_Start_Time { get; set; }
        public string Age { get; set; }
        public string Sex { get; set; }
        public string Genotype { get; set; }
        public string Strain { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsDeleted { get; set; }
        public string ErrorMessage { get; set; }
        public List<SessionInfo_Dynamic> SessionInfoDynamics { get; set; }

    }
}
