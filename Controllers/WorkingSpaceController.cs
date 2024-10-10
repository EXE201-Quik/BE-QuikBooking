using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quik_BookingApp.BOs.Request;
using Quik_BookingApp.DAO;
using Quik_BookingApp.DAO.Models;
using Quik_BookingApp.Helper;
using Quik_BookingApp.Repos.Interface;
using Swashbuckle.AspNetCore.Annotations;
using Firebase.Storage;
using System;
using System.Diagnostics;
using Firebase.Auth;

namespace Quik_BookingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkingSpaceController : ControllerBase
    {
        private static string ApiKey = "AIzaSyCIROgMN-g5iIsG9d9fCB88PTOWTqhNknk";
        private static string Bucket = "quik-a8158.appspot.com";
        private static string AuthEmail = "huylqse173543@fpt.edu.vn";
        private static string AuthPassword = "123456";

        private readonly IWebHostEnvironment environment;
        private readonly QuikDbContext context;
        private readonly IWorkingSpaceService workingSpaceService;

        public WorkingSpaceController(IWebHostEnvironment environment, QuikDbContext context, IWorkingSpaceService workingSpaceService)
        {
            this.environment = environment;
            this.context = context;
            this.workingSpaceService = workingSpaceService;
        }

        [HttpPost]
        private async Task<IActionResult> Index(FileUploadViewModel file)
        {
            var fileUpload = file.File;
            if (fileUpload.Length > 0)
            {
                var fs = fileUpload.OpenReadStream();

                // Firebase authentication
                var authProvider = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));
                
                 
                var auth = await authProvider.SignInWithEmailAndPasswordAsync(AuthEmail, AuthPassword);

                // Cancellation token
                var cancellation = new CancellationTokenSource();

                // Uploading to Firebase Storage
                var upload = new FirebaseStorage(Bucket, new FirebaseStorageOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(auth.FirebaseToken),
                    ThrowOnCancel = true
                })
                    .Child("assets")
                    .Child($"{Path.GetFileNameWithoutExtension(fileUpload.FileName)}{Path.GetExtension(fileUpload.FileName)}")
                    .PutAsync(fs, cancellation.Token);

                try
                {
                    var downloadUrl = await upload;
                    return Ok(new { link = downloadUrl });
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"**********{ex}**********");
                    return StatusCode(500, "An error occurred while uploading the file.");
                }
            }
            return BadRequest("File is empty");
        }


        [SwaggerOperation(
             Summary = "Retrieve all workign space",
             Description = "Returns a list of all working spaces. If no workign space are found, it returns a 404 Not Found response."
         )]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var data = await workingSpaceService.GetAll();
            if (data == null || data.Count == 0)
            {
                return NotFound();
            }
            return Ok(data);
        }

        [SwaggerOperation(
             Summary = "Retrieve user by wsId",
             Description = "This API allows you to get WS details by providing a wsId. If the WS is not found, a 404 Not Found response will be returned."
        )]
        [HttpGet("GetById/{workingSpaceId}")]
        public async Task<IActionResult> GetBySpaceId(string workingSpaceId)
        {
            var data = await workingSpaceService.GetBySpaceId(workingSpaceId);
            if (data == null)
            {
                return NotFound();
            }
            return Ok(data);
        }

        [SwaggerOperation(
             Summary = "Retrieve user by wsId",
             Description = "This API allows you to get WS details by providing a wsId. If the WS is not found, a 404 Not Found response will be returned."
        )]
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] WorkingSpaceRequestModel workingSpace)
        {
            var response = await workingSpaceService.CreateWS(workingSpace);
            if (response.ResponseCode == 201)
            {
                return CreatedAtAction(nameof(GetBySpaceId), new { workingSpaceId = workingSpace.SpaceId }, response);

            }
            return StatusCode(response.ResponseCode, response);
        }





        [HttpPut("UploadImage")]
        public async Task<IActionResult> UploadImage(IFormFile formFile, string code)
        {
            APIResponse response = new APIResponse();
            try
            {
                // Check if the formFile is null or empty
                if (formFile == null || formFile.Length == 0)
                {
                    response.ResponseCode = 400;
                    response.Message = "No file uploaded.";
                    return BadRequest(response);
                }

                // Prepare the Firebase storage path
                string imageFileName = $"{code}.png";
                string imagePathInStorage = $"Upload/workingspace/{code}/{imageFileName}";

                // Create a new FirebaseStorage instance using your Firebase project details
                var firebaseStorage = new FirebaseStorage("https://console.firebase.google.com/u/2/project/quik-a8158/storage/quik-a8158.appspot.com/files", // Replace with your Firebase storage URL
                    new FirebaseStorageOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult("https://oauth2.googleapis.com/token"), // Replace with your token
                        ThrowOnCancel = true
                    });

                // Upload the file to Firebase Storage
                using (var stream = new MemoryStream())
                {
                    await formFile.CopyToAsync(stream);
                    stream.Position = 0; // Reset stream position

                    var uploadTask = await firebaseStorage
                        .Child(imagePathInStorage)
                        .PutAsync(stream);

                    // Prepare the image URL
                    string imageUrl = uploadTask; // This returns the URL of the uploaded image

                    // Create an instance of ImageWS and save to the database
                    var imageWS = new ImageWS
                    {
                        WorkingSpaceName = "Your Working Space Name", // Set as appropriate
                        WSCode = code,
                        WSImages = stream.ToArray() // Get the byte array directly
                    };

                    // Assuming _context is your database context
                    context.Images.Add(imageWS);
                    await context.SaveChangesAsync();

                    // Set the response
                    response.ResponseCode = 200;
                    response.Result = "pass";
                    response.Message = $"Image uploaded to Firebase and saved to database with URL: {imageUrl}";
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }

            return Ok(response);
        }




        [HttpPut("MultiUploadImage")]
        public async Task<IActionResult> MultiUploadImage(IFormFileCollection filecollection, string code)
        {
            APIResponse response = new APIResponse();
            int passcount = 0; int errorcount = 0;
            try
            {
                string Filepath = GetFilepath(code);
                if (!System.IO.Directory.Exists(Filepath))
                {
                    System.IO.Directory.CreateDirectory(Filepath);
                }
                foreach (var file in filecollection)
                {
                    string imagepath = Filepath + "\\" + file.FileName;
                    if (System.IO.File.Exists(imagepath))
                    {
                        System.IO.File.Delete(imagepath);
                    }
                    using (FileStream stream = System.IO.File.Create(imagepath))
                    {
                        await file.CopyToAsync(stream);
                        passcount++;

                    }
                }


            }
            catch (Exception ex)
            {
                errorcount++;
                response.Message = ex.Message;
            }
            response.ResponseCode = 200;
            response.Result = passcount + " Files uploaded &" + errorcount + " files failed";
            return Ok(response);
        }

        [HttpGet("GetImage")]
        public async Task<IActionResult> GetImage(string code)
        {
            string Imageurl = string.Empty;
            string hosturl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";
            try
            {
                string Filepath = GetFilepath(code);
                string imagepath = Filepath + "\\" + code + ".png";
                if (System.IO.File.Exists(imagepath))
                {
                    Imageurl = hosturl + "/Upload/" + code + "/" + code + ".png";
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
            }
            return Ok(Imageurl);

        }

        [HttpGet("GetMultiImage")]
        public async Task<IActionResult> GetMultiImage(string code)
        {
            List<string> Imageurl = new List<string>();
            string hosturl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";
            try
            {
                string Filepath = GetFilepath(code);

                if (System.IO.Directory.Exists(Filepath))
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(Filepath);
                    FileInfo[] fileInfos = directoryInfo.GetFiles();
                    foreach (FileInfo fileInfo in fileInfos)
                    {
                        string filename = fileInfo.Name;
                        string imagepath = Filepath + "\\" + filename;
                        if (System.IO.File.Exists(imagepath))
                        {
                            string _Imageurl = hosturl + "/Upload/" + code + "/" + filename;
                            Imageurl.Add(_Imageurl);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
            }
            return Ok(Imageurl);

        }

        [HttpGet("Download")]
        public async Task<IActionResult> download(string code)
        {
            // string Imageurl = string.Empty;
            //string hosturl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";
            try
            {
                string Filepath = GetFilepath(code);
                string imagepath = Filepath + "\\" + code + ".png";
                if (System.IO.File.Exists(imagepath))
                {
                    MemoryStream stream = new MemoryStream();
                    using (FileStream fileStream = new FileStream(imagepath, FileMode.Open))
                    {
                        await fileStream.CopyToAsync(stream);
                    }
                    stream.Position = 0;
                    return File(stream, "image/png", code + ".png");
                    //Imageurl = hosturl + "/Upload/" + code + "/" + code + ".png";
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return NotFound();
            }


        }

        [HttpGet("Remove")]
        public async Task<IActionResult> remove(string code)
        {
            // string Imageurl = string.Empty;
            //string hosturl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";
            try
            {
                string Filepath = GetFilepath(code);
                string imagepath = Filepath + "\\" + code + ".png";
                if (System.IO.File.Exists(imagepath))
                {
                    System.IO.File.Delete(imagepath);
                    return Ok("pass");
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return NotFound();
            }


        }

        [HttpGet("MultiRemove")]
        public async Task<IActionResult> multiremove(string code)
        {
            // string Imageurl = string.Empty;
            //string hosturl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";
            try
            {
                string Filepath = GetFilepath(code);
                if (System.IO.Directory.Exists(Filepath))
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(Filepath);
                    FileInfo[] fileInfos = directoryInfo.GetFiles();
                    foreach (FileInfo fileInfo in fileInfos)
                    {
                        fileInfo.Delete();
                    }
                    return Ok("pass");
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return NotFound();
            }


        }

        [HttpPut("DBMultiUploadImage")]
        public async Task<IActionResult> DBMultiUploadImage(IFormFileCollection filecollection, string code)
        {
            APIResponse response = new APIResponse();
            int passcount = 0; int errorcount = 0;
            try
            {
                foreach (var file in filecollection)
                {
                    using (MemoryStream stream = new MemoryStream())
                    {
                        await file.CopyToAsync(stream);
                        this.context.Images.Add(new DAO.Models.ImageWS()
                        {
                            WSCode = code,
                            WSImages = stream.ToArray()
                        });
                        await this.context.SaveChangesAsync();
                        passcount++;
                    }
                }


            }
            catch (Exception ex)
            {
                errorcount++;
                response.Message = ex.Message;
            }
            response.ResponseCode = 200;
            response.Result = passcount + " Files uploaded &" + errorcount + " files failed";
            return Ok(response);
        }


        [HttpGet("GetDBMultiImage")]
        public async Task<IActionResult> GetDBMultiImage(string code)
        {
            List<string> Imageurl = new List<string>();
            //string hosturl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";
            try
            {
                var _image = this.context.Images.Where(item => item.WSCode == code).ToList();
                if (_image != null && _image.Count > 0)
                {
                    _image.ForEach(item =>
                    {
                        Imageurl.Add(Convert.ToBase64String(item.WSImages));
                    });
                }
                else
                {
                    return NotFound();
                }
                //string Filepath = GetFilepath(code);

                //if (System.IO.Directory.Exists(Filepath))
                //{
                //    DirectoryInfo directoryInfo = new DirectoryInfo(Filepath);
                //    FileInfo[] fileInfos = directoryInfo.GetFiles();
                //    foreach (FileInfo fileInfo in fileInfos)
                //    {
                //        string filename = fileInfo.Name;
                //        string imagepath = Filepath + "\\" + filename;
                //        if (System.IO.File.Exists(imagepath))
                //        {
                //            string _Imageurl = hosturl + "/Upload/" + code + "/" + filename;
                //            Imageurl.Add(_Imageurl);
                //        }
                //    }
                //}

            }
            catch (Exception ex)
            {
            }
            return Ok(Imageurl);

        }


        [HttpGet("DbDownload")]
        public async Task<IActionResult> dbdownload(string code)
        {

            try
            {

                var _image = await this.context.Images.FirstOrDefaultAsync(item => item.WSCode == code);
                if (_image != null)
                {
                    return File(_image.WSImages, "image/png", code + ".png");
                }


                //string Filepath = GetFilepath(code);
                //string imagepath = Filepath + "\\" + code + ".png";
                //if (System.IO.File.Exists(imagepath))
                //{
                //    MemoryStream stream = new MemoryStream();
                //    using (FileStream fileStream = new FileStream(imagepath, FileMode.Open))
                //    {
                //        await fileStream.CopyToAsync(stream);
                //    }
                //    stream.Position = 0;
                //    return File(stream, "image/png", code + ".png");
                //    //Imageurl = hosturl + "/Upload/" + code + "/" + code + ".png";
                //}
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return NotFound();
            }


        }

        [NonAction]
        private string GetFilepath(string code)
        {
            return this.environment.WebRootPath + "\\Upload\\" + code;
        }
    }
}
