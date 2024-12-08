namespace InSightWindowAPI.Models.Entity
{
    public class UserFireBaseTokens
    {
        public Guid UserId { get; set; }

        public Guid FireBaseTokenId { get; set; }

        public FireBaseToken FireBaseToken { get; set; }

        public User User { get; set; }
    }
}