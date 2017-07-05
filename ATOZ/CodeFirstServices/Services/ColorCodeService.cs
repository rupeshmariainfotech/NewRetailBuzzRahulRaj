using System.Collections.Generic;
using CodeFirstData.DBInteractions;
using CodeFirstData.EntityRepositories;
using CodeFirstEntities;
using CodeFirstServices.Interfaces;
using System;

namespace CodeFirstServices.Services
{
    public class ColorCodeService : IColorCodeService
    {
        private readonly IColorCodeRepository _colorCodeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ColorCodeService(IColorCodeRepository colorCodeRepository, IUnitOfWork unitOfWork)
        {
            this._colorCodeRepository= colorCodeRepository;
            this._unitOfWork = unitOfWork;
        }

        public IEnumerable<ColorCode> getAllColorCode()
        {
            var colorData = _colorCodeRepository.GetAll();
            return colorData;
        }

        public ColorCode GetColorCodebyId(int id)
        {
            var colorData = _colorCodeRepository.GetById(id);
            return colorData;
        }
        public ColorCode getColorNameByCode(string code)
        {
            var colorData = _colorCodeRepository.Get(cc => cc.colorCode == code);
            return colorData;
        }
    }
}
