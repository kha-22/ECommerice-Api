using ECommerice.Core.DTOs;
using ECommerice.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerice.Core.IRepository
{
    public interface IContactusRepo
    {
        Task<bool> Send(Contactus contactus);
        Task<bool> Replay(string userId, string replay);
        Task<List<Contactus>> GetAllData();
        Task<Contactus> GetData(int id);
        Task<List<Contactus>> GetQuestionWithReplayes(string userId);
        Task<List<Contactus>> GetQuestions();

        Task<bool> SendAnswer(ContactusReplayDTO contactusReplay);
    }
}
