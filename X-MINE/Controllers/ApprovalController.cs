using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using X_MINE.Data;

namespace X_MINE.Controllers
{

    public class ApprovalController : Controller
    {
        private readonly AppDBContext _context;
        private readonly ILogger<ApprovalController> _logger;
        private string controller_name = "Approval";
        private string title_name = "Approval";

        [Authorize]
        public ActionResult Index()
        {
            if (HttpContext.Session.GetString("is_login") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                var kategori_user_id = HttpContext.Session.GetString("kategori_user_id");

                var cek_kategori_user_id = _context.tbl_r_menu
                    .Where(x => x.link_controller == controller_name)
                    .Where(x => x.kategori_user_id == kategori_user_id)
                    .Count();

                if (cek_kategori_user_id > 0)
                {
                    ViewBag.Title = title_name;
                    ViewBag.Controller = controller_name;
                    // ViewBag.Setting = _context.tbl_m_setting_aplikasi.FirstOrDefault();
                    ViewBag.Menu = _context.tbl_r_menu
                        .Where(x => x.kategori_user_id == kategori_user_id)
                        .OrderBy(x => x.title)
                        .ToList();
                    ViewBag.MenuMasterCount = _context.tbl_r_menu
                        .Where(x => x.kategori_user_id == kategori_user_id)
                        .Where(x => x.type == "Master")
                        .OrderBy(x => x.title)
                        .Count();
                    ViewBag.MenuMineDocCount = _context.tbl_r_menu
                        .Where(x => x.kategori_user_id == kategori_user_id)
                        .Where(x => x.type == "MineDoc")
                        .OrderBy(x => x.title)
                        .Count();
                    ViewBag.insert_by = HttpContext.Session.GetString("nik");
                    ViewBag.departemen = HttpContext.Session.GetString("dept_code");
                    return View();
                }
                else
                {
                    return RedirectToAction("Index", "Login");
                }
            }
        }
        public ApprovalController(AppDBContext context, ILogger<ApprovalController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult GetAll()
        {
            try
            {
                var data = _context.dokumens.OrderBy(x => x.UploadTime).ToList();
                return Json(new { data = data });
            }
            catch (Exception ex)
            {
                var innerExceptionMessage = ex.InnerException?.Message ?? ex.Message;
                _logger.LogError(ex, "Error fetching data.");
                return Json(new { success = false, message = $"Terjadi kesalahan saat mengambil data ${ex.Message}." });
            }
        }
    }
}
