namespace BoluBul.ViewModels.Admin
{
    public class AdminUserRowViewModel
    {
        public string Id { get; set; } = string.Empty;

        public string? Email { get; set; }

        public string? FullName { get; set; }

        public DateTime CreatedAt { get; set; }

        public string Roles { get; set; } = string.Empty;
    }
}
