using Newtonsoft.Json;

namespace WebApi.Firebase.Model
{
    //var firebaseEx = JsonConvert.DeserializeObject<FirebaseError>(ex.ResponseData);
    public class FirebaseErrorModel
    {
        public Error error { get; set; }
    }

    public class Error
    {
        public int code { get; set; }
        public string message { get; set; }
        public List<Error> errors { get; set; }
    }
}
