namespace NHASoftware.Models
{
    public class EmailData
    {
        /*************************************************************************************
        *   This is a basic model to give us something to pass to the Services/IEmailService
        ***************************************************************************************/

        public string EmailToId { get; set; }
        public string EmailToName { get; set; }
        public string EmailSubject { get; set; }
        public string EmailBody { get; set; }

    }
}
