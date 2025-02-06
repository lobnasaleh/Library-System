using LMS.Core.Contracts;
using LMS.Core.Entities;
using LMS.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Infrastructure.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IConfiguration _config;


        public AuthService(UserManager<ApplicationUser> _userManager, RoleManager<IdentityRole> roleManager, IConfiguration _config)
        {
            this._userManager = _userManager;
            this.roleManager = roleManager;
            this._config = _config;

        }

        public async Task<AuthResponse> Login(LoginRequest loginRequest)
        {
          
            var user = await _userManager.FindByEmailAsync(loginRequest.Email);
            if (user is null || !await _userManager.CheckPasswordAsync(user, loginRequest.Password))
            {
               AuthResponse response= new AuthResponse() { isAuthenticated = false, Message = "Email Or Password is Incorrect" };
               return response;
            }

            //get roles of user 
            var userRoles = await _userManager.GetRolesAsync(user);

            //generate jwt token
            //ba strore fel token hagat zy el id bta3 el user ,roles,name 3shan keda ba3at el user lel generatejwt
            JwtSecurityToken jwt = await GenerateJWTTokenAsync(user);
            
            AuthResponse res = new AuthResponse()
            {
                //kol dool baraga3hom lel front l2eno momkn yehtaghom
                Token =new JwtSecurityTokenHandler().WriteToken(jwt),
                isAuthenticated = true,
                Message="Logged In Successfully !",
                Email=loginRequest.Email,
                Roles=userRoles,//agarb asheel .tolist
                ExpiresOn =DateTime.Now.AddDays(30),
                Username = user.UserName
            };

            return res;


        }

        public async Task<AuthResponse> Register(RegisterRequest registerRequest)
        {
            if (await _userManager.FindByEmailAsync(registerRequest.Email) is not null)//we found a user with this email
            {
                return new AuthResponse() { isAuthenticated = false,Message = "This Email is already Registered" };
            }
            if (await _userManager.FindByNameAsync(registerRequest.Username) is not null)//we found a user with this email
            {
                return new AuthResponse() { isAuthenticated = false,Message = "This Username is already Registered" };
            }
            //map RegisterRequest to ApplicationUser
            ApplicationUser user = new ApplicationUser()
            {
                DateOfBirth = registerRequest.DateOfBirth,
                Email = registerRequest.Email,
                Gender = registerRequest.Gender,
                Image = registerRequest.Image,
                Name = registerRequest.Name,
                UserName = registerRequest.Username,
                //el password ha ahotha ma3 el createasync
            };
            var errorslist = new List<string>();
            var res = await _userManager.CreateAsync(user, registerRequest.Password);
            if (!res.Succeeded)
            {
                foreach (var err in res.Errors)
                {
                  errorslist.Add(err.Description);
                }

              string errormessages=  string.Join(", ", errorslist);

                return new AuthResponse() {
                    isAuthenticated=false,
                    Message = errormessages 
                };
            }
            //assign role
            await _userManager.AddToRoleAsync(user, "Member");


            JwtSecurityToken jwt = await GenerateJWTTokenAsync(user);

            //create Token
          


               return new AuthResponse
            {
                Email = user.Email,
                ExpiresOn = DateTime.Now.AddDays(30),
                isAuthenticated = true,
                Message = $"Welcome {user.UserName}",
                Roles = new List<string> { "Member" },
                Token = new JwtSecurityTokenHandler().WriteToken(jwt),
                Username = user.UserName
            };
        }
        public async Task<JwtSecurityToken> GenerateJWTTokenAsync(ApplicationUser user)
        {
            var claims=await _userManager.GetClaimsAsync(user);

            var roles=await _userManager.GetRolesAsync(user);

            List<Claim> userclaims=new List<Claim>();//to store Id,name,roles and make jwt unique every time
           
            foreach (var role in roles) {
                userclaims.Add(new Claim(ClaimTypes.Role, role));
            }
            userclaims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
            userclaims.Add(new Claim(ClaimTypes.Email, user.Email));
            userclaims.Add(new Claim(ClaimTypes.Name, user.UserName));
            userclaims.Add(new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()));

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["jwt:secretkey"]));
            var SigningCredentials=new SigningCredentials(securityKey,SecurityAlgorithms.HmacSha256);

            JwtSecurityToken mytoken = new JwtSecurityToken(
                claims:userclaims,
                audience: _config["jwt:audience"],
                issuer: _config["jwt:issuer"],
                expires: DateTime.Now.AddDays(30),
                signingCredentials: SigningCredentials


                );
            

            foreach (var claim in claims)//for testing
            {
                Console.WriteLine($"Claim Type: {claim.Type}, Claim Value: {claim.Value}");
            }

            return mytoken;
        }

        public async Task<ApplicationUser> getUserById(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }
    }
}
