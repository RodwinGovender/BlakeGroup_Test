using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IS_Test_1.Controllers
{
    public class ClientController : Controller
    {
        IS_Test_DBEntities db = new IS_Test_DBEntities();
        // GET: Client
        public ActionResult Index()
        {
            var retdb = db.GetClientDetails();


            return View(retdb);
        }


        public ActionResult Login(string id)
        {            
            var OneClient = (from i in db.Clients
                             where i.Client_ID == id
                             select i).FirstOrDefault();

            Session["ID"] = OneClient.Client_ID;
            Session["Surname"] = OneClient.Client_Surname;
            Session["Name"] = OneClient.Client_Name;

            return View();
        }

        [HttpPost]
        public ActionResult Login(Client cl)
        {
            if (ModelState.IsValid)
            {
                var CheckClient = (from i in db.Clients
                                   where i.Client_ID == cl.Client_ID
                                   select i).FirstOrDefault();

                if (CheckClient == null)
                {
                    Session["Error"] = "Record does not exist in the database";
                    return RedirectToAction("Index");
                }
                else if (CheckClient.Client_ID == null)
                {
                    Session["Error"] = "ID Number not found";
                    return RedirectToAction("Index");
                }
                else if (CheckClient.Client_ID == cl.Client_ID)
                {
                    Session["Error"] = "Success";
                    Session["ID"] = CheckClient.Client_ID;
                    return RedirectToAction("FileUpload", "Document");
                }
                else
                {
                    Session["Error"] = "Please try again";
                    return RedirectToAction("Index");
                }
            }
            return View(cl);
        }

          



    }
}