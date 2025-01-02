using Microsoft.AspNetCore.Mvc;
using MPKWrocław.Database;

namespace MPKWrocław.Controller;

[Route("mapApi/")]
public class MapApiController : ControllerBase
{   
    MpkSingleton _singleton;
        
    MapApiController() => _singleton = new MpkSingleton();
    
    
    public IActionResult GetStops()
    {
        return Ok(_singleton.GetStopList());
    }
    
    public IActionResult GetVehiclesForStop(int stopId)
    {
        return Ok();
    }
}