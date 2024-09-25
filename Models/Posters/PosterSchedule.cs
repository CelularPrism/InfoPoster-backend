namespace InfoPoster_backend.Models.Posters
{
    public class PosterSchedule
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public int SalePercent { get; set; }
        public int Price { get; set; }
    }
}
