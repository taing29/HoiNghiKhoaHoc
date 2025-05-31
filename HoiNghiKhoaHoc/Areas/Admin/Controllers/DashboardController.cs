using HoiNghiKhoaHoc.Areas.Admin.Models.ViewModels;
using HoiNghiKhoaHoc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HoiNghiKhoaHoc.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var totalConferences = await _context.Conferences.CountAsync();

            var topFavoritedConferences = await _context.Favorites
                .GroupBy(f => f.ConferenceId)
                .Select(g => new
                {
                    ConferenceId = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(g => g.Count)
                .Take(5)
                .Join(_context.Conferences,
                    fav => fav.ConferenceId,
                    conf => conf.Id,
                    (fav, conf) => new TopConferenceInfo
                    {
                        Conference = conf,
                        Count = fav.Count
                    })
                .ToListAsync();

            var topRegisteredConferences = await _context.ConferenceRegistrations
                .GroupBy(r => r.ConferenceId)
                .Select(g => new
                {
                    ConferenceId = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(g => g.Count)
                .Take(5)
                .Join(_context.Conferences,
                    reg => reg.ConferenceId,
                    conf => conf.Id,
                    (reg, conf) => new TopConferenceInfo
                    {
                        Conference = conf,
                        Count = reg.Count
                    })
                .ToListAsync();

            var vm = new DashboardViewModel
            {
                TotalConferences = totalConferences,
                Top5Registered = topRegisteredConferences,
                Top5Favorited = topFavoritedConferences
            };

            return View(vm);
        }
    }
}
