using Microsoft.AspNetCore.Mvc;
using MPKWrocław.Database;

namespace MPKWrocław.Controllers;

[Route("api/[controller]")]
public class MapController : ControllerBase
{   
    private readonly MpkSingleton _singleton;

    // Constructor injected via DI
    public MapController(MpkSingleton singleton)
    {
        _singleton = singleton;
    }
    
    [HttpGet("stops")]
    public IActionResult GetStops()
    {
        return Ok(_singleton.GetStopList());
    }
    
    [HttpGet("vehicles/{stopId}")]
    public IActionResult GetVehiclesForStop(int stopId)
    {
        return Ok(_singleton.vehiclesForStop(stopId));
    }
    
    [HttpGet("departures/{routeId}/{stopId}")]
    public IActionResult GetDepartureForVehicle(string routeId, int stopId)
    {
        return Ok(_singleton.departuresForVehicle(routeId, stopId));
    }
    
    [HttpGet("departures/closestTen/{stopId}/{page}")]
    public IActionResult GetDeparturesClosestTen(int stopId, int page)
    {
        return Ok(_singleton.departuresClosestTen(stopId, page));
    }
    
    [HttpGet("getShape/{trip_id}")]
    public IActionResult getShape(string trip_id) 
    {
        return Ok(_singleton.getShape(trip_id));
    }
    
    [HttpGet("stops/info")]
    public IActionResult GetStopsInfo()
    {
        return Ok(_singleton.getStopsInfo());
    }
}