using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace BrainDoseIndices.ViewModels
{
    public class DoseIndicesViewModel : IDisposable
    {
        private CompositeDisposable Disposable { get; } = new CompositeDisposable();

        public List<string> StructureNames { get; set; }

        [Required(ErrorMessage = "Choose Target")]
        public ReactiveProperty<string> TargetRx { get; private set; }
        public ReactiveProperty<string> TargetRxErrorMessage { get; }

        [Required(ErrorMessage = "Choose Dose volume")]
        public ReactiveProperty<string> DoseVolumeRx { get; private set; }
        public ReactiveProperty<string> DoseVolumeRxErrorMessage { get; }

        public ReactiveCommand OkCommand { get; }

        public ReactiveCommand CancelCommand { get; }

        public bool IsOk { get; set; }

        public DoseIndicesViewModel()
        {
            StructureNames = new List<string>{ "PTV", "zDose100", "zDosePrescription" };

            TargetRx = new ReactiveProperty<string>().SetValidateAttribute(() => TargetRx).AddTo(this.Disposable);
            TargetRxErrorMessage = TargetRx.ObserveErrorChanged
            .Select(x => x?.OfType<string>()?.FirstOrDefault())
            .ToReactiveProperty().AddTo(this.Disposable);

            DoseVolumeRx = new ReactiveProperty<string>().SetValidateAttribute(() => DoseVolumeRx).AddTo(this.Disposable);
            DoseVolumeRxErrorMessage = DoseVolumeRx.ObserveErrorChanged
            .Select(x => x?.OfType<string>()?.FirstOrDefault())
            .ToReactiveProperty().AddTo(this.Disposable);

            OkCommand = new[] { TargetRx.ObserveHasErrors, DoseVolumeRx.ObserveHasErrors }.CombineLatest(x => x.All(y => !y)).ToReactiveCommand().AddTo(Disposable);
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
