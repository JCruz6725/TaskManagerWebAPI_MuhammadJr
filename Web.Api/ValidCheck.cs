using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;
using Web.Api.Dto.Request;
using Web.Api.Persistence;
using Web.Api.Persistence.Models;

namespace Web.Api
{
    public class ValidCheck
    {
        private readonly UnitOfWork _unitOfWork;                         //private readonly field to access the UofW class
        public ValidCheck(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }







       public string? ValidateUserTask (User? u, TaskItem? t)
       {


            // check if null.
            if(!DoesUserExist(u)) { 
                return "User not exist";
            }
            if(!DoesTaskExist(t)) { 
                return "Task not exist";
            }

            // check for default guid
            if(IsDefaultGuid(u!.Id)) {
                return "UserId Invalid";
            }
            if (IsDefaultGuid(t!.Id)) {
                return "TaskId Invalid";
            }

            // check if task belongs to user
            if(DoesTaskBelongToUser(u!, t!)) { 
                return "Task does not belong to user";
            }

            return null;
        }

        public bool DoesUserExist (User? u)
        {
            return u is not null;
        }

        public bool DoesTaskBelongToUser (User u, TaskItem  t)
        {
            return u.Id == t.CreatedUserId;
        }

        public bool DoesTaskExist (TaskItem? t)
        {
            return t is not null;
        }

        public bool IsDefaultGuid(Guid Id) { 
            return Id.Equals(Guid.Empty);

        }

















    
        //Validate if user exist before creating new user
        public async Task<string> ValidateRegistrationAsync(RegisterUserDto registerUserDto)
        {
            User? getUserRegistration = await _unitOfWork.User.GetUserByEmailAsync(registerUserDto.Email);

           if(getUserRegistration != null)
            {
                return ("User already exists");
            }
            return null;

        }
        //Validate user login and check if password matches to that user
        public async Task<string> ValidateLoginAsync(LoginDto userLoginDto)
        {
            User? getUserLogin = await _unitOfWork.User.GetUserByEmailAsync(userLoginDto.Email);

            if (getUserLogin == null)
            {
                return ("Invalid email entered");
            }
            
            if (getUserLogin.Password != userLoginDto.Password)
            {
                return ("Invalid pasword entered");
            }

            return null;
        }

        //Validate if userId is valid
        public async Task<string> ValidateUserAsync(Guid userId)
        {
            User? getUser = await _unitOfWork.User.GetUserByIdAsync(userId);
            if(getUser == null)
            {
                return ($"UserId {userId} is invalid");
            }
            return null;
        }

    }
}
