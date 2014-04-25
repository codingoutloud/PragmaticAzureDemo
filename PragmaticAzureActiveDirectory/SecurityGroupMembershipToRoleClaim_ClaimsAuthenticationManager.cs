using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
// ClaimsAuthenticationManager requires a reference to System.IdentityModel

// Can be hooked up with an entry in Web.config:
// <system.identityModel>
//    <identityConfiguration>
//       <claimsAuthenticationManager type="PragmaticAzureActiveDirectory.SecurityGroupMembershipToRoleClaim_ClaimsAuthenticationManager, PragmaticAzureActiveDirectory" />
// ...
namespace PragmaticAzureActiveDirectory
{
   /// <summary>
   /// Use to intercept authentication pipeline in ASP.NET and inject ActiveDirectory group memberships
   /// as Role claims in Principal's primary (and assumed sole) Identity.
   /// </summary>
   public class SecurityGroupMembershipToRoleClaim_ClaimsAuthenticationManager : ClaimsAuthenticationManager
   {
      public override ClaimsPrincipal Authenticate(string resourceName, ClaimsPrincipal incomingPrincipal)
      {
         var incomingClaimsPrincipal = incomingPrincipal;
         return incomingClaimsPrincipal.AddSecurityGroupsAsRoles();
      }
   }
}
