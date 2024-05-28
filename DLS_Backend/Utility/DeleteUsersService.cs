using System.Text.Json;
using DLS_Backend.Controller;
using Microsoft.EntityFrameworkCore;

namespace DLS_Backend.utility;

public class DeleteUsersService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public DeleteUsersService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DlsUsersContext>();

                var usersToDelete = context.UserTombstones
                    .AsNoTracking()
                    .Where(u => u.IsDeleted && u.DeletionDate <= DateTime.Now.AddSeconds(-14))
                    .ToList();

                // List to store the GUIDs of the deleted users
                var deletedUserGuids = new List<Guid>();
                
                // Delete the actual User records
                foreach (var userTombstone in usersToDelete)
                {
                    var user = await context.Users
                        .Include(u => u.UserInfos) // Load the related UserInfo records
                        .FirstOrDefaultAsync(u => u.id == userTombstone.UserId);
                    if (user != null)
                    {
                        // Add the user's GUID to the list
                        deletedUserGuids.Add(user.guid);

                        context.UserInfo.RemoveRange(user.UserInfos); // Delete the related UserInfo records
                        context.Users.Remove(user);
                    }
                }

                // Delete the UserTombstone records
                context.UserTombstones.RemoveRange(usersToDelete);

                await context.SaveChangesAsync();

                if (deletedUserGuids.Any())
                {
                    var message = JsonSerializer.Serialize(new { user_guid = deletedUserGuids });

                    // Send the message to RabbitMQ
                    var rabbitMQService = new RabbitMQService();
                    string queueName = "UserDeleteQueue";
                    Console.WriteLine(message);
                    rabbitMQService.SendMessage(queueName, message);
                    rabbitMQService.Close();
                }
            }
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }
}
