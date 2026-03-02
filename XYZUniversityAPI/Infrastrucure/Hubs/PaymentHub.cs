using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;

namespace XYZUniversityAPI.Infrastructure.Hubs
{
    [Authorize] 
    public class PaymentHub : Hub
    {
        // the mothod js calls
        public async Task SubscribeToMyUpdates(string admissionNumber)
        {
            // AdmissionNumber is the Unique Group Name
            await Groups.AddToGroupAsync(Context.ConnectionId, admissionNumber);
            
           
            await Clients.Caller.SendAsync("Subscribed", $"Listening for updates for: {admissionNumber}");
        }

        // cleanup
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            
            await base.OnDisconnectedAsync(exception);
        }
    }
}
