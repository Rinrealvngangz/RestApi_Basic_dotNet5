﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestAPI_Food.Dtos
{
    public class ResponseDtos
    {
        public List<string> Errors { get; set; }

        public string Token {get; set;}
        public bool Success { get; set; }
    }
}
