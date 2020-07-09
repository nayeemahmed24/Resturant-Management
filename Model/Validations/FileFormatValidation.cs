using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Model.Validations
{
    public class FileFormatValidation : ValidationAttribute
    {
        private readonly string[] _extensions;
        public FileFormatValidation(string extensions)
        {
            _extensions = extensions.Split("|");
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }
            var file = value as IFormFile;
            var extension = Path.GetExtension(file.FileName);

            return ValidationResult.Success;
        }
    }
}
