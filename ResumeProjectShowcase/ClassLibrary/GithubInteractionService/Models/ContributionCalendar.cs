namespace ClassLibrary.GithubInteractionService.Models
{
    /// <summary>
    /// Reference GraphQL schema:
    /// https://docs.github.com/en/graphql/reference/objects#contributionscollection
    /// https://docs.github.com/en/graphql/reference/objects#contributioncalendar
    /// </summary>
    internal class ContributionCalendar
    {
        public int TotalContributions { get; set; }
        public List<ContributionCalendarWeek> Weeks { get; set; }
    }
}
