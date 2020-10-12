using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitOrthoCamera
{
    [Transaction(TransactionMode.Manual)]
    public class SaveCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            // Save current 3D view
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

            // Get viewOrientation3D
            ViewOrientation3D viewOrientation3D = view3D.GetOrientation();
            ForwardDirection = viewOrientation3D.ForwardDirection;
            UpDirection = viewOrientation3D.UpDirection;
            var rightDirection = ForwardDirection.CrossProduct(UpDirection);
            EyePosition = viewOrientation3D.EyePosition;

            IList<UIView> views = uidoc.GetOpenUIViews();
            UIView currentView = views.FirstOrDefault(t => t.ViewId == view3D.Id);

            //Corners of the active UI view
            if (currentView == null)
            {
                return Result.Succeeded;
            }

            IList<XYZ> corners = currentView.GetZoomCorners();
            XYZ corner1 = corners[0];
            XYZ corner2 = corners[1];

            //center of the UI view
            double x = (corner1.X + corner2.X) / 2;
            double y = (corner1.Y + corner2.Y) / 2;
            double z = (corner1.Z + corner2.Z) / 2;
            XYZ viewCenter = new XYZ(x, y, z);
            EyePosition = viewCenter;

            // Calculate diagonal vector
            XYZ diagVector = corner1 - corner2;

            double height = 0.5 * Math.Abs(diagVector.DotProduct(UpDirection));
            double width = 0.5 * Math.Abs(diagVector.DotProduct(rightDirection));
            Scale = Math.Min(height, width);

            TaskDialog ts1 = new TaskDialog("Camera parameters")
            {
                MainContent = "Camera parameters have been successfully saved."
            };

            ts1.Show();
            return Result.Succeeded;
        }

        // Static variables were used to save Orthographic camera parameters.
        // These parameters can be serialize and uploaded to server.
        public static double Scale { get; private set; }

        public static XYZ EyePosition { get; private set; }

        public static XYZ UpDirection { get; private set; }

        public static XYZ ForwardDirection { get; private set; }
    }
}
