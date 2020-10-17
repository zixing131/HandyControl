﻿using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HandyControl.Controls;
using HandyControlDemo.Data;
using HandyControlDemo.UserControl;
using HandyControlDemo.Window;
#if !NET35 && !NET40
using System.Threading.Tasks;
#endif
#if !NET35
using HandyControl.Tools.Extension;
#endif

namespace HandyControlDemo.ViewModel
{
    public class DialogDemoViewModel : ViewModelBase
    {
        private string _dialogResult;

        public string DialogResult
        {
            get => _dialogResult;
#if NET35 || NET40
            set => Set(nameof(DialogResult), ref _dialogResult, value);
#else
            set => Set(ref _dialogResult, value);
#endif
        }

        public RelayCommand<FrameworkElement> ShowTextCmd => new RelayCommand<FrameworkElement>(ShowText);

        private void ShowText(FrameworkElement element)
        {
            if (element == null)
            {
                Dialog.Show(new TextDialog());
            }
            else
            {
                Dialog.Show(new TextDialog(), MessageToken.DialogContainer);
            }
        }

#if NET35 || NET40
        public RelayCommand<bool> ShowInteractiveDialogCmd => new RelayCommand<bool>(ShowInteractiveDialog);

        private void ShowInteractiveDialog(bool withTimer)
        {
#if NET40
            if (!withTimer)
            {
                Dialog.Show<InteractiveDialog>()
                    .Initialize<InteractiveDialogViewModel>(vm => vm.Message = DialogResult)
                    .GetResultAsync<string>().ContinueWith(str => DialogResult = str.Result);
            }
            else
            {
                Dialog.Show<TextDialogWithTimer>(MessageToken.MainWindow).GetResultAsync<string>();
            }      
#endif
        }
#else
        public RelayCommand<bool> ShowInteractiveDialogCmd => new RelayCommand<bool>(async withTimer => await ShowInteractiveDialog(withTimer));

        private async Task ShowInteractiveDialog(bool withTimer)
        {
            if (!withTimer)
            {
                DialogResult = await Dialog.Show<InteractiveDialog>()
                    .Initialize<InteractiveDialogViewModel>(vm => vm.Message = DialogResult)
                    .GetResultAsync<string>();
            }
            else
            {
                await Dialog.Show<TextDialogWithTimer>(MessageToken.MainWindow).GetResultAsync<string>();
            }
        }
#endif

        public RelayCommand NewWindowCmd => new RelayCommand(() => new DialogDemoWindow
        {
            Owner = Application.Current.MainWindow
        }.Show());

        public RelayCommand<string> ShowWithTokenCmd => new RelayCommand<string>(token => Dialog.Show(new TextDialog(), token));
    }
}