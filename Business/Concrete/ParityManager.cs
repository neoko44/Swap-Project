﻿using Business.Abstract;
using Business.Constants;
using Core.Utilities.Results;
using Core.Utilities.Security.Jwt;
using DataAccess.Abstract;
using Entities.Concrete;
using Entities.Dtos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Entities.Dtos.GetParitiesDto;

namespace Business.Concrete
{
    public class ParityManager : IParityService
    {
        ITokenHelper _tokenHelper;
        IUserDal _userDal;
        ICryptoDal _cryptoDal;

        public ParityManager(ITokenHelper tokenHelper, IUserDal userDal, ICryptoDal cryptoDal)
        {
            _tokenHelper = tokenHelper;
            _userDal = userDal;
            _cryptoDal = cryptoDal;
        }

        public async Task<IDataResult<Parity>> GetPrice(int id)
        {
            if (id==0||id<0)
            {
                return new ErrorDataResult<Parity>(Messages.IdInvalid);
            }
            var getName = _cryptoDal.Get(p => p.Id == id);
            if (getName == null)
            {
                return new ErrorDataResult<Parity>(Messages.ParityNotFound);
            }

            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.binance.com/api/v3/ticker/price?symbol={getName.Name}{Types.Parities.USDT.ToString().ToUpper()}");
            var response = client.SendAsync(request).Result;

            if (response.IsSuccessStatusCode)
            {
                using (var content = await response.Content.ReadAsStreamAsync())
                {
                    var getParity = Task.Run(() => JsonConvert.DeserializeObject<Parity>(new StreamReader(content).ReadToEnd())).Result;

                    return new SuccessDataResult<Parity>(getParity);
                }
            }

            return new ErrorDataResult<Parity>(Messages.ParityNotFound);
        }

        public IDataResult<List<Crypto>> GetParities()
        {
            List<Crypto> list = new();
            var getList = _cryptoDal.GetList().ToList();
            if (getList.Count == 0)
            {
                return new ErrorDataResult<List<Crypto>>(Messages.CryptoNotFound);
            }


            foreach (var item in getList)
            {
                Crypto crypto = new()
                {
                    Id = item.Id,
                    Name = item.Name,
                    Status = item.Status,
                    Commission = item.Commission,
                };
                list.Add(crypto);
            }

            return new SuccessDataResult<List<Crypto>>(list);
        }
    }
}
