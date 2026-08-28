namespace ITI_Project.Services
{
    public class FileServices : IFileServices
    {
        private readonly IWebHostEnvironment _environment;
        public FileServices(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<string> SaveFile(IFormFile file, string[] allowedExtensions)
        {
            var wwwPath = _environment.WebRootPath; //wwwroot هاتلي ال
            var path = Path.Combine(wwwPath, "Images"); // wwwroot/Images <=هيكون كدة path حدد ان ال
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);//ده مش موجوداعمله path لو ال 
            }
            var extension = Path.GetExtension(file.FileName);//بتاع الفايل extensionبجيب ال
            if (!allowedExtensions.Contains(extension))//Exception ارمي allowedExtensionsمش من ضمن ال extensionلو ال
            {
                throw new InvalidOperationException($"Only {string.Join(",", allowedExtensions)} files allowed");
            }
            string fileName = $"{Guid.NewGuid()}{extension}";//(عشان ميكونش فيه تكرار) Guid.NewGuid() عمل اسم ملف عشوائي باستخدام 
            string fileNameWithPath = Path.Combine(path, fileName);
            using var stream = new FileStream(fileNameWithPath, FileMode.Create);
            await file.CopyToAsync(stream);
            return fileName;
        }

        public void DeleteFile(string fileName)
        {
            var wwwPath = _environment.WebRootPath;
            var fileNameWithPath = Path.Combine(wwwPath, "images\\", fileName);
            if (!File.Exists(fileNameWithPath))
                throw new FileNotFoundException(fileName);
            File.Delete(fileNameWithPath);

        }
    }
}
