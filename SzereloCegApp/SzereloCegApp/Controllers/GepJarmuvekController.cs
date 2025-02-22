﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SzereloCegApp.DAL;
using SzereloCegApp.Models;
using SzereloCegApp.ViewModels;

namespace SzereloCegApp.Controllers
{
    [Authorize]
    public class GepJarmuvekController : Controller
    {
        private SzereloCegEntities db = new SzereloCegEntities();

        // GET Index
        public ActionResult Index()
        {
            var gepJarmuvek = db.GepJarmuvek
                .Include(g => g.Diagnosztikák)
                .Include(g => g.Ugyfel);
            return View(gepJarmuvek.ToList());
        }

        // GET Details
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GepJarmu gepJarmu = db.GepJarmuvek
                .Include(g => g.Ugyfel.Szerelo)
                .Where(i => i.ID == id)
                .Include(i => i.Diagnosztikák)
                .Single();
            if (gepJarmu == null)
            {
                return HttpNotFound();
            }
            return View(gepJarmu);
        }
        public ActionResult Szamla(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GepJarmu gepJarmu = db.GepJarmuvek
                .Include(g => g.Ugyfel.Szerelo)
                .Where(i => i.ID == id)
                .Include(i => i.Diagnosztikák)
                .Single();
            if (gepJarmu == null)
            {
                return HttpNotFound();
            }
            return View(gepJarmu);
        }
        [Authorize(Roles = "Admin,Normál")]
        // GET Create1
        public ActionResult Create()
        {
            TulajdonosDropDown();
            var gepjarmu = new GepJarmu();
            gepjarmu.Diagnosztikák = new List<Diagnosztika>();
            AutoDiagnosztikai(gepjarmu);
            return View();
        }

        // POST Create1
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Marka,Tipus,Rendszam,GyartasiEv,UgyfelID")] GepJarmu gepJarmu, string[] SelectedDiag, string[] NotSelectedDiag)
        {
            if (SelectedDiag != null)
            {
                gepJarmu.Diagnosztikák = new List<Diagnosztika>();
                foreach (var hiba in SelectedDiag)
                {
                    var addhiba = db.Diagnosztikák.Find(int.Parse(hiba));
                    gepJarmu.Diagnosztikák.Add(addhiba);
                }
            }
            if (ModelState.IsValid)
            {
                db.GepJarmuvek.Add(gepJarmu);
                db.SaveChanges();
                return RedirectToAction("Index", "Ugyfelek");
            }
            gepJarmu.Diagnosztikák = new List<Diagnosztika>();
            AutoDiagnosztikai(gepJarmu);
            TulajdonosDropDown(gepJarmu.UgyfelID);
            return View(gepJarmu);
        }

        [Authorize(Roles = "Admin,Normál")]
        // GET Edit1
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GepJarmu gepJarmu = db.GepJarmuvek
                .Include(g => g.Diagnosztikák)
                .Where(i => i.ID == id)
                .Single();
            if (gepJarmu == null)
            {
                return HttpNotFound();
            }
            TulajdonosDropDown(gepJarmu.UgyfelID);
            AutoDiagnosztikai(gepJarmu);
            return View(gepJarmu);
        }

        // POST Edit1
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int? id, string[] SelectedDiag, string[] NotSelectedDiag)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var gepJarmuEdit = db.GepJarmuvek
                .Include(g => g.Diagnosztikák)
                .Where(i => i.ID == id)
                .Single();
            if (TryUpdateModel(gepJarmuEdit, "", new string[] {
                "ID",
                "Marka",
                "Tipus",
                "Rendszam",
                "GyartasiEv",
                "UgyfelID" }))
            {
                UpdateAutoDiagnosztika(SelectedDiag, gepJarmuEdit);
                db.Entry(gepJarmuEdit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "Ugyfelek");
            }
            AutoDiagnosztikai(gepJarmuEdit);
            TulajdonosDropDown(gepJarmuEdit.UgyfelID);
            return View(gepJarmuEdit);
        }
        [Authorize(Roles = "Admin,Normál")]
        // GET Delete1
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GepJarmu gepJarmu = db.GepJarmuvek.Find(id);
            if (gepJarmu == null)
            {
                return HttpNotFound();
            }
            return View(gepJarmu);
        }
        
        // POST Delete1
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            GepJarmu gepJarmu = db.GepJarmuvek.Find(id);
            db.GepJarmuvek.Remove(gepJarmu);
            db.SaveChanges();
            return RedirectToAction("Index", "Ugyfelek");
        }

        [Authorize(Roles = "Admin,Normál")]
        //Get Delete2
        public ActionResult DeleteFor(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GepJarmu gepJarmu = db.GepJarmuvek.Find(id);
            if (gepJarmu == null)
            {
                return HttpNotFound();
            }
            return View(gepJarmu);
        }

        // POST Delete2
        [HttpPost, ActionName("DeleteFor")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmedFor(int id)
        {
            GepJarmu gepJarmu = db.GepJarmuvek.Find(id);
            db.GepJarmuvek.Remove(gepJarmu);
            db.SaveChanges();
            return RedirectToAction("Delete", "Ugyfelek", new { id = gepJarmu.UgyfelID });
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }






        #region helper


        //TULAJDONOS DROPDOWN
        private void TulajdonosDropDown(object selectedTulajdonos = null)
        {
            var Query = from s in db.Ugyfelek
                        orderby s.FelvetelIdeje descending
                        select s;
            ViewBag.UgyfelID = new SelectList(Query, "ID", "UgyfelNev", selectedTulajdonos);
        }
        //AUTI-DIAGNOSZTIKA Double ListBox
        private void AutoDiagnosztikai(GepJarmu gepjarmu)
        {
            var allHiba = db.Diagnosztikák; //minden meghibásodás
            var autoHibak = new HashSet<int>(gepjarmu.Diagnosztikák.Select(d => d.ID)); //auto meghibásodások
            var viewModelNotSelected = new List<JarmuDiagnosztikaViewModel>();//nem kiválasztott hibák
            var viewModelSelected = new List<JarmuDiagnosztikaViewModel>();//kiválasztott hibák
            foreach (var hiba in allHiba) //viewmodel feltöltése
            {
                if (autoHibak.Contains(hiba.ID))
                {
                    viewModelSelected.Add(new JarmuDiagnosztikaViewModel
                    {
                        DiagnosztikaID = hiba.ID,
                        DiagnosztikaNeve = hiba.HibaNeve,
                        Hibas = true
                    });
                }
                else
                {
                    viewModelNotSelected.Add(new JarmuDiagnosztikaViewModel
                    {
                        DiagnosztikaID = hiba.ID,
                        DiagnosztikaNeve = hiba.HibaNeve,
                        Hibas = false
                    });
                }
            }
            ViewBag.SelectedDiag = new MultiSelectList(viewModelSelected, "DiagnosztikaID", "DiagnosztikaNeve");
            ViewBag.NotSelectedDiag = new MultiSelectList(viewModelNotSelected, "DiagnosztikaID", "DiagnosztikaNeve");
        }
        //AUTO-Diagnosztika-Update
        private void UpdateAutoDiagnosztika(string[] selectedHibak, GepJarmu GepJarmuToUpdate)
        {
            if (selectedHibak == null)
            {
                GepJarmuToUpdate.Diagnosztikák = new List<Diagnosztika>();
                return;
            }
            var selectedHibakHash = new HashSet<string>(selectedHibak); //checkbox hibák
            var autoHibak = new HashSet<int>(GepJarmuToUpdate.Diagnosztikák.Select(g => g.ID));
            foreach (var hiba in db.Diagnosztikák)
            {
                string hibaid = hiba.ID.ToString();
                if (selectedHibakHash.Contains(hibaid))
                {
                    if (!autoHibak.Contains(hiba.ID))
                    {
                        GepJarmuToUpdate.Diagnosztikák.Add(hiba);
                    }
                }
                else
                {
                    if (autoHibak.Contains(hiba.ID))
                    {
                        GepJarmuToUpdate.Diagnosztikák.Remove(hiba);
                    }
                }
            }
        }
        #endregion

    }
}
