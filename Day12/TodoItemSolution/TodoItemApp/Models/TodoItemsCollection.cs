using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace TodoItemApp.Models
{
    public class TodoItemsCollection : ObservableCollection<TodoItem>   //IList<TodoItem>, List<TodoItem>
    {
        public void CopyFrom(IEnumerable<TodoItem> todoItems)
        {
            this.Items.Clear(); //  ObservableCollection<T> 자체가 Items 속성 가지고 있음. 모든 삭제들 삭제

            foreach (TodoItem item in todoItems)
            {
                this.Items.Add(item);   // 하나씩 다시 추가
            }

            // 데이터 바뀜 ( 전부 초기화 )
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }
}
