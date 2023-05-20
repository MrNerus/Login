using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Login.Controllers;

public class SignupController : Controller
{
    public IActionResult Index() 
    {
        return View();
    }


    [HttpPost("/Signup")] public IActionResult Signup([FromBody] JsonObject submittedData)
    {
        Dictionary<string, string> R = new Dictionary<string, string> {
            {"status", "failed"},
            {"message", "Unknown Error"}
        };
        try
        {
            Login.Models.SignupModel data = new Login.Models.SignupModel();
            data.username = submittedData["username"]?.ToString();
            data.password = submittedData["password"]?.ToString();
            data.name     = submittedData["name"    ]?.ToString();
            data.email    = submittedData["email"   ]?.ToString();
            data.address  = submittedData["address" ]?.ToString();
            data.phone    = submittedData["phno"    ]?.ToString();

            Login.GuardRail.SignupGuardRail dataValidation = new Login.GuardRail.SignupGuardRail(data);
            string validationResponse = dataValidation.Validation();
            if (validationResponse == "ValidationError_SQLProtection")
            {
                R["status" ] = "failed"; 
                R["message"] = "Bad Value";
                return Content(JsonSerializer.Serialize(R), "application/json");
            }
            else if (validationResponse == "ValidationError_BadValue")
            {
                R["status" ] = "failed"; 
                R["message"] = "Bad Value";
                return Content(JsonSerializer.Serialize(R), "application/json");
            }
            else if (validationResponse == "Ok")
            {
                Login.Factory.SignUpAuthorize signup = new Login.Factory.SignUpAuthorize(data);
                string signupResponse = signup.DatabaseInsert();
                Console.WriteLine("Signup response: " + signupResponse);
                
                if (signupResponse == "DataError_AlreadyUser")
                {
                    R["status" ] = "failed";
                    R["message"] = "Already a User";
                    return Content(JsonSerializer.Serialize(R), "application/json");
                }
                else if (signupResponse == "DataError_UserCreation")
                {
                    R["status" ] = "failed"; 
                    R["message"] = "Unknown Error";
                    return Content(JsonSerializer.Serialize(R), "application/json");
                }
                else if (signupResponse == "Ok")
                {
                    R["status" ] = "good"; 
                    R["message"] = "--0--0--";
                    return Content(JsonSerializer.Serialize(R), "application/json");
                }
                else
                {
                    R["status" ] = "failed"; 
                    R["message"] = "Unknown Error";
                    return Content(JsonSerializer.Serialize(R), "application/json");
                }
            }
            else
            {
                R["status" ] = "failed"; 
                R["message"] = "Bad Value";
                return Content(JsonSerializer.Serialize(R), "application/json");
            }
        }
        catch (System.Exception)
        {
            R["status" ] = "failed"; 
            R["message"] = "Bad Value";
            return Content(JsonSerializer.Serialize(R), "application/json");
        }
    }
}