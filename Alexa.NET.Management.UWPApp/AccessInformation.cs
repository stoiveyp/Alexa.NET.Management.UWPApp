using Newtonsoft.Json;

namespace Alexa.NET.Management.UWPApp
{
        public class AccessInformation
        {
            [JsonProperty("access_token")]
            public string AccessToken { get; set; }

            [JsonProperty("refresh_token")]
            public string RefreshToken { get; set; }

            [JsonProperty("expires_in")]
            public int ExpiresIn { get; set; }
        }
}

//https://developer.amazon.com/docs/smapi/ask-cli-intro.html#smapi-intro
//Scope Permissions
//alexa::ask:skills:read List the vendor IDs associated with the Amazon developer account
//Read skill details(excluding interaction models)). Get skill status.Get list of skills.
//alexa::ask:skills:readwrite All alexa::ask:skills:read permissions
//Create skills
//Update skills
//Enable skills
//Read and update account linking info for skills, Submit and withdraw skills.
//alexa::ask:models:read Read interaction models
//Get build status for interaction models
//alexa::ask:models:readwrite All alexa::ask:models:read permissions
//Update interaction models
//alexa::ask:skills:test Skill testing–invoke a skill, submit a simulation request, get simulation status