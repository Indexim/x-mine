﻿using Microsoft.AspNetCore.Authorization;
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

        [HttpPost("upload")]
        public async Task<IActionResult> uploadFile(Dokumen paramUploads)
        {
            try
            {
                if (paramUploads != null)
                {
                    // Ambil nilai 'nik' dari sesi
                    string nik = HttpContext.Session.GetString("nik");

                    // Jika nilai 'nik' tidak ada, kembalikan BadRequest
                    if (nik == null)
                    {
                        return BadRequest("Session 'nik' is not available.");
                    }

                    // Set nilai 'nik' ke properti UploadBy
                    paramUploads.UploadBy = nik;

                    paramUploads.Status = "OPEN";
                    string ids = DateTime.Now.Ticks.ToString();
                    paramUploads.Id = ids;

                    // Tambahkan dokumen ke konteks dan simpan
                    _context.dokumens.Add(paramUploads);
                    await _context.SaveChangesAsync();

                    return Ok();
                }
                return BadRequest("No file provided.");
            }
            catch (Exception ex)
            {
                return BadRequest($"error: {ex.Message}");
            }
        }


    }
}
