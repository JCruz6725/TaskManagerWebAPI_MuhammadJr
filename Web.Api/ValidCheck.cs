using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
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

        //Validate if userId and taskId are valid and if task belongs to user
        public async Task<string> ValidateUserTaskAsync(Guid userId,Guid taskId)
        {
            User? getUser = await _unitOfWork.User.GetUserByIdAsync(userId);
            TaskItem? getTask = await _unitOfWork.TaskItem.GetTaskByIdAsync(taskId);

            if (getTask == null && getUser == null)
            {
                return ($"UserId {userId} and TaskId {taskId} are invalid");
            }
            if (getTask == null && getUser != null)
            {
                return ($"TaskId {taskId} is invalid");
            }
            if (getTask != null && getUser == null)
            {
                return ($"UserId {userId} is invalid");
            }
            if (getTask.CreatedUserId != getUser.Id)
            {
                return ($"TaskId {taskId} does not belong to this UserId{userId} ");
            }
            return null;
        }


        //Validate if userId and listId are valid and if list belongs to user
        public async Task<string> ValidateUserListAsync(Guid userId, Guid listId)
        {
            List? getList = await _unitOfWork.List.GetListByIdAsync(listId);
            User? getUser = await _unitOfWork.User.GetUserByIdAsync(userId);

            if (getList == null && getUser == null)
            {
                return ($"UserId {userId} and ListId {listId} are invalid");
            }
            if (getList == null && getUser != null)
            {
                return ($"ListId {listId} is invalid");
            }
            if (getList != null && getUser == null)
            {
                return ($"UserId {userId} is invalid");
            }
            if (getList.CreatedUserId != getUser.Id)
            {
                return ($"ListId {listId} does not belong to this UserId{userId} ");
            }
            return null;
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
