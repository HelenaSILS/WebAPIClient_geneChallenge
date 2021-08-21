using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Encodings;
using System.Text;
using Newtonsoft.Json.Linq;

namespace WebAPIClient
{
    class Program
    {

        private static readonly HttpClient client = new HttpClient();
        private static Token token = new Token();
        private static Job job = new Job();
        public async static Task Main(string[] args)
        {

            var loginResponse = await Login();

            JObject jtoken = new JObject();
            jtoken = JObject.Parse(loginResponse);
            token.AccessToken=jtoken["accessToken"].ToString();
       

            var jobString = await RequestJob();
            JObject jjob = new JObject();
            jjob = JObject.Parse(jobString);
            
            BuildJob(jjob, job);
            
            
            var jprocessor = new JobProcessor(job);
            jprocessor.PrintJob();
            var r = jprocessor.BeginProcess();
            Console.WriteLine(r);

        }
        private static async Task<string> Login()
        {
            var user = new User();
            string jsonUser = JsonSerializer.Serialize(user);
            
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            var contentString = new StringContent(jsonUser, Encoding.UTF8,"application/json");
            
            contentString.Headers.ContentType = new MediaTypeHeaderValue("application/json");
           
            HttpResponseMessage response = await client.PostAsync("https://gene.lacuna.cc/api/users/login", contentString);
            
            var contents = await response.Content.ReadAsStringAsync();
        

            return contents;

        }

        private static async Task <string> RequestJob(){

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Authorization =  new AuthenticationHeaderValue("Bearer", token.AccessToken);

            var response = await client.GetStringAsync("https://gene.lacuna.cc/api/dna/jobs");
            //var job = await JsonSerializer.DeserializeAsync<Job>(response);

            return response;

        }

        private static void BuildJob(JObject json, Job job){

            job.Id=json["job"]["id"].ToString();
            job.Type=json["job"]["type"].ToString();
            
            if(job.Type=="EncodeStrand"){
                job.Strand=json["job"]["strand"].ToString();

            }else{
                if(job.Type=="DecodeStrand" | job.Type=="CheckGene")
                    job.StrandEncoded=json["job"]["strandEncoded"].ToString();

                if(job.Type=="CheckGene")

                    job.GeneEncoded=json["job"]["geneEncoded"].ToString();
            }

        }

    }
}
