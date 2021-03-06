﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        [ActionName("GetById")]
        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            var obj = GetByIdHelper(id);

            if (obj == null)
                return NotFound();
            var sat = GetSattellitesHelper(id);

            obj.Satellites = sat;

            return Ok(obj);

        }

        private List<CelestialObject> GetSattellitesHelper(int id)
        {
            return _context.CelestialObjects
                .Where(s => s.OrbitedObjectId == id)
                .ToList();
        }

        private CelestialObject GetByIdHelper(int id)
        {
            return _context.CelestialObjects
                .Where(s => s.Id == id)
                .FirstOrDefault();
        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var obj = GetByNameHelper(name);

            if (obj.Count == 0)
                return NotFound();

            foreach (var o in obj)
            {
                List<CelestialObject> sat = GetSattellitesHelper(o.Id);

                o.Satellites = sat;

            }

            return Ok(obj);
        }

        private List<CelestialObject> GetByNameHelper(string name)
        {
            return _context.CelestialObjects
                   .Where(s => s.Name == name).ToList();
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            List<CelestialObject> obj = _context.CelestialObjects.ToList();

            if (obj.Count == 0)
                return NotFound();

            foreach (var o in obj)
            {
                List<CelestialObject> sat = GetSattellitesHelper(o.Id);

                o.Satellites = sat;

            }

            return Ok(obj);
        }

        [HttpPost]
        public IActionResult Create([FromBody] CelestialObject obj)
        {
            _context.CelestialObjects.Add(obj);
            _context.SaveChanges();

            return (CreatedAtRoute("GetById", new { obj.Id },obj));
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id,CelestialObject obj)
        {
            var cObj = GetByIdHelper(id);

            if (cObj == null) return NotFound();

            cObj.Name = obj.Name;
            cObj.OrbitalPeriod = obj.OrbitalPeriod;
            cObj.OrbitedObjectId = obj.OrbitedObjectId;

            _context.Update(cObj);
            _context.SaveChanges();
            return NoContent();

        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id,string name)
        {
            var cObj = GetByIdHelper(id);


            if (cObj == null) return NotFound();

            cObj.Name = name;

            _context.CelestialObjects.Update(cObj);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            List<CelestialObject> obj = _context.CelestialObjects
                .Where(c => (c.Id == id) || (c.OrbitedObjectId == id)).ToList();
            if (obj.Count == 0) return NotFound();

            _context.CelestialObjects.RemoveRange(obj);
            _context.SaveChanges();
            return NoContent();

        }

    }
}
