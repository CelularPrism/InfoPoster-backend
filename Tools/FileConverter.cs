using MimeKit;
using System.IO;
using System.Text;

namespace InfoPoster_backend.Tools
{
    public class FileConverter
    {
        public static string ReadIFormFileContent(IFormFile file)
        {
            using (var memoryStream = new MemoryStream())
            {
                file.CopyTo(memoryStream);
                var result = Encoding.UTF8.GetString(memoryStream.ToArray());
                if (result.Length > 0 && result[0] == '\uFEFF')
                {
                    result = result.Remove(0, 1);
                }
                return result;
            }
        }
    }
}
