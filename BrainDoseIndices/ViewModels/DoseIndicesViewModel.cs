using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;

using BrainDoseIndices.Models;

namespace BrainDoseIndices.ViewModels
{
    public class DoseIndicesViewModel : IDisposable
    {
        private CompositeDisposable Disposable { get; } = new CompositeDisposable();

        public List<string> StructureNames { get; set; } = new List<string>();

        public List<StructureDetail> StructureDetails { get; set; } = new List<StructureDetail>();

        public string DefaultTargetName { get; set; }
        public string DefaultDose100VolumeName { get; set; }
        public string DefaultDose50VolumeName { get; set; }
        public string DefaultOverlapTargetDose100VolumeName { get; set; }

        [Required(ErrorMessage = "Choose Target")]
        public ReactiveProperty<string> TargetRx { get; private set; }
        public ReactiveProperty<string> TargetRxErrorMessage { get; }

        [Required(ErrorMessage = "Choose 100% Dose volume")]
        public ReactiveProperty<string> Dose100VolumeRx { get; private set; }
        public ReactiveProperty<string> Dose100VolumeRxErrorMessage { get; }

        [Required(ErrorMessage = "Choose 50% Dose volume")]
        public ReactiveProperty<string> Dose50VolumeRx { get; private set; }
        public ReactiveProperty<string> Dose50VolumeRxErrorMessage { get; }

        [Required(ErrorMessage = "Choose Target-100%-Dose Overlap volume")]
        public ReactiveProperty<string> OverlapTargetDose100VolumeRx { get; private set; }
        public ReactiveProperty<string> OverlapTargetDose100VolumeRxErrorMessage { get; }

        public ReactiveProperty<string> TargetVolumeValueRx { get; } = new ReactiveProperty<string>("");
        public ReactiveProperty<string> Dose100VolumeValueRx { get; } = new ReactiveProperty<string>("");
        public ReactiveProperty<string> Dose50VolumeValueRx { get; } = new ReactiveProperty<string>("");
        public ReactiveProperty<string> OverlapTargetDose100VolumeValueRx { get; } = new ReactiveProperty<string>("");

        public ReactiveProperty<string> GradientIndex100Rx { get; } = new ReactiveProperty<string>("");
        public ReactiveProperty<string> GradientIndexTargetRx { get; } = new ReactiveProperty<string>("");
        public ReactiveProperty<string> PaddickConformityIndexRx { get; } = new ReactiveProperty<string>("");

        public ReactiveCommand OkCommand { get; }

        public ReactiveCommand CancelCommand { get; }

        public bool IsOk { get; set; }

        public DoseIndicesViewModel()
        {
            StructureNames = new List<string>{ "PTV", "zDose100", "zDosePrescription", "zDose50" };

            StructureDetails = new List<StructureDetail> { new StructureDetail { Name="PTV", Volume=50},
             new StructureDetail { Name="zDose100", Volume=80},
             new StructureDetail { Name="zDose50", Volume=100},
             new StructureDetail { Name="zDosePrescription", Volume=90 } };

            TargetRx = new ReactiveProperty<string>().SetValidateAttribute(() => TargetRx).AddTo(this.Disposable);
            TargetRxErrorMessage = TargetRx.ObserveErrorChanged
            .Select(x => x?.OfType<string>()?.FirstOrDefault())
            .ToReactiveProperty().AddTo(this.Disposable);

            Dose100VolumeRx = new ReactiveProperty<string>().SetValidateAttribute(() => Dose100VolumeRx).AddTo(this.Disposable);
            Dose100VolumeRxErrorMessage = Dose100VolumeRx.ObserveErrorChanged
            .Select(x => x?.OfType<string>()?.FirstOrDefault())
            .ToReactiveProperty().AddTo(this.Disposable);

            Dose50VolumeRx = new ReactiveProperty<string>().SetValidateAttribute(() => Dose50VolumeRx).AddTo(this.Disposable);
            Dose50VolumeRxErrorMessage = Dose50VolumeRx.ObserveErrorChanged
            .Select(x => x?.OfType<string>()?.FirstOrDefault())
            .ToReactiveProperty().AddTo(this.Disposable);

            OverlapTargetDose100VolumeRx = new ReactiveProperty<string>().SetValidateAttribute(() => OverlapTargetDose100VolumeRx).AddTo(this.Disposable);
            OverlapTargetDose100VolumeRxErrorMessage = OverlapTargetDose100VolumeRx.ObserveErrorChanged
            .Select(x => x?.OfType<string>()?.FirstOrDefault())
            .ToReactiveProperty().AddTo(this.Disposable);

            new[]
            {
                this.Dose50VolumeRx.ObserveHasErrors,
                this.TargetRx.ObserveHasErrors
            }
            .CombineLatest(x => x.All(y => !y))
            .Where(x => x)
            .Subscribe(_ =>
            {
                var dose50Volume = StructureDetails.Where(s => s.Name == Dose50VolumeRx.Value).Select(s => s.Volume).Single();
                var targetVolume = StructureDetails.Where(s => s.Name == TargetRx.Value).Select(s => s.Volume).Single();

                Dose50VolumeValueRx.Value = dose50Volume.ToString("0.00");
                TargetVolumeValueRx.Value = targetVolume.ToString("0.00");

                GradientIndexTargetRx.Value = (dose50Volume / targetVolume).ToString("0.000");
            });

            new[]
            {
                this.Dose50VolumeRx.ObserveHasErrors,
                this.Dose100VolumeRx.ObserveHasErrors
            }
            .CombineLatest(x => x.All(y => !y))
            .Where(x => x)
            .Subscribe(_ =>
            {
                var dose50Volume = StructureDetails.Where(s => s.Name == Dose50VolumeRx.Value).Select(s => s.Volume).Single();
                var dose100Volume = StructureDetails.Where(s => s.Name == Dose100VolumeRx.Value).Select(s => s.Volume).Single();

                Dose50VolumeValueRx.Value = dose50Volume.ToString("0.00");
                Dose100VolumeValueRx.Value = dose100Volume.ToString("0.00");

                GradientIndex100Rx.Value = (dose50Volume / dose100Volume).ToString("0.000");
            });

            new[]
            {
                this.Dose100VolumeRx.ObserveHasErrors,
                this.TargetRx.ObserveHasErrors,
                this.OverlapTargetDose100VolumeRx.ObserveHasErrors
            }
            .CombineLatest(x => x.All(y => !y))
            .Where(x => x)
            .Subscribe(_ =>
            {
                var dose100Volume = StructureDetails.Where(s => s.Name == Dose100VolumeRx.Value).Select(s => s.Volume).Single();
                var targetVolume = StructureDetails.Where(s => s.Name == TargetRx.Value).Select(s => s.Volume).Single();
                var overlapVolume = StructureDetails.Where(s => s.Name == OverlapTargetDose100VolumeRx.Value).Select(s => s.Volume).Single();

                Dose100VolumeValueRx.Value = dose100Volume.ToString("0.00");
                TargetVolumeValueRx.Value = targetVolume.ToString("0.00");
                OverlapTargetDose100VolumeValueRx.Value = overlapVolume.ToString("0.00");

                PaddickConformityIndexRx.Value = ((overlapVolume*overlapVolume )/ (targetVolume*dose100Volume)).ToString("0.000");
            });

            OkCommand = new[] { TargetRx.ObserveHasErrors, Dose100VolumeRx.ObserveHasErrors }.CombineLatest(x => x.All(y => !y)).ToReactiveCommand().AddTo(Disposable);
            OkCommand.Subscribe(() => IsOk = true).AddTo(Disposable);

            CancelCommand = new ReactiveCommand();
            CancelCommand.Subscribe(() => IsOk = false).AddTo(Disposable);
        }

        public DoseIndicesViewModel(List<StructureDetail> structureDetails,
            string defautTargetName= "PTV", string defaultDose100VolumeName="zDose100", string defaultDose50VolumeName="zDose50", string defaultOverlapTargetDose100VolumeName="zOL_PTV_Dose100")
        {
            DefaultTargetName = defautTargetName;
            DefaultDose100VolumeName = defaultDose100VolumeName;
            DefaultDose50VolumeName = defaultDose50VolumeName;
            DefaultOverlapTargetDose100VolumeName = defaultOverlapTargetDose100VolumeName;

            StructureDetails = structureDetails;
            StructureNames = structureDetails.Select(s => s.Name).ToList();

            TargetRx = new ReactiveProperty<string>().SetValidateAttribute(() => TargetRx).AddTo(this.Disposable);
            TargetRxErrorMessage = TargetRx.ObserveErrorChanged
            .Select(x => x?.OfType<string>()?.FirstOrDefault())
            .ToReactiveProperty().AddTo(this.Disposable);

            if (StructureDetails.Exists(s => s.Name == DefaultTargetName))
            {
                TargetRx.Value = DefaultTargetName;
            }

            Dose100VolumeRx = new ReactiveProperty<string>().SetValidateAttribute(() => Dose100VolumeRx).AddTo(this.Disposable);
            Dose100VolumeRxErrorMessage = Dose100VolumeRx.ObserveErrorChanged
            .Select(x => x?.OfType<string>()?.FirstOrDefault())
            .ToReactiveProperty().AddTo(this.Disposable);

            if (StructureDetails.Exists(s => s.Name == DefaultDose100VolumeName))
            {
                Dose100VolumeRx.Value = DefaultDose100VolumeName;
            }

            Dose50VolumeRx = new ReactiveProperty<string>().SetValidateAttribute(() => Dose50VolumeRx).AddTo(this.Disposable);
            Dose50VolumeRxErrorMessage = Dose50VolumeRx.ObserveErrorChanged
            .Select(x => x?.OfType<string>()?.FirstOrDefault())
            .ToReactiveProperty().AddTo(this.Disposable);

            if (StructureDetails.Exists(s => s.Name == DefaultDose50VolumeName))
            {
                Dose50VolumeRx.Value = DefaultDose50VolumeName;
            }

            OverlapTargetDose100VolumeRx = new ReactiveProperty<string>().SetValidateAttribute(() => OverlapTargetDose100VolumeRx).AddTo(this.Disposable);
            OverlapTargetDose100VolumeRxErrorMessage = OverlapTargetDose100VolumeRx.ObserveErrorChanged
            .Select(x => x?.OfType<string>()?.FirstOrDefault())
            .ToReactiveProperty().AddTo(this.Disposable);

            if (StructureDetails.Exists(s => s.Name == DefaultOverlapTargetDose100VolumeName))
            {
                OverlapTargetDose100VolumeRx.Value = DefaultOverlapTargetDose100VolumeName;
            }


            new[]
            {
                this.Dose50VolumeRx.ObserveHasErrors,
                this.TargetRx.ObserveHasErrors
            }
            .CombineLatest(x => x.All(y => !y))
            .Where(x => x)
            .Subscribe(_ =>
            {
                var dose50Volume = StructureDetails.Where(s => s.Name == Dose50VolumeRx.Value).Select(s => s.Volume).Single();
                var targetVolume = StructureDetails.Where(s => s.Name == TargetRx.Value).Select(s => s.Volume).Single();

                Dose50VolumeValueRx.Value = dose50Volume.ToString("0.00");
                TargetVolumeValueRx.Value = targetVolume.ToString("0.00");

                GradientIndexTargetRx.Value = (dose50Volume / targetVolume).ToString("0.000");
            });

            new[]
            {
                this.Dose50VolumeRx.ObserveHasErrors,
                this.Dose100VolumeRx.ObserveHasErrors
            }
            .CombineLatest(x => x.All(y => !y))
            .Where(x => x)
            .Subscribe(_ =>
            {
                var dose50Volume = StructureDetails.Where(s => s.Name == Dose50VolumeRx.Value).Select(s => s.Volume).Single();
                var dose100Volume = StructureDetails.Where(s => s.Name == Dose100VolumeRx.Value).Select(s => s.Volume).Single();

                Dose50VolumeValueRx.Value = dose50Volume.ToString("0.00");
                Dose100VolumeValueRx.Value = dose100Volume.ToString("0.00");

                GradientIndex100Rx.Value = (dose50Volume / dose100Volume).ToString("0.000");
            });

            new[]
            {
                this.Dose100VolumeRx.ObserveHasErrors,
                this.TargetRx.ObserveHasErrors,
                this.OverlapTargetDose100VolumeRx.ObserveHasErrors
            }
            .CombineLatest(x => x.All(y => !y))
            .Where(x => x)
            .Subscribe(_ =>
            {
                var dose100Volume = StructureDetails.Where(s => s.Name == Dose100VolumeRx.Value).Select(s => s.Volume).Single();
                var targetVolume = StructureDetails.Where(s => s.Name == TargetRx.Value).Select(s => s.Volume).Single();
                var overlapVolume = StructureDetails.Where(s => s.Name == OverlapTargetDose100VolumeRx.Value).Select(s => s.Volume).Single();

                Dose100VolumeValueRx.Value = dose100Volume.ToString("0.00");
                TargetVolumeValueRx.Value = targetVolume.ToString("0.00");
                OverlapTargetDose100VolumeValueRx.Value = overlapVolume.ToString("0.00");

               PaddickConformityIndexRx.Value = ((overlapVolume * overlapVolume) / (targetVolume * dose100Volume)).ToString("0.000");
            });

            OkCommand = new[] { TargetRx.ObserveHasErrors, Dose50VolumeRx.ObserveHasErrors, Dose100VolumeRx.ObserveHasErrors, OverlapTargetDose100VolumeRx.ObserveHasErrors }.CombineLatest(x => x.All(y => !y)).ToReactiveCommand().AddTo(Disposable);
            OkCommand.Subscribe(() => IsOk = true).AddTo(Disposable);

            CancelCommand = new ReactiveCommand();
            CancelCommand.Subscribe(() => IsOk = false).AddTo(Disposable);
        }

        public void Dispose()
        {
            this.Disposable.Dispose();
        }
    }
}
