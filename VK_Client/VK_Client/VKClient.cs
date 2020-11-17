using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace VK_Client
{
    public class VkClient
    {
        private class GetIdsResponse
        {
            public List<int> response { get; set; }
        }

        private class GetInfoResponse
        {
            public List<AccountInfo> response { get; set; }
        }

        private class UrlResponse
        {
            public string upload_url { get; set; }
        }

        private class GetUrlResponse
        {
            public UrlResponse response { get; set; }
        }

        private class UploadData
        {
            public string server { get; set; }
            public string photo { get; set; }
            public string hash { get; set; }
        }

        public VkClient(string serviceToken, int applicationId)
        {
            this.serviceToken = serviceToken;
            this.applicationId = applicationId;
        }

        private readonly string serviceToken;
        private readonly int applicationId;

        private string Authorize(int id, string scope)
        {
            var authString = $"https://oauth.vk.com/authorize?client_id={applicationId}&display=page&scope={scope}&response_type=token";
            authString = authString.Replace("&", "^&");
            Process.Start(new ProcessStartInfo("cmd", $"/c start {authString}") { CreateNoWindow = true });
            Console.WriteLine("Enter access token here:");
            return Console.ReadLine();
        }

        private string ConnectIds(IEnumerable<int> ids)
        {
            var usersId = ids.Aggregate("", (current, id) => current + (id + ","));
            return usersId.Remove(usersId.Length - 1);
        }

        private async Task<List<AccountInfo>> GetInfo(IEnumerable<int> ids)
        {
            var usersId = ConnectIds(ids);
            var url = $"https://api.vk.com/method/users.get?user_ids={usersId}&v=5.52&access_token={serviceToken}";
            using var client = new HttpClient();
            var info = await client.GetStringAsync(url);
            return JsonConvert.DeserializeObject<GetInfoResponse>(info).response;
        }

        public async Task<List<AccountInfo>> GetOnlineFriends(int id)
        {
            var accessToken = Authorize(id, "friends");
            var url = $"https://api.vk.com/method/friends.getOnline?user_id={id}&v=5.52&access_token={accessToken}";
            using var client = new HttpClient();
            var friendsId = await client.GetStringAsync(url);
            return await GetInfo(JsonConvert.DeserializeObject<GetIdsResponse>(friendsId).response);
        }

        public async Task ChangeUserPicture(int id, string photoPath)
        {
            var accessToken = Authorize(id, "photos");
            using var client = new HttpClient();
            var getAddress = $"https://api.vk.com/method/photos.getOwnerPhotoUploadServer?owner_id={id}&v=5.52&access_token={accessToken}";
            var response = await client.GetStringAsync(getAddress);
            var uploadUrl = JsonConvert.DeserializeObject<GetUrlResponse>(response).response.upload_url;
            var form = new MultipartFormDataContent { { new ByteArrayContent(await File.ReadAllBytesAsync(photoPath)), "photo", photoPath } };
            var uploadPhotoResponse = await client.PostAsync(uploadUrl, form);
            var data = JsonConvert.DeserializeObject<UploadData>(await uploadPhotoResponse.Content.ReadAsStringAsync());
            var setPhoto = $"https://api.vk.com/method/photos.saveOwnerPhoto?server={data.server}&hash={data.hash}&photo={data.photo}&v=5.52&access_token={accessToken}";
            await client.GetStringAsync(setPhoto);
        }

    }
}