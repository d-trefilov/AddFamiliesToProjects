using AddFamiliesToProjects.Utilities;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Input;


namespace AddFamiliesToProjects.ViewModels
{
    public class AddFamiliesToProjectsViewModel : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        //Инициализация команд
        public AddFamiliesToProjectsViewModel()
        {
            // Инициализация команды выбора проектов
            SelectProjectCommand = new RelayCommand(SelectProject);
            // Инициализация команды выбора семейств
            SelectFamilyCommand = new RelayCommand(SelectFamily);
            // Инициализация команды сохранения семейств в Json
            SaveJsonFamiliesCommand = new RelayCommand(SaveJsonFamilies);
            // Инициализация команды открытия семейств в Json
            OpenJsonFamiliesCommand = new RelayCommand(OpenJsonFamilies);
            // Инициализация команды сохранения проектов в Json
            SaveJsonProjectsCommand = new RelayCommand(SaveJsonProjects);
            // Инициализация команды открытия проектов в Json
            OpenJsonProjectsCommand = new RelayCommand(OpenJsonProjects);
            // Инициализация команды добавления семейств в проекты
            AddFamiliesCommand = new RelayCommand(AddFamilies);
        }


        // Коллекция выбранных проектов
        private ObservableCollection<FileItem> selectedProjects = new ObservableCollection<FileItem>();

        public ObservableCollection<FileItem> SelectedProjects
        {
            get { return selectedProjects; }
            set
            {
                selectedProjects = value;
                OnPropertyChanged(nameof(SelectedProjects));
            }
        }

        //Команда для выбора проекта
        public ICommand SelectProjectCommand { get; }

        // Метод для выбора проекта
        private void SelectProject(object obj)
        {
            // Открытие диалогового окна для выбора проектов
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.DefaultExt = ".rvt";
            openFileDialog.Filter = "rvt files (*.rvt)|*.rvt|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 1;
            openFileDialog.Title = "Выберите необходимые проекты";
            openFileDialog.Multiselect = true; // Разрешение выбора нескольких файлов
            bool? result = openFileDialog.ShowDialog();

            if (result == true)
            {
                // Добавление выбранных проектов в коллекцию
                foreach (string projectName in openFileDialog.FileNames)
                {
                    var projectFile = new FileItem(Path.GetFileName(projectName), projectName);
                    bool containsFile = SelectedProjects.Any(item => item.FilePath == projectFile.FilePath);
                    //Проверка дублирования проектов в списке
                    if (containsFile)
                    {
                        TaskDialog.Show("Revit", $"Файл \"{projectFile.FileName}\" уже имеется в списке!");
                        continue;
                    }
                    //Проверка расширения файла(.rvt)
                    bool extensionFile = !projectFile.FilePath.Contains(".rvt");
                    if (extensionFile)
                    {
                        TaskDialog.Show("Revit", $"Файл \"{projectFile.FileName}\" не является проектом!");
                        continue;
                    }
                    //Добавление семейства в список семейств
                    SelectedProjects.Add(projectFile);
                }
            }
        }

        //Команда для сохранения списка проектов в Json
        public ICommand SaveJsonProjectsCommand { get; set; }
        //Сохранение Json файла списка проектов
        public void SaveJsonProjects(object obj)
        {
            var saveDialog = new System.Windows.Forms.SaveFileDialog();
            saveDialog.Filter = "json files (*.json)|*.json";
            System.Windows.Forms.DialogResult result = saveDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                string jsonFilePath = saveDialog.FileName;
                AddFamiliesToProjectsSettings AddFamiliesToProjectsSettings = new AddFamiliesToProjectsSettings();
                AddFamiliesToProjectsSettings.Save(SelectedProjects, jsonFilePath);
            }
        }

        //Команда для открытия списка проектов в Json
        public ICommand OpenJsonProjectsCommand { get; set; }
        //Открытие Json файла
        private void OpenJsonProjects(object obj)
        {
            var openDialog = new System.Windows.Forms.OpenFileDialog();
            openDialog.Filter = "json files (*.json)|*.json";
            System.Windows.Forms.DialogResult result = openDialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                string jsonFilePath = openDialog.FileName;
                AddFamiliesToProjectsSettings AddFamiliesToProjectsSettings = new AddFamiliesToProjectsSettings();
                SelectedProjects = AddFamiliesToProjectsSettings.GetSettings(jsonFilePath);

                //Проверка в JSON содержатся только файлы проектов
                var extensionFile = !SelectedProjects.All(item => item.FilePath.Contains(".rvt"));
                if (extensionFile)
                {
                    TaskDialog.Show("Revit", $"Элементы не являются файлами проектов!");
                    SelectedProjects.Clear();
                }
                else
                {
                    ObservableCollection<FileItem> tmpAddFamiliesToProjectsFileItemsList = new ObservableCollection<FileItem>(SelectedProjects);
                    foreach (FileItem item in tmpAddFamiliesToProjectsFileItemsList)
                    {
                        if (item == null)
                        {
                            SelectedProjects.Remove(item);
                        }
                    }
                }
            }
        }


        // Коллекция выбранных семейств
        private ObservableCollection<FileItem> selectedFamilies = new ObservableCollection<FileItem>();

        public ObservableCollection<FileItem> SelectedFamilies
        {
            get { return selectedFamilies; }
            set
            {
                selectedFamilies = value;
                OnPropertyChanged(nameof(SelectedFamilies));
            }
        }

        //Команда для выбора семейств
        public ICommand SelectFamilyCommand { get; }

        // Метод для выбора семейств
        private void SelectFamily(object obj)
        {
            // Открытие диалогового окна для выбора файлов
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.DefaultExt = ".rfa";
            openFileDialog.Filter = "rfa files (*.rfa)|*.rfa|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 1;
            openFileDialog.Title = "Выберите необходимые семейства";
            openFileDialog.Multiselect = true; // Разрешение выбора нескольких файлов
            bool? result = openFileDialog.ShowDialog();

            if (result == true)
            {
                // Добавление выбранных файлов в коллекцию
                foreach (string familyName in openFileDialog.FileNames)
                {
                    var familyFile = new FileItem(Path.GetFileName(familyName), familyName);
                    bool containsFile = SelectedFamilies.Any(item => item.FilePath == familyFile.FilePath);
                    //Проверка дублирования семейств в списке
                    if (containsFile)
                    {
                        TaskDialog.Show("Revit", $"Файл \"{familyFile.FileName}\" уже имеется в списке!");
                        continue;
                    }
                    //Проверка расширения файла(.rfa)
                    bool extensionFile = !familyFile.FilePath.Contains(".rfa");
                    if (extensionFile)
                    {
                        TaskDialog.Show("Revit", $"Файл \"{familyFile.FileName}\" не является семейством!");
                        continue;
                    }
                    //Добавление семейства в список семейств
                    SelectedFamilies.Add(familyFile);
                }
            }
        }

        //Команда для выбора семейств
        public ICommand AddFamiliesCommand { get; set; }

        //Данные для загрузки семейства
        Family newFam = null;
        IFamilyLoadOptions familyLoadOptions = new FamilyLoadOptions();

        //Команда для сохранения списка семейств в Json
        public ICommand SaveJsonFamiliesCommand { get; set; }
        //Сохранение Json файла списка семейств
        public void SaveJsonFamilies(object obj)
        {
            var saveDialog = new System.Windows.Forms.SaveFileDialog();
            saveDialog.Filter = "json files (*.json)|*.json";
            System.Windows.Forms.DialogResult result = saveDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                string jsonFilePath = saveDialog.FileName;
                AddFamiliesToProjectsSettings AddFamiliesToProjectsSettings = new AddFamiliesToProjectsSettings();
                AddFamiliesToProjectsSettings.Save(SelectedFamilies, jsonFilePath);
            }
        }

        //Команда для открытия списка файлов в Json
        public ICommand OpenJsonFamiliesCommand { get; set; }
        //Открытие Json файла
        private void OpenJsonFamilies(object obj)
        {
            var openDialog = new System.Windows.Forms.OpenFileDialog();
            openDialog.Filter = "json files (*.json)|*.json";
            System.Windows.Forms.DialogResult result = openDialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                string jsonFilePath = openDialog.FileName;
                AddFamiliesToProjectsSettings AddFamiliesToProjectsSettings = new AddFamiliesToProjectsSettings();
                SelectedFamilies = AddFamiliesToProjectsSettings.GetSettings(jsonFilePath);

                //Проверка в JSON содержатся только файлы семейств
                var extensionFile = !SelectedFamilies.All(item => item.FilePath.Contains(".rfa"));
                if (extensionFile)
                {
                    TaskDialog.Show("Revit", $"Элементы не являются файлами семейств!");
                    SelectedFamilies.Clear();
                }
                else
                {
                    ObservableCollection<FileItem> tmpAddFamiliesToProjectsFileItemsList = new ObservableCollection<FileItem>(SelectedFamilies);
                    foreach (FileItem item in tmpAddFamiliesToProjectsFileItemsList)
                    {
                        if (item == null)
                        {
                            SelectedFamilies.Remove(item);
                        }
                    }
                }
            }
        }


        //Метод добавления семейств в проекты
        public void AddFamilies(object obj)
        {
            var uidocOriginal = RevitAPI.UIDocument;
            using (TransactionGroup group = new TransactionGroup(RevitAPI.Document, "Добавление семейства в проект"))
            {
                group.Start();

                try
                {
                    foreach (var SelectedProject in SelectedProjects)
                    {
                        var docTemp = RevitAPI.UIApplication.Application.OpenDocumentFile(SelectedProject.FilePath); //Открытие проекта из списка
                        var uidocTemp = RevitAPI.UIDocument;

                        using (Transaction transaction = new Transaction(docTemp))
                        {
                            transaction.Start("Добавление семейства в проект");

                            foreach (var SelectedFamily in SelectedFamilies)
                            {
                                docTemp.LoadFamily(SelectedFamily.FilePath, familyLoadOptions, out newFam);
                            }

                            transaction.Commit();
                        }
                        uidocOriginal.RefreshActiveView(); // Активация начального документа, путём обновления активного вида
                        docTemp.Close(); //Закрытие и сохранение документа, в который добавлены семейства
                    }
                }
                catch
                {
                    group.Assimilate();
                }
            }
        }


        // Метод для уведомления об изменении свойства
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
