﻿namespace final_aerialview.Models
{
    public class DashboardDataModel
    {
        public int DashId { get; set; }
        public string? DashName { get; set; }  
        
        public string? DashPath { get; set; }   

        public bool DashStatus { get; set; }

        public bool DashDefault {  get; set; }
    }
}