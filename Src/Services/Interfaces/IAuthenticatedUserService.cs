﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aduaba.Services
{
    public interface IAuthenticatedUserService
    {
        string UserId { get; }
    }
}
