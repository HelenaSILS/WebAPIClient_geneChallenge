namespace WebAPIClient
{
    public class User
    {
        //deixei public para aparecer e com get e set para ser lido pelo json serializer
        public string username { get; set; }
        public string password { get; set; }

        public User(){
            username="helenasils";
            password="os101dalmatas";
        }

    }
}