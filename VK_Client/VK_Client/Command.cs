using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace VK_Client
{
    public class InitializingCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            const string serviceToken = "8142f9b08142f9b08142f9b0218136017b881428142f9b0def180d31df95a534d47611b";
            const int appId = 7665867;
            var client = new VkClient(serviceToken, appId);
            Console.WriteLine("Enter your id:");
            try
            {
                var id = int.Parse(Console.ReadLine() ?? "");
                var friendsOnline = client.GetOnlineFriends(id);
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

        public event EventHandler CanExecuteChanged;
    }
}