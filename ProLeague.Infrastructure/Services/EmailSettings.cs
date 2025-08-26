namespace ProLeague.Infrastructure.Services
{
    public class EmailSettings
    {
        public string SmtpServer { get; set; } = "";
        public int Port { get; set; } = 587;
        public string FromAddress { get; set; } = "";
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
    }
}