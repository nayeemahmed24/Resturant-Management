using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Model.Validations
{
    public class FileSizeValidation : ValidationAttribute
    {
        private readonly int _maxSize;

        public FileSizeValidation(int maxSize)
        {
            _maxSize = maxSize * 1024 * 1024;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }
            else
            {
                var file = value as IFormFile;
                var fileLength = file.Length;

                if (fileLength > _maxSize)
                {
                    return new ValidationResult(this.ErrorMessage);
                }
            }



            return ValidationResult.Success;
        }
    }
}
