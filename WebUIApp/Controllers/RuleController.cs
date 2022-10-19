using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebUIApp.Data;
using WebUIApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.Sqlite;
using System.Data;
using Microsoft.AspNetCore.Authorization;

namespace WebUIApp.Controllers
{
    [Authorize]
    public class RuleController : Controller
    {

        private readonly DataContext _db;
        private IConfiguration _configaration;
        public RuleController(DataContext db, IConfiguration configaration)
        {
            _db = db;
            _configaration = configaration;
        }

        public IActionResult Index()
        {

            List<CRuleInfo> oRuleList = new List<CRuleInfo>();
            //return View(oBotList);
            CRuleInfo oRule = new CRuleInfo();
           
            ServiceStatus oServiceStatus = new ServiceStatus();

            SqliteCommand oCmd = new SqliteCommand();
            DataTable table = new DataTable();
            string Connection = _configaration.GetConnectionString(Constants.DB_CONNECTION);
            using (SqliteConnection conn = new SqliteConnection(Connection))
            {
                conn.Open();
                string ssQl = " select ruleid, rulename,target,r.statusId,(select activestatus from servicestatus where statusid = r.statusId) activestatus " +
                              "  from ruleinfo r";

                oCmd = conn.CreateCommand();
                oCmd.CommandText = ssQl;
                SqliteDataReader oReader = oCmd.ExecuteReader();
                table.Load(oReader);
                oReader.Close();
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    oRule = new CRuleInfo();
                    oRule.RuleID = Convert.ToInt16(table.Rows[i]["ruleid"].ToString());
                    oRule.RuleName = table.Rows[i]["rulename"].ToString();
                    oRule.Target = Convert.ToInt16(table.Rows[i]["target"].ToString());



                    //Service Status
                    oServiceStatus = new ServiceStatus();
                    oServiceStatus.StatusId = Convert.ToInt16(table.Rows[i]["statusId"].ToString() == "" ? "0" : table.Rows[i]["statusId"].ToString());
                    oServiceStatus.ActiveStatus = table.Rows[i]["ActiveStatus"].ToString();
                    oRule.Status = oServiceStatus;


                    oRuleList.Add(oRule);
                }



            }

            return View(oRuleList);


        }

        //GET
        public IActionResult Create()
        {
            // Select all Status
            List<ServiceStatus> oStatuslList = new List<ServiceStatus>();
            oStatuslList = (from status in _db.serviceStatus
                            select status).ToList();
            ViewBag.StatusList = oStatuslList;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CRuleInfo objRule)
        {
            if (ModelState.IsValid)
            {
                _db.RuleInfo.Add(objRule);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(objRule);
        }

        public async Task<IActionResult> Edit(int? ruleId)
        {
            ViewBag.PageName = ruleId == null ? "Create Rule" : "Edit Rule";
            ViewBag.IsEdit = ruleId == null ? false : true;
            if (ruleId == null)
            {
                return View();
            }
            else
            {
                var rule = await _db.RuleInfo.FindAsync(ruleId);

                if (rule == null)
                {
                    return NotFound();
                }

                List<ServiceStatus> oStatuslList = new List<ServiceStatus>();
                oStatuslList = (from status in _db.serviceStatus
                                select status).ToList();
                ViewBag.StatusList = oStatuslList;

                return View(rule);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int ruleId, [Bind("ruleId,RuleName,Target,StatusId")] CRuleInfo ruleData)
        {
            bool IsRuleExist = false;

            CRuleInfo rule = await _db.RuleInfo.FindAsync(ruleId);

            if (rule != null)
            {
                IsRuleExist = true;
            }
            else
            {
                rule = new CRuleInfo();
            }

            if (ModelState.IsValid)
            {
                try
                {
                  
                    rule.RuleName = ruleData.RuleName;
                    rule.Target = ruleData.Target;
                    rule.StatusId = ruleData.StatusId;

                    if (IsRuleExist)
                    {
                        _db.Update(rule);
                    }
                    else
                    {
                        _db.Add(rule);
                    }
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(rule);
        }

        // GET: Rule/Delete/1
        public async Task<IActionResult> Delete(int? ruleId)
        {
            CAlert oAlert = new CAlert();
            
            if (ruleId == null)
            {
                return NotFound();
            }
           
            var rule = await _db.RuleInfo.FirstOrDefaultAsync(m => m.RuleID == ruleId);
            var alert = await _db.AlertInfo.FirstOrDefaultAsync(m => m.RuleID == ruleId);
           
            if (rule != null)
            {
                if (alert == null)
                {
                    _db.RuleInfo.Remove(rule);
                    _db.SaveChanges();
                }
                else
                { 
                
                }
            }
            return RedirectToAction("Index");
        }
        
    }
}
