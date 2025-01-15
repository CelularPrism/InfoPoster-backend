namespace InfoPoster_backend.Models.Offers
{
    public class OffersMultilangModel
    {
        public OffersMultilangModel() { }

        public OffersMultilangModel(Guid offerId, string lang, string name, string description, string address)
        {
            OfferId = offerId;
            Lang = lang;
            Name = name;
            Description = description;
            Address = address;
        }

        public void Update(string lang, string name, string description, string address)
        {
            if (string.IsNullOrEmpty(Name) || Lang == lang)
                Name = name;

            if (string.IsNullOrEmpty(Description) || Lang == lang)
                Description = description;

            if (string.IsNullOrEmpty(address) || Lang == lang)
                Address = address;
        }

        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid OfferId { get; set; }
        public string Lang { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
    }
}
