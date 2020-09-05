using NumberToWordConverter.Domain;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace NumberToWordConverter
{
    public partial class Form1 : Form
    {
        private readonly SimpleSiteService _siteService = new SimpleSiteService("https://chislitelnye.ru/chisla-slovami.html");

        public Form1()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                button1.Enabled = false;
                statusInfo.Text = "Загрузка доступных для перевода языков...";

                var languages = await _siteService.GetLanguagesAsync();

                button1.Enabled = true;
                comboBox1.DataSource = languages;
                comboBox1.SelectedItem = languages.First(lang => lang.Key == "ru");
                comboBox1.DisplayMember = "Name";
                statusInfo.Text = "Все загружено и готово к работе!";
            }
            catch
            {
                statusInfo.Text = "Не удалось загрузить языки, сайт недоступен.";
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (int.TryParse(textBox1.Text, out int value))
                {
                    statusInfo.Text = "Попытка получить число в виде строки...";
                    button1.Enabled = false;
                    richTextBox1.Text = await _siteService.ConvertToWordsAsync(value, (LanguageItem)comboBox1.SelectedItem);
                    statusInfo.Text = "Запрос успешно обработан!";
                }
                else MessageBox.Show("Входные данные должны быть числом", "Ошибка данных", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch
            {
                statusInfo.Text = "Не удалось получить ответ от сайта.";
            }
            finally
            {
                button1.Enabled = true;
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            e.Link.Visited = true;
            Process.Start(linkLabel1.Text);
        }
    }
}
