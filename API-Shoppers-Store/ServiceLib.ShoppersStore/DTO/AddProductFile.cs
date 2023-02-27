using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ServiceLib.ShoppersStore.DTO
{
    public class AddProductFile
    {
        public int ProductFileId { get; set; }

        [Required(ErrorMessage = "File Name is Required!")]
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public int ProductId { get; set; }
    }
}
