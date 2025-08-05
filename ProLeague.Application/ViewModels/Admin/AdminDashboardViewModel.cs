using ProLeague.Domain.Entities;
using System.Collections.Generic;

namespace ProLeague.Application.ViewModels.Admin
{
    public class AdminDashboardViewModel
    {
        public int TotalLeagues { get; set; }
        public int TotalTeams { get; set; }
        public int TotalPlayers { get; set; }
        public int TotalNews { get; set; }
        public int TotalComments { get; set; }

        public IEnumerable<Domain.Entities.News> RecentNews { get; set; } = new List<Domain.Entities.News>();
        public IEnumerable<NewsComment> RecentComments { get; set; } = new List<NewsComment>();
    }
}