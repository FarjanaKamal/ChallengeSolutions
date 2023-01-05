﻿using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace health_path.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NewsletterController : ControllerBase
{
    private readonly ILogger<NewsletterController> _logger;
    private readonly IDbConnection _connection;

    public NewsletterController(ILogger<NewsletterController> logger, IDbConnection connection)
    {
        _logger = logger;
        _connection = connection;
    }

    [HttpPost]
    public ActionResult Subscribe(string Email)
    {

        String[] emailList=Email.Split("@",StringSplitOptions.RemoveEmptyEntries);
        String UserName=emailList[0];
        String Edomain=emailList[1];
        String[] UserNameList=UserName.Split(".",StringSplitOptions.RemoveEmptyEntries);
        string EmailRemovedPeriod=" ";
        foreach(String uname in UserNameList){
            EmailRemovedPeriod+=uname;
        }
        EmailRemovedPeriod+=Edomain;
        
        //Email=EmailRemovedPeriod;
        // var inserted = _connection.Execute(@"
        //     INSERT INTO NewsletterSubscription (Email)
        //     SELECT *
        //     FROM ( VALUES (@Email) ) AS V(Email)
        //     WHERE NOT EXISTS ( SELECT * FROM NewsletterSubscription e WHERE e.Email = v.Email)
        // ", new { Email = Email });

        var inserted = _connection.Execute(@"
            INSERT INTO NewsletterSubscription (Email)
            SELECT *
            FROM ( VALUES (@Email) ) AS V(Email)
            WHERE NOT EXISTS ( SELECT * FROM NewsletterSubscription e WHERE e.Email = v.Email)
        ", new { Email = Email });

        return inserted == 0 ? Conflict("email is already subscribed") : Ok();
    }
}