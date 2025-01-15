using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorApp.Server.SignalRHub
{
    public class SeatHub : Hub
    {
        private static readonly Dictionary<string, Dictionary<int, string>> SeatSelections = new();

        private static string groupName = string.Empty;


        public Task JoinGroup(string group)
        {
            Groups.AddToGroupAsync(Context.ConnectionId, group);

            groupName = group;

            if (!SeatSelections.ContainsKey(group))
            {
                SeatSelections.Add(group, new Dictionary<int, string>());
            }


            var listSeats = SeatSelections[group];

            var objSeats = JsonConvert.SerializeObject(listSeats);

            return Clients.Group(group).SendAsync("SelectGroupSeats", objSeats);
        }

        public async Task SelectSeat(string group, int seatId, string userId)
        {

            
            if (SeatSelections[group].ContainsKey(seatId))
            {
                await Clients.Group(group).SendAsync("SeatAlreadySelected", seatId);
                return;
            }

           

            SeatSelections[group][seatId] = userId;

            await Clients.Group(group).SendAsync("SeatSelected", seatId, userId);


        }

        public async Task DeselectSeat(string group,int seatId, string userId)
        {
            if (SeatSelections[group].TryGetValue(seatId, out var currentUserId) && currentUserId == userId)
            {
                SeatSelections[group].Remove(seatId);
                await Clients.Group(group).SendAsync("SeatDeselected", seatId);
            }
        }

        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {

           // Groups.RemoveFromGroupAsync
            var disconnectedSeats = new List<int>();

            foreach (var (seatId, userId) in SeatSelections[groupName])
            {
                if (userId == Context.ConnectionId)
                {
                    disconnectedSeats.Add(seatId);
                }
            }

            foreach (var seatId in disconnectedSeats)
            {
                SeatSelections[groupName].Remove(seatId);
                await Clients.All.SendAsync("SeatDeselected", seatId);
            }


        }
    }
}
