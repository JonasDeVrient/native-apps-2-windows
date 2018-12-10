﻿using System.Collections.Generic;
using System.Linq;
using CityAppREST.Data.Repositories;
using CityAppREST.Helpers;
using CityAppREST.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CityAppREST.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : Controller
    {
        private readonly IRepository<User> _userRepository;
        private readonly TokenGenerator _tokenGenerator;

        public UsersController(IRepository<User> userRepository, TokenGenerator tokenGenerator)
        {
            _userRepository = userRepository;
            _tokenGenerator = tokenGenerator;
        }

        // GET: api/users
        [HttpGet]
        public IEnumerable<User> Get()
        {
            return _userRepository.GetAll();
        }

        // GET api/users/5
        [HttpGet("{id}")]
        public ActionResult<User> Get(int id)
        {
            var user = _userRepository.GetById(id);
            return user == null ? (ActionResult<User>)NotFound() : (ActionResult<User>)user;
        }

        // POST api/users
        [AllowAnonymous]
        [HttpPost]
        public ActionResult<User> Post(User user)
        {
            user.Password = PasswordHasher.GetPasswordAndSaltHash(user.Password);
            _userRepository.Create(user);
            _userRepository.SaveChanges();
            return Ok();
        }

        // PUT api/users/5
        [HttpPut("{id}")]
        public ActionResult<User> Put(int id, User user)
        {
            var toUpdate = _userRepository.GetById(id);
            if (toUpdate == null)
            {
                return NotFound();
            }

            user.Id = toUpdate.Id;
            _userRepository.Update(user);
            _userRepository.SaveChanges();

            return toUpdate;
        }

        // DELETE api/users/5
        [HttpDelete("{id}")]
        public ActionResult<User> Delete(int id)
        {
            var toDelete = _userRepository.GetById(id);
            if (toDelete == null)
            {
                return NotFound();
            }

            _userRepository.Delete(id);
            _userRepository.SaveChanges();

            return toDelete;
        }

        // POST api/users/authenticate
        [AllowAnonymous]
        [HttpPost("authenticate")]
        public ActionResult Authenticate(LoginDetails loginDetails)
        {
            var user = (_userRepository as UserRepository).GetByUsername(loginDetails.Username);
            if (user == null)
            {
                return NotFound();
            }

            var passwordsMatch = PasswordHasher.VerifyPasswordWithHash(loginDetails.Password, user.Password);
            if (!passwordsMatch)
            {
                return Unauthorized();
            }

            var token = _tokenGenerator.GenerateTokenForUser(user);
            return Ok(new { token });
        }
    }

    public class LoginDetails
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
