using Prism.Mvvm;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;

namespace OptimizatoinSettings.ViewModels
{
    public class SetParametersViewModel : BindableBase, INotifyDataErrorInfo
    {
        private Models.SettingParameters settingParameters;
        public Models.SettingParameters SettingParameters
        {
            get { return settingParameters; }
            set
            {
                settingParameters = value;
                MaxNumberOfIterations = settingParameters.MaxNumberOfIterations.ToString();
                IterationsInPreparationsPhase = settingParameters.IterationsInPreparationsPhase.ToString();
                ComputeFinalDose = settingParameters.ComputeFinalDose;
                CanOk = settingParameters.IsValid;
            }
        }

        private bool canOk;
        public bool CanOk
        {
            get { return canOk; }
            set { SetProperty(ref canOk, value); }
        }

        private string maxNumberOfIterations = "40";
        [Required(ErrorMessage = "Required")]
        [RegularExpression(@"([1-9]\d*)", ErrorMessage = "Input integer ≧ 1")]
        [CustomValidation(typeof(SetParametersViewModel), "MaxNumberOfIterationsConstraint")]
        public string MaxNumberOfIterations
        {
            get { return maxNumberOfIterations; }
            set
            {
                SetProperty(ref maxNumberOfIterations, value);
                if (!this.HasErrors)
                {
                    SettingParameters.MaxNumberOfIterations = int.Parse(value);
                    SettingParameters.IterationsInPreparationsPhase = int.Parse(IterationsInPreparationsPhase);
                    CanOk = true;
                }
                else
                {
                    CanOk = false;
                }
            }
        }

        private string iterationsInPreparationsPhase = "20";
        [Required(ErrorMessage = "Required")]
        [RegularExpression(@"([1-9]\d*|0)", ErrorMessage = "Input integer ≧ 0")]
        public string IterationsInPreparationsPhase
        {
            get { return iterationsInPreparationsPhase; }
            set
            {
                SetProperty(ref iterationsInPreparationsPhase, value);
                this.ValidateProperty(MaxNumberOfIterations, nameof(MaxNumberOfIterations));
                if (!this.HasErrors)
                {
                    SettingParameters.IterationsInPreparationsPhase = int.Parse(value);
                    SettingParameters.MaxNumberOfIterations = int.Parse(MaxNumberOfIterations);
                    CanOk = true;
                }
                else
                {
                    CanOk = false;
                }
            }
        }

        private bool computeFinalDose;
        public bool ComputeFinalDose
        {
            get { return computeFinalDose; }
            set
            {
                SetProperty(ref computeFinalDose, value);
                SettingParameters.ComputeFinalDose = computeFinalDose;
            }
        }

        //private bool computeFinalDose = true;
        //public bool ComputeFinalDose
        //{
        //    get { return computeFinalDose; }
        //    set
        //    {
        //        computeFinalDose = value;
        //        SettingParameters.ComputeFinalDose = computeFinalDose;
        //    }
        //}

        public SetParametersViewModel()
        {
            this.ErrorsContainer = new ErrorsContainer<string>(
                x => this.ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(x)));

            SettingParameters = new Models.SettingParameters();

            CanOk = SettingParameters.IsValid;
        }

        public static ValidationResult MaxNumberOfIterationsConstraint(string value, ValidationContext context)
        {
            var obj = context.ObjectInstance as SetParametersViewModel;
            if (obj != null)
            {

                if (!(int.TryParse(obj.MaxNumberOfIterations, out int m)))
                {
                    var msg = "Max number of iterations should be integer";
                    return new ValidationResult(msg);
                }

                if (!(int.TryParse(obj.IterationsInPreparationsPhase, out int i)))
                {
                    var msg = "Iterations before conversion should be integer";
                    return new ValidationResult(msg);
                }

                if (m < i)
                {
                    var msg = $"Max number of iterations ≧ Iterations before conversion ({i})";
                    return new ValidationResult(msg);
                }
            }
            return ValidationResult.Success;
        }

        #region Validation
        private ErrorsContainer<string> ErrorsContainer { get; }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        protected void ValidateProperty(object value, [CallerMemberName]string propertyName = null)
        {
            var context = new ValidationContext(this)
            {
                MemberName = propertyName
            };

            var errors = new List<ValidationResult>();
            if (!Validator.TryValidateProperty(value, context, errors))
            {
                this.ErrorsContainer.SetErrors(propertyName, errors.Select(x => x.ErrorMessage));
            }
            else
            {
                this.ErrorsContainer.ClearErrors(propertyName);
            }
        }

        protected override bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (!base.SetProperty<T>(ref storage, value, propertyName))
            {
                return false;
            }

            ValidateProperty(value, propertyName);

            return true;
        }

        public bool HasErrors
        {
            get
            {
                return this.ErrorsContainer.HasErrors;
            }
        }

        public IEnumerable GetErrors(string propertyName)
        {
            return this.ErrorsContainer.GetErrors(propertyName);
        }
        #endregion
    }
}
