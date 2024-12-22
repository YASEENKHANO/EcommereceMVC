using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Razor.Parser;
using System.Web.UI;
using System.Windows.Controls;
using Emarketing.Models;
using Microsoft.Ajax.Utilities;
using PagedList;
using PagedList.Mvc;

namespace Emarketing.Controllers
{
    public class AdminController : Controller
    {
        dbEmarketingEntities db = new dbEmarketingEntities();

        // GET: Admin
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }




        [HttpPost]
        public ActionResult Login(tb_admin tvm)
        {
            //it will fetch one record and check wheather iti exists or not
            tb_admin ad = db.tb_admin.Where(x => x.ad_username == tvm.ad_username && x.ad_password == tvm.ad_password).SingleOrDefault();

            if (ad != null)
            {
                Session["ad_id"] = ad.ad_id.ToString();
                return RedirectToAction("Create");
            }
            else
            {
                ViewBag.error = "Invalid Password or username";
            }

            return View();
        }



        public ActionResult Create()
        {
            //used for it is not login then it will be redirected to login again
            if (Session["ad_id"] == null)
            {
                return RedirectToAction("Login");
            }


            return View();
        }


        [HttpPost]
        public ActionResult Create(tb_category tbc, HttpPostedFileBase imgfile)
        {
            string path = uploadimgfile(imgfile);

            if (path.Equals("-1"))
            {
                ViewBag.error = "Image cannot be uploaded.....";
            }
            else
            {
                tb_category cat = new tb_category();
                cat.cat_name = tbc.cat_name;

                cat.cat_image = path;
                cat.cat_status = 1;
                cat.cat_fk_ad = Convert.ToInt32(Session["ad_id"]);

                db.tb_category.Add(cat);
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
                path= "-1";
            }
            return path;
            }




        public ActionResult ViewCategory(int ?page)
        {
            int pagesize = 9, pageindex=1;

            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;

            //orderbydescending will show the recent added on Top
            var list= db.tb_category.Where(x => x.cat_status ==  1 ).OrderByDescending(x => x.cat_id).ToList();

            IPagedList<tb_category> stu= list.ToPagedList(pageindex, pagesize);


       
                
                return View(stu);
        }




        }
}