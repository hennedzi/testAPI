using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.AspNetCore.Authorization;
using testAPI.Models;

namespace testAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CarController:ControllerBase
{
    private readonly SqliteConnection? _connection;
    private readonly ILogger<CarController> _logger;

    public CarController(IConfiguration configuration,ILogger<CarController> logger)
    {
        _connection = new SqliteConnection(configuration.GetConnectionString("DefaultConnection"));
        _logger = logger;
        //_connection.Open();
    }

    [HttpGet,Authorize(Roles = "Admin,User")]
    public async Task<ActionResult<List<Car>>> GetAllCar()
    {
        IEnumerable<Car> cars = await _connection.QueryAsync<Car>("SELECT*FROM Car");
        return Ok(cars);
    }

    [HttpGet("{Id}"),Authorize(Roles = "Admin,User")]
    public async Task<ActionResult<Car>> GetCar(int Id)
    {
        var car = await _connection.QueryFirstAsync<Car>("select*from Car where Id =@Id",
            new {Id = Id});
        return Ok(car);
    }
    
    [HttpPost,Authorize(Roles = "Admin,User")]
    public async Task<ActionResult<List<Car>>> CreateCar(Car car)
    {
        await _connection.ExecuteAsync("INSERT INTO Car (Firm, Number) VALUES(@Firm,@Number)",
            car);
        return Ok(await GetAllCar());
    }
    
    [HttpPut,Authorize(Roles = "Admin,User")]
    public async Task<ActionResult<Car>> UpdateCar(Car car)
    {
        await _connection.ExecuteAsync("UPDATE Car SET Firm=@Firm, Number=@Number WHERE Id=@Id",
            car);
        return Ok(await GetAllCar());
    }
    [HttpDelete("{Id}"),Authorize(Roles = "Admin,User")]
    public async Task<ActionResult<Car>> DeleteCar(int Id)
    {
        var car = await _connection.ExecuteAsync("DELETE FROM Car WHERE Id=@Id",
            new {Id = Id});
        return Ok(await GetAllCar());
    }
}