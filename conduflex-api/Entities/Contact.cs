namespace conduflex_api.Entities
{
    public class Contact
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Company { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
