using DE6.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace DE6.Controllers
{
    public class HomeController : Controller
    {
        DE6Entities db = new DE6Entities();

        public ActionResult Index()
        {
            //check token còn thời gian k
            bool check = checkToken();
            if (!check)
            {
                return RedirectToAction("Login");
            }
            List<ThongTinCB> list = db.ThongTinCBs.Where(x => x.IsDelete == false).ToList();
            return View(list);
        }

        public ActionResult LichSu(int id)
        {
            //check token còn thời gian k
            bool check = checkToken();
            if (!check)
            {
                return RedirectToAction("Login");
            }
            List<LichSu> list = db.LichSus.Where(x => x.IsDelete == false && x.IDTTCB == id && x.ThongTinCB.IsDelete == false).ToList();
            return View(list);
        }
        public ActionResult DenHan()
        {

            //check token còn thời gian k
            bool check = checkToken();
            if (!check)
            {
                return RedirectToAction("Login");
            }
            List<LichSu> l = new List<LichSu>();
            List<LichSu> list = db.LichSus.Where(x => x.IsDelete == false  && x.ThongTinCB.IsDelete == false).ToList();
            foreach (var item in list)
            {
                double test = ((double)((DateTimeOffset)item.NgayKetThuc).ToUnixTimeSeconds() - (double)((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds());
                if(test <= 604800 && test > 0)
                {
                    l.Add(item);
                }
}
            return View(l);
        }
        public ActionResult Login()
        {
            return View();
        }
        public ActionResult LogOut()
        {
            Session["Login"] = null;
            return View("Login");
        }
        public bool checkToken()
        {
            var access_token = Session["access_token"];
            if (access_token == null)
            {
                //return RedirectToAction("Login");
                return false;
            }
            else
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(Convert.ToString(ConfigurationManager.AppSettings["config:JwtKey"]));
                tokenHandler.ValidateToken(access_token.ToString(), new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero

                }, out SecurityToken validatedToken);

                // Corrected access to the validatedToken
                var jwtToken = (JwtSecurityToken)validatedToken;
                if (jwtToken.ValidTo < DateTime.UtcNow)
                {
                    return false;
                    //return RedirectToAction(Action);
                }


            }
            return true;
            //return RedirectToAction("Login");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Login user)
        {
            var securityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(ConfigurationManager.AppSettings["config:JwtKey"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            string hashedPassword = HashPassword(user.Password, "12345!#aB");

            User u = db.Users.FirstOrDefault(x => x.UserName == user.UserName && x.Pass == hashedPassword && x.Role == 1);

            if (u != null)
            {
                var claims = new[]
        { new Claim("ID", u.ID.ToString()),
                    new Claim("UserName", u.UserName),
                    new Claim("Role", u.Role.ToString())
                    // Add more claims if needed
                };

                var accessToken = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.UtcNow.AddHours(1), // Token expires in 1 hour
                    signingCredentials: credentials
                );

                var refreshToken = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.UtcNow.AddDays(7), // Token expires in 7day
                    signingCredentials: credentials
                );
                var access_token = new JwtSecurityTokenHandler().WriteToken(accessToken);
                var refresh_token = new JwtSecurityTokenHandler().WriteToken(refreshToken);
                Models.Token to = new Models.Token()
                {
                    Users_ID = u.ID,
                    access_token = access_token,
                    refresh_token = refresh_token,
                };
                db.Tokens.Add(to);
                db.SaveChanges();

                Session["access_token"] = access_token;
                //Session["refresh_token"] = refresh_token;
                Session["Login"] = true;
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "Login data is incorrect!");
            }
            return View();
        }
        public ActionResult Error(string MaError)
        {
            ViewBag.Error = MaError;
            return View();
        }

        public ActionResult Success(string Success)
        {
            ViewBag.Success = Success;
            return View();
        }
        public ActionResult Delete(int id)
        {

            //check token còn thời gian k
            bool check = checkToken();
            if (!check)
            {
                return RedirectToAction("Login");
            }
            if (id == null)
            {
                return Json(new { mess = "fail" }, JsonRequestBehavior.AllowGet);
            }
            ThongTinCB nen = db.ThongTinCBs.Find(id);
            if (nen == null)
            {
                return Json(new { mess = "fail" }, JsonRequestBehavior.AllowGet);
            }
            nen.IsDelete = true;
            db.SaveChanges();
            return Json(new { mess = "success" }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Create()
        {
            //check token còn thời gian k
            bool check = checkToken();
            if (!check)
            {
                return RedirectToAction("Login");
            }
            return View();
        }

        [HttpPost]
        public ActionResult Create(FormCollection collection, ThongTinCB tt)
        {
            //check token còn thời gian k
            bool check = checkToken();
            if (!check)
            {
                return RedirectToAction("Login");
            }
            try
            {
                int loai = Int32.Parse(collection["Loai"]);
                string text = "";
                switch (loai)
                {

                    case 0:
                        text = "Bến xe";
                        break;
                    case 1:
                        text = "Bãi";
                        break;
                    case 2:
                        text = "Chợ";
                        break;
                    default:
                        return RedirectToAction("Error", "Home", new { MaError = "Lỗi loại" });
                        break;
                }
                tt.Loai = text;
                tt.IsDelete = false;

                if (db.ThongTinCBs.SingleOrDefault(x => x.Ten.Equals(tt.Ten) && x.Loai.Equals(tt.Loai)) == null)
                {

                    db.ThongTinCBs.Add(tt);

                    db.SaveChanges();
                    return RedirectToAction("Success", "Home", new { Success = "Thêm thành công" });
                }
                else
                {
                    return RedirectToAction("Error", "Home", new { @MaError = "Tên or loại không được trùng" });
                }


            }
            catch (Exception x)
            {
                return RedirectToAction("Error", "Home", new { @MaError = x.Message });
            }
        }

        public ActionResult Details(int? id)
        {
            //check token còn thời gian k
            bool check = checkToken();
            if (!check)
            {
                return RedirectToAction("Login");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ThongTinCB nen = db.ThongTinCBs.Find(id);
            if (nen == null)
            {
                return HttpNotFound();
            }
            return View(nen);
        }
        public ActionResult Edit(int id)
        {
            //check token còn thời gian k
            bool check = checkToken();
            if (!check)
            {
                return RedirectToAction("Login");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ThongTinCB nen = db.ThongTinCBs.Find(id);
            //int num = -1;
            //switch (nen.Ten)
            //{

            //    case "Bến":
            //        num = 0;
            //        break;
            //    case "Bãi":
            //        num = 1;
            //        break;
            //    case "Chợ":
            //        num = 2;
            //        break;
            //    default:
            //        num = -1;
            //        break;
            //}
            //List<SelectListItem> items = new List<SelectListItem>
            //    {
            //        new SelectListItem { Value = "0", Text = "Option 1" },
            //        new SelectListItem { Value = "1", Text = "Option 2" },
            //        new SelectListItem { Value = "2", Text = "Option 3" }
            //    };
            //ViewBag.MaNCC = new SelectList(items, "Value", "Text", );
            if (nen == null)
            {
                return HttpNotFound();
            }
            return View(nen);
        }
        public static string HashPassword(string password, string salt)
        {
            using (var sha256 = SHA256.Create())
            {
                var saltedPassword = password + salt;
                var passwordBytes = Encoding.UTF8.GetBytes(saltedPassword);
                var hashBytes = sha256.ComputeHash(passwordBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }
        public ActionResult Register()
        {
          
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(FormCollection collection, User u)
        {
            string pass = u.Pass;
            string rePass = collection["RePassword"];
            if (!pass.Equals(rePass))
            {
                return RedirectToAction("Error", "Home", new { @MaError = "Mật khẩu không trùng khớp!" });

            }
            if (db.Users.SingleOrDefault(x => x.UserName.Equals(u.UserName)) != null)
            {

                return RedirectToAction("Error", "Home", new { @MaError = "Tên Username đã tồn tại!" });


            }

            string hashedPassword = HashPassword(pass, "12345!#aB");
            User user = new User()
            {
                UserName = u.UserName,
                Pass = hashedPassword,
                Role = 1,

            };
            db.Users.Add(user);
            db.SaveChanges();
            return RedirectToAction("Success", "Home", new { Success = "Tạo tài khoản thành công" });
        }

        [HttpPost]
        public ActionResult Edit(ThongTinCB tt, FormCollection collection)
        {
            //check token còn thời gian k
            bool check1 = checkToken();
            if (!check1)
            {
                return RedirectToAction("Login");
            }
            try
            {
                int loai = Int32.Parse(collection["Loai"]);
                string text = "";
                switch (loai)
                {
                    
                    case 0:
                        text = "Bến xe";
                        break;
                    case 1:
                        text = "Bãi";
                        break;
                    case 2:
                        text = "Chợ";
                        break;
                    default:
                        return RedirectToAction("Error", "Home", new { MaError = "Lỗi loại" });
                        break;
                }
                if (tt.ID != null)
                {
                    ThongTinCB check = db.ThongTinCBs.SingleOrDefault(n => n.ID == tt.ID);
                    if (check == null) return RedirectToAction("Error", "Home", new { MaError = "Không tìm thấy" });

                    check.IsDelete = false;
                    check.Ten = tt.Ten;
                    check.Loai = text;
                    check.DiaChi = tt.DiaChi;
                    check.MoTa = tt.MoTa;
                    check.GiaTri = tt.GiaTri;
                    check.TenDonViCQ = tt.TenDonViCQ;
                    
                    db.Entry(check).State = EntityState.Modified;
                    db.SaveChanges();
                }

                return RedirectToAction("Success", "Home", new { Success = "Sửa thành công" });
            }
            catch (Exception e)
            {
                return RedirectToAction("Error", "Home", new { MaError = e.Message });
            }


        }

    }
}