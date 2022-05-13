
using Attlassian.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Atlassian;
using Atlassian.Jira;
using Attlassian.Core.Domain;
using Attlassian.MailService;

namespace Attlassian.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IConfiguration _configuration { get; }
        string JiraUsername = String.Empty;
        string JiraPassword = String.Empty;
        string Mail = String.Empty;
        string MailPassword = String.Empty;
        string PriorityWords = String.Empty;
        public HomeController(IConfiguration configuration, ILogger<HomeController> logger)
        {
            _configuration = configuration;
            _logger = logger;
            JiraUsername = _configuration.GetSection("JiraDetail:username").Value;
            JiraPassword = _configuration.GetSection("JiraDetail:password").Value;
            Mail = _configuration.GetSection("MailDetail:mail").Value;
            MailPassword = _configuration.GetSection("MailDetail:username").Value;
            PriorityWords = _configuration.GetSection("Priority:Words").Value;
        }

        public async Task<IActionResult> Index()
        {

            var jira = Jira.CreateRestClient("https://jira.brighteventure.com/", JiraUsername, JiraPassword);
            List<IssueModel> issueModels = new List<IssueModel>();


            var jiraIssues = jira.Issues.Queryable.Where(x => x.Status == "Open" && x.Project == "SD" && x.Resolution == "Unresolved" && x.Created < DateTime.Now && x.Created >= DateTime.Now.AddDays(-1) && (x.Priority == "High" || x.Priority == "Low" || x.Priority == "Critical"));
            //project = SD AND  status = Open AND resolution = Unresolved  AND Created < Now() AND Created >= "-1d" and (priority =High OR  priority = Critical)

            int IssuesCount = jiraIssues.Count();
            foreach (var issue in jiraIssues)
            {
                if (IssuePriorityControl(issue.Summary, issue.Description))
                {
                    issueModels.Add(new IssueModel
                    {
                        Value = issue.Key.Value,
                        AssigneeUser = issue.AssigneeUser == null ? string.Empty : issue.AssigneeUser.ToString(),
                        Summary = issue.Summary,
                        Created = issue.Created,
                        Description = issue.Description
                    });
                }
            }
            if (issueModels.Count > 0)
            {
                string result = await MailManager.SendAsync(issueModels);
            }
            
            return View(issueModels);
        }

        public bool IssuePriorityControl(string Summary, string Description)
        {
            string[] Words = PriorityWords.Split(',');

            foreach (var sub in Words)
            {
                if (Summary.Contains(sub) || Description.Contains(sub))
                {
                    return true;
                }
            }
            return false;
        }





        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}