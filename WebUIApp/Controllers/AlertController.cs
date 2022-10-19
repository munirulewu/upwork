using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WebUIApp.Data;
using WebUIApp.Models;

namespace WebUIApp.Controllers
{
    [Authorize]
    public class AlertController : Controller
    {
        private readonly ILogger _logger;
        private IConfiguration _configaration;
        private readonly DataContext _db;
        public AlertController(DataContext db,IConfiguration configaration, ILogger<AlertController> logger)
        {
            _db = db;
            _configaration = configaration;
            _logger = logger;
        }

        public IActionResult Index()
        {
            #region Variable
            SqliteCommand oCmd = new SqliteCommand();
            DataTable table = new DataTable();
            CAlert oAlert = new CAlert();
            List<CAlert> Olist = new List<CAlert>();
            CMessage oMessage = new CMessage();
            CRuleInfo oRule = new CRuleInfo();
            #endregion
            string Connection = _configaration.GetConnectionString(Constants.DB_CONNECTION);

            try {

                using (SqliteConnection conn = new SqliteConnection(Connection))
                {
                    conn.Open();
                    string ssQl = " select a.ruleid,a.alertid," +
                                  " (select rulename from ruleinfo where ruleid = a.ruleid) rulename," +
                                  " (select target from ruleinfo where ruleid=a.ruleid) target,a.messageid," +
                                  " (select messagename from messageinfo where messageid = a.messageid) messagename," +
                                  " a.noOfTrigger," +
                                  " a.noOfAlert, a.lastTrueDate " +
                                  " from alertinfo a order by a.ruleid";


                    ssQl = " select ai.ruleid, ai.alertid,(select rulename from ruleinfo where ruleid = ai.ruleid) rulename, " +
                          " (select messagename from messageinfo where messageid = ai.messageid) messagename, ai.lasttruedate," +
                          "  ai.messageid,count(ai.ruleid) ttlMsg,count(ai.messageid) noOfTrigger, count(api.messageid) noOfAlert, " +
                          " (select target from ruleinfo where ruleid = ai.ruleid) target,round(count(api.messageid) * 100.0 / count(ai.messageid), 2) currentp" +
                          "  from alertinfo ai left join(select wh.*, mg.messageid from tradeviewAlert_api wh, messageinfo mg " +
                          "  where wh.messagecode = mg.messagecode) api " +
                          " on api.messageid = ai.messageid group by ai.ruleid, target,ai.messageid ";
                    
                    oCmd = conn.CreateCommand();
                    oCmd.CommandText = ssQl;
                    SqliteDataReader oReader = oCmd.ExecuteReader();
                    table.Load(oReader);
                    oReader.Close();

                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        oAlert = new CAlert();
                        oAlert.alertId = Convert.ToInt16(table.Rows[i]["alertid"].ToString());
                        oAlert.CurrentPercent = table.Rows[i]["currentp"].ToString();
                        oMessage = new CMessage();
                        oMessage.MessageName = table.Rows[i]["messagename"].ToString();
                        oMessage.MessageID = Convert.ToInt16(table.Rows[i]["messageid"].ToString());
                        oAlert.Message = oMessage;
                        oRule = new CRuleInfo();
                        oRule.RuleName = table.Rows[i]["rulename"].ToString();
                        oRule.RuleID = Convert.ToInt16(table.Rows[i]["ruleid"].ToString());
                        oRule.Target = Convert.ToInt16(table.Rows[i]["target"].ToString());
                        oAlert.Rule = oRule;
                        oAlert.NoOFAlert = Convert.ToInt16(table.Rows[i]["noOfAlert"].ToString());
                        oAlert.NoOFTrigger = Convert.ToInt16(table.Rows[i]["noOfTrigger"].ToString());
                        oAlert.LastTrueDate =table.Rows[i]["lastTrueDate"].ToString();
                        Olist.Add(oAlert);
                    }

                }

            }
            catch (Exception exp)
            {
                _logger.LogInformation(exp.ToString());
            }
          
            return View(Olist);
            
        }


        public IActionResult Summary()
        {
            #region Variable
            SqliteCommand oCmd = new SqliteCommand();
            DataTable table = new DataTable();
            CAlert oAlert = new CAlert();
            List<CAlert> Olist = new List<CAlert>();
            CMessage oMessage = new CMessage();
            CRuleInfo oRule = new CRuleInfo();
            #endregion
            string Connection = _configaration.GetConnectionString(Constants.DB_CONNECTION);

            try
            {

                using (SqliteConnection conn = new SqliteConnection(Connection))
                {
                    conn.Open();


                    string ssQl = "select m.ruleid,m.rulename, m.message, c.lasttruedate,c.ttlmsg,c.NoOfAlert,c.noOftrigger,c.target,c.currentp from vw_rulecalc c,  vw_rulemsg m where m.ruleid=c.ruleid";


                    oCmd = conn.CreateCommand();
                    oCmd.CommandText = ssQl;
                    SqliteDataReader oReader = oCmd.ExecuteReader();
                    table.Load(oReader);
                    oReader.Close();

                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        oAlert = new CAlert();
                        oAlert.CurrentPercent = table.Rows[i]["currentp"].ToString();
                        oMessage = new CMessage();
                        oMessage.MessageName = table.Rows[i]["message"].ToString();
                      
                        oAlert.Message = oMessage;
                        oRule = new CRuleInfo();
                        oRule.RuleName = table.Rows[i]["rulename"].ToString();
                        oRule.RuleID = Convert.ToInt16(table.Rows[i]["ruleid"].ToString());
                        oRule.Target = Convert.ToInt16(table.Rows[i]["target"].ToString());
                        oAlert.Rule = oRule;
                        oAlert.NoOFAlert = Convert.ToInt16(table.Rows[i]["NoOfAlert"].ToString());
                        oAlert.NoOFTrigger = Convert.ToInt16(table.Rows[i]["noOftrigger"].ToString());
                        oAlert.LastTrueDate = table.Rows[i]["lasttruedate"].ToString();
                        Olist.Add(oAlert);
                    }

                }

            }
            catch (Exception exp)
            {
                _logger.LogInformation(exp.ToString());
            }

            return View(Olist);

        }

        public IActionResult Create()
        {
            //Select All message
            List<CMessage> oMessageList = new List<CMessage>();
            oMessageList = (from message in _db.messageinfo
                            select message).ToList();
            ViewBag.MessageList = oMessageList;


            List<CRuleInfo> oRuleList = new List<CRuleInfo>();
            oRuleList = (from rule in _db.RuleInfo
                            select rule).ToList();
            ViewBag.RuleList = oRuleList;


            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CAlert objAlert)
        {
            if (ModelState.IsValid)
            {
                _db.AlertInfo.Add(objAlert);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(objAlert);
        }

        public async Task<IActionResult> Edit(int? alertId)
        {


            //Select All message
            List<CMessage> oMessageList = new List<CMessage>();
            oMessageList = (from message in _db.messageinfo
                            select message).ToList();
            ViewBag.MessageList = oMessageList;


            List<CRuleInfo> oRuleList = new List<CRuleInfo>();
            oRuleList = (from rule in _db.RuleInfo
                         select rule).ToList();
            ViewBag.RuleList = oRuleList;



            ViewBag.PageName = alertId == null ? "Create Alert" : "Edit Alert";
            ViewBag.IsEdit = alertId == null ? false : true;
            if (alertId == null)
            {
                return View();
            }
            else
            {
                var rule = await _db.AlertInfo.FindAsync(alertId);

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

        public IActionResult Delete(int? alertId)
        {
            SqliteCommand oCmd = new SqliteCommand();
            int iCount = 0;
            string Connection = _configaration.GetConnectionString(Constants.DB_CONNECTION);
            try
            {

                using (SqliteConnection conn = new SqliteConnection(Connection))
                {
                    conn.Open();
                    string ssQl = "delete from alertinfo where alertid=" + alertId;
                    oCmd = conn.CreateCommand();
                    oCmd.CommandText = ssQl;
                    iCount = oCmd.ExecuteNonQuery();
                   
                    _logger.LogInformation("Alert is deleted. ID:"+alertId);

                }

            }
            catch (Exception exp)
            {
                // add exception message into log file
                _logger.LogInformation(exp.ToString());
            }
            return RedirectToAction("Index");
        }
    }
}
