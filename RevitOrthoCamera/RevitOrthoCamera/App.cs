using System;
using System.Reflection;
using Autodesk.Revit.UI;

namespace RevitOrthoCamera
{
    class App : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication application)
        {
            AddRibbonPanel(application);
            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication a)
        {
            return Result.Succeeded;
        }

        static void AddRibbonPanel(UIControlledApplication application)
        {
            string tabName = "Ortho Camera Saving/Restoring";
            application.CreateRibbonTab(tabName);

            string thisAssemblyPath = Assembly.GetExecutingAssembly().Location;
            RibbonPanel ribbonPanel = application.CreateRibbonPanel(tabName, "Ortho Camera Saving/Restoring");

            PushButtonData bData0 = new PushButtonData(
                "Btn0",
                "Save Ortho" + Environment.NewLine + "Camera Position",
                thisAssemblyPath,
                "RevitOrthoCamera.SaveCommand");

            PushButtonData bData1 = new PushButtonData(
                "Btn1",
                "Restore Ortho" + Environment.NewLine + "Camera Position",
                thisAssemblyPath,
                "RevitOrthoCamera.RestoreCommand");

            ribbonPanel.AddItem(bData0);
            ribbonPanel.AddItem(bData1);

        }
    }
}