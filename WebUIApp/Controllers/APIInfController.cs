using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebUIApp.Data;
using WebUIApp.Models;

namespace WebUIApp.Controllers
{
    [Authorize]
    public class APIInfController : Controller
    {

        private readonly ILogger _logger;
        private IConfiguration _configaration;
        private readonly DataContext _db;
        public APIInfController(DataContext db, IConfiguration configaration, ILogger<AlertController> logger)
        {
            _db = db;
            _configaration = configaration;
            _logger = logger;
        }

        public IActionResult Index()
        {
            List<APICredentials> oAPIList = new List<APICredentials>();
            oAPIList= _db.APISettings.ToList();
            return View(oAPIList);
        }

        //GET
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(APICredentials apiCredentials)
        {
           
            
            if (ModelState.IsValid)
            {
                _db.APISettings.Add(apiCredentials);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(apiCredentials);
        }


        // GET: Employees/Delete/1
        public async Task<IActionResult> Delete(int? apiId)
        {
            if (apiId == null)
            {
                return NotFound();
            }
            var api = await _db.APISettings.FirstOrDefaultAsync(m => m.Id == apiId);

            if (api != null)
            {
                _db.APISettings.Remove(api);
                _db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        //AddOrEdit Get Method

        public async Task<IActionResult> Edit(int? apiId)
        {
            ViewBag.PageName = apiId == null ? "Create API Credentials" : "Edit API Credentials";
            ViewBag.IsEdit = apiId == null ? false : true;
            if (apiId == null)
            {
                return View();
            }
            else
            {
                var api = await _db.APISettings.FindAsync(apiId);

                if (api == null)
                {
                    return NotFound();
                }
                return View(api);
            }
        }

        //AddOrEdit Post Method

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int apiId, [Bind("Id,,APIKey,APISecret")] APICredentials oAPIData)
        {
            bool IsCredentialsExist = false;

            APICredentials oapi = await _db.APISettings.FindAsync(apiId);

            if (oapi != null)
            {
                IsCredentialsExist = true;
            }
            else
            {
                oapi = new APICredentials();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    oapi.APIKey = oAPIData.APIKey;
                    oapi.APISecret = oAPIData.APISecret;

                    if (IsCredentialsExist)
                    {
                        _db.Update(oapi);
                    }
                    else
                    {
                        _db.Add(oapi);
                    }
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(oAPIData);
        }

    }
}
