namespace ClassLibrary.GithubInteractionService.Models
{
    internal class ContributionCalendarWeek
    {
        public List<ContributionCalendarDay> ContributionDays { get; set; }
        public string FirstDay { get; set; }
    }
}
