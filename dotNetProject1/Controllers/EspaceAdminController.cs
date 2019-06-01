using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using dotNetProject1.Models;
using projet_asp_parieAdmin.Report;

namespace dotNetProject1.Controllers
{
    public class EspaceAdminController : Controller
    {
        FirstDBEntities db = new FirstDBEntities();
        // GET: EspaceAdmin
        public ActionResult Index()
        {
            ViewBag.info = db.Candidats.Count((p => p.filiere_choisie == "Génie informatique"));
            ViewBag.proc = db.Candidats.Count((p => p.filiere_choisie == "Génie procede"));
            ViewBag.indus = db.Candidats.Count((p => p.filiere_choisie == "Génie industrielle"));
            ViewBag.reso = db.Candidats.Count((p => p.filiere_choisie == "Génie reseaux"));


            return View("EspaceAdmin");
        }

        public ActionResult Recherche()
        {
            return View(db.Candidats.ToList());
        }

        public ActionResult preselection()
        {

            ViewBag.filiere = new SelectList(db.Candidats,"id", "filiere_choisie");
            ViewBag.diplome = new SelectList(db.Candidats, "id","diplome");

            ViewBag.totale = db.Candidats.Count();

            return View();
        }


        public ActionResult prer()
        {
            FirstDBEntities db = new FirstDBEntities();

            ViewBag.filiere = new SelectList(db.Candidats, "id", "filiere_choisie");


            return View();
        }
        public ActionResult mychar()
        {
            // var filiere = from c in db.Candidats select c.filiere_choisie;
            string[] xv = { "Genie Informatique", "Génie Telecome", "Génie Procéde", "Génie Industrielle" };

            var nbr = db.Candidats.Count(p=>p.filiere_choisie== "Génie informatique");
            var nbr1 = db.Candidats.Count(p => p.filiere_choisie == "Génie procede");
            var nbr2 = db.Candidats.Count(p => p.filiere_choisie == "Génie industrielle");
            var nbr3 = db.Candidats.Count(p => p.filiere_choisie == "Génie tElecome");

            int[] yv = { nbr, nbr3, nbr1, nbr2 };
            new System.Web.Helpers.Chart(width: 600, height: 400, theme: ChartTheme.Yellow)
                    .AddTitle("Nombre des Etudiant inscris pour chaque filiere")
                    .AddSeries("Default",
                           chartType: "Column",
                               xValue: xv,
                               yValues: yv).Write("png");
            
            return null;
        }


        public ActionResult ChartProduit()
        {
            var model = new EspaceAdminController();
            var dataView = db.Candidats.ToList();
            var nbr = db.Candidats.Count(p => p.filiere_choisie == "Génie informatique");
            var nbr1 = db.Candidats.Count(p => p.filiere_choisie == "Génie procede");
            var nbr2 = db.Candidats.Count(p => p.filiere_choisie == "Génie industrielle");
            var nbr3 = db.Candidats.Count(p => p.filiere_choisie == "Génie tElecome");
           string[] xv = { "Genie Informatique", "Génie Telecome", "Génie Procéde", "Génie Industrielle" };
            int[] yv = { nbr, nbr3, nbr1, nbr2 };
            var Mychart = new Chart(width: 600, height: 400, theme: ChartTheme.Vanilla3D)
               .AddSeries("Default", chartType: "pie",
                 xValue: xv,
                 yValues: yv)
               .Write("png");
            return null;
        }


        public ActionResult chartdeplome()
        {
            var model = new EspaceAdminController();
            var dataView = db.Candidats.ToList();
            var nbr = db.Candidats.Count(p => p.diplome == "DUT");
            var nbr1 = db.Candidats.Count(p => p.diplome == "DUG");
            var nbr2 = db.Candidats.Count(p => p.diplome == "LP");
            var nbr3 = db.Candidats.Count(p => p.diplome == "LF");
            var nbr4 = db.Candidats.Count(p => p.diplome == "LST");

            string[] xv = { "DUT", "DUG", "Licence Prof", "Licence Fond", "Licence Science et T" };
            int[] yv = { nbr, nbr1, nbr2, nbr3 , nbr4};
            var Mychart = new Chart(width: 600, height: 400, theme: ChartTheme.Vanilla3D)
               .AddSeries("Default", chartType: "pie",
                 xValue: xv,
                 yValues: yv)
               .Write("png");
            return null;
        }
       
        [HttpGet]
        public ActionResult listepreselection(int filiere, int deplome,  int seuil)
        {
            var e = db.Candidats.Find(filiere);
            var c = db.Candidats.Find(deplome);
            Session["filiere"] = e.filiere_choisie;
            Session["diplome"] = c.diplome;

            var liste = (from p in db.Candidats
                         where p.filiere_choisie ==e.filiere_choisie 
                         && p.diplome==c.diplome
                         orderby p.score descending
                         select p).Take(seuil);
            return View(liste);



        }

        public ActionResult reportGTR(Candidat candidat)
        {
            var filiere = Session["filiere"] as string;
            var diplome = Session["diplome"] as string;


            CandidatReport candidatreport = new CandidatReport();
            byte[] abyte = candidatreport.PrepareReport(gatCandidat(filiere,diplome));
            return File(abyte, "application/pdf");
        }

        public List<Candidat> gatCandidat(String filiere,String deplome)
        {
            List<Candidat> candidats = new List<Candidat>();

            var x = from c in db.Candidats where c.filiere_choisie == filiere && c.diplome == deplome select new { c.nom, c.prnom, c.cin, c.cne };
            foreach (var i in x)
            {
                Candidat c = new Candidat();
                c.nom = i.nom;
                c.prnom = i.prnom;
                c.cin = i.cin;
                c.cne = i.cne;
                candidats.Add(c);

            }
            return candidats;
        }

        // GET: Candidats/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Candidat candidat = db.Candidats.Find(id);
            if (candidat == null)
            {
                return HttpNotFound();
            }
            return View(candidat);
        }

        // GET: Candidats/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Candidat candidat = db.Candidats.Find(id);
            if (candidat == null)
            {
                return HttpNotFound();
            }
            return View(candidat);
        }

        // POST: Candidats/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Candidat candidat = db.Candidats.Find(id);
            db.Candidats.Remove(candidat);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        // GET: Candidats/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Candidat candidat = db.Candidats.Find(id);
            if (candidat == null)
            {
                return HttpNotFound();
            }
            return View(candidat);
        }

        // POST: Candidats/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,nom,prnom,date_naissance,lieu_naissance,adresse,nationalite,ville,tel,fix,cin,email,cne,type_bac,annee_bac,mention_bac,diplome,ecole,ville_ecole,redouble_1ere_annee,redouble_2eme_annee,redouble_3eme_annee,note_S1,note_S2,note_S3,note_S4,note_S5,note_S6,photo_identite,scan_bac,scan_diplome,score,note_concour,filiere_choisie,mopass,mdp")] Candidat candidat)
        {
            if (ModelState.IsValid)
            {
                db.Entry(candidat).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(candidat);
        }


    }
}