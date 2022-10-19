using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;

using System.Linq;
using System.Threading.Tasks;
using WebUIApp.Data;
using WebUIApp.Models;
namespace WebUIApp.Controllers
{
    [Authorize]
    public class BotController : Controller
    {
        private readonly DataContext _db;
        private IConfiguration _configaration;
        public BotController(DataContext db, IConfiguration configaration)
        {
            _db = db;
            _configaration = configaration;
        }

        public IActionResult Index()
        {

            List<CBot> botList = new List<CBot>(); 
            //return View(oBotList);
            CBot oBot = new CBot();
            BuySell oBuySell = new BuySell();
            CMessage oMessage = new CMessage();
            ServiceStatus oServiceStatus = new ServiceStatus();

            SqliteCommand oCmd = new SqliteCommand();
            DataTable table = new DataTable();
            string Connection = _configaration.GetConnectionString(Constants.DB_CONNECTION);
            using (SqliteConnection conn = new SqliteConnection(Connection))
            {
                conn.Open();
                string ssQl = " select b.id, b.pairs,b.botid,b.botname, b.messageid,(select messageName from messageinfo where messageid = b.messageid) messageName," +
                            " b.buyselId,(select buysellstatus from buysell where BuyselId = b.buyselId) buysellstatus," +
                            " b.baseordersize,b.delay,b.statusId,"+
                            " (select ActiveStatus from ServiceStatus where statusId = b.statusId) ActiveStatus "+
                            " from botinfo b";
               
                oCmd = conn.CreateCommand();
                oCmd.CommandText = ssQl;
                SqliteDataReader oReader = oCmd.ExecuteReader();
                table.Load(oReader);
                oReader.Close();
               
                string sBuyId = "";
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    oBot = new CBot();
                    oBot.id = Convert.ToInt16(table.Rows[i]["id"].ToString());
                    oBot.botid = table.Rows[i]["botid"].ToString();
                    oBot.botname = table.Rows[i]["botname"].ToString();
                    oBot.Pairs = table.Rows[i]["pairs"].ToString();
                    oBuySell = new BuySell();
                    sBuyId = table.Rows[i]["buyselId"].ToString();
                    oBuySell.BuySellStatus = table.Rows[i]["buysellstatus"].ToString();
                    oBuySell.BuyselId = Convert.ToInt16(sBuyId);
                    oBot.buySell = oBuySell;
                    
                    //Service Status
                    oServiceStatus = new ServiceStatus();
                    oServiceStatus.StatusId = Convert.ToInt16( table.Rows[i]["statusId"].ToString()==""?"0": table.Rows[i]["statusId"].ToString());
                    oServiceStatus.ActiveStatus =table.Rows[i]["ActiveStatus"].ToString();
                    oBot.Status= oServiceStatus;

                    //Message
                    oMessage = new CMessage();
                    oMessage.MessageID = Convert.ToInt16(table.Rows[i]["messageid"].ToString() == "" ? "0" : table.Rows[i]["messageid"].ToString());
                    oMessage.MessageName= table.Rows[i]["messageName"].ToString();
                    oBot.Message = oMessage;

                    oBot.baseordersize = Convert.ToInt16(table.Rows[i]["baseordersize"].ToString() == "" ? "0" : table.Rows[i]["baseordersize"].ToString());
                    oBot.delay = Convert.ToInt16(table.Rows[i]["delay"].ToString() == "" ? "0" : table.Rows[i]["delay"].ToString());
                    //Additing Values into List
                    botList.Add(oBot);
                }
               


            }

            return View(botList);
            

        }

        private bool IsExists(int Id)
        {
            bool isTrue = false;
            int iVal = 0;
            SqliteCommand oCmd = new SqliteCommand();
            DataTable table = new DataTable();
            string Connection = _configaration.GetConnectionString(Constants.DB_CONNECTION);
            using (SqliteConnection conn = new SqliteConnection(Connection))
            {
                conn.Open();
                string ssQl = " select count(*) ttl from botinfo b where b.id=" + Id;

                oCmd = conn.CreateCommand();
                oCmd.CommandText = ssQl;
                Object obj = oCmd.ExecuteScalar();
                iVal = Convert.ToInt16(obj);
                
            }

            if (iVal == 1)
                isTrue = true;
            return isTrue;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("id,botid,botname,Pairs,delay,StatusId,BuyselId, MessageID,baseordersize")] CBot BotData)
        {
            bool IsBotExists = false;

            int iVal = 0;
            CBot oBot = new CBot();
            if (IsExists(id))
            {
                IsBotExists = true;
            }
           

            if (ModelState.IsValid)
            {
                try
                {
                    //oBot.BuyselId=BotData.BuyselId;
                    //oBot.botid = BotData.botid;
                    //oBot.delay = BotData.delay;
                    //oBot.StatusId= BotData.StatusId;
                    //oBot.MessageID = BotData.MessageID;
                    //oBot.baseordersize = BotData.baseordersize;


                    SqliteCommand oCmd = new SqliteCommand();
                    DataTable table = new DataTable();
                    string Connection = _configaration.GetConnectionString(Constants.DB_CONNECTION);
                    using (SqliteConnection conn = new SqliteConnection(Connection))
                    {
                        conn.Open();
                        string ssQl = " update botinfo set "+
                                      "  messageid = "+ BotData.MessageID+","+
                                      " botid ='"+BotData.botid+"',"+
                                      " botname='"+BotData.botname+"',"+
                                      " buyselid ="+BotData.BuyselId + ","+
                                      " baseordersize ="+BotData.baseordersize+","+
                                      " delay ="+BotData.delay+","+
                                      " pairs ='" + BotData.Pairs + "'," +
                                      " statusid =" +BotData.StatusId+""+
                                      "  where id = "+ id;

                        oCmd = conn.CreateCommand();
                        oCmd.CommandText = ssQl;
                        iVal = oCmd.ExecuteNonQuery();

                    }

                }
                catch (Exception exp)
                {
                    throw;
                }
               if(iVal==1)
                return RedirectToAction(nameof(Index));
            }
            return View(BotData);
        }
        public async Task<IActionResult> Edit(int? Id)
        {
            CBot oBot = new CBot();
            BuySell oBuySell = new BuySell();
            ServiceStatus oServiceStatus = new ServiceStatus();
            CMessage oMessage = new CMessage();
            //Select All message
            

            if (Id == null)
            {
                return View();
            }
            else
            {
                
               // var selectedId = _db.messageinfo.Find(Id);

                //ViewBag.ProductParentCategoryId = new SelectList(_db.messageinfo.All() , "alertId", "ProductCategoryTitle", (int)selectedId.ProductParentCategoryId);
                //ViewBag.GroupFiltersId = new SelectList(_groupFiltersService.GetAllGroupFilter().Where(a => a.GroupFilterParentId == null), "GroupFilterId", "GroupFilterTitle");
                //return View(_productCategoryService.GetOneProductCategory(id));


                //var bot = await _db.BotInfo.FindAsync(Id);
                //if (bot == null)
                //{
                //    return NotFound();
                //}
                //else
                //{
                SqliteCommand oCmd = new SqliteCommand();
                DataTable table = new DataTable();
                string Connection = _configaration.GetConnectionString(Constants.DB_CONNECTION);
                using (SqliteConnection conn = new SqliteConnection(Connection))
                {
                    conn.Open();
                    string ssQl = " select b.id, b.botid,b.botname, b.pairs, b.messageid,(select messageName from messageinfo where messageid = b.messageid) messageName," +
                                " b.buyselId,(select buysellstatus from buysell where BuyselId = b.buyselId) buysellstatus," +
                                " b.baseordersize,b.delay,b.statusId," +
                                " (select ActiveStatus from ServiceStatus where statusId = b.statusId) ActiveStatus " +
                                " from botinfo b where b.id=" + Id;

                    oCmd = conn.CreateCommand();
                    oCmd.CommandText = ssQl;
                    SqliteDataReader oReader = oCmd.ExecuteReader();
                    table.Load(oReader);
                    oReader.Close();

                    string sBuyId = "";
                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        oBot = new CBot();
                        oBot.id = Convert.ToInt16(table.Rows[i]["id"].ToString());
                        oBot.botid = table.Rows[i]["botid"].ToString();
                        oBot.botname = table.Rows[i]["botname"].ToString();
                        oBot.Pairs = table.Rows[i]["pairs"].ToString();

                        oBuySell = new BuySell();
                        sBuyId = table.Rows[i]["buyselId"].ToString();
                        oBuySell.BuySellStatus = table.Rows[i]["buysellstatus"].ToString();
                        oBuySell.BuyselId = Convert.ToInt16(sBuyId);
                        oBot.BuyselId = Convert.ToInt16(sBuyId);
                        oBot.buySell = oBuySell;

                        //Service Status
                        oServiceStatus = new ServiceStatus();
                        oServiceStatus.StatusId = Convert.ToInt16(table.Rows[i]["statusId"].ToString() == "" ? "0" : table.Rows[i]["statusId"].ToString());
                        oServiceStatus.ActiveStatus = table.Rows[i]["ActiveStatus"].ToString();
                        oBot.StatusId = Convert.ToInt16(table.Rows[i]["statusId"].ToString() == "" ? "0" : table.Rows[i]["statusId"].ToString());
                        oBot.Status = oServiceStatus;

                        //Message
                        oMessage = new CMessage();
                        oMessage.MessageID = Convert.ToInt16(table.Rows[i]["messageid"].ToString() == "" ? "0" : table.Rows[i]["messageid"].ToString());
                        oMessage.MessageName = table.Rows[i]["messageName"].ToString();
                        oBot.MessageID = Convert.ToInt16(table.Rows[i]["messageid"].ToString() == "" ? "0" : table.Rows[i]["messageid"].ToString());
                        oBot.Message = oMessage;

                        oBot.baseordersize = Convert.ToInt16(table.Rows[i]["baseordersize"].ToString() == "" ? "0" : table.Rows[i]["baseordersize"].ToString());
                        oBot.delay = Convert.ToInt16(table.Rows[i]["delay"].ToString() == "" ? "0" : table.Rows[i]["delay"].ToString());
                        //Additing Values into List

                    }

                    
                    var MessageList = (from message in _db.messageinfo
                                    select message).ToList();
                    ViewBag.MessageList = ToSelectList(MessageList); 
                    // Select all BuySell
                    
                    var BuySellList = (from product in _db.buysell
                                    select product).ToList();
                    ViewBag.BuySellList = ToSelectList(BuySellList);
                   

                    // Select all Status
                   // List<ServiceStatus> oStatuslList = new List<ServiceStatus>();
                    var StatuslList = (from status in _db.serviceStatus
                                    select status).ToList();
                    ViewBag.StatusList = ToSelectList( StatuslList);
                }
               

                return View(oBot);
            }


            
        }

        [NonAction]
        public SelectList ToSelectList(List<BuySell> lstService)
        {
            List<SelectListItem> list = new List<SelectListItem>();

            foreach (BuySell item in lstService)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.BuySellStatus,
                    Value = Convert.ToString(item.BuyselId)
                });
            }

            return new SelectList(list, "Value", "Text");
        }

        //MsaageList
        [NonAction]
        public SelectList ToSelectList(List<CMessage> lstService)
        {
            List<SelectListItem> list = new List<SelectListItem>();

            foreach (CMessage item in lstService)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.MessageName,
                    Value = Convert.ToString(item.MessageID)
                });
            }

            return new SelectList(list, "Value", "Text");
        }

        // Active Status List
        [NonAction]
        public SelectList ToSelectList(List<ServiceStatus> lstService)
        {
            List<SelectListItem> list = new List<SelectListItem>();

            foreach (ServiceStatus item in lstService)
            {
                list.Add(new SelectListItem()
                {
                    Text = item.ActiveStatus,
                    Value = Convert.ToString(item.StatusId)
                });
            }

            return new SelectList(list, "Value", "Text");
        }

        //GET. This function will return the template
        public IActionResult Create()
        {
            //Select All message
            List<CMessage> oMessageList = new List<CMessage>();
            oMessageList = (from message in _db.messageinfo
                        select message).ToList();
            ViewBag.MessageList = oMessageList;
            // Select all BuySell
            List<BuySell> oBuySellList = new List<BuySell>();
            oBuySellList = (from product in _db.buysell
                            select product).ToList();
            ViewBag.BuySellList = oBuySellList;

            // Select all Status
            List<ServiceStatus> oStatuslList = new List<ServiceStatus>();
            oStatuslList = (from status in _db.serviceStatus
                            select status).ToList();
            ViewBag.StatusList = oStatuslList;

            return View();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CBot objBot)
        {
            if (ModelState.IsValid)
            {
                SqliteCommand oCmd = new SqliteCommand();
                int iCount = 0;
                string Connection = _configaration.GetConnectionString(Constants.DB_CONNECTION);
                try
                {

                    using (SqliteConnection conn = new SqliteConnection(Connection))
                    {
                        conn.Open();
                        string ssQl = "insert into  botinfo(messageid,botid,botname,BuyselId, baseordersize,delay, statusId, createdate,pairs) values("+objBot.MessageID+","+objBot.botid + ",'"+objBot.botname+"',"+objBot.BuyselId+","+objBot.baseordersize+","+objBot.delay+","+objBot.StatusId+ ",datetime(),'"+objBot.Pairs+"')";
                        oCmd = conn.CreateCommand();
                        oCmd.CommandText = ssQl;
                        iCount = oCmd.ExecuteNonQuery();

                    }

                }
                catch (Exception exp)
                {
                    // add exception message into log file
                }

                if (iCount == 1)
                {
                    return RedirectToAction("Index");
                }
              
            }
            return View(objBot);
        }

        public  IActionResult Delete(int? botid)
        {
            SqliteCommand oCmd = new SqliteCommand();
            int iCount = 0;
            string Connection = _configaration.GetConnectionString(Constants.DB_CONNECTION);
            try
            {

                using (SqliteConnection conn = new SqliteConnection(Connection))
                {
                    conn.Open();
                    string ssQl = "delete from botinfo where id="+ botid;
                    oCmd = conn.CreateCommand();
                    oCmd.CommandText = ssQl;
                    iCount = oCmd.ExecuteNonQuery();

                }

            }
            catch (Exception exp)
            {
                // add exception message into log file
            }
            return RedirectToAction("Index");
        }
    }
}
