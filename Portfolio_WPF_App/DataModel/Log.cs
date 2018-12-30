namespace Portfolio_WPF_App.DataModel
{
    class Log
    {
        public int Id { get; set; }
        public string Time { get; set; }
        public string MessageLevel { get; set; }
        public string Message { get; set; }

        public override string ToString()
        {
            return Id.ToString() + "; "
                + Time + "; "
                + MessageLevel + "; "
                + Message;
        }
    }
}
