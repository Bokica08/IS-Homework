using System;
using System.Collections.Generic;
using System.Text;
using MovieShop.Domain.Domain;
using MovieShop.Service.Implementation;
using System.Threading.Tasks;
using MovieShop.Repository.Interface;
using System.Linq;

namespace MovieShop.Service.Interface
{
    public class BackgroundEmailSender : IBackgroundEmailSender
    {

        private readonly IEmailService _emailService;
        private readonly IRepository<EmailMessage> _mailRepository;

        public BackgroundEmailSender(IEmailService emailService, IRepository<EmailMessage> mailRepository)
        {
            _emailService = emailService;
            _mailRepository = mailRepository;
        }
        public async Task DoWork()
        {
            await _emailService.SendEmailAsync(_mailRepository.listAll().Where(z => !z.Status).ToList());
        }
    }
}
