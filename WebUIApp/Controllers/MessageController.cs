using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebUIApp.Data;
using WebUIApp.Models;
namespace WebUIApp.Controllers
{
    [Authorize]
    public class MessageController : Controller
    {
        private readonly DataContext _db;
        private IConfiguration _configaration;
        public MessageController(DataContext db , IConfiguration configaration)
        {
            _db = db;
            _configaration = configaration;
        }

        public IActionResult Index()
        {
            //if (HttpContext.Session[Constants.LOGIN_USER] == null)
            //{

            //}
            //else
            //{ 
            
            //}
                

            IEnumerable<CMessage> oMessageList = _db.messageinfo;
            return View(oMessageList);
        }


        public string GetMessageCode()
        {
            SqliteCommand oCmd = new SqliteCommand();
            string sMessageCode = "";
            string Connection = _configaration.GetConnectionString(Constants.DB_CONNECTION);
            using (SqliteConnection conn = new SqliteConnection(Connection))
            {
                conn.Open();
                string ssQl = "select case when count(*)=0 then 'MG001' else 'MG'|| cast(max(messageid)+1 as varchar)  end from messageinfo";
                oCmd = conn.CreateCommand();
                oCmd.CommandText = ssQl;
                object obj = oCmd.ExecuteScalar();
                sMessageCode = obj.ToString();

            }
            return sMessageCode;

        }

        //GET
        public IActionResult Create()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CMessage objMessage)
        {

            string sMessageCode = GetMessageCode();
            objMessage.MessageCode = sMessageCode;
            if (ModelState.IsValid)
            {
                _db.messageinfo.Add(objMessage);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(objMessage);
        }

        // GET: Employees/Delete/1
        public async Task<IActionResult> Delete(int? messageId)
        {
            if (messageId == null)
            {
                return NotFound();
            }
            var message = await _db.messageinfo.FirstOrDefaultAsync(m => m.MessageID == messageId);

            if (message != null)
            {
                _db.messageinfo.Remove(message);
                _db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        //AddOrEdit Get Method

        public async Task<IActionResult> Edit(int? messageId)
        {
            ViewBag.PageName = messageId == null ? "Create Message" : "Edit Message";
            ViewBag.IsEdit = messageId == null ? false : true;
            if (messageId == null)
            {
                return View();
            }
            else
            {
                var message = await _db.messageinfo.FindAsync(messageId);

                if (message == null)
                {
                    return NotFound();
                }
                return View(message);
            }
        }
        
        //AddOrEdit Post Method
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int messageId, [Bind("MessageID,MessageName")] CMessage messageData)
        {
            bool IsMessageExist = false;

            CMessage message = await _db.messageinfo.FindAsync(messageId);

            if (message != null)
            {
                IsMessageExist = true;
            }
            else
            {
                message = new CMessage();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    message.MessageName = messageData.MessageName;
                    

                    if (IsMessageExist)
                    {
                        _db.Update(message);
                    }
                    else
                    {
                        _db.Add(message);
                    }
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(messageData);
        }
        

    }
}
