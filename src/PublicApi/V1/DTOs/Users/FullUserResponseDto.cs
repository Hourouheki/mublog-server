namespace Mublog.Server.PublicApi.V1.DTOs.Users
{
    public class FullUserResponseDto
    {
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string ProfileImageId { get; set; }
        public string HeaderImageId { get; set; }
        public long FollowersCount { get; set; }
        public long FollowingCount { get; set; }
        public bool FollowingStatus { get; set; }
    }
}