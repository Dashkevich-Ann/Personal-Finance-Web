namespace BusinessLayer
{
    public class EmailConfiguration
    {
        public string Address { get; set; }
        public string Password { get; set; }

        public string Smtp { get; set; }

        public int Port { get; set; }
    }
}