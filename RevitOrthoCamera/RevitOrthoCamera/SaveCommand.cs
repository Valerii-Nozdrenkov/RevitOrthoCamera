using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace RevitOrthoCamera
{
    [Transaction(TransactionMode.Manual)]
    public class SaveCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //set up document space
            ///each allows you access to seperate methods of RevitAPI
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;
            //Transactions-
            //Transactions must be initiatied to make any changes to Revit Period.
            //Usings
            //Wrapping transactions in a using statment is also good practice.
            //This allows for them to be released by the code when finished.
            //It will do this automatically if you choose not but isnt a good habit.
            using (Transaction Trans = new Transaction(doc))
            {
                //starts the transcation
                Trans.Start("Temp Trans");
                {
                    //try changes and return sucess if completed
                    //this prevents your code crashing on erros.
                    //always use try catch statments.
                    try
                    {
                        // prompt user for element
                        Reference eref = uidoc.Selection.PickObject(ObjectType.Element, "Select an object");
                        //create a quick prompt
                        TaskDialog ts = new TaskDialog("Test Dialogue");
                        //tell it to show the selected elements name
                        ts.MainContent = doc.GetElement(eref).Name;
                        //display the dialog
                        ts.Show();
                        //commit the changes
                        Trans.Commit();
                        //return success
                        return Result.Succeeded;
                    }
                    //catch failures
                    catch (System.Exception)
                    {
                        //undo changes
                        Trans.RollBack();
                        //return failed
                        return Result.Failed;
                    }
                }
            }
        }
    }
}
