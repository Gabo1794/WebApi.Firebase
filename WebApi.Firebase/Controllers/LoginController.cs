using Firebase.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebApi.Firebase.Model;

namespace WebApi.Firebase.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        FirebaseAuthProvider auth;
        public LoginController(IConfiguration configuration)
        {
            auth = new FirebaseAuthProvider(
                            new FirebaseConfig(configuration.GetValue<string>("FirebaseWebAPI")));
        }

        [HttpPost]
        public async Task<ActionResult> Login(LoginModel loginModel)
        {
            try
            {
                var fbAuthLink = await auth
                .SignInWithEmailAndPasswordAsync(loginModel.Email, loginModel.Password);
                string token = fbAuthLink.FirebaseToken;
                //saving the token in a session variable
                if (token != null)
                {
                    //HttpContext.Session.SetString("_UserToken", token);

                    return StatusCode(StatusCodes.Status200OK, token);
                }
                return StatusCode(StatusCodes.Status200OK);
            }
            catch (FirebaseAuthException ex)
            {
                var firebaseEx = JsonConvert.DeserializeObject<FirebaseErrorModel>(ex.ResponseData);
                return StatusCode(StatusCodes.Status500InternalServerError, firebaseEx);
            }
        }

        [HttpPost]
        [Route("Register")]
        public async Task<ActionResult> Register(RegisterModel loginModel)
        {
            try
            {
                //create the user
                await auth.CreateUserWithEmailAndPasswordAsync(loginModel.Email, loginModel.Password, loginModel.Name ?? $"User-{Guid.NewGuid}");
                //log in the new user
                var fbAuthLink = await auth
                                .SignInWithEmailAndPasswordAsync(loginModel.Email, loginModel.Password);
                string token = fbAuthLink.FirebaseToken;
                //saving the token in a session variable
                if (token != null)
                {
                    return StatusCode(StatusCodes.Status200OK, token);
                }

                return StatusCode(StatusCodes.Status400BadRequest);
            }
            catch (FirebaseAuthException ex)
            {
                var firebaseEx = JsonConvert.DeserializeObject<FirebaseErrorModel>(ex.ResponseData);
                return StatusCode(StatusCodes.Status500InternalServerError, firebaseEx);
            }
        }

        [HttpPost]
        [Route("testValidate")]
        [Authorize]
        public async Task<ActionResult> TestMethod()
        {
            try
            {
                return StatusCode(StatusCodes.Status200OK);
            }
            catch(FirebaseAuthException ex)
            {
                var firebaseEx = JsonConvert.DeserializeObject<FirebaseErrorModel>(ex.ResponseData);
                return StatusCode(StatusCodes.Status500InternalServerError, firebaseEx);
            }
        }
    }
}
