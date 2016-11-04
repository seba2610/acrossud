using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;

namespace Acrossud
{
    public class CustomPasswordValidator : IIdentityValidator<string>
    {
        public int RequiredLength { get; set; }
        public CustomPasswordValidator()
        {

        }

        public Task<IdentityResult> ValidateAsync(string item)
        {
            if (String.IsNullOrEmpty(item) || item.Length < RequiredLength)
            {
                return Task.FromResult(IdentityResult.Failed(String.Format("La contraseña debe ser de al menos {0} carácteres de largo.", RequiredLength)));
            }

            var any_letter = item.Any(char.IsLetter);
            var any_numbers = item.Any(char.IsNumber);

            if (!any_letter || !any_numbers)
            {
                return Task.FromResult(IdentityResult.Failed("La contraseña debe tener al menos un carácter y un número."));
            }

            return Task.FromResult(IdentityResult.Success);
        }
    }
}