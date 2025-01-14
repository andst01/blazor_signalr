using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorApp.Server.SignalRHub
{
    public class SeatHub : Hub
    {
        private static readonly Dictionary<int, string> SeatSelections = new();


        public Task JoinGroup(string group)
        {
            Groups.AddToGroupAsync(Context.ConnectionId, group);

            var objSeats = JsonConvert.SerializeObject(SeatSelections);

            return Clients.Group(group).SendAsync("SelectGroupSeats", objSeats);
        }

        public async Task SelectSeat(string group, int seatId, string userId)
        {
            if (SeatSelections.ContainsKey(seatId))
            {
                await Clients.Group(group).SendAsync("SeatAlreadySelected", seatId);
                return;
            }


            SeatSelections[seatId] = userId;

            await Clients.Group(group).SendAsync("SeatSelected", seatId, userId);


        }

        public async Task DeselectSeat(int seatId, string userId)
        {
            if (SeatSelections.TryGetValue(seatId, out var currentUserId) && currentUserId == userId)
            {
                SeatSelections.Remove(seatId);
                await Clients.All.SendAsync("SeatDeselected", seatId);
            }
        }

        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var disconnectedSeats = new List<int>();

            foreach (var (seatId, userId) in SeatSelections)
            {
                if (userId == Context.ConnectionId)
                {
                    disconnectedSeats.Add(seatId);
                }
            }

            foreach (var seatId in disconnectedSeats)
            {
                SeatSelections.Remove(seatId);
                await Clients.All.SendAsync("SeatDeselected", seatId);
            }


        }
    }
}
