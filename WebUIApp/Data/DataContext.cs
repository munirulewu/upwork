using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebUIApp.Models;
using Microsoft.EntityFrameworkCore;

namespace WebUIApp.Data
{
    public class DataContext:DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        { 

        }
        public DbSet<CRuleInfo> RuleInfo { get; set; }
        public DbSet<CMessage> messageinfo { get; set; }
        public DbSet<CAlert> AlertInfo { get; set; }
        public DbSet<CBot> BotInfo { get; set; }
        public DbSet<TradeviewAlert> TradeviewAlert_API { get; set; }
        public DbSet<BuySell> buysell { get; set; }
        public DbSet<ServiceStatus> serviceStatus { get; set; }
        public DbSet<UserInfo> UserLogIn { get; set; }
        public DbSet<APICredentials> APISettings { get; set; }
    }

    public static class Constants
    {
        public const string DB_CONNECTION = "DefaultConnection";
        public const string LOGIN_USER = "SESSION_USERLOGIN";
        //public const string DB_CONNECTION = "DefaultConnection";

    }
}
