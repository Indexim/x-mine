namespace X_MINE
{
	public static class PathHelper
	{
        public static string WebRootPath { get; set; }

        public static string GetFullPathNormalized(string path)
        {
            return Path.TrimEndingDirectorySeparator(Path.GetFullPath(path));
        }

        public static string MapPath(string path, string basePath = null)
        {
            basePath = string.IsNullOrEmpty(basePath) ? WebRootPath : basePath;

            if (string.IsNullOrEmpty(basePath))
            {
                throw new ArgumentException("PathHelper does not have WebRootPath or basePath configured.");
            }
            /*Console.WriteLine($"PATH: {path}, BASEPATH: {basePath}");*/
            path = path.Replace("~/", "").TrimStart('/').Replace('/', '\\');
			/*Console.WriteLine($"Result PATH: {path}");*/
            var fullPath = GetFullPathNormalized(Path.Combine(basePath, path));
            /*Console.WriteLine($"fullPath: {fullPath}");*/
            return fullPath;
        }
    }
}
