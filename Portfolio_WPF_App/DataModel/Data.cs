namespace Portfolio_WPF_App.DataModel
{
    class Data
    {
        public int Id { get; set; }
        public string Time { get; set; }
        public string Name { get; set; }
        public string SureName { get; set; }
        public string Gender { get; set; }

        public override string ToString()
        {
            return Id.ToString() + "; "
                + Time + "; "
                + Name + "; "
                + SureName + "; "
                + Gender;
        }
    }
}
