namespace GINotificationManager.Model
{
    public class NotificaGI
    {
        public string ProtocolloGI { get; set; } = string.Empty;
        public int PrgVersioneGI { get; set; }
        public int[] TipoNotificaGI { get; set; } = new int[0];
        public DateTime DataOraNotifica { get; set; }

    }
}
