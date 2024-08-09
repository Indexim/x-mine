using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using X_MINE.Data;
using X_MINE.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;

namespace X_MINE.Controllers
{
    [Authorize]
    public class MenuController : Controller
    {
        private readonly AppDBContext _context;
        private readonly ILogger<MenuController> _logger;
        private string controller_name = "Menu";
        private string title_name = "Menu";

        public MenuController(AppDBContext context, ILogger<MenuController> logger)
        {
            _context = context;
            _logger = logger;
        }

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

        [Authorize]
        public IActionResult GetAll()
        {
            try
            {
                var data = _context.tbl_r_menu.OrderBy(x => x.type).ToList();
                return Json(new { data = data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching data.");
                return Json(new { success = false, message = $"Terjadi kesalahan saat mengambil data ${ex.Message}." });
            }
        }

        [Authorize]
        public ActionResult GetAllByKategoriUser()
        {
            try
            {
                var kategoriId = HttpContext.Session.GetInt32("kategori_id");
                if (kategoriId.HasValue)
                {
                    var results = _context.tbl_r_menu
                        .Where(x => x.kategori_user_id == kategoriId.Value.ToString())
                        .OrderBy(x => x.type)
                        .ToList();
                    return Json(new { success = true, data = results });
                }
                else
                {
                    return Json(new { success = false, message = "ID kategori pengguna tidak ditemukan dalam sesi." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching data.");
                return Json(new { success = false, message = $"Terjadi kesalahan saat mengambil data {ex.Message}." });
            }
        }

        [Authorize]
        public IActionResult Get(int id)
        {
            try
            {
                var result = _context.tbl_r_menu.Where(x => x.id == id).FirstOrDefault();

                if (result == null)
                {
                    return Json(new { success = false, message = "Data tidak ditemukan." });
                }

                return Json(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Terjadi kesalahan saat mengambil data karyawan.");
                return Json(new { success = false, message = $"Terjadi kesalahan saat mengambil data: {ex.Message}" });
            }
        }

        [HttpPost]
        public IActionResult InsertData(tbl_r_menu a)
        {
            try
            {
                //a.ip = System.Environment.MachineName;
                //a.created_at = DateTime.Now;
                _context.tbl_r_menu.Add(a);
                _context.SaveChanges();
                return Json(new { success = true, message = "Data berhasil ditambahkan." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Terjadi kesalahan saat menambahkan data.");
                return Json(new { success = false, message = $"Terjadi kesalahan saat menambahkan data: {ex.Message}" });
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult Update(tbl_r_menu a)
        {
            try
            {
                var tbl_ = _context.tbl_r_menu.FirstOrDefault(f => f.id == a.id);
                if (tbl_ != null)
                {
                    tbl_.id = a.id;
                    tbl_.kategori_user_id = a.kategori_user_id;
                    tbl_.type = a.type;
                    tbl_.title = a.title;
                    tbl_.link_controller = a.link_controller;
                    tbl_.link_function = a.link_function;
                    tbl_.hidden = a.hidden;
                    tbl_.new_tab = a.new_tab;
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
                var tbl_ = _context.tbl_r_menu.FirstOrDefault(f => f.id == id);
                if (tbl_ != null)
                {
                    _context.tbl_r_menu.Remove(tbl_);
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
                _logger.LogError(ex, "Terjadi kesalahan saat menghapus data.");
                return Json(new { success = false, message = $"Terjadi kesalahan saat menghapus data: {ex.Message}" });
            }
        }
    }
}
