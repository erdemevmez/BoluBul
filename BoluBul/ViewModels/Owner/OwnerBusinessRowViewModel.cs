namespace BoluBul.ViewModels.Owner
{
    public class OwnerBusinessRowViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string CategoryName { get; set; } = string.Empty;

        public bool IsApproved { get; set; }

        public bool IsActive { get; set; }

        public int ViewCount { get; set; }

        public int PhoneClickCount { get; set; }

        public int WhatsAppClickCount { get; set; }

        public int DirectionClickCount { get; set; }
    }
}
