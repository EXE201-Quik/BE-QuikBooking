using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quik_BookingApp.Helper;
using Quik_BookingApp.Models;
using Quik_BookingApp.Repos;
using Quik_BookingApp.Repos.Request;
using Quik_BookingApp.Repos.Response;
using Quik_BookingApp.Service;

namespace Quik_BookingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkingSpaceController : ControllerBase
    {
        private readonly IWebHostEnvironment environment;
        private readonly QuikDbContext context;
        private readonly IWorkingSpaceService workingSpaceService;

        public WorkingSpaceController(IWebHostEnvironment environment, QuikDbContext context,IWorkingSpaceService workingSpaceService)
        {
            this.environment = environment;
            this.context = context;
            this.workingSpaceService = workingSpaceService;
        }

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

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] WorkingSpaceRequestModel workingSpace)
        {
            var response = await workingSpaceService.CreateWS(workingSpace);
            if (response.ResponseCode == 201)
            {
                return CreatedAtAction(nameof(GetBySpaceId), new { id = response }, response);
            }
            return StatusCode(response.ResponseCode, response);
        }


        [HttpPut("UploadImage")]
        public async Task<IActionResult> UploadImage(IFormFile formFile, string code)
        {
            APIResponse response = new APIResponse();
            try
            {
                string Filepath = GetFilepath(code);
                if (!System.IO.Directory.Exists(Filepath))
                {
                    System.IO.Directory.CreateDirectory(Filepath);
                }

                string imagepath = Filepath + "\\" + code + ".png";
                if (System.IO.File.Exists(imagepath))
                {
                    System.IO.File.Delete(imagepath);
                }
                using (FileStream stream = System.IO.File.Create(imagepath))
                {
                    await formFile.CopyToAsync(stream);
                    response.ResponseCode = 200;
                    response.Result = "pass";
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
                        this.context.Images.Add(new Repos.Models.ImageWS()
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
