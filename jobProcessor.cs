using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebAPIClient
{
    public class JobProcessor
    {

        private Job job;

    public JobProcessor(Job job){
        this.job = job;
    }

    /*
    * PrintJob prints in the console the job type and its respective data
    */
     public string PrintJob(){
         Console.WriteLine(job.Id);
         Console.WriteLine(job.Type);
         switch (job.Type){
            case "DecodeStrand":
                Console.WriteLine(job.StrandEncoded.ToString()); break;
            case "EncodeStrand":
                Console.WriteLine(job.Strand.ToString()); break;
            case "CheckGene":
                Console.WriteLine("strandEncoded: {0}", job.StrandEncoded.ToString());
                Console.WriteLine("gene: {0}", job.GeneEncoded.ToString()); break;
         }

         return "ok";
     }

    /*
    * Calls the functions according to the respective job type
    */
     public string BeginProcess(){
        string result = "";
        switch (job.Type){
            case "DecodeStrand":
                result = Decode(job.StrandEncoded.ToString()); break;
            case "EncodeStrand":
                result = Encode(job.Strand.ToString()); break;
            case "CheckGene":
                result = CheckGene(job.GeneEncoded.ToString(), job.StrandEncoded.ToString()); break;
         }
        return result;
     }

    /*
    * Decode recieves a string encoded as base64 and returns its binary as a string
    */
     private string Decode(string strandEncoded){

         /* 
         * strand Encoded in base64
         */
         byte[] arrayBytes = Convert.FromBase64String(strandEncoded);
         string bin = "0b";

        /* 
        *  PadLeft(8, '0') returns a string of a specified length and pads in the left with '0'
        *  Convert.ToString(Byte, Int32) converts a byte (numeric, like in8) 
        *  with the specified base to its string (in this case, 2 for binary)
        */
         foreach(byte b in arrayBytes){
             bin += Convert.ToString(b,2).PadLeft(8, '0');
         }
         
         return bin;
     }

    /*
    * Encodes recieves the strand in the "ATCG" template, and returns the binary code, but as a string
    */
    private string Encode(string strand){
        int i;
        string bin = "0b";
        for (i=0; i<strand.Length; i++){
            switch (strand[i]){
                case 'A':
                    bin=bin+"00"; break;
                case 'T':
                    bin=bin+"11"; break;
                case 'C':
                    bin=bin+"01"; break;
                case 'G':
                    bin=bin+"10"; break;
            }
        }
    

         return bin;
     }

     /*
     * CheckGene returns a strig with value "True", if the 50% of the gene is found in the strand
     * and returns the string "False" otherwise.
     */

    private string CheckGene(string geneEncoded, string strandEncoded){

         string strandbin = Decode(strandEncoded);
         string genebin = Decode(geneEncoded);

         string strand = ToNucleobase(strandbin);
         string gene = ToNucleobase(genebin);

         if(!isTemplate(strand))
            strand = ToTemplate(strand);

         int geneHalf = (int) Math.Round((float) gene.Length/2);

        /*
        * The reasoning of the following block is that we must check if, at least every possible half of the gene is
        * in the strand thus, we must begin with the character 0 until the half way, always comparing only 50% of the whole
        * size of the gene, and then look if the strand contains such substring.
        * Once found, the search cand be stopped and it returns true. Otherwise, it stops when it reaches the gene's middle.
        */
         bool loop=false;
         int i=0;
         while(i<geneHalf & !loop){
             loop=strand.Contains(gene.Substring(i, geneHalf));
             i++;
         }

         return loop.ToString();;
     }

    /*
    * IsTemplate answers as true if the input is according to the template and false otherwise.
    */
    private bool isTemplate(string nucleobase){
        // Substring takes the first 3 characters from the nucleobase parameter
        string aux = nucleobase.Substring(0, 3);
        if (aux == "CAT")
            return true;
        else
            return false;
    }

    /*
    * ToTemplate invert a strand that does not follow the standard, that is, does not begin with "CAT"
    */
    private string ToTemplate(string inverted){

        StringBuilder stringBuilder = new StringBuilder("", inverted.Length);

        int j=0;
        for(int i =inverted.Length-1; i>= 0; i--){
            stringBuilder.Insert(j, inverted[i]);
        }

        string template = stringBuilder.ToString();     

        return template;
    }


    /*
    * toNucleobase returns the binary input (in this context, binary is decode from base64) as a "ATCG" string.
    * It is easier to manipulate in further functions after this conversion. 
    */
    private string ToNucleobase(string bin){
        StringBuilder stringBuilder = new StringBuilder();
        string aux = "";
        string result = "";

        //the two first characters are always "0b", so they are ignored, that is way int i = 2
        for(int i=2; i<bin.Length; i+=2){
            stringBuilder.Append(bin[i]);
            stringBuilder.Append(bin[i+1]);

            aux = stringBuilder.ToString();
            if(aux=="00"){
                result += 'A';
            }else{
                if(aux=="01"){
                    result += 'C';
                }else{
                    if(aux=="10"){
                        result += 'G';
                    }else{
                        result += 'T';
                    }
                }
            }

        }

        return result;
    }

    }
}