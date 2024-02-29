using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using TechnicalInterview_FrontEnd.API.Models;

namespace TechnicalInterview_FrontEnd.API
{
    public class DataService
    {
        private readonly HttpClient Client = new HttpClient();
        private readonly string BASE_ADDRESS = "https://localhost:7270/";

        public DataService()
        {
            ConfigureHttpClient();
        }

        public async Task<List<District>?> GetDistrictsAsync()
        {
            HttpResponseMessage responseMessage = await Client.GetAsync("/districts");

            if (!responseMessage.IsSuccessStatusCode) return null;

            string? responseContent = await responseMessage.Content.ReadAsStringAsync();

            // Try to convert the response to Json.
            if (JsonConvert.DeserializeObject<List<District>>(responseContent) is List<District> validated) 
            {
                return validated;
            }

            return null;
        }

        /// <summary>
        /// Gets the Statuses table from the database.
        /// </summary>
        /// <returns></returns>
        public async Task<List<Status>?> GetStatusesAsync()
        {
            HttpResponseMessage responseMessage = await Client.GetAsync("/statuses");

            if (!responseMessage.IsSuccessStatusCode) return null;

            string? responseContent = await responseMessage.Content.ReadAsStringAsync();

            // Try to convert the response to Json.
            if (JsonConvert.DeserializeObject<List<Status>>(responseContent) is List<Status> validated)
            {
                return validated;
            }

            return null;
        } 

        /// <summary>
        /// Validates Username and Password combinations. 
        /// Returns true if Username and Password were validated from the API.
        /// </summary>
        /// <param name="username">A string for the username</param>
        /// <param name="password">A string for the password</param>
        /// <returns></returns>
        public async Task<bool> ValidateUserAsync(string username, string password)
        {
            HttpResponseMessage responseMessage = await Client.GetAsync($"/users?username={username}&password={password}");

            if (!responseMessage.IsSuccessStatusCode) return false;
            
            string? responseContent = await responseMessage.Content.ReadAsStringAsync();

            // Try to convert string to Json. The API is trying to return a bool.
            if (JsonConvert.DeserializeObject<bool>(responseContent) is bool validated)
            {
                return validated;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Searches the database for a voter with the specified VoterId.
        /// </summary>
        /// <param name="voterId"></param>
        /// <returns>A single voter if the VoterId matches a record in the database.</returns>
        public async Task<VoterActivity?> SearchVotersByIdAsync(string voterId)
        {
            // Convert string voterId to int
            if (int.TryParse(voterId, out int id))
            {
                HttpResponseMessage responseMessage = await Client.GetAsync($"/voteractivity/{id}");

                if (!responseMessage.IsSuccessStatusCode) return null;

                string? responseContent = await responseMessage.Content.ReadAsStringAsync();

                // See if we can parse a response into a single voter.
                if (JsonConvert.DeserializeObject<VoterActivity>(responseContent) is VoterActivity voter)
                {
                    return voter;
                }
            }

            return null;
        }

        /// <summary>
        /// Searches the database for voters with names that start with the first and last names provided.
        /// Full names do not need to be provided.
        /// </summary>
        /// <param name="lastname"></param>
        /// <param name="firstname"></param>
        /// <returns>A list of voters who have names that match the criteria.</returns>
        public async Task<List<VoterActivity>?> SearchVotersByNameAsync(string? lastname, string? firstname)
        {
            HttpResponseMessage responseMessage = await Client.GetAsync($"/voteractivity/name?FName={firstname}&LName={lastname}");

            if (!responseMessage.IsSuccessStatusCode) return null;

            string? responseContent = await responseMessage.Content.ReadAsStringAsync();

            // See if we can parse a response into a list of Voters.
            if (JsonConvert.DeserializeObject<List<VoterActivity>>(responseContent) is List<VoterActivity> voterList)
            {
                return voterList;
            }

            return null;
        }

        public async Task<Signature?> SaveSignatureAsync(Signature signature)
        {
            HttpResponseMessage responseMessage = await Client.PostAsJsonAsync("/signatures", signature);

            if (!responseMessage.IsSuccessStatusCode) return null;

            string? responseString = await responseMessage.Content.ReadAsStringAsync();

            if (JsonConvert.DeserializeObject<Signature>(responseString) is Signature returnSignature)
            {
                return returnSignature;
            }

            return null;
        }

        public async Task<Signature?> GetSignatureAsync(int voterId)
        {
            HttpResponseMessage responseMessage = await Client.GetAsync($"/signatures/{voterId}");

            if (!responseMessage.IsSuccessStatusCode) return null;

            string? responseString = await responseMessage.Content.ReadAsStringAsync();

            if (JsonConvert.DeserializeObject<Signature>(responseString) is Signature signature)
            {
                return signature;
            }

            return null;
        }

        public async Task<bool> MarkVoterAsync(VoterActivity activity)
        {
            HttpResponseMessage responseMessage = await Client.PutAsJsonAsync("/VoterActivity", activity);

            if (!responseMessage.IsSuccessStatusCode) return false;

            return true;
        }

        /// <summary>
        /// Creates an HttpClient. HttpClient is intended to be instantiated once per application, rather than per-use.
        /// </summary>
        private void ConfigureHttpClient()
        {
            Client.BaseAddress = new Uri(BASE_ADDRESS);
            Client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("text/plain"));
            Client.Timeout = TimeSpan.FromMinutes(60);
        }
    }
}
