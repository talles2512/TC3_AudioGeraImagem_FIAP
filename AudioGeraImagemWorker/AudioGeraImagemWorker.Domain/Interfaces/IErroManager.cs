﻿using AudioGeraImagemWorker.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioGeraImagemWorker.Domain.Interfaces
{
    public interface IErroManager
    {
        Task TratarErro(Comando comando);
    }
}