using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dapper;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Net;

namespace IS_Test_1.Controllers
{
    public class DocumentController : Controller
    {

        IS_Test_DBEntities db = new IS_Test_DBEntities();

        #region Upload Download file  
        public ActionResult FileUpload()
        {
            return View();
        }
        [HttpPost]
        public ActionResult FileUpload(HttpPostedFileBase files)
        {

            String FileExt = Path.GetExtension(files.FileName).ToUpper();

            if (FileExt == ".PDF")
            {
                Stream str = files.InputStream;
                BinaryReader Br = new BinaryReader(str);
                Byte[] FileDet = Br.ReadBytes((Int32)str.Length);

                Document Fd = new Document();
                Fd.Doc_Name = files.FileName;
                Fd.Doc_Content = FileDet;
                Fd.Client_ID = Session["ID"].ToString();

                db.Documents.Add(Fd);
                db.SaveChanges();
                return RedirectToAction("FileUpload");
            }
            else
            {

                ViewBag.FileStatus = "Invalid file format.";
                return View();

            }

        }

        [HttpGet]
        public FileResult DownLoadFile(int id)
        {
            List<Document> ObjFiles = GetFileList();

            var FileById = (from FC in ObjFiles
                            where FC.Doc_ID.Equals(id)
                            select new { FC.Doc_Name, FC.Doc_Content }).ToList().FirstOrDefault();

            return File(FileById.Doc_Content, "application/pdf", FileById.Doc_Name);

        }
        #endregion

        #region View Uploaded files  
        [HttpGet]
        public PartialViewResult FileDetails()
        {
            //List<Document> DetList = GetFileList();

            IS_Test_DBEntities db = new IS_Test_DBEntities();

            string j = Session["ID"].ToString();

            var docs = (from i in db.Documents
                        where i.Client_ID == j
                        select i).ToList();

            return PartialView("FileDetails", docs);
        }
        private List<Document> GetFileList()
        {
            List<Document> DetList = new List<Document>();

            DbConnection();
            con.Open();
            DetList = SqlMapper.Query<Document>(con, "GetDocumentForClient", commandType: CommandType.StoredProcedure).ToList();
            con.Close();
            return DetList;
        }

        #endregion


        [HttpGet]
        public ActionResult DeleteFile(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var Resul = db.Documents.Find(id);
            if (Resul == null)
            {
                return HttpNotFound();
            }
             
            db.Documents.Remove(Resul);
            db.SaveChanges();
            return RedirectToAction("FileUpload");
        }

        public ActionResult Logout()
        {
            Session["ID"] = "";
            Session["Name"] = "";
            Session["Surname"] = "";

            return RedirectToAction("Index" , "Client");
        
        }


        #region Database related operations  
        private void SaveFileDetails(Document objDet)
        {

            DynamicParameters Parm = new DynamicParameters();
            Parm.Add("@Doc_Name", objDet.Doc_Name);
            Parm.Add("@Doc_Content", objDet.Doc_Content);
            Parm.Add("@Client_ID", objDet.Client_ID);
            DbConnection();
            con.Open();
            con.Execute("AddDocument", Parm, commandType: System.Data.CommandType.StoredProcedure);
            con.Close();
        }
        #endregion

        #region Database connection  

        private SqlConnection con;
        private string constr;
        private void DbConnection()
        {
            //constr = ConfigurationManager.ConnectionStrings["IS_Test_DBEntities"].ToString();
            con = new SqlConnection("data source=DESKTOP-RODZ\\SA;initial catalog=IS_Test_DB;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework");

        }
        #endregion
    }
}