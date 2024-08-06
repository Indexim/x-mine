using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using X_MINE.Data;
using X_MINE.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;

namespace Xwatch.Controllers
{
    public class DeptController : Controller
    {
        private readonly AppDBContext _context;
        private readonly ILogger<UserController> _logger;
        private string controller_name = "Dept";
        private string title_name = "Dept";

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
                    ViewBag.MenuRosterCount = _context.tbl_r_menu
                        .Where(x => x.kategori_user_id == kategori_user_id)
                        .Where(x => x.type == "Roster")
                        .OrderBy(x => x.title)
                        .Count();
                    ViewBag.MenuMasterCount = _context.tbl_r_menu
                        .Where(x => x.kategori_user_id == kategori_user_id)
                        .Where(x => x.type == "Master")
                        .OrderBy(x => x.title)
                        .Count();
                    ViewBag.MenuTransaksiCount = _context.tbl_r_menu
                        .Where(x => x.kategori_user_id == kategori_user_id)
                        .Where(x => x.type == "Transaksi")
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
        public DeptController(AppDBContext context, ILogger<UserController> logger)
        {
            _context = context;
            _logger = logger;
        }
        public IActionResult GetAll()
        {
            try
            {
                var data = _context.tbl_r_dept.OrderBy(x => x.created_at).ToList();
                return Json(new { data = data });
            }
            catch (Exception ex)
            {
                var innerExceptionMessage = ex.InnerException?.Message ?? ex.Message;
                _logger.LogError(ex, "Error fetching data.");
                return Json(new { success = false, message = $"Terjadi kesalahan saat mengambil data ${ex.Message}." });
            }
        }

        public IActionResult Get(int id)
        {
            try
            {
                var result = _context.tbl_r_dept.Where(x => x.id == id).FirstOrDefault();

                if (result == null)
                {
                    return Json(new { success = false, message = "Data tidak ditemukan." });
                }

                return Json(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                var innerExceptionMessage = ex.InnerException?.Message ?? ex.Message;
                _logger.LogError(ex, "Terjadi kesalahan saat mengambil data karyawan.");
                return Json(new { success = false, message = $"Terjadi kesalahan saat mengambil data: {ex.Message}" });
            }
        }
        [Authorize]
        [HttpPost]
        public IActionResult Insert(tbl_r_dept a)
        {
            try
            {
                a.ip = System.Environment.MachineName;
                //a.created_at = DateTime.Now;
                _context.tbl_r_dept.Add(a);
                _context.SaveChanges();
                return Json(new { success = true, message = "Data berhasil disimpan." });
            }
            catch (Exception ex)
            {
                var innerExceptionMessage = ex.InnerException?.Message ?? ex.Message;
                _logger.LogError(ex, $"Terjadi kesalahan saat menambahkan data. Detail: {innerExceptionMessage}");
                return Json(new { success = false, message = $"Terjadi kesalahan saat menambahkan data: {innerExceptionMessage}" });
            }
        }

        [Authorize]

        [HttpPost]
        public ActionResult Update(tbl_r_dept a)
        {
            try
            {
                var tbl_ = _context.tbl_r_dept.FirstOrDefault(f => f.id == a.id);
                if (tbl_ != null)
                {
                    tbl_.id = a.id;
                    tbl_.dept_code = a.dept_code;
                    tbl_.departemen = a.departemen;
                    tbl_.insert_by = a.insert_by;
                    a.ip = System.Environment.MachineName;
                    //tbl_.updated_at = DateTime.Now;
                    _context.SaveChanges();
                    return Json(new { success = true, message = "Data berhasil diubah." });
                }
                else
                {
                    return Json(new { success = false, message = "Data tidak ditemukan." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Terjadi kesalahan saat update data.");
                return Json(new { success = false, message = $"Terjadi kesalahan saat update data: {ex.Message}" });
            }
        }
        [Authorize]
        [HttpPost]
        public IActionResult Delete(int id)
        {
            try
            {
                var tbl_ = _context.tbl_r_dept.FirstOrDefault(f => f.id == id);
                if (tbl_ != null)
                {
                    _context.tbl_r_dept.Remove(tbl_);
                    _context.SaveChanges();
                    return Json(new { success = true, message = "Data berhasil dihapus." });
                }
                else
                {
                    return Json(new { success = false, message = "Data tidak ditemukan." });
                }
            }
            catch (Exception ex)
            {
                var innerExceptionMessage = ex.InnerException?.Message ?? ex.Message;
                _logger.LogError(ex, "Terjadi kesalahan saat menghapus data.");
                return Json(new { success = false, message = $"Terjadi kesalahan saat menghapus data: {ex.Message}" });
            }
        }
    }
}
