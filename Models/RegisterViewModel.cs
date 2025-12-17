using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

public class RegisterViewModel
{
    [Required]
    public string Name { get; set; }

    [Required]
    public string Surname { get; set; }

    [Required, EmailAddress]
    public string Email { get; set; }

    public string Phone { get; set; }

    [Required]
    public string Password { get; set; }
}


