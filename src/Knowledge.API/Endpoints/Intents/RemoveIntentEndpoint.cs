using FastEndpoints;
using Knowledge.API.Services;
using Microsoft.AspNetCore.Authorization;

namespace Knowledge.API.Endpoints.Intents;

// TODO uncomment if resolved

// [AllowAnonymous]
// [HttpDelete("intents/{id}")]
// // TODO investigate why this is not working - due to grpc
// public class RemoveIntentEndpoint : Endpoint<RemoveIntentRequest>
// {
//     private readonly IIntentService _intentService;
//     
//     public RemoveIntentEndpoint(IIntentService intentService)
//     {
//         _intentService = intentService;
//     }
//
//     public override async Task HandleAsync(RemoveIntentRequest req, CancellationToken ct)
//     {
//         var success = _intentService.RemoveIntent(req.Id);
//         if (!success)
//         {
//             await SendNotFoundAsync();
//             return;
//         }
//
//         await SendOkAsync();
//     }
// }
//
// public class RemoveIntentRequest // : Knowledge.API.Contracts.Requests.RemoveIntentRequest
// {
//     public int Id { get; set; }
// }
