using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitOrthoCamera
{
    [Transaction(TransactionMode.Manual)]
    public class RestoreCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            if (SaveCommand.Scale.Equals(0))
            {
                TaskDialog ts1 = new TaskDialog("Camera parameters")
                {
                    MainContent = "Please save parameters before restoring."
                };

                ts1.Show();
                return Result.Succeeded;
            }

            // Perform restoring
            var orientation = new ViewOrientation3D(SaveCommand.EyePosition, SaveCommand.UpDirection, SaveCommand.ForwardDirection);
            var view3D = doc.ActiveView as View3D;

            if (view3D == null || view3D.IsPerspective)
            {
                TaskDialog ts = new TaskDialog("Incorrect View selected")
                {
                    MainContent = "Please, select 3D Orthographic view."
                };

                ts.Show();
                return Result.Succeeded;
            }

            view3D.SetOrientation(orientation);
            
            double scale = SaveCommand.Scale;

            XYZ corner1 = SaveCommand.EyePosition + SaveCommand.UpDirection * scale - uidoc.ActiveView.RightDirection * scale;
            XYZ corner2 = SaveCommand.EyePosition - SaveCommand.UpDirection * scale + uidoc.ActiveView.RightDirection * scale;

            uidoc.GetOpenUIViews().FirstOrDefault(t => t.ViewId == view3D.Id)?.ZoomAndCenterRectangle(corner2, corner1);
            uidoc.UpdateAllOpenViews();

            TaskDialog ts = new TaskDialog("View restored")
            {
                MainContent = "The camera parameters have been successfully restored."
            };

            ts.Show();            
        }
    }
}
