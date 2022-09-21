using Azure.Identity;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotNetCoreSpecflowBase.Extensions
{
    public class GraphAPI
    {

        // Read application settings from appsettings.json (tenant ID, app ID, client secret, etc.)
        public static  GraphServiceClient CreateGraphApi(String TenantId, String ClientSecret, String AppID)
        {
            try
            {
                var scopes = new[] { "https://graph.microsoft.com/.default" };
                var options = new TokenCredentialOptions
                {
                    AuthorityHost = AzureAuthorityHosts.AzurePublicCloud
                };
                var clientSecretCredential = new ClientSecretCredential(TenantId, AppID, ClientSecret, options);
                var graphClient = new GraphServiceClient(clientSecretCredential, scopes);

                return graphClient;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static async Task<string> CreateGaphAPIUser(GraphServiceClient graphClient, UserData userData)
        {     
           
                // Create user
                var  result = await graphClient.Users
                .Request()
                .AddAsync(new User
                {
                    GivenName = userData.UsersDetails.givenName,
                    Surname = userData.UsersDetails.surname,
                    DisplayName = userData.UsersDetails.displayName,
                    Identities = new List<ObjectIdentity>
                    {
                        new ObjectIdentity()
                        {
                            SignInType = userData.UsersDetails.identities[0].signInType,
                            Issuer = userData.UsersDetails.identities[0].Issuer,
                            IssuerAssignedId = userData.UsersDetails.identities[0].issuerAssignedId
                        }
                    },
                    PasswordProfile = new PasswordProfile()
                    {
                        Password = userData.UsersDetails.PasswordProfile[0].Password,
                        ForceChangePasswordNextSignIn = Convert.ToBoolean(userData.UsersDetails.PasswordProfile[0].ForceChangePasswordNextSignIn)

                    },
                    PasswordPolicies = userData.UsersDetails.PasswordPolicies
                });
              
                return result.Id;         
        }


        public static async Task<String> DeleteUserById(GraphServiceClient graphClient, string userId)
        {
            try
            {
                // Delete user by object ID
                await graphClient.Users[userId]
                   .Request()
                   .DeleteAsync();
                return " User Deleted ";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}

