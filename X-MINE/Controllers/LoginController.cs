using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Xwatch.Data;
using Xwatch.Models;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Xwatch.Controllers
{
    public class LoginController : Controller
    {
        private readonly AppDBContext _context;
        private readonly ILogger<LoginController> _logger;

        public LoginController(AppDBContext context, ILogger<LoginController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("is_login") != null)
            {
                var kategoriUserId = HttpContext.Session.GetString("kategori_user_id");

                var cekKategoriUserId = _context.tbl_r_kategori_user
                    .FirstOrDefault(x => x.kategori == kategoriUserId);

                if (cekKategoriUserId != null)
                {
                    return RedirectToAction(cekKategoriUserId.login_function, cekKategoriUserId.login_controller);
                }
                else
                {
                    _logger.LogWarning("Category user ID not found: {KategoriUserId}", kategoriUserId);
                    return View();
                }
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> ProsesLogin(string nik, string password)
        {
            try
            {
                var user = _context.tbl_m_user_login
                    .FirstOrDefault(x => x.nik == nik && x.password == password);

                if (user != null)
                {
                    HttpContext.Session.SetString("is_login", "true");
                    HttpContext.Session.SetString("nik", user.nik);
                    HttpContext.Session.SetString("nama", user.nama);

                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.nik)
            };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
                    };

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                    var kategori_user = _context.tbl_m_user_login
                        .Where(x => x.nik == user.nik)
                        .ToList();

                    return Json(new { status = true, remarks = "Login Sukses: " + user.kategori_user_id, data = kategori_user });
                }
                else
                {
                    return Json(new { status = false, remarks = "Login gagal. Username/password salah" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { status = false, remarks = "Gagal", data = ex.Message.ToString() });
            }
        }


        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Login");
        }

        [HttpGet]
        public ActionResult CekKategoriUser(string kategori_user_id)
        {
            try
            {
                var results = _context.tbl_r_kategori_user
                    .FirstOrDefault(x => x.kategori == kategori_user_id);

                if (results != null)
                {
                    HttpContext.Session.SetString("kategori_user_id", results.kategori);
                    return Json(new { status = true, remarks = "Sukses", data = results });
                }
                else
                {
                    return Json(new { status = false, remarks = "Kategori user tidak ditemukan" });
                }
            }
            catch (Exception e)
            {
                return Json(new { status = false, remarks = "Gagal", data = e.Message.ToString() });
            }
        }
    }
}
