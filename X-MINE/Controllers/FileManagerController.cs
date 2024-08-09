using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Reflection.Metadata;
using X_MINE.Data;
using X_MINE.Models;
using static NuGet.Packaging.PackagingConstants;

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
        public IActionResult uploadFile([FromBody] List<ParamUpload> paramUploads) {
            try
            {
                Console.WriteLine($"{paramUploads.Count} - paramUploads: {paramUploads}");
                if (paramUploads != null)
                {
                    paramUploads.ForEach(x =>
                    {
                        string ids = DateTime.Now.Ticks.ToString();
                        string fileName = x.FileName;
                        string filePath = x.PathFile;
                        Dokumen dokumen = new Dokumen();
                        dokumen.Id = ids;
                        dokumen.FileName = x.FileName;
                        dokumen.PathFile = x.PathFile;
                        dokumen.UploadTime = DateTime.UtcNow;
                        dokumen.UploadBy = HttpContext.Session.GetString("nik");
                        dokumen.PathHash = x.PathHash;

                        _context.dokumens.Add(dokumen);
                        _context.SaveChanges();
                    });
                }
                return Ok(new { success = true, message = "Data telah diupload" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = $"error: {ex}" });
            }
        }

        [HttpPost("delete")]
        public async Task<IActionResult> deleteFile([FromBody] List<String> param)
        {
            try
            {
                List<Dokumen> dokumens = new List<Dokumen>();;
                if (param != null)
                {
                    foreach (var x in param)
                    {
                        Dokumen dokumen = await _context.dokumens.FirstOrDefaultAsync(o => o.PathHash == x);
                        if (dokumen != null) { 
                            dokumens.Add(dokumen);
                        }
                    }
                    if(dokumens.Count > 0)
                    {
                        _context.RemoveRange(dokumens);
                        _context.SaveChanges();
                    }

                }
                return Ok(new { success = true, message = "Data telah dihapus" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = $"error: {ex}" });
            }
        }
    }
}
