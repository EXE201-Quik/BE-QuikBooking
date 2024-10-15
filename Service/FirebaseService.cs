using Firebase.Auth;
using Firebase.Storage;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Quik_BookingApp.Helper;
using Quik_BookingApp.Modal;
using Quik_BookingApp.Repos.Interface;
using RestSharp;
using System.Net;

namespace Quik_BookingApp.Service
{
    public class FirebaseService : IFirebaseService
    {

        private static string ApiKey = "AIzaSyCr3D6-Z3owDLYkN8nW4sF4pWLnGXIbzEE";
        private static string Bucket = "quik-77aae.appspot.com";
        private static string AuthEmail = "huylqse173543@fpt.edu.vn";
        private static string AuthPassword = "123456";


        private readonly FirebaseConfiguration _firebaseConfiguration;
        private readonly IConfiguration _configuration;
        public FirebaseService(IConfiguration configuration, IOptions<FirebaseConfiguration> firebaseConfiguration)
        {
            this._firebaseConfiguration = firebaseConfiguration.Value;
            this._configuration = configuration;
        }

        public async Task<IServiceResult> DeleteFileFromFirebase(string pathFileName)
        {
            var _result = new ServiceResult();
            try
            {
                var auth = new FirebaseAuthProvider (new FirebaseConfig(ApiKey));
                var account = await auth.SignInWithEmailAndPasswordAsync(AuthEmail, AuthPassword);
                var storage = new FirebaseStorage(
             _firebaseConfiguration.Bucket,
             new FirebaseStorageOptions
             {
                 AuthTokenAsyncFactory = () => Task.FromResult(account.FirebaseToken),
                 ThrowOnCancel = true
             });
                await storage
                    .Child(pathFileName)
                    .DeleteAsync();
                _result.Message = "Delete image successful";
                _result.Status = 200;
            }
            catch (FirebaseStorageException ex)
            {
                _result.Message = $"Error deleting image: {ex.Message}";
            }
            return _result;
        }

        public async Task<string> GetUrlImageFromFirebase(string pathFileName)
        {
            //var a = pathFileName.Split("/");
            var a = pathFileName.Split("/o/")[1];
            //pathFileName = $"{a[0]}%2F{a[1]}";
            var api = $"https://console.firebase.google.com/u/0/project/quik-77aae/storage/quik-77aae.appspot.com/files?hl={a}";
            if (string.IsNullOrEmpty(pathFileName))
            {
                return string.Empty;
            }

            var client = new RestClient();
            var request = new RestRequest(api);
            var response = client.Execute(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var jmessage = JObject.Parse(response.Content);
                var downloadToken = jmessage.GetValue("downloadTokens").ToString();
                return
                    $"https://console.firebase.google.com/u/0/project/quik-77aae/storage/{_configuration["quik-77aae.appspot.com"]}/files/{pathFileName}?alt=media&token={downloadToken}";
            }

            return string.Empty;
        }

        public async Task<IServiceResult> UploadFileToFirebase(IFormFile file, string pathFileName)
        {
            var _result = new ServiceResult();
            bool isValid = true;
            if (file == null || file.Length == 0)
            {
                isValid = false;
                _result.Message = "The file is empty";
            }
            if (isValid)
            {
                var stream = file!.OpenReadStream();
                var auth = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));
                var account = await auth.SignInWithEmailAndPasswordAsync(AuthEmail, AuthPassword);
                string destinationPath = $"{pathFileName}";

                var task = new FirebaseStorage(
                _firebaseConfiguration.Bucket,
                new FirebaseStorageOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(account.FirebaseToken),
                    ThrowOnCancel = true
                })
                .Child(destinationPath)
                .PutAsync(stream);
                var downloadUrl = await task;

                if (task != null)
                {
                    _result.Status = 200;
                    _result.Message = "Success";
                    _result.Data = downloadUrl;
                }
                else
                {
                    _result.Status = 500;
                    _result.Message = "Upload failed";
                }
            }
            return _result;
        }
        public async Task<IServiceResult> UploadFilesToFirebase(List<IFormFile> files, string basePath)
        {
            var _result = new ServiceResult();
            var uploadResults = new List<string>();

            var auth = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));
            var account = await auth.SignInWithEmailAndPasswordAsync(AuthEmail, AuthPassword);
            var storage = new FirebaseStorage(
                _firebaseConfiguration.Bucket,
                new FirebaseStorageOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(account.FirebaseToken),
                    ThrowOnCancel = true
                });

            foreach (var file in files)
            {
                if (file == null || file.Length == 0)
                {
                    _result.Message = "One or more files are empty";
                    continue;
                }

                var stream = file.OpenReadStream();
                string destinationPath = $"{basePath}/{file.FileName}";

                var task = storage.Child(destinationPath).PutAsync(stream);
                var downloadUrl = await task;

                if (task != null)
                {
                    uploadResults.Add(downloadUrl);
                }
                else
                {
                    _result.Status = 500;
                    _result.Message = $"Upload failed for file: {file.FileName}";
                }
            }

            _result.Data = uploadResults;
            if (uploadResults.Count == files.Count)
            {
                _result.Status = 200;
                _result.Message = "All files uploaded successfully";
            }
            else
            {
                _result.Status = 500;
                _result.Message = "Some files failed to upload";
            }

            return _result;
        }
    }
}
