using EMCR.DRR.Controllers;
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace EMCR.DRR.API.Utilities.Extensions
{
#pragma warning disable CS8765 // nullability
#pragma warning disable CS8603 // Possible null reference return.
#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
    public class Mandatory : ValidationAttribute
    {
        RequiredAttribute _innerAttribute = new RequiredAttribute();
        public Type _mandatoryClass { get; set; }

        public Mandatory(Type mandatoryClass)
        {
            _mandatoryClass = mandatoryClass;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = validationContext.ObjectInstance;

            if (model.GetType() == _mandatoryClass && value == null)
            {
                var propertyInfo = validationContext.ObjectType.GetProperty(validationContext.MemberName);
                return new ValidationResult($"{propertyInfo.Name} is required");
            }
            return ValidationResult.Success;
        }
    }

    public class CurrencyNotNegativeForSubmission : ValidationAttribute
    {
        RequiredAttribute _innerAttribute = new RequiredAttribute();
        public Type _mandatoryClass { get; set; }

        public CurrencyNotNegativeForSubmission(Type mandatoryClass)
        {
            _mandatoryClass = mandatoryClass;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = validationContext.ObjectInstance;

            if (model.GetType() == _mandatoryClass)
            {
                var propertyInfo = validationContext.ObjectType.GetProperty(validationContext.MemberName);
                if (value == null) return ValidationResult.Success;
                decimal dec = Convert.ToDecimal(value);
                if (dec < 0) return new ValidationResult($"{propertyInfo.Name} cannot be negative");
            }
            return ValidationResult.Success;
        }
    }
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable IDE0300 // Simplify collection initialization
#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
    public class MandatoryIf : ValidationAttribute
    {
        RequiredAttribute _innerAttribute = new RequiredAttribute();
        public Type _mandatoryClass { get; set; }
        public string _dependentProperty { get; set; }
        public object _targetValue { get; set; }

        public MandatoryIf(Type mandatoryClass, string dependentProperty, object targetValue)
        {
            _mandatoryClass = mandatoryClass;
            _dependentProperty = dependentProperty;
            _targetValue = targetValue;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = validationContext.ObjectInstance;

            if (model.GetType() == _mandatoryClass && value == null)
            {

                var field = validationContext.ObjectType.GetProperty(_dependentProperty);
                if (field != null)
                {
                    var dependentValue = field.GetValue(validationContext.ObjectInstance, null);
                    if ((dependentValue == null && _targetValue == null) || (dependentValue != null && dependentValue.Equals(_targetValue)))
                    {
                        if (!_innerAttribute.IsValid(value))
                        {
                            string name = validationContext.DisplayName;
                            string specificErrorMessage = ErrorMessage;
                            if (specificErrorMessage != null && specificErrorMessage.Length < 1)
                                specificErrorMessage = $"{name} is required.";

                            return new ValidationResult(specificErrorMessage, new[] { validationContext.MemberName });
                        }
                    }
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult(FormatErrorMessage(_dependentProperty));
                }
            }
            return ValidationResult.Success;
        }
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class MandatoryIfAny : ValidationAttribute
    {
        public Type _mandatoryClass { get; set; }
        public string[] Values { get; set; }
        public string PropertyName { get; set; }

        public MandatoryIfAny(Type mandatoryClass)
        {
            _mandatoryClass = mandatoryClass;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = validationContext.ObjectInstance;
            if (model == null || Values == null)
            {
                return ValidationResult.Success;
            }

            if (model.GetType() == _mandatoryClass && value == null)
            {
                var currentValue = model.GetType().GetProperty(PropertyName)?.GetValue(model, null)?.ToString();
                if (Values.Contains(currentValue) && value == null)
                {
                    var propertyInfo = validationContext.ObjectType.GetProperty(validationContext.MemberName);
                    return new ValidationResult($"{propertyInfo.Name} is required for the current {PropertyName} value {currentValue}");
                }
                return ValidationResult.Success;
            }
            return ValidationResult.Success;
        }
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public class CollectionStringLengthValid : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (!(value is IList)) return false;
            foreach (string item in (IList)value)
            {
                if (item?.Length > ApplicationValidators.ACCOUNT_MAX_LENGTH) return false;
            }
            return true;
        }
    }
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning restore CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
#pragma warning restore IDE0300 // Simplify collection initialization
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning restore CS8604 // Possible null reference argument.
#pragma warning restore CS8603 // Possible null reference return.
#pragma warning restore CS8765
}
