using Microsoft.AspNetCore.Mvc;
using MPKWrocław.Database;

namespace MPKWrocław.Controllers;

[Route("api/[controller]")]
public class MapController : ControllerBase
{   
    private readonly MpkSingleton _singleton;
    private readonly UserSingleton _userSingleton;
    // Constructor injected via DI
    public MapController(MpkSingleton singleton, UserSingleton userSingleton)
    {
        _singleton = singleton;
        _userSingleton = userSingleton;
    }

    bool verifyGuid(string authHeader)
    {
        return true;
        var guid = authHeader.ToString().Substring("Bearer ".Length).Trim();
        
        try
        {
            return _userSingleton.VerifyToken(Guid.Parse(guid));
        }
        catch (Exception e)
        {
            return false;
        }
    }
    
    [HttpGet("stops")]
    public IActionResult GetStops()
    {
        if (!verifyGuid(Request.Headers["Authorization"]))
            return StatusCode(401);
        return Ok(_singleton.GetStopList());
    }
    
    [HttpGet("vehicles/{stopId}")]
    public IActionResult GetVehiclesForStop(int stopId)
    {
        if (!verifyGuid(Request.Headers["Authorization"]))
            return StatusCode(401);
        return Ok(_singleton.vehiclesForStop(stopId));
    }
    
    [HttpGet("getAllVehicleDeparturesForStop/{stopId}/{routeID}")]
    public IActionResult GetAllVehicleDeparturesForStop(int stopId, string routeID)
    {
        if (!verifyGuid(Request.Headers["Authorization"]))
            return StatusCode(401);
        return Ok(_singleton.departuresForStop(stopId,routeID));
    }
    
    [HttpGet("departures/{routeId}/{stopId}")]
    public IActionResult GetDepartureForVehicle(string routeId, int stopId)
    {   
        if (!verifyGuid(Request.Headers["Authorization"]))
            return StatusCode(401);
        return Ok(_singleton.departuresForVehicle(routeId, stopId));
    }
    
    [HttpGet("departures/closestTen/{stopId}/{page}")]
    public IActionResult GetDeparturesClosestTen(int stopId, int page)
    {
        if (!verifyGuid(Request.Headers["Authorization"]))
            return StatusCode(401);
        return Ok(_singleton.departuresClosestTen(stopId, page));
    }
    
    [HttpGet("getShape/{trip_id}")]
    public IActionResult getShape(string trip_id) 
    {
        if (!verifyGuid(Request.Headers["Authorization"]))
            return StatusCode(401);
        return Ok(_singleton.getShape(trip_id));
    }
    
    [HttpGet("stops/info")]
    public IActionResult GetStopsInfo()
    {
        if (!verifyGuid(Request.Headers["Authorization"]))
            return StatusCode(401);
        return Ok(_singleton.getStopsInfo());
    }
}