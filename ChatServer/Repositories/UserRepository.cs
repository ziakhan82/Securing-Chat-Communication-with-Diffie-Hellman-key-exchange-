using CryptoChat.Shared;

namespace ChatServer.Repositories
{
    public class UserRepository : IUserRepository
    {

        private readonly Dictionary<int, User> _users;
        private int _sharedSecretBob;
        private int _sharedSecretAlice;

        public UserRepository()
        {
            _users = new Dictionary<int, User>();
        }

        public void UpdateUserPublicKey(string userName, int newPublicKey)
        {
            var user = _users.Values.FirstOrDefault(u => u.Name == userName);
            if (user != null)
            {
                user.PublicKey = newPublicKey;
                _users[user.Id] = user;
            }
        }

        public void AddOrUpdateUser(User user)
        {
            _users[user.Id] = user;
        }

        public void AddUser(User user)
        {
            _users.Add(user.Id, user);
        }

        public User GetUser(int userId)
        {
            _users.TryGetValue(userId, out User user);
            return user;
        }

        public List<User> GetUsers()
        {
            return _users.Values.ToList();
        }

        public bool RemoveUser(int userId)
        {
            return _users.Remove(userId);
        }

        public void SetSharedSecretBob(int secret)
        {
            _sharedSecretBob = secret;
        }

        public int GetSharedSecretBob()
        {
            return _sharedSecretBob;
        }

        public void SetSharedSecretAlice(int secret)
        {
            _sharedSecretAlice = secret;
        }

        public int GetSharedSecretAlice()
        {
            return _sharedSecretAlice;
        }
    }
}

