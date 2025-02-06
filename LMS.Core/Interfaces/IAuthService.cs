using LMS.Core.Contracts;
using LMS.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Interfaces
{
    public interface IAuthService
    {
  
        Task<AuthResponse> Register(RegisterRequest registerRequest);//mapped from dtos to avoid CORE referencing to API 
        Task<AuthResponse> Login(LoginRequest loginRequest);
        // Task<AuthResponse> Logout();
        Task <ApplicationUser> getUserById(string userId);



    }
}
