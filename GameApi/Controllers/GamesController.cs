using GameApi.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GameApi.Controllers;


[Route("api/[controller]")]
[ApiController]


public class GamesController : ControllerBase
{
    #region hard coded input
    List<Game> games = new()
{
    new Game()
    {
        Id=1,
        Name="Street Fighter 2",
        Genre="Fighting",
        Price=19.99M,
        ReleaseDate=new DateTime(1992,2,1),
        ImageUri="https://placehold.com/100"
    },
     new Game()
    {
        Id=2,
        Name="Final Fantasy XIV",
        Genre="RolePlaying",
        Price=59.99M,
        ReleaseDate=new DateTime(2010,9,30),
        ImageUri="https://placehold.com/100"
    },
      new Game()
    {
        Id=3,
        Name="FIFA 23",
        Genre="Sports",
        Price=69.99M,
        ReleaseDate=new DateTime(2022,9,27),
        ImageUri="https://placehold.com/100"
    }
};
    #endregion
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Game>>> GetGames()
    {
        return games;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Game>> GetGameById(int id)
    {
        return games.Find(game => game.Id == id);
    }

    //api/student
    [HttpPost]
    public async Task<ActionResult<Game>> AddStudent(Game g)
    {
        int id=games.Max(game => game.Id) + 1;
        g.Id = id;
        games.Add(g);
        return g;
    }
}
