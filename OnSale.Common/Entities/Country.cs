using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace OnSale.Common.Entities
{
    public class Country
    {
        public int Id { get; set; }

        [MaxLength(50, ErrorMessage = "El campo {0} no debe contener mas de {1} caracteres")]
        [Required]
        public string Name { get; set; }
    }

}
