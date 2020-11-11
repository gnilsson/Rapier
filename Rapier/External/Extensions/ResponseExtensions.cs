using Microsoft.AspNetCore.Mvc;
using Rapier.External.Models;
using System.Threading.Tasks;

namespace Rapier.External.Extensions
{
    public static class ResponseExtensions
    {
        public static async Task<IActionResult> ToOkResult<TResponse>(
               this Task<TResponse> resultTask)
        {
            var result = await resultTask;
            return result == null ?
                (IActionResult)new NotFoundResult() :
                new OkObjectResult(result);
        }

        public static async Task<IActionResult> ToCreatedAtResult<TResponse>(
            this Task<TResponse> resultTask,
            string action,
            string controller)
            where TResponse : EntityResponse
        {
            var result = await resultTask;
            return result == null ?
                (IActionResult)new NotFoundResult() :
                new CreatedAtActionResult(action, controller, 
                new { id = result.Id }, result);
        }
    }
}
