using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TbsCore.Models.AccModels;

namespace Web.Client.Pages
{
    public partial class General
    {
        [Parameter]
        public Account acc { get; set; }
    }
}
