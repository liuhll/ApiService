namespace Jueci.ApiService.Base.Dtos
{
    public abstract class BasicDto
    {
        public string AppId { get; set; }

        public string Sign { get; set; }

        public long Timestamp { get; set; }
    }
}