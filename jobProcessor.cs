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
                  
         string result = ToNucleobase(bin);

         return result;
     }

    /*
    * Encodes recieves the strand in the "ATCG" template, and returns the binary code, but as a string
    */
    private string Encode(string strand){
        int i;
        string bin = "";
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

        int len = (int)bin.Length/8;
        var arrayBytes = new byte[len];

        for(i=0; i<len; i++){
            arrayBytes[i]=Convert.ToByte(bin.Substring((i*8),8),2);
            
        }


        var result = Convert.ToBase64String(arrayBytes,0,arrayBytes.Length);

         return result;
     }

     /*
     * CheckGene returns a strig with value "True", if the 50% of the gene is found in the strand
     * and returns the string "False" otherwise. The data is firstly decode to "ATCG" template.
     */

    private string CheckGene(string geneEncoded, string strandEncoded){

         string strand = Decode(strandEncoded);
         string gene = Decode(geneEncoded);

         if(!isTemplate(strand))
            strand = ToTemplate(strand);

         int geneHalf = (int) Math.Round((float) gene.Length/2);

        /*
        * The reasoning of the following code block is that we must check if, at least every possible half of the gene is
        * in the strand, thus, we must begin with the character 0 until the half way, always comparing only 50% of the whole
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
    * ToTemplate turns the strand that does not begin with CAT into the standard by switching C <=> G and A <=> T
    */
    private string ToTemplate(string untemplated){

        StringBuilder stringBuilder = new StringBuilder("", untemplated.Length);

        for(int i=0; i <untemplated.Length; i++){
            if(untemplated[i]=='C')
                stringBuilder.Insert(i, 'G');
            if(untemplated[i]=='G')
                stringBuilder.Insert(i, 'C');
            if(untemplated[i]=='A')
                stringBuilder.Insert(i, 'T');
            if(untemplated[i]=='T')
                stringBuilder.Insert(i, 'A');
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
            stringBuilder.Clear();
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