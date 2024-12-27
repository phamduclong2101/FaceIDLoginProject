using FaceRecognitionDotNet;
using System.Linq;
using System.IO;

namespace FaceIDLoginProject.Services
{
    public class FaceRecognitionService
    {
        private readonly FaceRecognition _faceRecognition;

        public FaceRecognitionService(string modelPath)
        {
            var modelFullPath = Path.Combine(Directory.GetCurrentDirectory(), modelPath);

            // Kiểm tra sự tồn tại của các file mô hình
            if (!File.Exists(Path.Combine(modelFullPath, "dlib_face_recognition_resnet_model_v1.dat")) ||
                !File.Exists(Path.Combine(modelFullPath, "mmod_human_face_detector.dat")) ||
                !File.Exists(Path.Combine(modelFullPath, "shape_predictor_68_face_landmarks.dat")))
            {
                throw new FileNotFoundException("One or more required model files are missing in the Models directory.");
            }

            _faceRecognition = FaceRecognition.Create(modelFullPath);
        }

        // Mã hóa khuôn mặt từ ảnh
        public FaceEncoding EncodeFace(string imagePath)
        {
            using var image = FaceRecognition.LoadImageFile(imagePath);

            // Phát hiện vị trí khuôn mặt
            var faceLocations = _faceRecognition.FaceLocations(image);

            if (!faceLocations.Any())
                return null;

            // Phát hiện landmarks trên khuôn mặt
            var landmarks = faceLocations
                .SelectMany(location => _faceRecognition.FaceLandmark(image, new[] { location }))
                .FirstOrDefault();


            if (landmarks == null)
                return null;

            // Mã hóa khuôn mặt
            var encodings = _faceRecognition.FaceEncodings(image, faceLocations).ToArray();

            return encodings.Length > 0 ? encodings[0] : null;
        }

        // So sánh mã hóa khuôn mặt
        public bool CompareFaces(FaceEncoding encoding1, FaceEncoding encoding2, double tolerance = 0.6)
        {
            return FaceRecognition.FaceDistance(encoding1, encoding2) < tolerance;
        }
    }
}
