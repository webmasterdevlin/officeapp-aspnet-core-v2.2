using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using aspnetcorebackend.Contracts;
using aspnetcorebackend.Helpers;
using aspnetcorebackend.Models;
using aspnetcorebackend.Models.Entities;
using LoginModel = aspnetcorebackend.Identity.LoginModel;

namespace aspnetcorebackend.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public User Authenticate(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password)) return null;

            var userEntity = _context.Users.SingleOrDefault(c =>
                c.Email == email);

            // check if username exists
            if (userEntity == null) return null;

            // check if password is correct
            // authentication successful
            return !VerifyPasswordHash(password, userEntity.PasswordHash, userEntity.PasswordSalt) ? null : userEntity;
        }

        public IEnumerable<User> GetAll()
        {
            return _context.Users;
        }

        public User GetById(Guid id)
        {
            return _context.Users.Find(id);
        }

        public async Task<User> CreateAsync(User user, string password)
        {
            // validation
            if (string.IsNullOrWhiteSpace(password))
                throw new AppException("Password is required");

            if (_context.Users.Any(x => x.Email == user.Email))
                throw new AppException("Username \"" + user.Email + "\" is already taken");

            CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.Id = Guid.NewGuid();
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<User> UpdateAsync(User user, string password = null)
        {
            var userEntity = _context.Users.Find(user.Id);

            if (userEntity == null)
                throw new AppException("User not found");

            if (user.Email != userEntity.Email)
            {
                // username has changed so check if the new username is already taken
                if (_context.Users.Any(u => u.Email == user.Email))
                    throw new AppException("Username " + user.Email + " is already taken");
            }

            // update User properties here
            userEntity.FirstName = user.FirstName;
            userEntity.LastName = user.LastName;

            // update password if it was entered
            if (!string.IsNullOrWhiteSpace(password))
            {
                CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

                userEntity.PasswordHash = passwordHash;
                userEntity.PasswordSalt = passwordSalt;
            }

            _context.Users.Update(userEntity);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task DeleteAsync(Guid id)
        {
            var user = _context.Users.Find(id);
            if (user == null) return;
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        public async Task Update(User userParam, string password = null)
        {
            var user = _context.Users.Find(userParam.Id);

            if (user == null)
                throw new AppException("User not found");

            if (userParam.Email != user.Email)
            {
                // username has changed so check if the new username is already taken
                if (_context.Users.Any(x => x.Email == userParam.Email))
                    throw new AppException("Username " + userParam.Email + " is already taken");
            }

            // update user properties
            user.FirstName = userParam.FirstName;
            user.LastName = userParam.LastName;
            user.Email = userParam.Email;

            // update password if it was entered
            if (!string.IsNullOrWhiteSpace(password))
            {
                byte[] passwordHash, passwordSalt;
                CreatePasswordHash(password, out passwordHash, out passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
            }

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }


        // private helper methods

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password == null) throw new ArgumentNullException(nameof(password));
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Value cannot be empty or whitespace only string.", nameof(password));

            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null) throw new ArgumentNullException(nameof(password));

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Value cannot be empty or whitespace only string.", nameof(password));

            if (storedHash.Length != 64)
                throw new ArgumentException("Invalid length of password hash (64 bytes expected).", nameof(storedHash));

            if (storedSalt.Length != 128)
                throw new ArgumentException("Invalid length of password salt (128 bytes expected).",
                    nameof(storedSalt));

            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                if (computedHash.Where((t, i) => t != storedHash[i]).Any()) return false;
            }

            return true;
        }
    }
}