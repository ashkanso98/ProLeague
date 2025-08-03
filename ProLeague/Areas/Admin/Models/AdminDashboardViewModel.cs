// File: ~/Areas/Admin/Models/AdminDashboardViewModel.cs
using ProLeague.Domain.Entities;
using System.Collections.Generic;

namespace ProLeague.Areas.Admin.Models
{
    public class AdminDashboardViewModel
    {
        public int TotalLeagues { get; set; }
        public int TotalTeams { get; set; }
        public int TotalPlayers { get; set; }
        public int TotalNews { get; set; }
        public int TotalComments { get; set; }

        public List<News> RecentNews { get; set; } = new List<News>();
        public List<NewsComment> RecentComments { get; set; } = new List<NewsComment>();
    }
}