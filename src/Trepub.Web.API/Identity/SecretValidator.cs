using IdentityServer4.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;

namespace Trepub.Web.API.Identity
{
    public class SecretValidator : ISecretValidator
    {
        public Task<SecretValidationResult> ValidateAsync(IEnumerable<Secret> secrets, ParsedSecret parsedSecret)
        {
            //parsedSecret.
            //IdentityServer4.Validation.ClientSecretValidator
            var result = new SecretValidationResult { Success = false };
            return Task.FromResult(result);
        }
    }
}
