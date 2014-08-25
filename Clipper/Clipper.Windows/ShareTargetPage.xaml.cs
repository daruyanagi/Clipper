using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// 共有ターゲット コントラクトのアイテム テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=234241 を参照してください

namespace Clipper
{
    using Windows.ApplicationModel.DataTransfer.ShareTarget;

    /// <summary>
    /// このページを使用すると、他のアプリケーションがこのアプリケーションを介してコンテンツを共有できます。
    /// </summary>
    public sealed partial class ShareTargetPage : Page
    {
        public ShareTargetPage()
        {
            this.InitializeComponent();

            DataContext = new ViewModels.ShareTargetPageViewModel();
        }

        /// <summary>
        /// 他のアプリケーションがこのアプリケーションを介してコンテンツの共有を求めた場合に呼び出されます。
        /// </summary>
        /// <param name="e">Windows と連携して処理するために使用されるアクティベーション データ。</param>
        public void Activate(ShareTargetActivatedEventArgs e)
        {
            ((ViewModels.ShareTargetPageViewModel)DataContext).Initialize(e.ShareOperation);

            Window.Current.Content = this;
            Window.Current.Activate();
        }
    }
}
