﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP_BusinessLogic.Options
{
    public class JwtOptions
    {
        public string Key { get; set; }
        public string ValidAudience { get; set; }
        public string ValidIssuer { get; set; }
        public double Lifetime { get; set; }
    }
}
