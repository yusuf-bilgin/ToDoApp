using Microsoft.AspNetCore.Identity;
using ToDoApp.Models;

namespace ToDoApp.Models;
    
public class Note
{
    public int Id { get; set; }             
    public string Title { get; set; }         
    public string Content { get; set; }     
    public DateTime CreatedAt { get; set; }

    // Kullanıcıya özel sayfa göstermek için
    public string? UserId { get; set; }

    public ApplicationUser? User { get; set; }  // 👈 Navigation property
}