using Microsoft.AspNetCore.Mvc;
using MPKWrocław.Database;

namespace MPKWrocław.Controllers;

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
        return Ok(_singleton.vehiclesForStop(stopId));
    }
    
    public IActionResult GetDepartureForVehicle(string routeId, int stopId)
    {
        return Ok(_singleton.departuresForVehicle(routeId, stopId));
    }
    
    public IActionResult GetDeparturesClosestTen(int stopId, int page)
    {
        return Ok(_singleton.departuresClosestTen(stopId, page));
    }
    
    public IActionResult GetStopsInfo()
    {
        return Ok(_singleton.getStopsInfo());
    }
}