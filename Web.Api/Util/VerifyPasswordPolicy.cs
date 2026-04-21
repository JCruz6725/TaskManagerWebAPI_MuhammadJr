using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using Web.Api.Controllers;
using Web.Api.Dto.Request;

namespace Web.Api.Util
{
    public class VerifyPasswordPolicy
    {
        public bool Verify(string password)
        {
            if (password.Length < 8)
            {
                return false;
            }
            Regex regex = new Regex(@"[^DevicePostDto-zA-Z0-9\s]");
            if (!regex.IsMatch(password)) //if there are no special chars
            {
                return false;
            }
            return true;
        }
    }
}
