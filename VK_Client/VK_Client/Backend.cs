using System;
using System.Threading.Tasks;

namespace VK_Client
{
    public class Backend
    {
        static async Task dowork(string[] args)
        {
            Console.WriteLine("Enter the service token here:");
            var serviceToken = Console.ReadLine();
            var appId = 7663498;
            var client = new VkClient(serviceToken, appId);
            Console.WriteLine("Enter your id:");
            try
            {
                var id = int.Parse(Console.ReadLine() ?? "");
                var friendsOnline = await client.GetOnlineFriends(id);
                Console.WriteLine("Online friends");
                foreach (var friend in friendsOnline)
                {
                    Console.WriteLine($"{friend.first_name} {friend.last_name} {friend.id}");
                }
                Console.WriteLine("Enter the path:");
                var path = Console.ReadLine();
                await client.ChangeUserPicture(id, path);
            }
            catch (Exception)
            {
                Console.WriteLine("Invalid id");
                return;
            }
        }
    }
}