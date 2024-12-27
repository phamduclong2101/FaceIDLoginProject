using FaceIDLoginProject.Data;
using FaceIDLoginProject.Models;
using FaceIDLoginProject.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using FaceRecognitionDotNet;

namespace FaceIDLoginProject.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly DatabaseContext _context;
        private readonly FaceRecognitionService _faceService;

        public AuthenticationController(DatabaseContext context, FaceRecognitionService faceService)
        {
            _context = context;
            _faceService = faceService;
        }

        [HttpPost]
        public IActionResult Register(string userName, IFormFile faceImage)
        {
            if (faceImage == null || faceImage.Length == 0)
            {
                return Json(new { message = "No face image provided." });
            }

            var uploadDir = Path.Combine("wwwroot", "uploads");
            if (!Directory.Exists(uploadDir))
            {
                Directory.CreateDirectory(uploadDir);
            }

            var imagePath = Path.Combine(uploadDir, faceImage.FileName);
            using (var stream = new FileStream(imagePath, FileMode.Create))
            {
                faceImage.CopyTo(stream);
            }

            var encoding = _faceService.EncodeFace(imagePath);
            if (encoding == null)
            {
                return Json(new { message = "No face detected. Please try again." });
            }

            var user = new User
            {
                UserName = userName,
                FaceEncoding = JsonConvert.SerializeObject(encoding) // Serialize FaceEncoding to JSON
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return Json(new { message = "Registration successful." });
        }

        [HttpPost]
        public IActionResult Login(IFormFile faceImage)
        {
            if (faceImage == null || faceImage.Length == 0)
            {
                return Json(new { message = "No face image provided." });
            }

            var uploadDir = Path.Combine("wwwroot", "uploads");
            if (!Directory.Exists(uploadDir))
            {
                Directory.CreateDirectory(uploadDir);
            }

            var imagePath = Path.Combine(uploadDir, faceImage.FileName);
            using (var stream = new FileStream(imagePath, FileMode.Create))
            {
                faceImage.CopyTo(stream);
            }

            var encoding = _faceService.EncodeFace(imagePath);
            if (encoding == null)
            {
                return Json(new { message = "No face detected. Please try again." });
            }

            var users = _context.Users.ToList();
            foreach (var user in users)
            {
                var storedEncoding = JsonConvert.DeserializeObject<FaceEncoding>(user.FaceEncoding);
                if (_faceService.CompareFaces(encoding, storedEncoding))
                {
                    return Json(new { message = "Login successful." });
                }
            }

            return Json(new { message = "No matching face found." });
        }
    }
}
