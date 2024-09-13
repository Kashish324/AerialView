namespace final_aerialview.Models
{
    public class ReportConnectionModel
    {
        public int ConNo { get; set; }
        public string ConName{get; set;}

        public string ConType{get; set;}

        public string ServerName{get; set;}
        public string DatabaseName{get; set; }
        public string? UserName{get; set; }
        public string? Password{get; set; }
        public string? stringName{get; set; }

        //"Hccb_Khudra_Gea": "XpoProvider=MSSqlServer;data source=192.168.1.202;user id=sa;password=admin@123;initial catalog=HCCB_Khurda_GEA;Persist Security Info=true;TrustServerCertificate=true"
    }
}


