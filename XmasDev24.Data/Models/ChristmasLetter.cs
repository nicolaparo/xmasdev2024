namespace XmasDev24.Data.Models
{
    public class ChristmasLetter
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string? KidFirstName { get; set; }
        public string? KidLastName { get; set; }
        public string[] Gifts { get; set; } = [];
        public string? Address { get; set; }
        public string? LetterPhotoUri { get; set; }
        public string? LetterText { get; set; }
        public bool Delivered { get; set; }
    }
}
