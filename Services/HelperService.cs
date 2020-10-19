using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace CBAS.Helpers
{
    public class HelperService
    {
        public static int? ConvertToNullableInt(string s)
        {
            int i;
            if (int.TryParse(s, out i)) return i;
            return null;
        }

        public static string NullToString(object Value)
        {

            return Value == null ? "" : Value.ToString();
            
        }

        public static DateTime? ConvertToNullableDateTime(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return null;
            }
            else
            {
                return DateTime.Parse(s).ToLocalTime();
            }
        }

        public static string EscapeSql(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return null;
            }
            s = s.Replace("'", @"''");
            return s;
        }

        public static bool SendEmail(string fromEmailAddress, string toEmailAddress, string subject, string body)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.office365.com");

                if (fromEmailAddress == "")
                {
                    fromEmailAddress = "mousebytes@uwo.ca";
                }

                if(toEmailAddress == "")
                {
                    toEmailAddress = "smemarko@uwo.ca";
                }
                
                mail.From = new MailAddress(fromEmailAddress);
                mail.To.Add(toEmailAddress);
                mail.Subject = subject;
                mail.IsBodyHtml = true;
                mail.Body = body;

                SmtpServer.Port = 587;

                SmtpServer.Credentials = new System.Net.NetworkCredential("mousebyt@uwo.ca", "");

                SmtpServer.EnableSsl = true;

                SmtpServer.Send(mail);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }


        public static List<int> GetGenoID(int? strainID)
        {
            List<int> lstGenoID = new List<int>();

            switch (strainID)
            {
                case 1:
                    lstGenoID.Add(1);
                    lstGenoID.Add(4);
                    break;
                case 2:
                    lstGenoID.Add(2);
                    lstGenoID.Add(5);
                    break;
                case 3:
                    lstGenoID.Add(3);
                    lstGenoID.Add(6);
                    break;
                case 4:
                    lstGenoID.Add(4);
                    break;
                case 5:
                    lstGenoID.Add(5);
                    break;
                case 6:
                    lstGenoID.Add(6);
                    break;
                case 7:
                    lstGenoID.Add(7);
                    lstGenoID.Add(11);
                    break;
                case 8:
                    lstGenoID.Add(8);
                    lstGenoID.Add(11);
                    break;
                case 9:
                    lstGenoID.Add(9);
                    lstGenoID.Add(10);
                    lstGenoID.Add(12);
                    break;
                case 10:
                    lstGenoID.Add(12);
                    break;
                case 11:
                    lstGenoID.Add(11);
                    break;
                case 12:
                    lstGenoID.Add(6);
                    lstGenoID.Add(13);
                    lstGenoID.Add(14);
                    lstGenoID.Add(15);
                    break;
                case 13:
                    lstGenoID.Add(6);
                    lstGenoID.Add(13);
                    lstGenoID.Add(16);
                    lstGenoID.Add(17);
                    lstGenoID.Add(18);
                    break;
                case 14:
                    lstGenoID.Add(6);
                    lstGenoID.Add(13);
                    lstGenoID.Add(19);
                    lstGenoID.Add(20);
                    lstGenoID.Add(21);
                    break;
                case 15:
                    lstGenoID.Add(22);
                    lstGenoID.Add(23);
                    lstGenoID.Add(24);
                    break;

                case 16:
                    lstGenoID.Add(11);
                    lstGenoID.Add(25);
                    lstGenoID.Add(26);
                    break;

                case 17:
                    lstGenoID.Add(26);
                    break;

                case 18:
                    lstGenoID.Add(27);
                    lstGenoID.Add(6);
                    break;
                case 19:
                    lstGenoID.Add(28);
                    lstGenoID.Add(6);
                    break;
                case 20:
                    lstGenoID.Add(29);
                    lstGenoID.Add(6);
                    break;
                case 21:
                    lstGenoID.Add(30);
                    lstGenoID.Add(6);
                    break;
                case 22:
                    lstGenoID.Add(31);
                    break;
                case 23:
                    lstGenoID.Add(31);
                    lstGenoID.Add(32);
                    break;
                case 24:
                    lstGenoID.Add(26);
                    lstGenoID.Add(33);
                    break;
                case 25:
                    lstGenoID.Add(31);
                    lstGenoID.Add(34);
                    break;



            }

            return lstGenoID;
        }





    }
}
