using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NumpadTrainer
{
    public partial class MainWindow : Window
    {
        // Random Generator
        private Random _randGen = new Random();

        

        // UIElement Arrays
        private TextBlock[] _leftNumberBlocks;
        private TextBlock[] _rightNumberBlocks;
        private Rectangle[] _graphBars;

        // Number, Number Arrays
        private List<int> _pastNumbers = new List<int>();
        private List<int> _futureNumbers = new List<int>();
        private int _currentNumber;
        private List<int> _incorrectIndexes = new List<int>(); // Indexes of Incorrect Presses

        // Settings
        private bool _leftHidden = false;
        private bool _allowIncorrect = true;
        private bool _trackStats = true;

        // Statistics
        private int _totalCorrectEntries = 0;
        private int _totalIncorrectEntries = 0;
        private float _accuracy = 0;
        private int[] _incorrectEntryIndividuals = new int[10]; // Tracking each Incorrect Entry on an Individual Digit Level
        private int _longestCorrectChain = 0;
        private int _longestIncorrectChain = 0;
        private int _currentCorrectChain = 0;
        private int _currentIncorrectChain = 0;

        public MainWindow()
        {
            InitializeComponent();

            _leftNumberBlocks = new TextBlock[]
            {
                LeftSeven, LeftSix, LeftFive, LeftFour,
                LeftThree, LeftTwo, LeftOne
            };

            _rightNumberBlocks = new TextBlock[]
            {
                RightOne, RightTwo, RightThree, RightFour,
                RightFive, RightSix, RightSeven
            };

            _graphBars = new Rectangle[]
            {
                ZeroBar, OneBar, TwoBar, ThreeBar,
                FourBar, FiveBar, SixBar, SevenBar,
                EightBar, NineBar
            };
            
            for (int i = 0; i < 8; i++)
            {
                NewNumber();
            }

            this.KeyDown += new KeyEventHandler(MainWindow_KeyDown);

            OpenMain(null, null);
            UpdateStatistics();

            _currentNumber = _randGen.Next(0, 10);
            DrawNumbers();
        }

        /// <summary>
        /// Update the TextBlock Statistics Values in the Statistics Page with the Private Variables within this Program
        /// </summary>
        private void UpdateStatistics()
        {
            TotalCorrectEntries.Text = _totalCorrectEntries.ToString();
            TotalIncorrectEntries.Text = _totalIncorrectEntries.ToString();

            // Accuracy
            try // Try/Catch to avoid Dividing by Zero
            {
                _accuracy = 100 * ((float)_totalCorrectEntries / (float)(_totalCorrectEntries + _totalIncorrectEntries));
            }
            catch (Exception ex) { }
            Accuracy.Text = Math.Round(_accuracy, 3).ToString();
            Accuracy.Text += "%";

            // Best and Worst Number
            int minCount = _incorrectEntryIndividuals.Min();
            int maxCount = _incorrectEntryIndividuals.Max();

            // Find the Indexes (numbers) corresponding to the Minimum and Maximum Counts
            var bestNumbers = Enumerable.Range(0, _incorrectEntryIndividuals.Length)
                                        .Where(i => _incorrectEntryIndividuals[i] == minCount)
                                        .ToArray();
            var worstNumbers = Enumerable.Range(0, _incorrectEntryIndividuals.Length)
                                         .Where(i => _incorrectEntryIndividuals[i] == maxCount)
                                         .ToArray();

            // Format the Result
            string bestNumbersStr = string.Join(", ", bestNumbers);
            string worstNumbersStr = string.Join(", ", worstNumbers);

            BestNumber.Text = bestNumbersStr;
            WorstNumber.Text = worstNumbersStr;

            // Longest Chains
            LongestCorrectChain.Text = _longestCorrectChain.ToString();
            LongestIncorrectChain.Text = _longestIncorrectChain.ToString();
        }


        /// <summary>
        /// Generate a New Number for the Array of Future Numbers
        /// </summary>
        private void NewNumber()
        {
            int nextNumber = _randGen.Next(0, 10);
            _futureNumbers.Add(nextNumber);
        }

        /// <summary>
        /// Draws the Numbers into the TextBlocks
        /// </summary>
        private void DrawNumbers()
        {
            Center.Text = _currentNumber.ToString();
            for (int i = 0; i < 7; i++)
            {
                _rightNumberBlocks[i].Text = _futureNumbers[i].ToString();
            }

            for (int i = 0; i < Math.Min(7, _pastNumbers.Count); i++)
            {
                _leftNumberBlocks[i].Foreground = Brushes.White;

                // Convert Forebground Colour to Red if Current Index is in Incorrect Array
                if (_incorrectIndexes.Contains(i))
                {
                    _leftNumberBlocks[i].Foreground = Brushes.Red;
                }

                _leftNumberBlocks[i].Text = _pastNumbers[i].ToString();

                if (_leftHidden == false)
                {
                    // Fix for Alignment Issues
                    if (_leftNumberBlocks[i].Visibility == Visibility.Hidden)
                    {
                        _leftNumberBlocks[i].Visibility = Visibility.Visible;
                    }
                }
            }

            for (int j = 0; j < _incorrectIndexes.Count; j++)
            {
                _incorrectIndexes[j]++; // Increment All Valid Indexes
            }
        }

        /// <summary>
        /// Reset the Game, including Visually and all Currently Tracked Stats
        /// </summary>
        private void Reset(object sender, RoutedEventArgs e)
        {
            FlipUCR(null, null);

            // Resetting Numners
            _pastNumbers = new List<int>();
            _futureNumbers = new List<int>();

            for (int i = 0; i < 8; i++)
            {
                NewNumber();
            }

            // Resetting Left Number Visibility
            for (int i = 0; i < 7; i++)
            {
                _leftNumberBlocks[i].Visibility = Visibility.Hidden;
            }

            _currentNumber = _randGen.Next(0, 10);
            DrawNumbers();
        }

        private void ResetStats(object sender, RoutedEventArgs e)
        {
            ResetStatsConfirmation.Visibility = Visibility.Collapsed;

            Reset(null, null);
            FlipUCR(null, null);

            // Statistics
            _totalCorrectEntries = 0;
            _totalIncorrectEntries = 0;
            _accuracy = 0;
            _incorrectEntryIndividuals = new int[10]; // Tracking each Incorrect Entry on an Individual Digit Level
            _longestCorrectChain = 0;
            _longestIncorrectChain = 0;
            _currentCorrectChain = 0;
            _currentIncorrectChain = 0;
    }

        /// <summary>
        /// Reset all Settings to Default
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResetSettings(object sender, RoutedEventArgs e)
        {
            FutureNumbersSlider.Value = 7;
            SetFutureNumberVisibility(null, null);

            PreviousNumbersSlider.Value = 1;
            SetPreviousVisibility(null, null);

            AllowIncorrectSlider.Value = 1;
            SetAllowIncorrect(null, null);
        }

        /// <summary>
        /// Set Appropriate Value of Previous Numbers Visibility
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetPreviousVisibility(object sender, RoutedEventArgs e)
        {
            // Previous Numbers Visible
            if (PreviousNumbersSlider.Value == 1)
            {
                _leftHidden = false;
                for (int i = 0; i < Math.Min(7, _pastNumbers.Count); i++)
                {
                    _leftNumberBlocks[i].Visibility = Visibility.Visible;
                }
            }

            // Previous Numbers Invisible
            if (PreviousNumbersSlider.Value == 2)
            {
                _leftHidden = true;
                for (int i = 0; i < Math.Min(7, _pastNumbers.Count); i++)
                {
                    _leftNumberBlocks[i].Visibility = Visibility.Hidden; 
                }
            }

            DrawNumbers();
        }

        /// <summary>
        /// Call Set Previous Visibility, fixes issue of slider being Clicked and Appropriate Method not being called
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CallSPV(object sender, MouseButtonEventArgs e)
        {
            SetPreviousVisibility(null, null);
        }


        /// <summary>
        /// Sets the Visibility of the Specified Amount of Future Numbers, based on the Slider in the Settings
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetFutureNumberVisibility(object sender, RoutedEventArgs e)
        {
            int amountVisible = (int)(FutureNumbersSlider.Value);
            for (int i = 0; i < 7; i++)
            {
                if (i < amountVisible)
                {
                    _rightNumberBlocks[i].Visibility = Visibility.Visible;
                }
                else
                {
                    _rightNumberBlocks[i].Visibility = Visibility.Hidden;
                }
            }

            DrawNumbers();
        }

        /// <summary>
        /// Calls Set Future Number Visibility, Fixes issue of Slider being Clicked to set Value and not calling appropriate Method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CallSFNV(object sender, MouseButtonEventArgs e)
        {
            SetFutureNumberVisibility(null, null);
        }

        /// <summary>
        /// Sets the Ability for the User to enter Incorrect Numbers
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetAllowIncorrect(object sender, RoutedEventArgs e)
        {
            // Allow Incorrect Values
            if (AllowIncorrectSlider.Value == 1)
            {
                _allowIncorrect = true;
                _trackStats = true;
            }

            // Don't Allow Incorrect Values
            else
            {
                _allowIncorrect = false;
                _trackStats = false;
            }
        }

        /// <summary>
        /// Calls Set Allow Incorrect, Fixes issue of Slider being Clicked to set Value and not calling appropriate Method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CallSAI(object sender, MouseButtonEventArgs e)
        {
            SetAllowIncorrect(null, null);
        }

        /// <summary>
        /// Open the Settings Screen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenSettings(object sender, RoutedEventArgs e)
        {
            MainScreen.Visibility = Visibility.Collapsed;
            SettingsScreen.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Open the Statistics Screen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenStats(object sender, RoutedEventArgs e)
        {
            UpdateStatistics();
            MainScreen.Visibility = Visibility.Collapsed;
            StatisticsScreen.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Open the Main Screen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenMain(object sender, RoutedEventArgs e)
        {
            MainScreen.Visibility = Visibility.Visible;
            StatisticsScreen.Visibility = Visibility.Collapsed;
            SettingsScreen.Visibility = Visibility.Collapsed;
            KeyGraphScreen.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Opens Graph Screen of Individual Key's Incorrect Entries
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenKeyGraph(object sender, RoutedEventArgs e)
        {
            SetKeyGraphBars();
            StatisticsScreen.Visibility = Visibility.Collapsed;
            KeyGraphScreen.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Sets the Appropriate Values of the Bars on the Key Graph
        /// </summary>
        private void SetKeyGraphBars()
        {
            // Maximum Defined as 600px in Height from a Canvas Top of 84 Pixels
            int maxValue = _incorrectEntryIndividuals.Max();

            // Ugliest Code in the world I just want this finished!
            ZeroBar.Height = _incorrectEntryIndividuals[0] != 0 ? _incorrectEntryIndividuals[0] * 600 / maxValue : 0;
            OneBar.Height = _incorrectEntryIndividuals[1] != 0 ? _incorrectEntryIndividuals[1] * 600 / maxValue : 0;
            TwoBar.Height = _incorrectEntryIndividuals[2] != 0 ? _incorrectEntryIndividuals[2] * 600 / maxValue : 0;
            ThreeBar.Height = _incorrectEntryIndividuals[3] != 0 ? _incorrectEntryIndividuals[3] * 600 / maxValue : 0;
            FourBar.Height = _incorrectEntryIndividuals[4] != 0 ? _incorrectEntryIndividuals[4] * 600 / maxValue : 0;
            FiveBar.Height = _incorrectEntryIndividuals[5] != 0 ? _incorrectEntryIndividuals[5] * 600 / maxValue : 0;
            SixBar.Height = _incorrectEntryIndividuals[6] != 0 ? _incorrectEntryIndividuals[6] * 600 / maxValue : 0;
            SevenBar.Height = _incorrectEntryIndividuals[7] != 0 ? _incorrectEntryIndividuals[7] * 600 / maxValue : 0;
            EightBar.Height = _incorrectEntryIndividuals[8] != 0 ? _incorrectEntryIndividuals[8] * 600 / maxValue : 0;
            NineBar.Height = _incorrectEntryIndividuals[9] != 0 ? _incorrectEntryIndividuals[9] * 600 / maxValue : 0;
        }

        /// <summary>
        /// Opens the Stats Screen from the Key Graph Screen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenStatsKeyGraph(object sender, RoutedEventArgs e)
        {
            StatisticsScreen.Visibility = Visibility.Visible;
            KeyGraphScreen.Visibility = Visibility.Collapsed;
        }


        /// <summary>
        /// Turns the User Confirm Reset Box On if Off, and Off if On
        /// </summary>
        private void FlipUCR(object sender, RoutedEventArgs e)
        {
            // Turning On
            if (UserConfirmReset.Visibility == Visibility.Collapsed)
            {
                UserConfirmReset.Visibility = Visibility.Visible;
            }

            // Turning Off
            else if (UserConfirmReset.Visibility == Visibility.Visible)
            {
                UserConfirmReset.Visibility -= Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Turns the Reset Stats Confirm Box On if Off, and Off if On
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FlipRSC(object sender, RoutedEventArgs e)
        {
            // Turning On
            if (ResetStatsConfirmation.Visibility == Visibility.Collapsed)
            {
                UserConfirmReset.Visibility = Visibility.Collapsed;
                ResetStatsConfirmation.Visibility = Visibility.Visible;
            }

            // Turning Off
            else if (ResetStatsConfirmation.Visibility == Visibility.Visible)
            {
                ResetStatsConfirmation.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Check for Key Press
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void MainWindow_KeyDown(object sender, KeyEventArgs eventArgs)
        {
            switch (eventArgs.Key)
            {
                // Numpad Zero
                case Key.NumPad0:
                    CheckKeyPress(0);
                    break;

                // Numpad One
                case Key.NumPad1:
                    CheckKeyPress(1);
                    break;

                // Numpad Two
                case Key.NumPad2:
                    CheckKeyPress(2);
                    break;

                // Numpad Three
                case Key.NumPad3:
                    CheckKeyPress(3);
                    break;

                // Numpad Four
                case Key.NumPad4:
                    CheckKeyPress(4);
                    break;

                // Numpad Five
                case Key.NumPad5:
                    CheckKeyPress(5);
                    break;

                // Numpad Six
                case Key.NumPad6:
                    CheckKeyPress(6);
                    break;

                // Numpad Seven
                case Key.NumPad7:
                    CheckKeyPress(7);
                    break;

                // Numpad Eight
                case Key.NumPad8:
                    CheckKeyPress(8);
                    break;

                // Numpad Nine
                case Key.NumPad9:
                    CheckKeyPress(9);
                    break;

                default: break;

            }
            
        }

        private void CheckKeyPress(int press)
        {
            // Incorrect Key Press
            if (press != _currentNumber && _allowIncorrect == true)
            {
                // Updating Statistics
                if (_trackStats)
                {
                    // Incrementers/Counters
                    _currentIncorrectChain++;
                    _totalIncorrectEntries++;
                    _incorrectEntryIndividuals[_currentNumber]++;

                    // Setting Longest Chains
                    if (_currentCorrectChain > _longestCorrectChain)
                    {
                        _longestCorrectChain = _currentCorrectChain;
                    }
                    if (_currentIncorrectChain > _longestIncorrectChain)
                    {
                        _longestIncorrectChain = _currentIncorrectChain;
                    }

                    _currentCorrectChain = 0;
                    
                }

                _incorrectIndexes.Add(0);
            }

            // Correct Key Press
            else if (press == _currentNumber)
            {
                // Updating Statistics
                if (_trackStats)
                {
                    // Incrementers/Counters
                    _totalCorrectEntries++;
                    _currentCorrectChain++;

                    // Setting Longest Chains
                    if (_currentIncorrectChain > _longestIncorrectChain)
                    {
                        _longestIncorrectChain = _currentIncorrectChain;
                    }
                    if (_currentCorrectChain > _longestCorrectChain)
                    {
                        _longestCorrectChain = _currentCorrectChain;
                    }
                    _currentIncorrectChain = 0;
                    
                }
            }

            if ((_allowIncorrect == false && press == _currentNumber) || _allowIncorrect == true)
            { 
                // Run Next Number
                _pastNumbers.Insert(0, _currentNumber);
                _currentNumber = _futureNumbers[0];
                _futureNumbers.RemoveAt(0);
                NewNumber();
                DrawNumbers();
            }

           

        }
    }
}
