using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MovieShop.Service.Implementation
{
    public interface IBackgroundEmailSender
    {
        Task DoWork();
    }
}
