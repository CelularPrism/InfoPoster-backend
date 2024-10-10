using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace InfoPoster_backend.Models.Selectel
{
    public class SelectelKeystoreRequest
    {
        [JsonInclude]
        public SelectelAuth auth;  
        
        public SelectelKeystoreRequest()
        {
            auth = new SelectelAuth()
            {
                identity = new SelectelIdentity()
                {
                    methods = new List<string>() { "password" },
                    password = new SelectelPassword()
                    {
                        user = new SelectelUser()
                        {
                            name = "Phoebe",
                            domain = new SelectelDomain()
                            {
                                name = "279273"
                            },
                            password = "Dy9MSsds"
                        }
                    }
                },
                scope = new SelectelScope()
                {
                    project = new SelectelProject()
                    {
                        name = "My First Project",
                        domain = new SelectelDomain()
                        {
                            name = "279273"
                        }
                    }
                }
            };
        }
    }

    public class SelectelAuth
    {
        [JsonInclude]
        public SelectelIdentity identity { get; set; }
        
        [JsonInclude]
        public SelectelScope scope { get; set; }
    }

    public class SelectelDomain
    {
        [JsonInclude]
        public string name { get; set; }
    }

    public class SelectelIdentity
    {
        [JsonInclude]
        public List<string> methods { get; set; }

        [JsonInclude]
        public SelectelPassword password { get; set; }
    }

    public class SelectelPassword
    {
        [JsonInclude]
        public SelectelUser user { get; set; }
    }

    public class SelectelProject
    {
        [JsonInclude]
        public string name { get; set; }

        [JsonInclude]
        public SelectelDomain domain { get; set; }
    }

    public class SelectelRoot
    {
        [JsonInclude]
        public SelectelAuth auth { get; set; }
    }

    public class SelectelScope
    {
        [JsonInclude]
        public SelectelProject project { get; set; }
    }

    public class SelectelUser
    {
        [JsonInclude]
        public string name { get; set; }

        [JsonInclude]
        public SelectelDomain domain { get; set; }

        [JsonInclude]
        public string password { get; set; }
    }
}
