using System;
using System.Windows;
using System.Windows.Media;
using Microsoft.Win32;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Timers;
using System.Collections.Generic;

namespace MusicPlayer1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer timer = new DispatcherTimer();
        private MediaPlayer mediaPlayer = new MediaPlayer();
        public bool play = false;
        public bool loaded = false;
        public bool AddClick = false;
        public MainWindow()
        {
            InitializeComponent();
            PlayList.SelectedIndex = 0;
            timer.Tick += timer_Tick;
            Play1.Visibility = Visibility.Hidden;
            Pause.Visibility = Visibility.Visible;
            Repeat_Button.Visibility = Visibility.Visible;
            Repeat_Button_clc.Visibility = Visibility.Hidden;
            Shuffle_Button.Visibility = Visibility.Visible;
            //Shuffle_Button_clc.Visibility = Visibility.Hidden;
            SliderVolume.Value = 0.5;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            //TimerStatus.Content = String.Format("{0} / {1}", mediaPlayer.Position.ToString(@"mm\:ss"), mediaPlayer.NaturalDuration.TimeSpan.ToString(@"mm\:ss")); // Отображение таймера
            SliderTime.Value = mediaPlayer.Position.TotalSeconds; // Движение слайдера
        }

        private void AddList_Click(object sender, RoutedEventArgs e) // Добавление трека
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = "C:\\Users\\User\\Downloads\\VK audio"; // Открывающаяся папка    
            openFileDialog.Filter = "MP3 files (*.mp3)|*.mp3|All files (*.*)|*.*"; // Фильтр файлов
            openFileDialog.RestoreDirectory = true; // Сохранение последней папки
            openFileDialog.Multiselect = true; // Выбор нескольких файлов
        
            if ((openFileDialog.ShowDialog() == true) && (AddClick = false))
            {
                foreach (string File in openFileDialog.FileNames)
                {
                    string[] Array = File.Split('\\'); // Разбиение пути файла
                    string Name = Array[Array.Length - 1]; // Выбор последнего элемента пути
                    PlayList.Items.Add(new AudioFile(File, Name) { Foreground = Brushes.Purple, BorderBrush = Brushes.White }) ; // Добавление элемента в плей-лист
                }
                mediaPlayer.MediaOpened += MediaPlayer_MediaOpened;
                mediaPlayer.MediaEnded += MediaPlayer_MediaEnded;
                AddClick = true;
            }
            else
            {
                PlayList.Items.Clear();
                foreach (string File in openFileDialog.FileNames)
                {
                    string[] Array = File.Split('\\'); // Разбиение пути файла
                    string Name = Array[Array.Length - 1]; // Выбор последнего элемента пути
                    PlayList.Items.Add(new AudioFile(File, Name) { Foreground = Brushes.Purple }); // Добавление элемента в плей-лист
                }
                mediaPlayer.MediaOpened += MediaPlayer_MediaOpened;
                mediaPlayer.MediaEnded += MediaPlayer_MediaEnded;
            }
        }

        private void ClearList_Click(object sender, RoutedEventArgs e) // Удаление трека из листа
        {
            if (PlayList.SelectedItem != null)
            {
                PlayList.Items.RemoveAt(PlayList.SelectedIndex);
                PlayList.SelectedIndex = -1;
            }
        }

        private void MediaPlayer_MediaOpened(object sender, EventArgs e) // При открытии файла
        {
            SliderTime.Maximum = mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds; // Присвоение максимального значения слайдера 1
            SliderTimeMain.Maximum = SliderTime.Maximum; // Присвоение максимального значения слайдера 2
            loaded = true; // Индикация загрузки файла          
            playmusic();
        }

        private void PlayList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) // Выбор трека из плей-листа
        {
            if (PlayList.SelectedIndex != -1)
            {
                mediaPlayer.Open(new Uri(((AudioFile)PlayList.SelectedValue).FilePath));
                AudioFile File = (AudioFile)PlayList.SelectedItem;
                TrackName.Text = File.FileName;
            }
        }

        private void SliderTime_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) // Изменение значения слайдера времени
        {
            mediaPlayer.Position = TimeSpan.FromSeconds(SliderTime.Value);
        }

        private void SliderTimeMain_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e) // Свойства слайдера
        {
            SliderTimeMain.Opacity = 0;
            SliderTime.Value = SliderTimeMain.Value;
            mediaPlayer.Position = TimeSpan.FromSeconds(SliderTime.Value);
        }

        private void SliderTimeMain_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) // Свойства слайдера при нажатии мыши
        {
            SliderTimeMain.Opacity = 1;
        }

        private void SliderVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) // Громкость
        {
            if (mediaPlayer != null)
            {
                mediaPlayer.Volume = SliderVolume.Value;
            }
        }

        private void Play_Click(object sender, RoutedEventArgs e) // Воспроизведение
        {
            if ((loaded) && (!play)) // Условия запуска воспроизведения
            {
                playmusic();
                Play1.Visibility = Visibility.Hidden;
                Pause.Visibility = Visibility.Visible;
                play = true;
            }
        }

        private void Pause_Click(object sender, RoutedEventArgs e) // Пауза
        {
            if (play)
            {
                mediaPlayer.Pause();
                timer.Stop();
                Play1.Visibility = Visibility.Visible;
                Pause.Visibility = Visibility.Hidden;
                play = false;
            }
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Stop();
            Pause.Visibility = Visibility.Hidden;
            Play1.Visibility = Visibility.Visible;
            timer.Stop();
            SliderTime.Value = 0;
            mediaPlayer.Position = TimeSpan.FromSeconds(SliderTime.Value);
            play = false;
        }

        public void MediaPlayer_MediaEnded (object sender, EventArgs e) // Конец трека
        {
            NextTrackPlay();
        }

        private void PrewTrack_Click(object sender, RoutedEventArgs e) // Предыдущий трек
        {
            if (PlayList.SelectedIndex != 0)
            {
                PlayList.SelectedIndex--;
            }
        }

        private void NextTrack_Click(object sender, RoutedEventArgs e) // Следующий трек
        {
            NextTrackPlay();
        }

        public void playmusic() // Функция воспроизведения
        {
            mediaPlayer.Play();
            timer.Start();
            play = true;
        }

        public void NextTrackPlay () // Функция смены трека
        {
            if (PlayList.SelectedIndex < (PlayList.Items.Count)) // Конец листа?
            {
                if (Repeat_Button.IsChecked == true) // Повтор
                    mediaPlayer.Position = TimeSpan.Zero;
                else
                {
                    PlayList.SelectedIndex++; // След. трек  
                }

                //if (Shuffle_Button.IsChecked == true) // Рандом
                //{
                    

                //    //mediaPlayer.Open(new Uri(((AudioFile)PlayList.SelectedValue).FilePath));
                //    //AudioFile File = (AudioFile)PlayList.SelectedItem;
                //    //TrackName.Text = File.FileName;

                //    //var Random = new Random();
                //    //int index = Random.Next(0, PlayList.Items.Count);
                //    //PlayList.SelectedIndex = index;
                //    //Repeat_Button_clc.IsChecked = false;
                //}

                //if (Shuffle_Button_clc.IsChecked == true)
                //{
                //    PlayList.SelectedIndex++; // След. трек 
                //    Repeat_Button.IsChecked = false;
                //}
            }
            else
                PlayList.SelectedIndex = 0; // Воспроизведение сначала списка
        } 

        private void Grid_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) // Передвижение окна плеера
        {
            DragMove();
        }

        private void Close_Click(object sender, RoutedEventArgs e) // Закрытие приожения
        {
            Application.Current.Shutdown();
        }

        private void Repeat_Button_Checked(object sender, RoutedEventArgs e) // Нажатие кнопки повтора
        {
            Repeat_Button.Visibility = Visibility.Hidden;
            Repeat_Button_clc.Visibility = Visibility.Visible;
        }

        private void Repeat_Button_clc_Checked(object sender, RoutedEventArgs e)
        {
            Repeat_Button_clc.Visibility = Visibility.Hidden;
            Repeat_Button.Visibility = Visibility.Visible;
        }

        private void Shuffle_Button_Click(object sender, RoutedEventArgs e)
        {
            Random rnd = new Random(); 
            List<Label> newlist = new List<Label>(); 

            while (PlayList.Items.Count > 0) 
            {
                int value = rnd.Next(0, PlayList.Items.Count); 
                newlist.Add((Label)PlayList.Items[value]); 
                PlayList.Items.Remove(PlayList.Items[value]); 
            }

            foreach (Label l in newlist) 
            {
                PlayList.Items.Add(l);
            }
        }
    }

    public class AudioFile : Label
    {
        public string FileName;
        public string FilePath;

        public AudioFile(string in_path, string in_name)
        {
            FileName = in_name;
            FilePath = in_path;
            Content = FileName;
        }
    }
}
