using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace JeopardyCalculator
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        List<double> undo = new List<double>();
        bool DDCheck = false;
        SolidColorBrush AccentColor = new SolidColorBrush();
        public static ObservableCollection<JCalcScores> scores = new ObservableCollection<JCalcScores>();

        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;

            AccentColor = Application.Current.Resources["PhoneAccentBrush"] as SolidColorBrush;

            //Initialize();
        }

        async public void Initialize()
        {
            var scoresResponse = await App.MobileService.GetTable<JCalcScores>().ToListAsync();
            foreach (JCalcScores s in scoresResponse)
            {
                scores.Add(s);
            }
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.
        }

        private void ButtonAmount(Button btn)
        {
            Addend.Text = btn.Content.ToString();
        }
        private void ButtonUndo()
        {
            if (undo.Count > 0)
            {
                Total.Text = (Double.Parse(Total.Text) + undo.Last()).ToString();
                ContextMessage.Visibility = Visibility.Visible;
                ContextMessage.Foreground = AccentColor;
                if (undo.Last() > 0)
                {
                    ContextMessage.Text = "Undo: +" + (undo.Last()).ToString();
                }
                else
                {
                    ContextMessage.Text = "Undo: " + (undo.Last()).ToString();
                }
                undo.Remove(undo.Last());
            }
        }
        private void ButtonDailyDouble()
        {
            if (WagerOptions.Visibility == Visibility.Collapsed)
            {
                DDCheck = true;

                WagerOptions.Visibility = Visibility.Visible;
                Undo.Content = "";
                Undo.Click -= ButtonScore_Click;
                Undo.Background = NormalGray.Background;
                DailyDouble.Background = AccentColor;
                FinalJeopardy.Content = "";
                FinalJeopardy.Click -= ButtonScore_Click;
                Addend.Text = "";
                WagerBox.Text = "";

                if (Double.Parse(Total.Text) > 0)
                {
                    SetPercentageWagerButtons(true);
                }
                else
                {
                    SetPercentageWagerButtons(false);
                }
            }
            else
            {
                DDCheck = false;
                WagerOptions.Visibility = Visibility.Collapsed;
                Undo.Content = "Undo";
                Undo.Background = AccentColor;
                DailyDouble.Background = NormalGray.Background;
                FinalJeopardy.Content = "FINAL JEOPARDY";
                FinalJeopardy.Click += ButtonScore_Click;
                Undo.Click += ButtonScore_Click;
                Addend.Text = "";
            }
        }
        private void ButtonFinalJeopardy()
        {
            if (WagerOptions.Visibility == Visibility.Collapsed)
            {
                if (Double.Parse(Total.Text) < 1)
                {
                    ContextMessage.Visibility = Visibility.Visible;
                    ContextMessage.Foreground = new SolidColorBrush(Windows.UI.Colors.Red);
                    ContextMessage.Text = "Score too low to wager"; 
                    Addend.Text = "";
                }
                else
                {
                    WagerOptions.Visibility = Visibility.Visible;
                    Undo.Content = "";
                    Undo.Click -= ButtonScore_Click;
                    Undo.Background = NormalGray.Background;
                    FinalJeopardy.Background = AccentColor;
                    DailyDouble.Content = "";
                    DailyDouble.Click -= ButtonScore_Click;
                    Addend.Text = "";
                    WagerBox.Text = "";
                    
                    SetPercentageWagerButtons(true);
                }
            }
            else
            {
                WagerOptions.Visibility = Visibility.Collapsed;
                Undo.Content = "Undo";
                DailyDouble.Content = "DAILY DOUBLE";
                DailyDouble.Click += ButtonScore_Click;
                FinalJeopardy.Background = NormalGray.Background;
                Undo.Click += ButtonScore_Click;
                Undo.Background = AccentColor;
                Addend.Text = "";
            }
        }
        private void ButtonWagerAmount_Click(Button btn)
        {
            if (Double.Parse(Total.Text) > 0)
            {
                if (btn.Name.ToString() == "TENp")
                {
                    Addend.Text = Double.Parse(Tenth.Text).ToString();
                }
                else if (btn.Name.ToString() == "TWENTYp")
                {
                    Addend.Text = Double.Parse(Fifth.Text).ToString();
                }
                else if (btn.Name.ToString() == "TWENTYFIVEp")
                {
                    Addend.Text = Double.Parse(Quarter.Text).ToString();
                }
                else if (btn.Name.ToString() == "THIRTYp")
                {
                    Addend.Text = Double.Parse(Third.Text).ToString();
                }
                else if (btn.Name.ToString() == "FIFTYp")
                {
                    Addend.Text = Double.Parse(Half.Text).ToString();
                }
                else if (btn.Name.ToString() == "SEVENTYFIVEp")
                {
                    Addend.Text = Double.Parse(ThreeQuarters.Text).ToString();
                }
                else if (btn.Name.ToString() == "HUNDREDp")
                {
                    Addend.Text = Double.Parse(All.Text).ToString();
                }
            }
        }
        private void ButtonCustomWager(Button btn)
        {
            Addend.Text = "";
            ContextMessage.Foreground = new SolidColorBrush(Windows.UI.Colors.White);
            if (Double.Parse(Total.Text) < 1000 && DDCheck)
            {
                ContextMessage.Text = "You may wager up to 1000";
            }
            else
            {
                ContextMessage.Text = "You may wager up to " + Total.Text;
            }

            ContextMessage.Visibility = Visibility.Visible;
            Addend.Visibility = Visibility.Collapsed;
            WagerBox.Visibility = Visibility.Visible;
            WagerBox.Focus(FocusState.Programmatic);
        }
        private void ButtonArithmetic(Button btn)
        {
            if (Addend.Text != "")
            {
                Total.Text = (Double.Parse(Total.Text) + (Double.Parse(Addend.Text)) * Double.Parse(btn.Tag.ToString())).ToString();
                undo.Add(Double.Parse(Addend.Text) * Double.Parse(btn.Tag.ToString()) * -1);
                ResetUDF();
            }
            if (WagerBox.Text != "")
            {
                if (Math.Round(Double.Parse(WagerBox.Text), 0) != (Double.Parse(WagerBox.Text)))
                {
                    ContextMessage.Text = "You may wager whole numbers only";
                    ContextMessage.Foreground = new SolidColorBrush(Windows.UI.Colors.Red);
                    ContextMessage.FontSize = 20;
                    ContextMessage.Visibility = Visibility.Visible;
                }
                else
                {
                    if ((DDCheck && Double.Parse(Total.Text) < 1000)
                        && (Double.Parse(WagerBox.Text) < 1 || Double.Parse(WagerBox.Text) > 1000))
                    {
                        ContextMessage.Text = "You may wager between 1 and 1000";
                        ContextMessage.Foreground = new SolidColorBrush(Windows.UI.Colors.Red);
                        ContextMessage.FontSize = 20;
                        ContextMessage.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        if (Double.Parse(Total.Text) > 1000
                            && (Double.Parse(WagerBox.Text) > Double.Parse(Total.Text)
                                || Double.Parse(WagerBox.Text) < 1))
                        {
                            ContextMessage.Text = "You may wager between 1 and " + Total.Text;
                            ContextMessage.Foreground = new SolidColorBrush(Windows.UI.Colors.Red);
                            ContextMessage.FontSize = 20;
                            ContextMessage.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            Total.Text = (Double.Parse(Total.Text) + (Double.Parse(WagerBox.Text)) * Double.Parse(btn.Tag.ToString())).ToString();
                            undo.Add(Double.Parse(WagerBox.Text) * Double.Parse(btn.Tag.ToString()) * -1);
                            ResetUDF();
                        }
                    }
                }
            }
        }

        private void ButtonScore_Click(object sender, RoutedEventArgs e)
        {
            ContextMessage.Visibility = Visibility.Collapsed;
            Addend.Visibility = Visibility.Visible;
            WagerBox.Visibility = Visibility.Collapsed;
            ContextMessage.FontSize = 25;

            Button btn = (Button)sender;

            if (Regex.IsMatch(btn.Content.ToString(), @"^\d+$"))
            {
                ButtonAmount(btn);
            }
            else if (btn.Content.ToString() == "+" || btn.Content.ToString() == "-")
            {
                ButtonArithmetic(btn);
            }
            else if (btn.Content.ToString() == "Undo")
            {
                ButtonUndo();
            }
            else if (btn.Content.ToString() == "DAILY DOUBLE")
            {
                ButtonDailyDouble();
            }
            else if (btn.Content.ToString() == "FINAL JEOPARDY")
            {
                ButtonFinalJeopardy();
            }
            else if (btn.Tag.ToString() == "CUSTOM")
            {
                ButtonCustomWager(btn);
            }
            else
            {
                ButtonWagerAmount_Click(btn);
            }
        }

        public void ResetUDF()
        {
            WagerOptions.Visibility = Visibility.Collapsed;
            DailyDouble.Content = "DAILY DOUBLE";
            DailyDouble.Click -= ButtonScore_Click;
            DailyDouble.Click += ButtonScore_Click;
            DailyDouble.Background = NormalGray.Background;
            FinalJeopardy.Content = "FINAL JEOPARDY";
            FinalJeopardy.Click -= ButtonScore_Click;
            FinalJeopardy.Click += ButtonScore_Click;
            FinalJeopardy.Background = NormalGray.Background;
            Undo.Content = "Undo";
            Undo.Click -= ButtonScore_Click;
            Undo.Click += ButtonScore_Click;
            Undo.Background = AccentColor;
            WagerBox.Text = "";
            Addend.Text = "";
            DDCheck = false;
        }

        public void SetPercentageWagerButtons(bool sign)
        {
            if (sign)
            {
                TENp.Foreground = new SolidColorBrush(Windows.UI.Colors.White);
                TWENTYp.Foreground = new SolidColorBrush(Windows.UI.Colors.White);
                TWENTYFIVEp.Foreground = new SolidColorBrush(Windows.UI.Colors.White);
                THIRTYp.Foreground = new SolidColorBrush(Windows.UI.Colors.White);
                FIFTYp.Foreground = new SolidColorBrush(Windows.UI.Colors.White);
                SEVENTYFIVEp.Foreground = new SolidColorBrush(Windows.UI.Colors.White);
                HUNDREDp.Foreground = new SolidColorBrush(Windows.UI.Colors.White);

                Tenth.Foreground = new SolidColorBrush(Windows.UI.Colors.DarkSlateGray);
                Fifth.Foreground = new SolidColorBrush(Windows.UI.Colors.DarkSlateGray);
                Quarter.Foreground = new SolidColorBrush(Windows.UI.Colors.DarkSlateGray);
                Third.Foreground = new SolidColorBrush(Windows.UI.Colors.DarkSlateGray);
                Half.Foreground = new SolidColorBrush(Windows.UI.Colors.DarkSlateGray);
                ThreeQuarters.Foreground = new SolidColorBrush(Windows.UI.Colors.DarkSlateGray);
                All.Foreground = new SolidColorBrush(Windows.UI.Colors.DarkSlateGray);

                Tenth.Text = (Math.Floor(Double.Parse(Total.Text) / Double.Parse(TENp.Tag.ToString()))).ToString();
                Fifth.Text = (Math.Floor(Double.Parse(Total.Text) / Double.Parse(TWENTYp.Tag.ToString()))).ToString();
                Quarter.Text = (Math.Floor(Double.Parse(Total.Text) / Double.Parse(TWENTYFIVEp.Tag.ToString()))).ToString();
                Third.Text = (Math.Floor(Double.Parse(Total.Text) / Double.Parse(THIRTYp.Tag.ToString()))).ToString();
                Half.Text = (Math.Floor(Double.Parse(Total.Text) / Double.Parse(FIFTYp.Tag.ToString()))).ToString();
                ThreeQuarters.Text = (Math.Floor(Double.Parse(Total.Text) / Double.Parse(SEVENTYFIVEp.Tag.ToString()))).ToString();
                All.Text = Total.Text;
            }
            else
            {
                TENp.Foreground = new SolidColorBrush(Windows.UI.Colors.DimGray);
                TWENTYp.Foreground = new SolidColorBrush(Windows.UI.Colors.DimGray);
                TWENTYFIVEp.Foreground = new SolidColorBrush(Windows.UI.Colors.DimGray);
                THIRTYp.Foreground = new SolidColorBrush(Windows.UI.Colors.DimGray);
                FIFTYp.Foreground = new SolidColorBrush(Windows.UI.Colors.DimGray);
                SEVENTYFIVEp.Foreground = new SolidColorBrush(Windows.UI.Colors.DimGray);
                HUNDREDp.Foreground = new SolidColorBrush(Windows.UI.Colors.DimGray);

                Tenth.Foreground = new SolidColorBrush(Windows.UI.Colors.DimGray);
                Fifth.Foreground = new SolidColorBrush(Windows.UI.Colors.DimGray);
                Quarter.Foreground = new SolidColorBrush(Windows.UI.Colors.DimGray);
                Third.Foreground = new SolidColorBrush(Windows.UI.Colors.DimGray);
                Half.Foreground = new SolidColorBrush(Windows.UI.Colors.DimGray);
                ThreeQuarters.Foreground = new SolidColorBrush(Windows.UI.Colors.DimGray);
                All.Foreground = new SolidColorBrush(Windows.UI.Colors.DimGray);

                Tenth.Text = "0";
                Fifth.Text = "0";
                Quarter.Text = "0";
                Third.Text = "0";
                Half.Text = "0";
                ThreeQuarters.Text = "0";
                All.Text = "0";
            }
        }

        private void ButtonSaveScore_Click(object sender, RoutedEventArgs e)
        {
            JCalcScores item = new JCalcScores
            {
                Name = "Corbin",
                Score = Double.Parse(Total.Text),
                Time = DateTime.Now
            };
            System.Diagnostics.Debug.WriteLine(item.Time);
            //await App.MobileService.GetTable<JCalcScores>().InsertAsync(item);
            scores.Add(item);
            //var scoresResponse = await App.MobileService.GetTable<JCalcScores>().ToListAsync();
            ContextMessage.Text = "Score saved!";
            ContextMessage.Foreground = AccentColor;
            ContextMessage.Visibility = Visibility.Visible;
            Total.Text = "0";
            Addend.Text = "";
            undo.Clear();
        }
        private void ButtonViewScores_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Scores));
        }
        private void ButtonNewGame_Click(object sender, RoutedEventArgs e)
        {
            Total.Text = "0";
            Addend.Text = "";
            undo.Clear();
        }
        private void ButtonAbout_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(About));
        }
    }
}
