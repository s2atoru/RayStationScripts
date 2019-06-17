using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace RoiManager.ViewModels
{
    public class RoiViewModel : BindableBase, IDisposable
    {
        private CompositeDisposable Disposable { get; } = new CompositeDisposable();

        public ReadOnlyReactiveProperty<string> Name { get; }
        public ReadOnlyReactiveProperty<string> IsDerived { get; }
        public ReadOnlyReactiveCollection<string> DependentRois { get; }
        public ReactiveProperty<bool> CanUnderive { get; }
        public ReactiveProperty<bool> CanDeleteGeometry { get; }

        public ReadOnlyReactiveProperty<string> ExaminationName { get; }
        public ReadOnlyReactiveProperty<string> HasGeometry { get; }

        public string DependentRoisDisplay { get { return string.Join(", ", DependentRois); }}

        public RoiViewModel(Models.Roi roi)
        {
            Name = roi.ObserveProperty(x => x.Name).ToReadOnlyReactiveProperty().AddTo(Disposable);
            IsDerived = roi.ObserveProperty(x => x.IsDerived).Select(x => x? "✓" : "").ToReadOnlyReactiveProperty().AddTo(Disposable);
            DependentRois = (new ObservableCollection<string>(roi.DependentRois)).ToReadOnlyReactiveCollection(x => x).AddTo(Disposable);

            CanUnderive = roi.ToReactivePropertyAsSynchronized(x => x.CanUnderive).AddTo(Disposable);
            CanDeleteGeometry = roi.ToReactivePropertyAsSynchronized(x => x.CanDeleteGeometry).AddTo(Disposable);

            ExaminationName = roi.ObserveProperty(x => x.ExaminationName).ToReadOnlyReactiveProperty().AddTo(Disposable);
            HasGeometry = roi.ObserveProperty(x => x.HasGeometry).Select(x => x ? "✓" : "").ToReadOnlyReactiveProperty().AddTo(Disposable);
        }

        void IDisposable.Dispose()
        {
            Disposable.Dispose();
        }
    }
}
