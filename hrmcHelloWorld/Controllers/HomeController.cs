using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using hrmcHelloWorld.Models;
using hrmcHelloWorld.Services;

namespace hrmcHelloWorld.Controllers
{
    public class HomeController : Controller
    {
        private HttpClient client = new HttpClient
        {
            BaseAddress = new Uri("https://test-api.service.hmrc.gov.uk"),
            DefaultRequestHeaders = { Accept = { new MediaTypeWithQualityHeaderValue("application/vnd.hmrc.1.0+json") }}

        };
        private readonly ITokenService tokenService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, ITokenService tokenService)
        {
            _logger = logger;
            this.tokenService = tokenService;
        }

        public IActionResult Index()
        {
            return View();
        }
        
        public async Task<IActionResult> HelloWorld()
        {
            HttpResponseMessage response = await client.GetAsync("/hello/world");
            if (!response.IsSuccessStatusCode)
            {
                return Error();
            }

            var content = await response.Content.ReadAsStringAsync();
            _logger.Log(LogLevel.Information, $"{content}");
            return Ok(content);
        }
        
        public async Task<IActionResult> HelloApplication()
        {
            var accessToken = await tokenService.GetToken();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            
            HttpResponseMessage response = await client.GetAsync("/hello/application");
            if (!response.IsSuccessStatusCode)
            {
                return Error();
            }

            String content = await response.Content.ReadAsStringAsync();
            _logger.Log(LogLevel.Information, $"{content}");
            return Ok(content);
        }
        
        public IActionResult HelloUser()
        {
            var uriBuilder = new UriBuilder("https://test-api.service.hmrc.gov.uk/oauth/authorize");
            
            var parameters = HttpUtility.ParseQueryString(string.Empty);
            parameters.Add("response_type", "code");
            parameters.Add("client_id", "RcDuTvgdu8xmwUYqUhZvz2TxFT88");
            parameters.Add("scope", "hello");
            parameters.Add("redirect_uri", "https://localhost:5001/redirect");
            
            uriBuilder.Query = parameters.ToString() ?? "";

            return Redirect(uriBuilder.Uri.ToString());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}