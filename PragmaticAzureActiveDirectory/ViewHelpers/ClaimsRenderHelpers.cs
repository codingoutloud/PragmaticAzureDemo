using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PragmaticAzureActiveDirectory.ViewHelpers
{
    public static class ClaimsRenderHelpers
    {
        /// <summary>
        /// input: http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier
        /// output: http://schemas.xmlsoap.org/ws/2005/05/identity/claims/<mark>nameidentifier</mark>
        /// </summary>
        /// <param name="claimType"></param>
        /// <returns></returns>
        public static string RenderClaimTypeBold(string claimType)
        {
            var indexOfLastFragment = claimType.LastIndexOf('/') + 1;
            return String.Format("{0}/<mark>{1}</mark>", 
                claimType.Substring(0, indexOfLastFragment - 1), 
                claimType.Substring(indexOfLastFragment));
        }
    }
}
