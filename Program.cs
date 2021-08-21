using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Encodings;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace WebAPIClient
{
    class Program
    {

        private static readonly HttpClient client = new HttpClient();
        private static Token token = new Token();
        private static Job job = new Job();
        private static JobProcessor jprocessor = new JobProcessor(job);
        public async static Task Main(string[] args)
        {

            // Login with user information with a POST method
            var loginResponse = await Login();

            // The login generates the access token, obtained in "loginResponse"
            JObject jtoken = new JObject();
            jtoken = JObject.Parse(loginResponse);
            token.AccessToken=jtoken["accessToken"].ToString();
       
            // With the object "token", the job is requested with a GET method
            var jobString = await RequestJob();

            // The GET sends the job in "jobString", that is used to set the "job" object, with all job's data
            JObject jjob = new JObject();
            jjob = JObject.Parse(jobString);
            BuildJob(jjob, job);
            
            // Prints in Console all data related to the job (id, type and genetic data)
            Console.WriteLine("Job received: ");
            jprocessor.PrintJob();

            // The class for processing the genetic data, JobProcessor, is called under the object jprocessor. The solution is obtained in BuildMessageSoluction
            var message = BuildMessageSolution();
            Console.WriteLine("Message sent: ");
            Console.WriteLine(message);

            // The job solution is then send by POST. The return message is the last thing printed.
            var finalResponse = await PostMessage(message);
            Console.WriteLine("Response after sent job: ");
            Console.WriteLine(finalResponse);


        }

        /*
        * Assyncronous method to do the login with the user set in "user" object. It obtains and then returns the access token for the user. 
        */
        private static async Task<string> Login()
        {
            var user = new User();
            string jsonUser = System.Text.Json.JsonSerializer.Serialize(user);
            
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            var contentString = new StringContent(jsonUser, Encoding.UTF8,"application/json");
            
            contentString.Headers.ContentType = new MediaTypeHeaderValue("application/json");
           
            HttpResponseMessage response = await client.PostAsync("https://gene.lacuna.cc/api/users/login", contentString);
            
            var contents = await response.Content.ReadAsStringAsync();
        

            return contents;

        }

        /*
        * Assyncronous method to request a new job after the successful login. Returns the job as a string.
        */
        private static async Task <string> RequestJob(){

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Authorization =  new AuthenticationHeaderValue("Bearer", token.AccessToken);

            var response = await client.GetStringAsync("https://gene.lacuna.cc/api/dna/jobs");

            return response;

        }

        /*
        * The solution in the send back by the this assyncronous method. It returns the acceptance/error message as a string.
        */
        private static async Task<string> PostMessage(string message)
        {
            
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Authorization =  new AuthenticationHeaderValue("Bearer", token.AccessToken);
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            var contentString = new StringContent(message, Encoding.UTF8,"application/json");
            
            contentString.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            
            string URLbase = "https://gene.lacuna.cc/api/dna/jobs/";
            StringBuilder stringBuilder = new StringBuilder(URLbase);
            stringBuilder.Append(job.Id.ToString());
            stringBuilder.Append('/');
            stringBuilder.Append(job.Path.ToString());
            var URL = stringBuilder.ToString();

            Console.Write('\n'+URL+'\n');

            HttpResponseMessage response = await client.PostAsync(URL, contentString);
            
            var contents = await response.Content.ReadAsStringAsync();
        

            return contents;

        }

        /*
        * "job" object is build, receiving its data from a previous build Json object.
        */
        private static void BuildJob(JObject json, Job job){

            job.Id=json["job"]["id"].ToString();
            job.Type=json["job"]["type"].ToString();
            
            if(job.Type=="EncodeStrand"){
                job.Strand=json["job"]["strand"].ToString();
                job.Path="encode";

            }else{
                if(job.Type=="DecodeStrand" )
                    job.Path="decode";
                if(job.Type=="DecodeStrand" | job.Type=="CheckGene")
                    job.StrandEncoded=json["job"]["strandEncoded"].ToString();

                if(job.Type=="CheckGene"){ 
                    job.Path="gene";
                    job.GeneEncoded=json["job"]["geneEncoded"].ToString();
                    }
            }

        }

        /*
        * Job is solved according to its type (decode, encode or check gene). Returns the string with the solution.
        */
        private static string BuildMessageSolution(){
            string message="";
            switch (job.Type){
            case "DecodeStrand":
                DecodeStrand ds = new DecodeStrand();
                ds.strand = jprocessor.BeginProcess();
                message=JsonConvert.SerializeObject(ds); break;

            case "EncodeStrand":
                EncodeStrand es = new EncodeStrand(); 
                es.strandEncoded = jprocessor.BeginProcess(); 
                message=JsonConvert.SerializeObject(es); break;

            case "CheckGene":
                CheckGene cg = new CheckGene();
                var res = jprocessor.BeginProcess();
                if(res=="True")
                    cg.isActivated=true;
                else
                    cg.isActivated=false;
                message=JsonConvert.SerializeObject(cg); break;
         }
            return message;

        }

    }
}
