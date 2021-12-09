using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

using System.Net;
using System.Net.NetworkInformation;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using HostAccessibilityCheckingSite.Data.Models;

namespace HostAccessibilityCheckingSite.Controllers
{
    [Route("api")]
    [ApiController]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class ApiController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return new JsonResult(new { Result = "Me (the API) working really hard! (API ready to go)" });
        }



        [NonAction]
        static public IActionResult CheckHostLocal(string host, int siteNoteId)
        {
            Ping ping = new Ping();

            if (string.IsNullOrEmpty(host))
                return new JsonResult(new { Result = "Host is empty" });

            PingReply pingReply = null;

            using (var db = new AppDbContext())
            {
                try
                {
                    pingReply = ping.Send(host);
                }
                catch (Exception)
                {
                    return new JsonResult(new PingResult(-1, -1, DateTime.Now, "Error"));
                }

                var result = new PingResult(DateTime.Now, pingReply.Status.ToString());

                foreach (var item in db.SiteList)
                {
                    if (item.Id == siteNoteId && item.Host == host)
                    {
                        result.SiteId = item.Id;
                        db.PingHistory.Add(result);
                    }
                }
                db.SaveChanges();

                if (result.SiteId >= 0)
                    return new JsonResult(result);



                result.SiteId = -1;
                result.Id = -1;


                return new JsonResult(result);
            }
        }

        [HttpGet]
        [Route("host")]
        public IActionResult CheckHost(string host, int siteNoteId) => CheckHostLocal(host, siteNoteId);

        [HttpPost]
        [Route("host")]
        public IActionResult AddHost(string host, int interval = 0)
        {
            var context = HttpContext;

            if(string.IsNullOrEmpty(host))
                host = context.Request.Query["host"];

            if (interval == 0 && int.TryParse(context.Request.Query["interval"], out int myInt))
            {
                if (myInt <= 0)
                    return BadRequest();

                interval = myInt;
            }

            if(interval < 10)
                return new JsonResult(new { Result = "Interval is too small. Minimum is 10" });

            try
            {
                var exists = Dns.GetHostAddresses(host).Length > 0;

                if (exists)
                {
                    using (var db = new AppDbContext())
                    {
                        foreach (var item in db.SiteList)
                        {
                            if(item.Host == host && item.Interval == interval)
                            {
                                return new JsonResult(new { Result = "Already exists", HostId = item.Id});
                            }
                        }

                        var nextCheck = DateTime.Now.AddSeconds(interval);

                        var newSite = new SiteSettings(host, interval, nextCheck);

                        db.SiteList.Add(newSite);
                        db.SaveChanges();

                        HostChecker.AddThread(newSite);

                        return new JsonResult(new { Result = "Added", HostId = newSite.Id }); 
                    }
                }
            }
            catch (Exception)
            {
                return new JsonResult(new { Result = "Host is invalid" }); 
            }

            return new JsonResult(new { Result = "Host does not exists" });

        }

        [HttpGet]
        [Route("hostList")]
        public async Task<IActionResult> GetHostList()
        {
            try
            {
                var userId = Convert.ToInt32(User.FindFirst("userId").Value);

                using (var db = new AppDbContext())
                {

                    var allSites = (from i in db.Relations where i.UserId == userId select i.SiteId).ToArray();


                    if (allSites.Count() <= 0)
                        return new JsonResult(new { Result = "No relations" });

                    var result = new List<SiteSettings>();

                    foreach (var item in db.SiteList)
                    {
                        if(allSites.Contains(item.Id))
                            result.Add(item);
                    }

                    if (result.Count() <= 0)
                        return new JsonResult(new { Result = "No relations" });

                    return new JsonResult(result);
                }
                
            }
            catch (Exception)
            {
                await HttpContext.SignOutAsync();
                return Redirect("/");
            }

        }


        [HttpGet]
        [Route("history")]
        public IActionResult GetHistory(int SiteId)
        {

            using (var db = new AppDbContext())
            {
                var userId = Convert.ToInt32(User.FindFirst("userId").Value);

                var isLegal = false;
                foreach (var item in db.Relations)
                {
                    if (item.SiteId == SiteId && item.UserId == userId)
                    {
                        isLegal = true;
                        break;
                    }
                }

                if (!isLegal)
                    return new JsonResult(new { Result = "Access denied" });

                var result = db.PingHistory.Where(p => p.SiteId == SiteId).ToArray();

                if (result.Count() > 0)
                    return new JsonResult(result);
                else
                    return new JsonResult(new { Result = "No notes"});
            }
        }

        [HttpGet]
        [Route("selectHistory")]
        public IActionResult GetHistorySelection(int SiteId, string StartDateTime, string EndDateTime)
        {

            var first = DateTime.TryParse(StartDateTime, out DateTime start);
            var second = DateTime.TryParse(EndDateTime, out DateTime end);

            end = end.AddDays(1);

            if (!first || !second)
                return new JsonResult(new { Result = "Wrong DateTime" });

            using (var db = new AppDbContext())
            {

                var userId = Convert.ToInt32(User.FindFirst("userId").Value);

                var isLegal = false;
                foreach (var item in db.Relations)
                {
                    if (item.SiteId == SiteId && item.UserId == userId)
                    {
                        isLegal = true;
                        break;
                    }
                }

                if (!isLegal)
                    return new JsonResult(new { Result = "Access denied" });

                var result = db.PingHistory.Where(p => (p.SiteId == SiteId) && (p.Time >= start) && (p.Time <= end)).ToList();

                SiteSettings siteSettings = null;

                foreach (var item in db.SiteList)
                {
                    if (item.Id == SiteId)
                        siteSettings = item;
                }

                if (siteSettings == null)
                    return new JsonResult(new { Result = "No such site" });


                if (result.Count() > 0)
                    return new JsonResult(new History(result, siteSettings));
                else
                    return new JsonResult(new { Result = "No notes" });
            }
        }



        [HttpPost]
        [Route("relation")]
        public IActionResult AddRelation(int userId, int siteId)
        {
            if(userId == 0)
                userId = Convert.ToInt32(User.FindFirst("userId").Value);

            //todo
            // if role is not admin and userid is not the same then reject
            // also i need roles......

            using (var db = new AppDbContext())
            {
                var user = db.Users.FirstOrDefault(u => u.Id == userId);
                var site = db.SiteList.FirstOrDefault(s => s.Id == siteId);

                if(user == null || site == null)
                    return new JsonResult(new { Result = "No such user or site" });

                var note = db.Relations.FirstOrDefault(r => r.UserId == userId && r.SiteId == siteId);

                if(note != null)
                    return new JsonResult(new { Result = "Relation already exists" });

                db.Relations.Add(new Relation(userId, siteId));
                db.SaveChanges();
            }

            return new JsonResult(new { Result = "Relation created!" });
        }

        [HttpDelete]
        [Route("relation")]
        public IActionResult DeleteRelation(int userId, int siteId)
        {
            if (userId == 0)
                userId = Convert.ToInt32(User.FindFirst("userId").Value);

            //todo
            // if role is not admin and userid is not the same then reject
            // also i need roles......

            using (var db = new AppDbContext())
            {
                var note = db.Relations.FirstOrDefault(r => r.UserId == userId && r.SiteId == siteId);

                if (note != null)
                {
                    db.Relations.Remove(note);
                    db.SaveChanges();

                    var needToDelete = true;

                    foreach (var item in db.Relations)
                    {
                        if (item.SiteId == siteId)
                            needToDelete = false;
                    }

                    if (needToDelete)
                    {

                        foreach (var item in db.SiteList)
                        {
                            if (item.Id == siteId)
                                db.SiteList.Remove(item);
                        }
                        db.SaveChanges();
                            

                    }

                    return new JsonResult(new { Result = "Relation deleted" });
                }

            }

            return new JsonResult(new { Result = "Relation does not exists" });
        }



        [HttpGet]
        [Route("user")]
        public IActionResult GetUser(string login)
        {
            using (var db = new AppDbContext())
            {
                foreach (var item in db.Users)
                {
                    if (item.Username == login)
                        return new JsonResult(item);
                }
            }
            return new JsonResult(new { Result = "No such user" });
        }

        [HttpDelete]
        [Route("user")]
        public IActionResult DeleteUser(string login)
        {
            using (var db = new AppDbContext())
            {
                User toDelete = null;
                foreach (var item in db.Users)
                {
                    if (item.Username == login)
                        toDelete = item;
                }

                if(toDelete != null)
                {
                    db.Users.Remove(toDelete);
                    db.SaveChanges();
                    return new JsonResult(new { Result = "User deleted" });
                }

            }
            return new JsonResult(new { Result = "No such user" });
        }

        [HttpPut]
        [Route("user")]
        public IActionResult UpdatePassword(string login, string password)
        {
            if (string.IsNullOrEmpty(login))
                login = User.FindFirst("username").Value;
                
            //todo
            // if role is not admin and userid is not the same then reject
            // also i need roles......

            using (var db = new AppDbContext())
            {
                var affected = false;
                foreach (var item in db.Users)
                {
                    if (item.Username == login)
                    {
                        item.Password = password;
                        affected = true;
                    }  
                }

                if (affected)
                {
                    db.SaveChanges();
                    return new JsonResult(new { Result = "Password updated" });
                }

            }
            return new JsonResult(new { Result = "No such user" });
        }

        [HttpPost]
        [Route("user")]
        public IActionResult CreateUser(string login, string password)
        {

            using (var db = new AppDbContext())
            {
                foreach (var item in db.Users)
                {
                    if (item.Username == login)
                        return new JsonResult(new { Result = "User already exists" });
                }

                db.Users.Add(new User(login, password));
                db.SaveChanges();

            }
            return new JsonResult(new { Result = "User added" });
        } 

    }
}
