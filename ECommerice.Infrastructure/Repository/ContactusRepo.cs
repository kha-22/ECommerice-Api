using ECommerice.Core.DTOs;
using ECommerice.Core.Entities;
using ECommerice.Core.IRepository;
using ECommerice.Core.IUniteOfWork;
using ECommerice.Infrastructure.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerice.Infrastructure.Repository
{
    public class ContactusRepo : IContactusRepo
    {
        private readonly IUniteOfWork _uniteOfWork;
        private readonly IMailService _mailService; 
        private readonly StoreContext _storeContext; 

        public ContactusRepo(IUniteOfWork uniteOfWork, IMailService mailService,
            StoreContext storeContext)
        {
            _uniteOfWork = uniteOfWork;
            _mailService = mailService;
            _storeContext = storeContext;
        }

        public async Task<bool> Send(Contactus contactus)
        {
            contactus.SendDate = DateTime.Now;
            _uniteOfWork.Repository<Contactus>().Add(contactus);
            return await _uniteOfWork.CompleteAsync() > 0;
        }

        public async Task<bool> Replay(string userId, string replay)
        {
            var currentContactus = await _uniteOfWork.Repository<Contactus>()
               .GetWhereObject(x => x.UserId == userId);
            if (currentContactus != null)
            {
                currentContactus.RepayDate = DateTime.Now;
                currentContactus.Replay = replay;

                _uniteOfWork.Repository<Contactus>().Update(currentContactus);
                return await _uniteOfWork.CompleteAsync() > 0;
            }
            return false;
        }

        public async Task<List<Contactus>> GetAllData()
        {
            var result = await _uniteOfWork.Repository<Contactus>().GetAllPaging();
            return result.ToList();
        }

        public async Task<Contactus> GetData(int id)
        {
            return await _uniteOfWork.Repository<Contactus>().GetByIdAsync(id);
        }

        public async Task<List<Contactus>> GetQuestionWithReplayes(string userId)
        {
            var resut = await _uniteOfWork.Repository<Contactus>()
                .GetWherePagig(x => x.UserId == userId);
                   
            return resut.ToList();
        }

        public async Task<List<Contactus>> GetQuestions()
        {
            var resut = await _uniteOfWork.Repository<Contactus>()
                .GetAllPaging();

            return resut.ToList();
        }

        public async Task<bool> SendAnswer(ContactusReplayDTO contactusReplay)
        {
            var contactus = await _uniteOfWork.Repository<Contactus>()
               .GetWhereObject(x => x.Id == contactusReplay.Id);

            if (contactus != null)
            {
                contactus.Replay = contactusReplay.Replay;
                contactus.RepayDate = DateTime.Now;

                _uniteOfWork.Repository<Contactus>().Update(contactus);
                return await _uniteOfWork.CompleteAsync() > 0;
            }
            return false;
        }



    }
}
