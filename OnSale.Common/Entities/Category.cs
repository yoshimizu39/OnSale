using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace OnSale.Common.Entities
{
    public class Category
    {
        public int Id { get; set; }

        [MaxLength(50, ErrorMessage = "El campo {0} no debe contener mas de {1} caracteres")]
        [Required]
        public string Name { get; set; }

        [Display(Name = "Image")]
        public Guid ImageId { get; set; } //con Guid crea imàgenes ùnicas asignandoles un còdigo

        //TODO: Pending to put the correct paths
        [Display(Name = "Image")]
        public string ImageFullPath => ImageId == Guid.Empty
            ? $"https://localhost:44321/images/noimage.png"
            : $"https://onsale.blob.core.windows.net/categories/{ImageId}";
    }

}
