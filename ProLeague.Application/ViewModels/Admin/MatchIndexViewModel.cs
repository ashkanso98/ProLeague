using Microsoft.AspNetCore.Mvc.Rendering;
using ProLeague.Domain.Entities;
using System.Collections.Generic;

namespace ProLeague.Application.ViewModels.Admin
{
    public class MatchIndexViewModel
    {
        public IEnumerable<Domain.Entities.Match> Matches { get; set; } = new List<Domain.Entities.Match>();
        public SelectList Leagues { get; set; }
        public SelectList Weeks { get; set; }

        public int? SelectedLeagueId { get; set; }
        public int? SelectedWeek { get; set; }
    }
}