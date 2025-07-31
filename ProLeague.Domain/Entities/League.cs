//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.Linq;
//using System.Numerics;
//using System.Text;
//using System.Threading.Tasks;

//namespace ProLeague.Domain.Entities
//{
//    public class League
//    {
//        public int Id { get; set; }
//        [Required]
//        [StringLength(100)]
//        public string Name { get; set; } = string.Empty;

//        [StringLength(50)] public string Country { get; set; } = string.Empty;
//        public string? ImagePath { get; set; } // Path to the league logo
//        public ICollection<Team> Teams { get; set; } = new List<Team>();
//        public ICollection<News> News { get; set; } = new List<News>();
//    }
//}

// ProLeague.Domain/Entities/League.cs
using System.ComponentModel.DataAnnotations;

namespace ProLeague.Domain.Entities
{
    public class League
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(50)]
        public string Country { get; set; }

        public string? ImagePath { get; set; } // Path to the league logo

        public ICollection<Team> Teams { get; set; } = new List<Team>();
        public ICollection<News> News { get; set; } = new List<News>();
    }
}