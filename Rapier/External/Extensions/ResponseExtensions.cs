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
            Func<object, OkObjectResult> okObject)
        {
            var result = await resultTask;
            return result == null ?
                (IActionResult)new NotFoundResult() :
                okObject(result);
        }

        public static async Task<IActionResult> ToResult<TResponse>(
            this Task<TResponse> resultTask,
            Func<string, object, object, CreatedAtActionResult> createdAt,
            string action)
            where TResponse : EntityResponse
        {
            var result = await resultTask;
            return result == null ?
                (IActionResult)new NotFoundResult() :
                createdAt(action, new { id = result.Id }, result);
        }
    }
}
