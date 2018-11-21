using System;
using System.Collections.Generic;
using System.Linq;

namespace ECApi.Data
{
    public class Film
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Director { get; set; }

        public short? Year { get; set; }

        public string ImdbLink { get; set; }

        public float ImdbScore { get; set; }

        private string keywords;
        public List<string> Keywords => keywords.Split('|').ToList();
    }
}