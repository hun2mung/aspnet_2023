using MahApps.Metro.Controls;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Windows;
using TodoItemApp.Models;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using MahApps.Metro.Controls.Dialogs;
using System;

namespace TodoItemApp
{
    public class DivCode
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private List<DivCode> divCodes = new List<DivCode>();
        HttpClient client = new HttpClient();
        TodoItemsCollection todoItems = new TodoItemsCollection();
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            divCodes.Add(new DivCode { Key = "True", Value = "1" });
            divCodes.Add(new DivCode { Key = "False", Value = "0" });
            CboIsComplete.ItemsSource = divCodes;
            CboIsComplete.DisplayMemberPath = "Key";    // 콤보박스에 True/False 추가

            // yyyy-MM-dd HH:mm:ss 날짜 포맷(오전/오후 포함)
            DtpTodoDate.Culture = new System.Globalization.CultureInfo("ko-KR");

            //RestAPI 호출 시작 / 아래 두 문장 다시 호출하면 안됨
            client.BaseAddress = new System.Uri("https://localhost:7188/"); // RestAPI 서버 기본 URL
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            GetData();
        }

        private async void GetData()
        {
            GrdTodoItems.ItemsSource = todoItems;   // 미리 바인딩

            try    // API 호출
            {
                // https://localhost:7188/api/TodoItems
                HttpResponseMessage? response = await client.GetAsync("api/TodoItems");  // Get method 비동기 호출
                response.EnsureSuccessStatusCode(); // 에러 발생하면 오류 코드를 던짐(예외발생)

                // 응답에서 List<TodoItem> 형식으로 읽어옴
                var items = await response.Content.ReadAsAsync<IEnumerable<TodoItem>>();
                todoItems.CopyFrom(items);
            }
            catch (Newtonsoft.Json.JsonException jEx)
            {
                await this.ShowMessageAsync("error", jEx.Message, MessageDialogStyle.Affirmative, new MetroDialogSettings()
                {
                    AnimateShow = true,
                    AnimateHide = true
                });
            }
            catch (HttpRequestException ex)
            {
                await this.ShowMessageAsync("error", ex.Message, MessageDialogStyle.Affirmative, new MetroDialogSettings()
                {
                    AnimateShow = true,
                    AnimateHide = true
                });
            }
        }


        private async void BtnInsert_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var todoItem = new TodoItem()
                {
                    Id = 0,
                    Title = TxtTitle.Text,
                    TodoDate = ((DateTime)DtpTodoDate.SelectedDateTime).ToString("yyyy-MM-dd HH:mm:ss"),
                    IsComplete = Int32.Parse((CboIsComplete.SelectedItem as DivCode).Value)
                };

                // Insert 할 때 POST 메서드 사용
                var response = await client.PostAsJsonAsync("api/TodoItems", todoItem);
                response.EnsureSuccessStatusCode();

                GetData();

                TxtId.Text = TxtTitle.Text = string.Empty;

                CboIsComplete.SelectedIndex = -1;
            }
            catch (HttpRequestException ex) { }
        }

        private async void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var todoItem = new TodoItem()
                {
                    Id = Int32.Parse(TxtId.Text),
                    Title = TxtTitle.Text,
                    TodoDate = ((DateTime)DtpTodoDate.SelectedDateTime).ToString("yyyy-MM-dd HH:mm:ss"),
                    IsComplete = Int32.Parse((CboIsComplete.SelectedItem as DivCode).Value)
                };

                // Update 할 때 POST 메서드 사용
                var response = await client.PutAsJsonAsync($"api/TodoItems/{todoItem.Id}", todoItem);
                response.EnsureSuccessStatusCode();

                GetData();

                TxtId.Text = TxtTitle.Text = string.Empty;

                CboIsComplete.SelectedIndex = -1;
            }
            catch (HttpRequestException ex) { }
        }

        private async void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var Id = Int32.Parse(TxtId.Text);

                // Update 할 때 POST 메서드 사용
                var response = await client.DeleteAsync($"api/TodoItems/{Id}");
                response.EnsureSuccessStatusCode();

                GetData();

                TxtId.Text = TxtTitle.Text = string.Empty;
                CboIsComplete.SelectedIndex = -1;
            }
            catch (HttpRequestException ex) { }
        }

        private async void GrdTodoItems_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

            try    // API 호출
            {
                var Id = ((TodoItem)GrdTodoItems.SelectedItem).Id;
                Debug.WriteLine(Id);

                // https://localhost:7188/api/TodoItems
                HttpResponseMessage? response = await client.GetAsync($"api/TodoItems/{Id}");  // Get method 비동기 호출
                response.EnsureSuccessStatusCode(); // 에러 발생하면 오류 코드를 던짐(예외발생)

                // 응답에서 List<TodoItem> 형식으로 읽어옴
                var item = await response.Content.ReadAsAsync<TodoItem>();
                Debug.WriteLine(item.Title);

                TxtId.Text = item.Id.ToString();
                TxtTitle.Text = item.Title;
                DtpTodoDate.SelectedDateTime = DateTime.Parse(item.TodoDate);
                CboIsComplete.SelectedIndex = item.IsComplete == 1 ? 0 : 1;
            }
            catch (Newtonsoft.Json.JsonException jEx)
            {
                await this.ShowMessageAsync("error", jEx.Message, MessageDialogStyle.Affirmative, new MetroDialogSettings()
                {
                    AnimateShow = true,
                    AnimateHide = true
                });
            }
            catch (HttpRequestException ex)
            {
                await this.ShowMessageAsync("error", ex.Message, MessageDialogStyle.Affirmative, new MetroDialogSettings()
                {
                    AnimateShow = true,
                    AnimateHide = true
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"이외 예외 {ex.Message}");
            }

        }

        private void BtnLoad_Click(object sender, RoutedEventArgs e)
        {
            GetData();
        }
    }
}
