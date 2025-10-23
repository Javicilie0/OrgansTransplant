using System.Collections.ObjectModel;
using System.Windows.Input;
using OrgnTransplant.Data;

namespace OrgnTransplant.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IDonorService _donorService;
        private readonly IMessageService _messageService;

        public MainViewModel(IDonorService donorService, IMessageService messageService)
        {
            _donorService = donorService;
            _messageService = messageService;

            // Initialize collections and commands here
            // This is a placeholder - full implementation to be completed
        }

        // Properties and commands will be added here
    }
}
