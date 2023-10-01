namespace ArticlesProject.Helper
{
    public class UploadImage
    {
        public const string ArticlesImagesFolderLocation = @"C:\Users\Administrator\Desktop\Yesterday\ArticlesProject\ArticlesProject\wwwroot\images\articles";
        public const string UserProfilePicturesFolderLocation = @"C:\Users\Administrator\Desktop\Yesterday\ArticlesProject\ArticlesProject\wwwroot\images\UserProfilePictures";
        public const long MaxAllowedImageSize = 1048576;
        public static new List<string> AllowedExtenstions = new() { ".jpeg", ".jpg", ".png" };
    }
}