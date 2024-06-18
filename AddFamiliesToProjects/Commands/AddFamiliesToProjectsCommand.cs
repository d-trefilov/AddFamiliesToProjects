using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using AddFamiliesToProjects.Utilities;
using AddFamiliesToProjects.Views;
using AddFamiliesToProjects.ViewModels;


namespace AddFamiliesToProjects.Commands
{
    [Regeneration(RegenerationOption.Manual)]
    [Transaction(TransactionMode.Manual)]

    //Основная логика плагина
    public class AddFamiliesToProjectsCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            if (RevitAPI.UIApplication == null)
            {
                RevitAPI.Initialize(commandData);
            }

            AddFamiliesToProjectsViewModel vm = new AddFamiliesToProjectsViewModel();
            AddFamiliesToProjectsView wnd = new AddFamiliesToProjectsView();

            wnd.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            wnd.ShowDialog();

            return Result.Succeeded;
        }
    }
}
