using System;

namespace WebAPIClient
{
    public class Job
    {
        public string Id {get; set;}

        public string Path{get;set;}

        public string Type {get; set;}

        public string Strand {get; set;}

        public string StrandEncoded {get; set;}

        public string GeneEncoded {get; set;}

        public Job(){

        }
    }
}
