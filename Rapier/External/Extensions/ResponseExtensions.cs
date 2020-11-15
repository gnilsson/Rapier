using Microsoft.AspNetCore.Mvc;
using Rapier.External.Models;
using System;
using System.Threading.Tasks;

namespace Rapier.External.Extensions
{
    public static class ResponseExtensions
    {
        public static async Task<IActionResult> ToResult<TResponse>(
            this Task<TResponse> resultTask,
            Func<object, OkObjectResult> OkObject)
        {
            var result = await resultTask;
            return result == null ?
                (IActionResult)new NotFoundResult() :
                OkObject(result);
        }

        public static async Task<IActionResult> ToResult<TResponse>(
            this Task<TResponse> resultTask,
            Func<string, object, object, CreatedAtActionResult> CreatedAt,
            string action)
            where TResponse : EntityResponse
        {
            var result = await resultTask;
            return result == null ?
                (IActionResult)new NotFoundResult() :
                CreatedAt(action, new { id = result.Id }, result);
        }
    }
}
