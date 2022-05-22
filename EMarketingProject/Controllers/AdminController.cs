using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using EMarketingProject.Models;
using PagedList;
namespace EMarketingProject.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        dbemarketingEntities db = new dbemarketingEntities();
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(tbl_admin ad)
        {
            tbl_admin admin = (from st in db.tbl_admin where st.ad_username == ad.ad_username && st.ad_password == ad.ad_password select st).FirstOrDefault();
            if(admin==null)
            {
                ViewBag.error = "Invalid Password or UserName";
            }
            else
            {
                FormsAuthentication.SetAuthCookie(ad.ad_username, true);
                Session["ad_id"] =admin.ad_id;
                return RedirectToAction("Create");
            }
            return View();
        }
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(tbl_category cat,HttpPostedFileBase imgfile)
        {
            string path = uploadimgfile(imgfile);
            if (path == "-1")
            {
                ViewBag.Error = "Image Could not Uploaded";
            }
            else
            {
                int id = Convert.ToInt32(Session["ad_id"].ToString());

                tbl_category cate = new tbl_category()
                {
                    cat_name = cat.cat_name,
                    cat_image = uploadimgfile(imgfile),
                    cat_status = 1,
                    cat_fk_ad = Convert.ToInt32(Session["ad_id"].ToString()),

                };

                db.tbl_category.Add(cate);
                db.SaveChanges();
                return RedirectToAction("ViewCategory");
            }
            
            return View();
        }

        public string uploadimgfile(HttpPostedFileBase file)
        {
            Random r = new Random();
            string path = "-1";
            int random = r.Next();
            if (file != null && file.ContentLength > 0)
            {
                string extension = Path.GetExtension(file.FileName);
                if (extension.ToLower().Equals(".jpg") || extension.ToLower().Equals(".jpeg") || extension.ToLower().Equals(".png"))
                {
                    try
                    {

                        path = Path.Combine(Server.MapPath("~/Content/upload"), random + Path.GetFileName(file.FileName));
                        file.SaveAs(path);
                        path = "~/Content/upload/" + random + Path.GetFileName(file.FileName);

                        //    ViewBag.Message = "File uploaded successfully";
                    }
                    catch (Exception ex)
                    {
                        path = "-1";
                    }
                }
                else
                {
                    Response.Write("<script>alert('Only jpg ,jpeg or png formats are acceptable....'); </script>");
                }
            }

            else
            {
                Response.Write("<script>alert('Please select a file'); </script>");
                path = "-1";
            }



            return path;
        }

        public ActionResult ViewCategory(int? page)
        {
            int pagesize = 9, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            //List<tbl_category> all = (from st in db.tbl_category where st.cat_status == 1 select st).OrderByDescending(x=>x.cat_id).ToList();
            var list = db.tbl_category.Where(x => x.cat_status == 1).OrderByDescending(x => x.cat_id).ToList();
            IPagedList<tbl_category> stu = list.ToPagedList(pageindex, pagesize);
            return View(stu);
        }
    }
}