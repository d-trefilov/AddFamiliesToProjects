namespace AddFamiliesToProjects.Utilities
{
    // Модель для представления каждого файла
    public class FileItem
    {
        // Имя файла
        public string FileName { get; set; }

        // Путь к файлу
        public string FilePath { get; set; }

        // Конструктор для создания объекта FileItem
        public FileItem(string fileName, string filePath)
        {
            FileName = fileName;
            FilePath = filePath;
        }
    }
}
