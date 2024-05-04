namespace ClassLibrary.Modules.GitHubInteractions.Models
{
    internal class ContributionCalendarWeek
    {
        public required List<ContributionCalendarDay> ContributionDays { get; set; }
        public required string FirstDay { get; set; }
    }
}
