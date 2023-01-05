using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using health_path.Model;

namespace health_path.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ScheduleController : ControllerBase
{
    private readonly ILogger<ScheduleController> _logger;
    private readonly IDbConnection _connection;

    public ScheduleController(ILogger<ScheduleController> logger, IDbConnection connection)
    {
        _logger = logger;
        _connection = connection;
    }

    [HttpGet]
    public ActionResult<IEnumerable<ScheduleEvent>> Fetch()
    {
        var dbResults = ReadData();
        var preparedResults = dbResults.Select((t) => {
            t.Item1.Recurrences.Add(t.Item2);
            return t.Item1;
        }); 
        //creating a new list of ScheduleEvent 
        var returnResults=new List<ScheduleEvent>();
        //This for loop is to check whether there is duplicate Id of Schedule Event
        foreach(var evnt in preparedResults){
           //When there is a duplicate Id then the Recurrences will be added to it
           if(returnResults.Any(x=>x.Id==evnt.Id)){
               int index=returnResults.FindIndex(x=>x.Id==evnt.Id);
               returnResults[index].Recurrences.Add(evnt.Recurrences.FirstOrDefault()); 
           }
           //otherwise there will be a new item in the list
           else{
              returnResults.Add(evnt);
           }
              
        }
        return Ok(returnResults);
    }

    private IEnumerable<(ScheduleEvent, ScheduleEventRecurrence)> ReadData() {
        var sql = @"
            SELECT e.*, r.*
            FROM Event e
            JOIN EventRecurrence r ON e.Id = r.EventId
            GROUP BY e.Id ,e.Name,e.Description,r.Id,r.EventId, r.DayOfWeek, r.StartTime, r.EndTime      
            ORDER BY e.Id, r.DayOfWeek, r.StartTime, r.EndTime         
        ";
        return _connection.Query<ScheduleEvent, ScheduleEventRecurrence, (ScheduleEvent, ScheduleEventRecurrence)>(sql, (e, r) => (e, r));
    }
}
