using System.ComponentModel;
using System.Runtime.CompilerServices;
using GroupDStegafy.Annotations;

namespace GroupDStegafy.ViewModel
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
