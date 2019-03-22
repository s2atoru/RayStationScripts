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
        public Models.SettingParameters SettingParameters { get; set; }

        private bool canOk;
        public bool CanOk
        {
            get { return canOk; }
            set { SetProperty(ref canOk, value); }
        }

        private string maximumNumberOfIterations = "40";
        [Required(ErrorMessage = "Required")]
        [RegularExpression(@"([1-9]\d*)", ErrorMessage = "Input integer ≧ 1")]
        [CustomValidation(typeof(SetParametersViewModel), "MaximumNumberOfIterationsConstraint")]
        public string MaximumNumberOfIterations
        {
            get { return maximumNumberOfIterations; }
            set
            {
                SetProperty(ref maximumNumberOfIterations, value);
                if (!this.HasErrors)
                {
                    SettingParameters.MaximumNumberOfIterations = int.Parse(value);
                    CanOk = true;
                }
                else
                {
                    CanOk = false;
                }
            }
        }

        private string initialNumberOfIterations = "20";
        [Required(ErrorMessage = "Required")]
        [RegularExpression(@"([1-9]\d*|0)", ErrorMessage = "Input integer ≧ 0")]
        public string InitialNumberOfIterations
        {
            get { return initialNumberOfIterations; }
            set
            {
                SetProperty(ref initialNumberOfIterations, value);
                this.ValidateProperty(MaximumNumberOfIterations, nameof(MaximumNumberOfIterations));
                if (!this.HasErrors)
                {
                    SettingParameters.InitialNumberOfIterations = int.Parse(value);
                    CanOk = true;
                }
                else
                {
                    CanOk = false;
                }
            }
        }

        public bool FinalDoseCalculation { get; set; }

        public SetParametersViewModel()
        {
            this.ErrorsContainer = new ErrorsContainer<string>(
                x => this.ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(x)));

            SettingParameters = new Models.SettingParameters();

            CanOk = SettingParameters.IsValid;
        }

        public static ValidationResult MaximumNumberOfIterationsConstraint(string value, ValidationContext context)
        {
            var obj = context.ObjectInstance as SetParametersViewModel;
            if (obj != null)
            {
                int maximumNumberOfIterations = 0;
                int initialNumberOfIterations = 0;

                if (!(int.TryParse(obj.MaximumNumberOfIterations, out maximumNumberOfIterations)))
                {
                    var msg = "Maximum number of iterations should be integer";
                    return new ValidationResult(msg);
                }

                if (!(int.TryParse(obj.InitialNumberOfIterations, out initialNumberOfIterations)))
                {
                    var msg = "Initial number of iterations should be integer";
                    return new ValidationResult(msg);
                }

                if (maximumNumberOfIterations < initialNumberOfIterations)
                {
                    var msg = $"Maximum number of iterations ≧ Initial number of iterations ({initialNumberOfIterations})";
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
