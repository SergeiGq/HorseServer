using ClassLibrary;
using ClassLibrary.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;

namespace WebApplication2.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HorseController : ControllerBase
{
    private readonly DbHorse _context;
    private readonly ILogger<HorseController> _logger;


    public HorseController(DbHorse context, ILogger<HorseController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public IEnumerable<HorseGet>? Get()
    {
        
        List<Horse> list = new List<Horse>();
        list = _context.Horses.ToList();
        List<HorseGet> listres = list.Select(item => new HorseGet()
            { Id = item.Id, Desc = item.Desc, Name = item.Name, ShortDesc = item.ShortDesc }).ToList();
        
        return listres;
    }

    [HttpPost]
    [Authorize]
    [Route("/Add")]
    public async Task Post([FromForm] string name, [FromForm] string desc, [FromForm] string shortDesc, IFormFile image)
    {
        var bytes = await image.GetBytes();
        var horse = new Horse()
        {
            Name = name,
            Desc = desc,
            ShortDesc = shortDesc,
            Img = bytes,
            ImgExtension = image.ContentType,
        };
        _context.Horses.Add(horse);
        await _context.SaveChangesAsync();
    }


    [HttpDelete]
    [Authorize]
    [Route("/Delete")]
    public void DeleteBehavior(int id)
    {
        Horse horse = _context.Horses.FirstOrDefault(n => n.Id == id);
        _context.Horses.Remove(horse);
        _context.SaveChanges();
    }

    [HttpGet]
    [Route("{horseId}/image")]
    public FileContentResult ReturnFileContent([FromRoute] int horseId)
    {
        var filecontent = _context.Horses.First(n => n.Id == horseId);
        return new FileContentResult(filecontent.Img, filecontent.ImgExtension);
    }
}