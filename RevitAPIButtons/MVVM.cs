using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitAPIButtons
{
    public class MVVM
    {
        private ExternalCommandData _commandData;

        public DelegateCommand countPipes { get; }

        public DelegateCommand countDoors { get; }

        public DelegateCommand volumeWalls { get; }

        public MVVM(ExternalCommandData commandData)
        {
            _commandData = commandData;
            countPipes = new DelegateCommand(CountPipes);
            countDoors = new DelegateCommand(CountDoors);
            volumeWalls = new DelegateCommand(VolumeWalls);
        }

        private void VolumeWalls()
        {
            RaiseCloseRequest();
            UIApplication uiapp = _commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            List<Wall> fInstance = new FilteredElementCollector(doc, doc.ActiveView.Id)
                .OfCategory(BuiltInCategory.OST_Walls)
                .WhereElementIsNotElementType()
                .Cast<Wall>()
                .ToList();

            double sumVolume = 0;
            foreach (var wall in fInstance)
            {
                Parameter volumeParameter = wall.LookupParameter("Volume");
                if (volumeParameter.StorageType == StorageType.Double)
                {
                    double volumeValue = UnitUtils.ConvertFromInternalUnits(volumeParameter.AsDouble(), UnitTypeId.CubicMeters);
                    sumVolume += volumeValue;
                }
            }
            TaskDialog.Show("Walls volume", sumVolume.ToString());
        }

        private void CountDoors()
        {
            RaiseCloseRequest();
            UIApplication uiapp = _commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            List<FamilyInstance> fInstance = new FilteredElementCollector(doc, doc.ActiveView.Id)
                .OfCategory(BuiltInCategory.OST_Doors)
                .WhereElementIsNotElementType()
                .Cast<FamilyInstance>()
                .ToList();
            TaskDialog.Show("Doors count", fInstance.Count.ToString());
        }

        public event EventHandler CloseRequest;
        private void RaiseCloseRequest()
        {
            CloseRequest?.Invoke(this, EventArgs.Empty);
        }

        private void CountPipes()
        {
            RaiseCloseRequest();
            UIApplication uiapp = _commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            List<MEPCurve> fInstance = new FilteredElementCollector(doc, doc.ActiveView.Id)
                .OfCategory(BuiltInCategory.OST_DuctCurves)
                .WhereElementIsNotElementType()
                .Cast<MEPCurve>()
                .ToList();
            TaskDialog.Show("Duct count", fInstance.Count.ToString());
        }
    }
}
