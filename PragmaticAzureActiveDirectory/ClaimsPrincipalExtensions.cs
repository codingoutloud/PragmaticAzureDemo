using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Diagnostics;
using Microsoft.Azure.ActiveDirectory.GraphClient;     // NuGet: Microsoft Azure ActiveDirectory Graph API Client library, id="Microsoft.Azure.ActiveDirectory.GraphClient"
using Microsoft.IdentityModel.Clients.ActiveDirectory; // NuGet: Active Directory Authentication Library (ADAL), id="Microsoft.IdentityModel.Clients.ActiveDirectory"

namespace PragmaticAzureActiveDirectory
{
    public static class ClaimsPrincipalExtensions
    {
        public const string GraphUrl = "https://graph.windows.net";
        public const string GraphApiVersion = "2013-11-08";
        public const string AddedRoleMembershipOriginalIssuer = "Security Group Membership";
        public const string AadObjectIdClaimType = "http://schemas.microsoft.com/identity/claims/objectidentifier";

        private const string AadAuthorityUrlTemplate = "https://login.windows.net/{0}";

        public static ClaimsPrincipal AddSecurityGroupsAsRoles(this ClaimsPrincipal principal)
        {
            Debug.Assert(principal.Identities.Count() == 1); // todo: Code Contract
            var ci = (ClaimsIdentity)principal.Identity;

            var groups = principal.GetGroups();
            var roleClaims = groups.Select(group => new Claim(ClaimTypes.Role, group, ClaimValueTypes.String, null, AddedRoleMembershipOriginalIssuer)).ToList();
            ci.AddClaims(roleClaims);

            return principal;
        }

        /// <summary>
        /// For a ClaimsPrincipal, attempts to find a AAD Group membership names and returns them as a List of strings.
        /// A ClaimsPrincipal not sourced from AAD generally will not find any AAD Groups.
        /// </summary>
        /// <param name="principal"></param>
        /// <returns>List of strings; List length is >= 0; List is never null</returns>
        public static List<string> GetGroups(this ClaimsPrincipal principal)
        {
            var groups = new List<string>();

            var aadObjectIdClaim = GetAadObjectIdClaim(principal);
            if (aadObjectIdClaim != null)
            {
                var aadObjectId = aadObjectIdClaim.Value;

                var graphConnection = BuildGraphConnectionForApplication();

                var directoryObject = new DirectoryObject
                {
                    ObjectId = aadObjectId
                };
                var pagedLinks = graphConnection.GetLinks(directoryObject, "memberOf", "");
                if (pagedLinks != null)
                {
                    var groupCollection =
                       pagedLinks.Results.Where(l => l.ObjectType == "Group").Cast<Group>().Select(g => g.DisplayName);
                    groups = groupCollection.ToList();
                }
            }

            // TODO: warn if no groups found?
            return groups;
        }

        /// <summary>
        /// Assumes zero or one target claim
        /// </summary>
        /// <param name="principal"></param>
        /// <returns>Claim, if found, else null</returns>
        public static Claim GetAadObjectIdClaim(this ClaimsPrincipal principal)
        {
            return principal.Claims.FirstOrDefault(c => c.Type == AadObjectIdClaimType);
        }

        /// <summary>
        /// GraphConnection is done based on application parameters. The connection is not based on any
        /// particular user.
        /// </summary>
        /// <returns></returns>
        private static GraphConnection BuildGraphConnectionForApplication()
        {
            var clientId = ConfigurationManager.AppSettings.Get("ida:ClientID");
            var clientSecret = ConfigurationManager.AppSettings.Get("ida:Password");
            var clientCred = new ClientCredential(clientId, clientSecret);

            var authorityDomain = ConfigurationManager.AppSettings.Get("ida:AuthorityDomain"); // custom appSetting in web.config - <add key="ida:AuthorityDomain" value="pragmaticazure.com" />
            Debug.Assert(!String.IsNullOrWhiteSpace(authorityDomain));
            var authorityUrl = String.Format(AadAuthorityUrlTemplate, authorityDomain);
            var authenticationContext = new AuthenticationContext(authorityUrl);
            var authenticationResult = authenticationContext.AcquireToken(GraphUrl, clientCred);

            var callContext = new CallContext(authenticationResult.AccessToken, Guid.NewGuid(), GraphApiVersion);
            var graphConnection = new GraphConnection(callContext);

            return graphConnection;
        }
    }
}
