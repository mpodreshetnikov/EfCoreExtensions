namespace TestApp.Entities
{
    public class User
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Nickname { get; set; }

        public int Age { get; set; }

        public string SSN { get; set; }

        public int? MotherId { get; set; }

        public User Mother { get; set; }
    }
}
