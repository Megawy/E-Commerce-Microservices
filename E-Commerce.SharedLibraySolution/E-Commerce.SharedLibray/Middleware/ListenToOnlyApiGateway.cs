﻿using Microsoft.AspNetCore.Http;
namespace E_Commerce.SharedLibray.Middleware
{
	public class ListenToOnlyApiGateway(RequestDelegate next)
	{
		public async Task InvokeAsync(HttpContext context)
		{
			//Extract specific header from the request 
			var signedHeader = context.Request.Headers["Api-Gateway"];

			//NULL means, the request is not coming from the Api-Gateway // Status503ServiceUnavailable
			if (signedHeader.FirstOrDefault() is null)
			{
				context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
				await context.Response.WriteAsync("Sorry, service is unavailable");
				return;	
			}else
			{
				await next(context);
			}
		}
	}
}
