namespace InfoPoster_backend.Models.Posters
{
    public class PosterContactsModel
    {
        public void Update(string phone, string firstName)
        {
            Phone = phone;
            FirstName = firstName;
        }

        public PosterContactsModel(Guid posterId, string phone, string firstName)
        {
            Id = Guid.NewGuid();
            PosterId = posterId;
            Phone = phone;
            FirstName = firstName;
        }

        public Guid Id { get; set; }
        public Guid PosterId { get; set; }
        public string FirstName { get; set; }
        public string Phone {  get; set; }
    }
}
