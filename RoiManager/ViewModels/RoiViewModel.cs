using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;

namespace RoiManager.ViewModels
{
    public class RoiViewModel : BindableBase, IDisposable
    {
        private CompositeDisposable Disposable { get; } = new CompositeDisposable();

        public ReadOnlyReactiveProperty<string> Name { get; }
        public ReadOnlyReactiveProperty<bool> IsDerived { get; }
        public ReadOnlyReactiveCollection<string> DependentRois { get; }
        public ReactiveProperty<bool> CanUnderive { get; }
        public ReactiveProperty<bool> CanDeleteGeometry { get; }

        

        public RoiViewModel(Models.Roi roi)
        {
            Name = roi.ObserveProperty(x => x.Name).ToReadOnlyReactiveProperty().AddTo(Disposable);
            IsDerived = roi.ObserveProperty(x => x.IsDerived).ToReadOnlyReactiveProperty().AddTo(Disposable);
            DependentRois = (new ObservableCollection<string>(roi.DependentRois)).ToReadOnlyReactiveCollection(x => x);
            CanUnderive = roi.ToReactivePropertyAsSynchronized(x => x.CanUnderive).AddTo(Disposable);
            CanDeleteGeometry = roi.ToReactivePropertyAsSynchronized(x => x.CanDeleteGeometry).AddTo(Disposable);
        }

        void IDisposable.Dispose()
        {
            Disposable.Dispose();
        }
    }
}
