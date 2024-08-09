using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X_MINE.Data;
using X_MINE.Models;

namespace X_MINE.Controllers
{
    [Route("file-manager")]
    public class FileManagerController : Controller
    {
        private readonly AppDBContext _context;
        private readonly ILogger<DeptController> _logger;
        private string controller_name = "FileManager";
        private string title_name = "File Manager";

        public FileManagerController(AppDBContext context, ILogger<DeptController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [Authorize]
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("is_login") == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                var kategori_user_id = HttpContext.Session.GetString("kategori_user_id");

                var cek_kategori_user_id = _context.tbl_r_menu
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

        [HttpPost("upload")]
        public IActionResult uploadFile(Dokumen paramUploads) {
            try
            {
                if (paramUploads != null)
                {
                    /*paramUploads.ForEach(x => {
                        string ids = DateTime.Now.Ticks.ToString();
                        string fileName = x.FileName;
                        string filePath = x.FilePath;
                        Dokumen dokumen = new Dokumen();
                        dokumen.Id = ids;
                        dokumen.FileName = fileName;
                        dokumen.PathFile = filePath;
                        dokumen.UploadTime = DateTime.Now;
                        dokumen.UploadBy = "";

                        _context.dokumens.Add(dokumen);
                        _context.SaveChangesAsync();
                    });*/
                    string ids = DateTime.Now.Ticks.ToString();
                    /*string fileName = paramUploads.FileName;
                    string filePath = paramUploads.FilePath;
                    Dokumen dokumen = new Dokumen();
                    dokumen.Id = ids;
                    dokumen.FileName = fileName;
                    dokumen.PathFile = filePath;
                    dokumen.UploadTime = DateTime.Now;
                    dokumen.UploadBy = "";*/
                    paramUploads.Id = ids;

                    _context.dokumens.Add(paramUploads);
                    _context.SaveChanges();

                }
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest($"error: {ex}");
            }
            
        }
    }
}
