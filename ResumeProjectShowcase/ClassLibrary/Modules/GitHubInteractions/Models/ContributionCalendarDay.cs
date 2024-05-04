namespace ClassLibrary.Modules.GitHubInteractions.Models
{
    internal class ContributionCalendarDay
    {
        public int ContributionCount { get; set; }
        public required string Date { get; set; }
        public int WeekDay { get; set; }
    }
}
