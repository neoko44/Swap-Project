﻿using Core.Utilities.Results;
using Entities.Concrete;
using Entities.Dtos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstract
{
    public interface ICryptoService
    {
        Task<IResult> BuyCrypto(BuyCryptoDto buyCryptoDto);
        Task<IResult> SellCrypto(SellCryptoDto sellCryptoDto);

        IDataResult<List<Crypto>> GetList();

    }
}
