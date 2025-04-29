using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
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
                var innerException = ex.InnerException as SqlException;
                if (innerException != null)
                {
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
                    }
                }
            }
        }
    }
}
