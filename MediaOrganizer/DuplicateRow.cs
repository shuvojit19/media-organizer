namespace MediaOrganizer
{
    public class DuplicateRow
    {
        public int GroupId { get; set; }
        public string Path { get; set; } = "";
        public string Hash { get; set; } = "";
        public long SizeKB { get; set; }
        public bool SelectedForDelete { get; set; }
    }
}
