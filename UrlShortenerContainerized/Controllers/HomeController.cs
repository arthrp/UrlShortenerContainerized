using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using UrlShortenerContainerized.Models;
using UrlShortenerContainerized.Repositories;

namespace UrlShortenerContainerized.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IUrlRepository _repository;

    public HomeController(ILogger<HomeController> logger, IUrlRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [HttpPost]
    public IActionResult AddUrl(AddUrlModel model)
    {
        var url = model.FullUrl;
        if (!IsValidUri(url)) return View("LinkError", new LinkErrorModel("Supplied Url is invalid"));
        var key = GenerateKey();
        
        _repository.Add(key, url);

        var resultUrl = $"{Request.Scheme}://{Request.Host}/{key}";
        return View("Index", new LinkResultModel() { Url = resultUrl });
    }

    [Route("{urlKey:regex(^\\w{{32}}$)}")]
    [HttpGet]
    public IActionResult RedirectTo(string urlKey)
    {
        var url = _repository.Get(urlKey);
        if (string.IsNullOrEmpty(url)) return View("LinkNotFound");
        return Redirect(url);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    private static bool IsValidUri(string uri)
    {
        var success = Uri.TryCreate(uri, UriKind.Absolute, out _);
        return success;
    }

    private static string GenerateKey()
    {
        var result = Guid.NewGuid().ToString().Replace("-", "");
        return result;
    }
}