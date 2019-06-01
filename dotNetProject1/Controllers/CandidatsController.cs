using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using dotNetProject1.Models;

namespace projet_concour.Controllers
{
    public class CandidatsController : Controller
    {
        private FirstDBEntities db = new FirstDBEntities();

        public ActionResult confirmation()
        {

            return View();
        }

        // GET: Candidats/Create
        public ActionResult Create()
        {

            return View();
        }
        public ActionResult etape1()
        {
            return View();
        }
        public ActionResult etape2()
        {
            ViewBag.m = new SelectList(
             new List<SelectListItem>
             {
                   new SelectListItem { Text = "excellent",Value=" excellent" },
        new SelectListItem { Text = "tres bien",Value=" tres bien" },
        new SelectListItem { Text = "bien", Value = "bien" },  new SelectListItem { Text = "passable", Value = "passable" }, }, "Value", "Text");
            ViewBag.e = new SelectList(
               new List<SelectListItem>
               {
                   new SelectListItem { Text = "math",Value=" math" },
        new SelectListItem { Text = "technique",Value=" technique" },
        new SelectListItem { Text = "pc", Value = "pc" },  new SelectListItem { Text = "svt", Value = "svt" }, }, "Value", "Text");
            return View();
        }
        // POST: Candidats/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create([Bind(Include = "redouble_1ere_annee,redouble_2eme_annee,redouble_3eme_annee,note_S1,note_S2," +
            "note_S3,note_S4,note_S5,note_S6")]
        Candidat candidat3)
        {
            Candidat c1 = new Candidat();
            c1 = (Candidat)Session["etape1_can"];
            Candidat c2 = new Candidat();
            c2 = (Candidat)Session["etape2_can"];

            Candidat candidat = new Candidat(c1.nom, c1.prnom, c1.date_naissance, c1.lieu_naissance, c1.adresse, c1.nationalite, c1.ville, c1.tel, c1.fix, c1.cin, c1.email, c1.password, c1.password2
             , c2.cne, c2.type_bac, c2.annee_bac, c2.mention_bac, c2.diplome, c2.ecole, c2.ville_ecole, candidat3.redouble_1ere_annee, candidat3.redouble_2eme_annee, candidat3.redouble_3eme_annee, candidat3.note_S1, candidat3.note_S2,
            candidat3.note_S3, candidat3.note_S4, candidat3.note_S5, candidat3.note_S6, c1.photo_identite, c2.scan_bac, c2.scan_diplome, c2.filiere_choisie);
            //Génerer score
            //note diplome
            if (candidat.note_S5 == null && candidat.note_S6 == null)
            {
                double? note = (candidat.note_S1 + candidat.note_S2 + candidat.note_S3 + candidat.note_S4) / 4;
                if (note <= 12)
                    candidat.score = 0;
                if (12 < note && note <= 14)
                    candidat.score = 4;
                if (14 < note && note <= 16)
                    candidat.score = 8;
                if (16 < note && note <= 18)
                    candidat.score = 12;
            }
            if (candidat.note_S5 != null && candidat.note_S6 != null)
            {
                double? note = (candidat.note_S1 + candidat.note_S2 + candidat.note_S3 + candidat.note_S4 + candidat.note_S5 + candidat.note_S6) / 4;
                if (note <= 12)
                    candidat.score = 0;
                if (12 < note && note <= 14)
                    candidat.score = 4;
                if (14 < note && note <= 16)
                    candidat.score = 8;
                if (16 < note && note <= 18)
                    candidat.score = 12;
            }
            //redoublement
            if (candidat.redouble_1ere_annee == true || candidat.redouble_2eme_annee == true || candidat.redouble_3eme_annee == true) { 
                candidat.score = 0; }
            //type bac
            switch (candidat.type_bac)
            {
                case "math":candidat.score += 3;
                    break;

                case "technique": candidat.score += 2;
                    break;

                case "pc":candidat.score += 1;
                    break;
                default:
               candidat.score += 0;
                    break;


            }
            //mention bac
            switch (candidat.mention_bac)
            {
                case "excellent":
                    candidat.score += 3;
                    break;

                case "tres bien":
                    candidat.score += 2;
                    break;

                case "bien":
                    candidat.score += 1;
                    break;
                default:
                    candidat.score += 0;
                    break;


            }

            ModelState.Clear();
            if (ModelState.IsValid == true)
            {
                try
                {
                    db.Candidats.Add(candidat);
                    db.SaveChanges();
                }
                catch (DbEntityValidationException e)
                {
                    var errors = db.GetValidationErrors();
                    ViewBag.nom = errors;
                    return View();

                }
            }
                return RedirectToAction("confirmation");
            



        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult etape1([Bind(Include = "nom,prnom,date_naissance,lieu_naissance,adresse," +
            "nationalite,ville,tel,fix,cin,email,password,password2")]
        Candidat candidat1, HttpPostedFileBase img1)
        {
            if (img1 != null)
            {

                candidat1.photo_identite = new byte[img1.ContentLength];
                img1.InputStream.Read(candidat1.photo_identite, 0, img1.ContentLength);
            }

            if (ModelState.IsValid==true){
                Session["etape1_can"] = candidat1;
                return RedirectToAction("etape2");

           }
         
            return View();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult etape2([Bind(Include = "cne,type_bac,annee_bac,mention_bac,diplome," +
            "ecole,ville_ecole,scan_bac,scan_diplome,filiere_choisie")]
        Candidat candidat2, string x,string d,HttpPostedFileBase img2, HttpPostedFileBase img3)
        {
            candidat2.type_bac = x;
            candidat2.mention_bac = d;
            if (img2 != null)
            {
                candidat2.scan_bac = new byte[img2.ContentLength];
                img2.InputStream.Read(candidat2.scan_bac, 0, img2.ContentLength);
            }
            if (img3 != null)
            {
                candidat2.scan_diplome = new byte[img2.ContentLength];
                img3.InputStream.Read(candidat2.scan_diplome, 0, img3.ContentLength);
            }
          
               
                Session["etape2_can"] = candidat2;

                return RedirectToAction("Create");
           
        }
    }
}
