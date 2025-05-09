﻿using eCommerceApp.Application.Services.Interfaces.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCommerceApp.Infrastructure.Middleware
{
    public class ExceptionHandlingMiddleware(RequestDelegate _next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (DbUpdateException ex)
            {
                var logger = context.RequestServices.GetRequiredService<IAppLogger<ExceptionHandlingMiddleware>>(); //Activates Loggertivities
                context.Response.ContentType = "application/json"; //this is so we can send the Response.WriteAsync to the database response along with the Status Code. Mostly used for developers so it is good to stay Developer Friendly rather than end user friendly
                if (ex.InnerException is SqlException innerException)
                {
                    logger.LogError(innerException, "Sql Exception");
                    switch (innerException.Number)
                    {
                        case 2627: //Unique Constraint Violation
                            context.Response.StatusCode = StatusCodes.Status409Conflict;
                            await context.Response.WriteAsync("Unique Constraint Violation");
                            break;
                        case 515: //Cannot insert nulls
                            context.Response.StatusCode = StatusCodes.Status400BadRequest;
                            await context.Response.WriteAsync("Cannot insert null");
                            break;
                        case 547: //Foreign Key Constraint violation
                            context.Response.StatusCode = StatusCodes.Status409Conflict;
                            await context.Response.WriteAsync("Foreign Key Constraint Violation");
                            break;
                        default:
                            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                            await context.Response.WriteAsync("An error occurred while processing your request.");
                            break;
                    }
                }
                else
                {
                    logger.LogError(ex, "Related EFCore Exception"); //if its not a lower level exception
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    await context.Response.WriteAsync($"An error occured while processing your request.");
                }
            }
            catch (Exception ex) //Handles everything else not related to a database update exceptions
            {
                var logger = context.RequestServices.GetRequiredService<IAppLogger<ExceptionHandlingMiddleware>>();
                logger.LogError(ex, "Unknown Exception");
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsync($"An error occured: {ex.Message}"); //in production this should be taken out and placed into logger for the developers
            }
        }
    }
}