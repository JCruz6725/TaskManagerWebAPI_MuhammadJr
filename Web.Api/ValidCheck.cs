using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Web.Api.Dto.Request;
using Web.Api.Persistence;
using Web.Api.Persistence.Models;

namespace Web.Api
{
    public class ValidCheck
    {
        public string? ValidateUserAndTask(User? user, TaskItem? task)
        {
            //Validate if user and task are valid and if task belongs to user
            if (!IsUserValid(user))
            {
                throw new Exception("User is Invalid");
            }
            if (!IsTaskValid(task!))
            {
                throw new Exception("TaskId is invalid");
            }
            if (!IsTaskAssignedToUser(user!, task!))
            {
                throw new Exception("TaskId does not belong to UserId");
            }
            if (!IsDefaultGuid(user!.Id))
            {
                throw new Exception("UserId is invalid");
            }
            return null;
        }

        public string? ValidateUserAndList(User? user, List? list)
        {
            //Validate if user and list are valid and if list belongs to user
            if (!IsUserValid(user))
            {
                throw new Exception("UserId is invalid");
            }
            if (!IsListValid(list!))
            {
                throw new Exception("ListId is invalid");
            }
            if (!IsListAssignedToUser(user!, list!))
            {
                throw new Exception("ListId does not belong to UserId");
            }
            if (!IsDefaultGuid(list!.Id))
            {
                throw new Exception("ListId is invalid");
            }
            return null;
        }
        public string? ValidateUserId(User? userId)
        {
            //Validate if userId is valid
            if (!IsUserValid(userId))
            {
                throw new Exception("UserId is invalid");
            }
            return null;
        }
        public string? ValidateUserRegistration(User? registerUser)
        {
            //Validate if registerUserDto is valid
            if (IsRegistrationValid(registerUser))
            {
                throw new Exception("Registration is invalid User already exits");
            }
            return null;
        }

        //All validations
        public bool IsUserValid(User? user)
        {
            return user != null;
        }
        public bool IsTaskValid(TaskItem? task)
        {
            return task != null;
        }
        public bool IsListValid(List? list)
        {
            return list != null;
        }
        public bool IsTaskAssignedToUser(User? user, TaskItem? taskItem)
        {
            return user!.Id == taskItem!.CreatedUserId;
        }
        public bool IsListAssignedToUser(User? user, List? list)
        {
            return user!.Id == list!.CreatedUserId;
        }
        public bool IsLoginValid(User? loginDto)
        {
            return loginDto != null;
        }
        public bool IsRegistrationValid(User? registerUser)
        {
            return registerUser != null;
        }
        public bool IsDefaultGuid(Guid id)
        {
            return id != Guid.Empty;
        }
    }
}
