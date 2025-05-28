using System.Collections.Generic;

namespace TestApp.Models
{
    public class Question
    {
        public int Number { get; set; }
        public string Meaning { get; set; }
        public List<string> Variables { get; set; }
        public string RightAnswer { get; set; }
    }
}