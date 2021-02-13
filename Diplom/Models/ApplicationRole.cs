using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Diplom.Models
{
    public class ApplicationRole : IdentityRole
    {
        public ApplicationRole() : base()
        {

        }
        public ApplicationRole(string roleName) : base(roleName)
        {
            NameSecond = roleName;
        }
        [Required(ErrorMessage = "Обязательное для заполнения*")]
        [StringLength(30)]
        [RegularExpression(@"^[а-яА-Я]{1,30}$",
         ErrorMessage = "Некоректное значение поля")]
        public string NameSecond { get; set; }
    }
}
