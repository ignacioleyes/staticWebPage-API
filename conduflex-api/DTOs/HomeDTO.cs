﻿namespace conduflex_api.DTOs
{
    public class HomeDTO
    {
        public int Id { get; set; }
        public string[] Images { get; set; }
        public string Title { get; set; }
        public string EnglishTitle { get; set; }
        public string[] Description { get; set; }
        public string[] EnglishDescription { get; set; }
    }
}
