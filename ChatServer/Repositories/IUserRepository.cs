using CryptoChat.Shared;

namespace ChatServer.Repositories
{
    public interface IUserRepository
    {
        void UpdateUserPublicKey(string userName, int newPublicKey);
        void AddOrUpdateUser(User user);
        void AddUser(User user);
        User GetUser(int userId);
        List<User> GetUsers();
        bool RemoveUser(int userId);
    }
}
