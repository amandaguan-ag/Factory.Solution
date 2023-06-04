using Microsoft.AspNetCore.Mvc;
using Factory.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Factory.Controllers
{
    public class EngineersController : Controller
    {
        private readonly FactoryContext _db;

        public EngineersController(FactoryContext db)
        {
            _db = db;
        }

        public ActionResult Index()
        {
            return View(_db.Engineers.ToList());
        }

        public ActionResult Details(int id)
        {
            Engineer thisEngineer = _db.Engineers
                .Include(engineer => engineer.JoinEntities)
                .ThenInclude(join => join.Machine)
                .FirstOrDefault(engineer => engineer.EngineerId == id);
            return View(thisEngineer);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Engineer engineer)
        {
            _db.Engineers.Add(engineer);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult AddMachine(int id)
        {
            Engineer thisEngineer = _db.Engineers.FirstOrDefault(engineers => engineers.EngineerId == id);
            ViewBag.MachineId = new SelectList(_db.Machines, "MachineId", "Description");
            return View(thisEngineer);
        }

        [HttpPost]
        public ActionResult AddMachine(Engineer engineer, int machineId)
        {
#nullable enable
            MachineEngineer? joinEntity = _db.MachineEngineers.FirstOrDefault(join => (join.MachineId == machineId && join.EngineerId == engineer.EngineerId));
#nullable disable
            if (joinEntity == null && machineId != 0)
            {
                _db.MachineEngineers.Add(new MachineEngineer() { MachineId = machineId, EngineerId = engineer.EngineerId });
                _db.SaveChanges();
            }
            return RedirectToAction("Details", new { id = engineer.EngineerId });
        }
    }
}