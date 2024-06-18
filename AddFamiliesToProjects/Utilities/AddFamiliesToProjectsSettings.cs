using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.IO;


namespace AddFamiliesToProjects.Utilities
{
    //Работа с JSON файлом
    public class AddFamiliesToProjectsSettings
    {
        //Сохранение JSON файла
        public void Save(ObservableCollection<FileItem> selectedFiles, string jsonFilePath)
        {
            if (File.Exists(jsonFilePath))
            {
                File.Delete(jsonFilePath);
            }

            using (FileStream fs = new FileStream(jsonFilePath, FileMode.Create))
            {
                fs.Close();
            }
            JsonSerializer serializer = new JsonSerializer();
            using (StreamWriter sw = new StreamWriter(jsonFilePath))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Formatting = Formatting.Indented;
                serializer.Serialize(writer, selectedFiles);
            }
        }

        //Открытие JSON файла
        public ObservableCollection<FileItem> GetSettings(string jsonFilePath)
        {
            ObservableCollection<FileItem> fileItemsTmp = new ObservableCollection<FileItem>();
            if (File.Exists(jsonFilePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                using (StreamReader sr = new StreamReader(jsonFilePath))
                using (JsonTextReader reader = new JsonTextReader(sr))
                {
                    ObservableCollection<FileItem> tmp = (ObservableCollection<FileItem>)serializer
                        .Deserialize(reader, typeof(ObservableCollection<FileItem>));
                    if (tmp != null)
                    {
                        fileItemsTmp = tmp;
                    }
                }
            }
            else
            {
                fileItemsTmp = new ObservableCollection<FileItem>();
            }
            return fileItemsTmp;
        }
    }
}
