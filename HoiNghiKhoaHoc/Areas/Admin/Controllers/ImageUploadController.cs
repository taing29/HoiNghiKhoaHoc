using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HoiNghiKhoaHoc.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ImageUploadController : Controller
    {
        private readonly IWebHostEnvironment _env;

        public ImageUploadController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [HttpPost]
        public ActionResult UploadImage(List<IFormFile> upload)
        {
            var filepath = "";
            foreach (IFormFile photo in Request.Form.Files)
            {
                string serverMapPath = Path.Combine(_env.WebRootPath, "Image", photo.FileName);
                using (var stream = new FileStream(serverMapPath, FileMode.Create))
                {
                    photo.CopyTo(stream);
                }
                filepath = "http://localhost:5128/" + "Image/" + photo.FileName;
            }

            return Json(new { url = filepath });
        }

    }
}
