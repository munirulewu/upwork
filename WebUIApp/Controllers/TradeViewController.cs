/**
 * File name            : TradeviewAPI.cs
 * Author               : Munirul Islam
 * Date                 : September 24.09.2022
 * Version              : 1.0
 *
 * Description          : This is the webhook API from the Tradeview 
 *
 * Modification history:
 * Name                         Date                            Desc
 * 
 * DOT NOR WORK WITH THIS BUYER. HE DOES NOT PAY MY BILL.   MUNIRUL32@GMAIL.COM
 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebUIApp.Data;
using WebUIApp.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data;
using XCommas.Net.Objects;

// Wen hook Url is developed.
namespace WebUIApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TradeViewController : ControllerBase
    {

        private DataContext _context;
        private IConfiguration _configaration;
        private readonly ILogger<TradeViewController> _logger;
        private string NEWDEAL = "NEW_DEAL";
        private string SELL = "SELL";
        public TradeViewController(DataContext context, IConfiguration configaration, ILogger<TradeViewController> logger)
        {
            _context = context;
            _configaration = configaration;
            _logger = logger;

        }


        private bool isValid()
        {

            SqliteCommand oCmd = new SqliteCommand();
            int iCount = 0;
            bool isVal = false;
            string Connection = _configaration.GetConnectionString(Constants.DB_CONNECTION);
            try {
                using (SqliteConnection conn = new SqliteConnection(Connection))
                {
                    conn.Open();
                    string ssQl = "select count(*) from sys_settings where expiry_date>datetime('Now')";
                    oCmd = conn.CreateCommand();
                    oCmd.CommandText = ssQl;
                    object o = oCmd.ExecuteScalar();
                    int iTrue=Convert.ToInt16(o);
                    if (iTrue == 1)
                    {
                        isVal = true;
                    }
                    else
                        isVal = false;

                }
            }
            catch (Exception exp)
            { 
            }
            return isVal;
        
        }
        [HttpPost]
        public JsonResult post(Trade alertMessage)
        {

            if(isValid()==false)
                return new JsonResult("Expired");

            SqliteCommand oCmd = new SqliteCommand();
            int iCount = 0;
            string[] sMessage = alertMessage.text.Split('-');
            string sMessageCode = sMessage[0];
            string Connection = _configaration.GetConnectionString(Constants.DB_CONNECTION);
            try {

                using (SqliteConnection conn = new SqliteConnection(Connection))
                {
                    conn.Open();
                    string ssQl = "INSERT Into TradeviewAlert_api(alertmessage,messagecode) values('" + alertMessage.text + "','" + sMessageCode + "')";
                    oCmd = conn.CreateCommand();
                    oCmd.CommandText = ssQl;
                    iCount = oCmd.ExecuteNonQuery();

                    // Update last TrueDate
                    ssQl = "update alertinfo set noOfTrigger=noOfTrigger+1,lastTruedate = Datetime('Now') where messageid = (select messageid from messageinfo where messagecode = '" + sMessageCode + "')";
                    oCmd = conn.CreateCommand();
                    oCmd.CommandText = ssQl;
                    iCount = oCmd.ExecuteNonQuery();
                }
                calculatingAlertTrigger();
            }
            catch (Exception exp)
            {
                // add exception message into log file
                _logger.LogInformation(exp.ToString());
            }

            if (iCount == 1)
            {
                _logger.LogInformation("Addedd Successfully");
                return new JsonResult("Addedd Successfully");
               
            }
            else
            {
                _logger.LogInformation("Addedd Successfully");
                return new JsonResult("Information is not added");
                
            }


        }

        private APICredentials GetAPICredentials()
        {
            //APICredentials oAPI = new APICredentials();
            var oAPI= _context.APISettings.FirstOrDefault();
            

            return oAPI;
        }
        private void ResetStatistics(int RuleId)
        {
            SqliteCommand oCmd = new SqliteCommand();
            SqliteCommand oCmd1 = new SqliteCommand();
            int iCount = 0;
            string ssQl = "";
            string Connection = _configaration.GetConnectionString(Constants.DB_CONNECTION);

            try {
                using (SqliteConnection conn = new SqliteConnection(Connection))
                {
                    conn.Open();
                    
                    // Reset Statistics last TrueDate
                    ssQl = "update alertinfo set noOfTrigger=0,lastTruedate = '' where ruleid ="+RuleId;
                    oCmd = conn.CreateCommand();
                    oCmd.CommandText = ssQl;
                    iCount = oCmd.ExecuteNonQuery();
                    ssQl = "delete from tradeviewalert_api where messagecode in(  select messagecode from messageinfo where messageid in( select messageid from alertinfo where ruleid=" + RuleId + "))";
                    oCmd1 = conn.CreateCommand();
                    oCmd1.CommandText = ssQl;
                    iCount = oCmd1.ExecuteNonQuery();
                    _logger.LogInformation("Reset is successful");
                }
            }
            catch (Exception exp)
            {
                _logger.LogInformation(exp.ToString());
            }
        }

        private void UpdateBotInfo(int botId,string spairs, decimal dbaseOrderSize, string botname,APICredentials oAPI)
        {



            //var apiKey = _configaration.GetValue<string>("AppSettings:APIKEY");
            //var secret = _configaration.GetValue<string>("AppSettings:APISECRET");

            //var oAPI = _context.APISettings.FirstOrDefault();
            var apiKey = oAPI.APIKey;
            var secret = oAPI.APISecret;

            var client = new XCommas.Net.XCommasApi(apiKey, secret);

            try
            {


                Bot botdata = new Bot
                {
                    Pairs = new[] { spairs },
                    //Pairs = spairs ,
                    ActiveSafetyOrdersCount = 0,//2
                    BaseOrderVolume = dbaseOrderSize,
                    SafetyOrderVolume =  dbaseOrderSize * 2,
                    MartingaleStepCoefficient = 1m,
                    MartingaleVolumeCoefficient = 1m,
                    MinVolumeBtc24h = 100m,
                    MaxActiveDeals = 1,
                    ProfitCurrency = ProfitCurrency.QuoteCurrency,
                    SafetyOrderStepPercentage = 2.5m,
                    StartOrderType = StartOrderType.Market,
                    MaxSafetyOrders = 0,
                    TakeProfitType = TakeProfitType.Total,
                    TakeProfit = 10m,
                    //Name = "Updated",
                    Name=botname,
                    
                    // deal_start_delay_seconds=10,
                    TrailingEnabled = false,
                    Strategies = new BotStrategy[]
                        {
                            new CqsTelegramBotStrategy
                            {

                            },
                        }.ToList(),

                };
                BotUpdateData botUpdateData = new BotUpdateData(botdata);
                
                var response =  client.UpdateBot(botId, botUpdateData);
                _logger.LogInformation(response.RawData);

            }
            catch (Exception exp)
            {
                _logger.LogInformation(exp.ToString());
            }
        }
        private async void calculatingAlertTrigger()
        {
            APICredentials oapiCredentials = new APICredentials();   
            DataTable tableCalc = new DataTable();
            DataTable tableBot = new DataTable();
            SqliteCommand oCmd = new SqliteCommand();
            SqliteCommand oCmd1 = new SqliteCommand();
            int iCount = 0;
            double CurrentPercent = 0.0;
            double Target = 0.0;
            int triggerNo = 0;
            int NoOfAlert = 0;
            int RuleId = 0;
            oapiCredentials = GetAPICredentials();
            string Connection = _configaration.GetConnectionString(Constants.DB_CONNECTION);
            try
            {

                using (SqliteConnection conn = new SqliteConnection(Connection))
                {
                    conn.Open();
                    string ssQl = " select m.ruleid,m.rulename, m.message, c.lasttruedate,c.ttlmsg,c.NoOfAlert,c.noOftrigger,c.target,c.currentp from vw_rulecalc c,  vw_rulemsg m where m.ruleid=c.ruleid";

                    oCmd = conn.CreateCommand();
                    oCmd.CommandText = ssQl;
                    SqliteDataReader oReader = oCmd.ExecuteReader();
                    tableCalc.Load(oReader);
                    oReader.Close();
                   
                    for (int i = 0; i < tableCalc.Rows.Count; i++)
                    {
                        RuleId = Convert.ToInt16(tableCalc.Rows[i]["ruleid"].ToString());
                        triggerNo = Convert.ToInt16(tableCalc.Rows[i]["noOftrigger"].ToString());
                        Target = Convert.ToDouble(tableCalc.Rows[i]["target"].ToString());
                        NoOfAlert = Convert.ToInt16(tableCalc.Rows[i]["NoOfAlert"].ToString());
                        CurrentPercent = Convert.ToDouble(tableCalc.Rows[i]["currentp"].ToString());

                        if (CurrentPercent >= Target)
                        {
                            // Rule is True
                            //Now select all the message of this Rule
                            ssQl = "    select a.ruleid,a.messageid,b.botid,b.botname, b.buyselid,b.pairs,b.baseordersize," +
                                   "   (select bstype from buysell where buyselid = b.buyselid) bstype "+
                                   "    from alertinfo a, botinfo b where a.messageid = b.messageid and a.ruleid = "+ RuleId + " and b.statusId = 2";

                            tableBot = new DataTable();
                            oCmd1 = conn.CreateCommand();
                            oCmd1.CommandText = ssQl;
                            SqliteDataReader oReader1 = oCmd1.ExecuteReader();
                            tableBot.Load(oReader1);
                            oReader1.Close();

                            //Calling BOT
                            Int32 BotId = 0;
                            string sellType = "";
                            int iBotId = 0;
                            string pairs = "";
                            decimal baseordersize = 0;
                            string messageId = "";
                            string botname = "";
                            for (int j = 0; j < tableBot.Rows.Count; j++)
                            {
                                iBotId = Convert.ToInt32(tableBot.Rows[j]["botid"].ToString());
                                botname= tableBot.Rows[j]["botname"].ToString();
                                sellType = tableBot.Rows[j]["bstype"].ToString();
                                pairs= tableBot.Rows[j]["pairs"].ToString();
                                messageId = tableBot.Rows[j]["messageid"].ToString();
                                baseordersize =Convert.ToDecimal(tableBot.Rows[j]["baseordersize"].ToString());
                                //iBotId = Convert.ToInt32(BotId);
                                UpdateBotInfo(iBotId, pairs, baseordersize,botname,oapiCredentials);
                                if (sellType.Equals(SELL))
                                {
                                    await PanicSellAllBotDeals(iBotId, oapiCredentials);
                                }
                                if (sellType.Equals(NEWDEAL))
                                {
                                    await  StartNewDeal(iBotId, oapiCredentials);
                                }

                                
                            }

                            //Reset All Trigger Now
                            ResetStatistics(RuleId);
                        }
                    }

                }
                
            }
            catch (Exception exp)
            {
                // add exception message into log file
            }
        }
        
      //  [HttpPost]
        public async Task<JsonResult> PanicSellAllBotDeals(int botId, APICredentials oAPI)
        {
            //var apiKey = _configaration.GetValue<string>("AppSettings:APIKEY");
            //var secret = _configaration.GetValue<string>("AppSettings:APISECRET");

            //var oAPI = _context.APISettings.FirstOrDefault();
            var apiKey = oAPI.APIKey;
            var secret = oAPI.APISecret;


            var client = new XCommas.Net.XCommasApi(apiKey, secret);
            //int botId = 9767479;
            try {
                
                var response = await client.PanicSellAllBotDealsAsync(botId);
                _logger.LogInformation(response.RawData);
                _logger.LogInformation("Bot is Executed");
                return new JsonResult(response);
                //var response = await client.StartNewDealAsync(botId);
            }
            catch (Exception exp)
            {
                _logger.LogInformation(exp.ToString());
            }
            return new JsonResult("Bot is not Executed");
        }

        public async Task<JsonResult> StartNewDeal(int botId, APICredentials oAPI)
        {
            //var apiKey = _configaration.GetValue<string>("AppSettings:APIKEY");
            //var secret = _configaration.GetValue<string>("AppSettings:APISECRET");

            //var oAPI = _context.APISettings.FirstOrDefault();
            var apiKey = oAPI.APIKey;
            var secret = oAPI.APISecret;

            var client = new XCommas.Net.XCommasApi(apiKey, secret);
            //int botId = 9767479;
            try
            {
                //client.StartNewDeal ()

                var  response = await client.StartNewDealAsync(botId);
                _logger.LogInformation(response.RawData);

                _logger.LogInformation("Bot is Executed");
                return new JsonResult(response);
               
            }
            catch (Exception exp)
            {
                _logger.LogInformation(exp.ToString());
            }
            return new JsonResult("Bot is not Executed");
        }
    }
}
