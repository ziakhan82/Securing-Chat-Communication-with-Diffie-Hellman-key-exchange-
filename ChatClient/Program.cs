using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using ChatClient;
using CryptoChat.Shared;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using static System.Net.WebRequestMethods;

var _client = new HttpClient();
var BaseURL = new Uri("https://localhost:7246");

var connection = new HubConnectionBuilder()
        .WithUrl("https://localhost:7160/chathub")
        .Build();

Console.WriteLine("Connected. Enter your name:");
var userName = Console.ReadLine();
Console.WriteLine("Write your Secret Key (Write a number)");
var secretKey = new PrivateKey
{
    privatekey = Convert.ToInt32(Console.ReadLine())
};
var publicSharedKey = await GetPublicSharedKeyAsync();


var publickey = publicSharedKey.Generator ^ secretKey.privatekey % publicSharedKey.Prime;

await UpdateUserPublicKeyAsync(userName, publickey);

var readyToContinue = false;

while (!readyToContinue)
{
    Console.WriteLine("When both Clients are ready, Type Ready");
    var input = Console.ReadLine();

    if (input.ToLower() == "ready")
    {
        readyToContinue = true;
    }

    Console.Clear();
}

var otherUserPublicKey = GetOtherUserInfo(userName).Result.PublicKey;
Console.WriteLine("The Other User's Public key is:" + otherUserPublicKey);
var sharedSecret = otherUserPublicKey ^ secretKey.privatekey % publicSharedKey.Prime;
var bigIntShared = CalculateSharedSecret(otherUserPublicKey, secretKey.privatekey, publicSharedKey.Prime);

connection.On<string, string>("ReceiveMessage", (user, message) =>
{
    Console.WriteLine($"{user}: {EncryptionManager.Decrypt(message, sharedSecret, BitConverter.GetBytes(publicSharedKey.Generator))}");
});
await connection.StartAsync();
//Main Loop
while (true)
{
    var inputMessage = Console.ReadLine();


    if (string.IsNullOrEmpty(inputMessage))
    {
        break;
    }

    var outPutMessage = EncryptionManager.Encrypt(inputMessage,sharedSecret, BitConverter.GetBytes(publicSharedKey.Generator));

    await connection.SendAsync("SendMessage", userName, outPutMessage);
}

await connection.DisposeAsync();





async Task<SecurityParameters> GetPublicSharedKeyAsync()
{
    try
    {
        // Update the URI if your API is hosted on a different base URL
        string requestUri = $"{BaseURL}api/Login/PublicSharedKey";

        // Send a GET request to the specified URI
        var response = await _client.GetAsync(requestUri);

        // Ensure we got a successful response
        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Error: {response.StatusCode}");
            return null;
        }

        // Read the response content as a string
        var content = await response.Content.ReadAsStringAsync();

        // Deserialize the JSON string to PublicSharedKey object
        var publicSharedKey = JsonConvert.DeserializeObject<SecurityParameters>(content);
        return publicSharedKey;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Exception occurred: {ex.Message}");
        return null;
    }
}

async Task<User> GetOtherUserInfo(string userName)
{
    try
    {
        // Construct the request URI
        string requestUri = $"{BaseURL}api/Login/{Uri.EscapeDataString(userName)}";

        // Send a GET request to the specified URI
        var response = await _client.GetAsync(requestUri);

        // Ensure we got a successful response
        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Error: {response.StatusCode}");
            return null;
        }

        // Read the response content as a string
        var content = await response.Content.ReadAsStringAsync();

        // Deserialize the JSON string to User object
        var user = JsonConvert.DeserializeObject<User>(content);
        return user;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Exception occurred: {ex.Message}");
        return null;
    }
}

BigInteger CalculateSharedSecret(int otherPartyPublicKeyInt, int yourPrivateKeyInt, int pInt)
{
    BigInteger otherPartyPublicKey = new BigInteger(otherPartyPublicKeyInt);
    BigInteger yourPrivateKey = new BigInteger(yourPrivateKeyInt);
    BigInteger p = new BigInteger(pInt);

    return BigInteger.ModPow(otherPartyPublicKey, yourPrivateKey, p);
}

async Task<bool> UpdateUserPublicKeyAsync(string userName, int newPublicKey)
{
    try
    {
        string requestUri = $"{BaseURL}/api/Login/PublicSharedKey/{(userName)}";

        var content = new StringContent(JsonConvert.SerializeObject(newPublicKey), Encoding.UTF8, "application/json");

        var response = await _client.PutAsync(requestUri, content);

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Error updating public key: {response.StatusCode}");
            return false;
        }

        Console.WriteLine("Public key updated successfully.");
        return true;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Exception occurred: {ex.Message}");
        return false;
    }
}