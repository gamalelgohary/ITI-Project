namespace ITI_Project.Services
{
    public interface IFileServices
    {
        void DeleteFile(string fileName);
        Task<string> SaveFile(IFormFile file, string[] allowedExtensions);
    }
}
