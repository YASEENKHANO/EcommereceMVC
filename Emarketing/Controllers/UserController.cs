using Emarketing.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace Emarketing.Controllers
{
    public class UserController : Controller
    {
        dbEmarketingEntities db = new dbEmarketingEntities();

        // GET: User
        public ActionResult Index(int? page)
        {


            int pagesize = 9, pageindex = 1;

            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;

            //orderbydescending will show the recent added on Top
            var list = db.tb_category.Where(x => x.cat_status == 1).OrderByDescending(x => x.cat_id).ToList();

            IPagedList<tb_category> stu = list.ToPagedList(pageindex, pagesize);





            return View(stu);
        }


        [HttpGet]
        public ActionResult SignUp()
        {
            return View();
        }


        [HttpPost]
        public ActionResult SignUp(tb_user uvm, HttpPostedFileBase imgfile)
        {

            string path = uploadimgfile(imgfile);

            if (path.Equals("-1"))
            {
                ViewBag.error = "Image cannot be uploaded.....";
            }
            else
            {
                tb_user u = new tb_user();

                u.u_name = uvm.u_name;
                u.u_contact = uvm.u_contact;
                u.u_email = uvm.u_email;
                u.u_image = path;
                u.u_password = uvm.u_password;
                db.tb_user.Add(u);
                db.SaveChanges();
                return RedirectToAction("Login");

            }



            return View();
        }




        public ActionResult login()
        {
            return View();
        }






        [HttpPost]
        public ActionResult Login(tb_user tvm)
        {
            //it will fetch one record and check wheather iti exists or not
            tb_user ad = db.tb_user.Where(x => x.u_email == tvm.u_email && x.u_password == tvm.u_password).SingleOrDefault();

            if (ad != null)
            {
                Session["u_id"] = ad.u_id.ToString();
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.error = "Invalid Password or username";
            }

            return View("");
        }

        public string uploadimgfile(HttpPostedFileBase file)
        {
            Random r = new Random();
            string path = "-1";
            int random = r.Next();

            if (file != null && file.ContentLength > 0)
            {
                string extension = Path.GetExtension(file.FileName);
                if (extension.ToLower().Equals(".jpg") || extension.ToLower().Equals(".png") || extension.ToLower().Equals(".jpeg"))
                {
                    try
                    {
                        path = Path.Combine(Server.MapPath("~/Content/upload"), random + Path.GetFileName(file.FileName));
                        file.SaveAs(path);
                        path = "~/Content/upload/" + random + Path.GetFileName(file.FileName);
                    }
                    catch (Exception ex)
                    {
                        path = "-1";

                    }

                }

                else
                {
                    Response.Write("<script>alert('only image is available to select');</script>");
                }


            }
            else
            {
                Response.Write("<script>alert('select a file');</script>");
                path = "-1";
            }
            return path;
        }









        [HttpGet]
        public ActionResult CreateAd()
        {
            List<tb_category> li = db.tb_category.ToList();
            ViewBag.categorylist = new SelectList(li, "cat_id", "cat_name");

            return View();
        }

        [HttpPost]
        public ActionResult CreateAd(tb_product pvm, HttpPostedFileBase imgfile)
        {
            List<tb_category> li = db.tb_category.ToList();
            ViewBag.categorylist = new SelectList(li, "cat_id", "cat_name");


            string path = uploadimgfile(imgfile);
            if (path.Equals("-1"))
            {
                ViewBag.error = "Image could not be uploaded....";
            }
            else
            {
                tb_product p = new tb_product();
                p.pro_name = pvm.pro_name;
                p.pro_price = pvm.pro_price;
                p.pro_image = path;
                p.pro_fk_cat = pvm.pro_fk_cat;
                p.pro_desc = pvm.pro_desc;
                p.pro_fk_user = Convert.ToInt32(Session["u_id"].ToString());
                db.tb_product.Add(p);
                db.SaveChanges();
                Response.Redirect("index");

            }

            return View();
        }


        public ActionResult Ads(int? id, int? page)
        {
            int pagesize = 9, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            var list = db.tb_product.Where(x => x.pro_fk_cat == id).OrderByDescending(x => x.pro_id).ToList();
            IPagedList<tb_product> stu = list.ToPagedList(pageindex, pagesize);

            //comment added...


            return View(stu);


        }

        public ActionResult ViewAd(int? id)
        {
            AdViewModel ad = new AdViewModel();
            tb_product p = db.tb_product.Where(x => x.pro_id == id).SingleOrDefault();

            ad.pro_id = p.pro_id;
            ad.pro_name = p.pro_name;
            ad.pro_image = p.pro_image;
            ad.pro_desc = p.pro_desc;
            ad.pro_price = p.pro_price;
            tb_category cat = db.tb_category.Where(x => x.cat_id == p.pro_fk_cat).SingleOrDefault();
            ad.cat_name = cat.cat_name;
            tb_user u = db.tb_user.Where(x => x.u_id == p.pro_fk_user).SingleOrDefault();
            ad.u_name = u.u_name;
            ad.u_image = u.u_image;
            ad.u_contact = u.u_contact;

            ad.pro_fk_user = u.u_id;

            return View(ad);

        }


        public ActionResult Signout()
        {
            Session.RemoveAll();
            Session.Abandon();

            return RedirectToAction("Index");
        }

        public ActionResult DeleteAd(int? id)
        {
            tb_product p= db.tb_product.Where(x => x.pro_id == id).SingleOrDefault(); 
            db.tb_product.Remove(p);

            db.SaveChanges(); 



            return View("Index");
        }



        [HttpPost]
        public ActionResult Ads(int? id, int? page, string search)
        {
            int pagesize = 9, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            var list = db.tb_product.Where(x => x.pro_name.Contains(search)).OrderByDescending(x => x.pro_id).ToList();

            IPagedList<tb_product> stu = list.ToPagedList(pageindex, pagesize);

            //comment added...


            return View(stu);


        }
    }
}