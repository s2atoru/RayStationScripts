using Prism.Mvvm;
using System.Collections.Generic;
using System.Windows.Media;

namespace RoiManager.Models
{
    public class Roi : BindableBase
    {
        public string CaseName { get; set; }
        public string ExaminationName { get; set; }
        public string Name { get; set; }
        public bool IsDerived { get; set; }
        public bool CanUpdate { get; set; }
        public bool HasGeometry { get; set; }
        public List<string> DependentRois { get; set; } = new List<string>();
        public bool CanUnderive { get; set; }
        public bool CanDeleteGeometry { get; set; }
        public bool CanDeleteRoi { get; set; }
        public bool CanRename { get; set; }
        public string NewName { get; set; }

        public bool CanChangeColor { get; set; }
        public Color Color { get; set; }

        public bool CanChangeRoiType { get; set; }
        public RoiType RoiType { get; set; } = RoiType.Undefined;


        public override string ToString()
        {
            return $"ROI Name: {Name}, HasGeometry: {HasGeometry}, CanUnderive: {CanUnderive}, CanUpdate: {CanUpdate}, CanDeleteGeometry: {CanDeleteGeometry}, CanDeleteRoi: {CanDeleteRoi}"
                + $", IsDerved: {IsDerived}, DependentRois: [{string.Join(",", DependentRois)}], Case: {CaseName}, Examination: {ExaminationName}"
                + $", CanRename: {CanRename}, NewName: {NewName}, CanChageColor: {CanChangeColor}, Color: {Color}"
                + $", CanChageRoiType: {CanChangeRoiType}, RoiType: {RoiType}";
        }
    }

    public enum RoiType
    {
        Control,                  //ROI to be used in control of dose optimization and calculation.
        Ptv,                     //Planning target volume (as defined in ICRU50).
        Ctv,                     //Clinical target volume (as defined in ICRU50).
        Gtv,                     //Gross tumor volume (as defined in ICRU50).
        Organ,                    //Patient organ.
        Avoidance,                //Region in which dose is to be minimized.
        External,                //External patient contour.
        Support,                  //External patient support device.
        TreatedVolume,            //Treated volume (as defined in ICRU50).
        IrradiatedVolume,         //Irradiated Volume (as defined in ICRU50).
        Bolus,                    //Patient bolus to be used for external beam therapy.
        Marker,                   //Patient marker or marker on a localizer.
        Registration,             //Registration ROI
        Isocenter,                //Treatment isocenter to be used for external beam therapy.
        ContrastAgent,            //Volume into which a contrast agent has been injected.
        Cavity,                   //Patient anatomical cavity.
        BrachyChannel,            //Branchy therapy channel
        BrachyAccessory,          //Brachy therapy accessory device.
        BrachySourceApplicator,   //Brachy therapy source applicator.
        BrachyChannelShield,      //Brachy therapy channel shield.
        Fixation,                 //External patient fixation or immobilisation device.
        DoseRegion,               //ROI to be used as a dose reference.
        FieldOfView,              //ROI to be used for representing the Field-of-view in, e.g., a cone beam CT image.
        AcquisitionIsocenter,     //Acquisition isocenter, the position during acquisition.
        InitialLaserIsocenter,    //Initial laser isocenter, the position before acquisition.
        InitialMatchIsocenter,     //Initial match isocenter, the position after acquisition.
        Undefined
    }
}
