using Microsoft.AspNetCore.Mvc;

namespace HoiNghiKhoaHoc.Controllers
{
    public class ConferencesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
