// ProLeague.Infrastructure/Repositories/UnitOfWork.cs
using ProLeague.Application.Interfaces;
using ProLeague.Infrastructure.Data;
using System.Threading.Tasks;

namespace ProLeague.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public IUserRepository Users { get; private set; }
        public ILeagueRepository Leagues { get; private set; }
        public ITeamRepository Teams { get; private set; }
        public IPlayerRepository Players { get; private set; }
        public INewsRepository News { get; private set; }
        public ICommentRepository Comments { get; private set; }
        public IMatchRepository Matches { get; private set; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Leagues = new LeagueRepository(_context);
            Teams = new TeamRepository(_context);
            Players = new PlayerRepository(_context);
            News = new NewsRepository(_context);
            Comments = new CommentRepository(_context);
            Matches = new MatchRepository(_context);
            Users = new UserRepository(_context);
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            //_context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}