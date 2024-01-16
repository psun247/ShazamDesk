using System.Diagnostics;

namespace ShazamCore.Helpers
{
    public static class GeneralHelper
    {
        public static async Task<string?> OpenWithBrowserAsync(string videoUri)
        {
            string? error = null;

            try
            {
                if (videoUri.IsNotBlank())
                {
                    await GeneralHelper.ExecuteOpenUrlCommandAsync(videoUri);
                }
                else
                {
                    error = "YouTube video or search query not found";
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            return error;
        }

        public static async Task ExecuteOpenUrlCommandAsync(string url, bool lanuchMSEdge = false)
        {
            if (!string.IsNullOrEmpty((string)url))
            {
                await Task.Run(() =>
                {
                    if (lanuchMSEdge)
                    {
                        url = $"microsoft-edge:{url}";
                    }

                    try
                    {
                        // https://brockallen.com/2016/09/24/process-start-for-urls-on-net-core/                        
                        // Suppressing the second command prompt,
                        // and escaping the “&” with “^&” so the shell does not treat them as command separators.                       
                        url = url.Replace("&", "^&");
                        Process.Start(new ProcessStartInfo("cmd", $"/c start {url}")
                        {
                            UseShellExecute = false, // Already false in .NET 6
                            CreateNoWindow = true
                        });
                    }
                    catch (Exception ex)
                    {
                        string error = ex.Message;
                    }
                });
            }
        }

        public static bool IsValidUrl(string url) => Uri.IsWellFormedUriString(url, UriKind.Absolute);

        // https://www.youtube.com/watch?v=0fT2amS9KZ4
        // https://youtu.be/0fT2amS9KZ4
        // https://youtu.be/0fT2amS9KZ4?t=17         
        public static bool IsYouTubeVideoUrl(string input)
        {
            return input.IsNotBlank() &&
                    (input.Contains("youtube.com/watch?v=") || input.Contains("youtu.be/"));
        }

        public static string CleanYouTubeUrl(string youTubeVideoUrl)
        {
            // Remove & and the rest (could be &t=24, &list=..., etc.)
            // https://www.youtube.com/watch?v=xQIDnNHC1rfPXLh&index=1
            int index = youTubeVideoUrl.IndexOf("&");
            if (index > 0)
            {
                youTubeVideoUrl = youTubeVideoUrl.Remove(index, youTubeVideoUrl.Length - index);
            }
            return youTubeVideoUrl;
        }
    }
}
