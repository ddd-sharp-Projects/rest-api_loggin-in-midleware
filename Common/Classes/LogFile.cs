namespace Common.Classes
{
    public class LogFile
    {
        public static void Log(string message)
        {
            string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "Logs");

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            string fileName = DateTime.Now.ToString("yyyy-MM-dd") + "log.txt";
            string filePath = Path.Combine(directoryPath, fileName);

            if (!System.IO.File.Exists(filePath))
            {
                System.IO.File.Create(filePath).Dispose();
            }

            System.IO.File.AppendAllText(filePath, $"{DateTime.Now}: {message}\n");
        }

    }
}