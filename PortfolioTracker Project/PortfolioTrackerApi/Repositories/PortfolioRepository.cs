using Microsoft.EntityFrameworkCore;
using PortfolioTrackerApi.DAL;
using PortfolioTrackerApi.DTOS;
using PortfolioTrackerApi.Entities;

namespace PortfolioTrackerApi.Repositories
{
    public class PortfolioRepository : IPortfolioRepository
    {
        private readonly AppDbContext _context;

        public PortfolioRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddPortfolioAsync(Portfolio entity)
        {
            _context.Portfolios.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<(IEnumerable<PortfolioResponseDto>, int)> GetPaginatedPortfoliosAsync(string userId, int pageNumber, int pageSize)
        {
            var query = _context.Portfolios.Where(p => p.UserId == userId).Select(p => new PortfolioResponseDto
            {
                Id=p.Id,
                Name = p.Name,
                StocksCount = p.Stocks.Count()
            }); ;
            int totalCount = await query.CountAsync();

            var portfolios = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (portfolios, totalCount);
        }

        public async Task<Portfolio> GetPortfolioByIdAsync(int portfolioId)
        {
            return await _context.Portfolios.FirstOrDefaultAsync(p => p.Id == portfolioId);
        }

        public async Task<Portfolio> GetPortfolioStocksAsync(int portfolioId)
        {
            return await _context.Portfolios.Include(p => p.Stocks).FirstOrDefaultAsync(p => p.Id == portfolioId);
        }
        public async Task<IEnumerable<Portfolio>> GetUserPortfoliosWithStocksAsync(string userId)
        {
            return await _context.Portfolios
                .Where(p => p.UserId == userId)
                .Include(p => p.Stocks)
                .ToListAsync();
        }
        public async Task<List<Portfolio>> GetAllPortfoliosWithStocksAsync()
        {
            return await _context.Portfolios
                .Include(p => p.Stocks)
                .ToListAsync();
        }
    }
}
